using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace cherrydev
{
    public class DialogNode : Node
    {
        //Node Data
        [SerializeField] private NodeData _nodeData;
        public NodeData nodeData => _nodeData;

        //Child Node Data Structure Definition
        [System.Serializable] public struct ChildNodeStruct {
            public string ChoiceText;
            public Node ChildNode;
            public Vector2 ChildConnectionPoint;

            public ChildNodeStruct(string choiceText = "Choice")
            {
                ChoiceText = choiceText;
                ChildNode = null;
                ChildConnectionPoint = Vector2.zero;
            }
        }

        //Child Nodes
        [HideInInspector] public List<ChildNodeStruct> ChildNodes;

        //constants
        private const float LabelFieldSpace = 70f;
        private const float TextFieldWidth = 100f;

        private const float ChoiceLabelFieldSpace = 20f;
        private const float ChoiceTextFieldWidth = 130f;

        private const float DialogNodeWidth = 210f;

        //DialogBaseNodeHeight = height of Dialog Text + Ext Function + buttons + vertical padding
        private const float DialogBaseNodeHeight = 190f;

        private const float DialogNodeDataHeight = 180f;

        private const float ChoiceNodeHeight = 20f;

        private const float StartNodeWidth = 120f;
        private const float StartNodeHeight = 60f;

        public const string StartNodeSentinel = "!START!";

        private const float collapseButtonHeight = 20f;

        GUIStyle _collapseButtonStyle;

#if UNITY_EDITOR

        /// <summary>
        /// Dialog node initialisation method.  Called when we create a new dialog node
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="nodeName"></param>
        /// <param name="nodeGraph"></param>
        public override void Initialize(Rect rect, string nodeName, DialogNodeGraph nodeGraph)
        {
            Debug.Log("DialogNode:Initialize()...");
            base.Initialize(rect, nodeName, nodeGraph);

            //create the list of child nodes
            ChildNodes = new();

            //initialize the node data
            _nodeData = new NodeData(nodeName);

        }


        /// <summary>
        /// Draw Dialog Node method
        /// </summary>
        /// <param name = "nodeStyle" ></param>
        /// < param name="labelStyle"></param>
        public override void Draw(GUIStyle nodeStyle, GUIStyle labelStyle)
        {
            SetDialogNodeSize();

            base.Draw(nodeStyle, labelStyle);
            GUILayout.BeginArea(Rect, nodeStyle);

            if (IsStartNode())
            {
                EditorGUILayout.LabelField("START", labelStyle);

                if (ChildNodes.Count > 0)
                {
                    //set the first node's connection point to our center (we are the START node).
                    ChildNodeStruct cns = ChildNodes[0];
                    cns.ChildConnectionPoint = Rect.center;
                    ChildNodes[0] = cns;

                    //disable the first node's "Use Prev Vals" function (there are no prev vals)
                    DialogNode childDialogNode = ChildNodes[0].ChildNode as DialogNode;
                    childDialogNode._nodeData.UseCurrentVals = false;
                }
            }
            else
            {
                const int CLIP_LENGTH = 18;
                string labelStr =" " + NodeID.ToString("D4") + " - ";
                if (nodeData.DialogText.Length > CLIP_LENGTH)
                {
                    labelStr += nodeData.DialogText.Substring(0, CLIP_LENGTH) + "...";
                }
                else
                {
                    labelStr += nodeData.DialogText;
                }
                
                EditorGUILayout.LabelField(labelStr, labelStyle);

                DrawNodeData();
                DrawExternalFunctionTextField();

                //now draw the choice buttons
                for (int i = 0; i < ChildNodes.Count; i++)
                {
                    DrawChoiceLine(i, StringConstants.GreenDot);
                }

                DrawDialogNodeButtons();
            }
            
            GUILayout.EndArea();
        }

        /// <summary>
        /// Draws the DialogNode
        /// </summary>
        void DrawNodeData()
        {
            string tooltip;

            //draw avatar sprite
            EditorGUILayout.BeginHorizontal();
            tooltip = "The avatar sprite.";
            EditorGUILayout.LabelField(new GUIContent($"Avatar", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.AvatarImage = (Sprite)EditorGUILayout.ObjectField(_nodeData.AvatarImage,
                typeof(Sprite), false, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw DialogText
            EditorGUILayout.BeginHorizontal();
            tooltip = _nodeData.DialogText;
            EditorGUILayout.LabelField(new GUIContent($"Dialog Txt", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.DialogText = EditorGUILayout.TextField(_nodeData.DialogText, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw use prev values checkbox
            EditorGUILayout.BeginHorizontal();
            tooltip = "If checked, the values from the previous node will be used.";
            const int ExtraSpace = 5;
            EditorGUILayout.LabelField(new GUIContent($"Use Prv Vals", tooltip), GUILayout.Width(LabelFieldSpace + ExtraSpace));
            _nodeData.UseCurrentVals = EditorGUILayout.Toggle(_nodeData.UseCurrentVals, GUILayout.Width(TextFieldWidth - ExtraSpace));
            EditorGUILayout.EndHorizontal();

            //if we are using previous values or the user has collapsed the node, skip drawing the node data fields
            if (_nodeData.UseCurrentVals || _nodeData.Collapsed) return;

            //Font Size
            EditorGUILayout.BeginHorizontal();
            tooltip = "The dialog text font size.  For VR apps (World Space Dialog), start with a value of 0.05 and adjust from there.";
            EditorGUILayout.LabelField(new GUIContent($"Font Size", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.FontSize = EditorGUILayout.FloatField(_nodeData.FontSize, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //Typewriter rate
            EditorGUILayout.BeginHorizontal();
            tooltip = "Dialog text is typed out to the screen with a typewritter effect.  Use thie parameter to set the seconds delay between typed characters.  Set to zero to eliminate the typewriter effect";
            EditorGUILayout.LabelField(new GUIContent($"Type Delay", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.TypeDelay = EditorGUILayout.FloatField(_nodeData.TypeDelay, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw Panel Horz Size Pct %
            EditorGUILayout.BeginHorizontal();
            tooltip = "Horizontal size of the dialog panel as % of screen height (range 0.0 to 1.0).  In VR, this is a % of the base size of 1m (range 0.0 to infinity)";
            EditorGUILayout.LabelField(new GUIContent($"HSize %", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.PanelHorzSizePct = EditorGUILayout.FloatField(_nodeData.PanelHorzSizePct, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw Panel Vert Size Pct %
            EditorGUILayout.BeginHorizontal();
            tooltip = "Vertical size of the dialog panel as % of screen height (range 0.0 to 1.0).  In VR, this is a % of the base size of 1m (range is 0.0 to infinity)";
            EditorGUILayout.LabelField(new GUIContent($"VSize %", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.PanelVertSizePct = EditorGUILayout.FloatField(_nodeData.PanelVertSizePct, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw backgroud color
            EditorGUILayout.BeginHorizontal();
            tooltip = "The background color of the dialog window.";
            EditorGUILayout.LabelField(new GUIContent($"Back Color", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.BackgroundColor = (Color)EditorGUILayout.ColorField(_nodeData.BackgroundColor, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw backgroud sprite
            EditorGUILayout.BeginHorizontal();
            tooltip = "A sprite to be drawn as part of the window background.  Use the background color to change the opacity.";
            EditorGUILayout.LabelField(new GUIContent($"Back Img", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.BackgroundImage = (Sprite)EditorGUILayout.ObjectField(_nodeData.BackgroundImage,
                typeof(Sprite), false, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw AvatarImgToTxtRatio
            EditorGUILayout.BeginHorizontal();
            tooltip = "The horizontal fraction of the dialog panel the avatar image will take up.  The balance of the horizontal space will be allocated to the dialog text.  Set this value to 0 if you are not using an avatar image.";
            EditorGUILayout.LabelField(new GUIContent($"Avtr Ratio", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.AvatarImgToTxtRatio = EditorGUILayout.FloatField(_nodeData.AvatarImgToTxtRatio, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw Avatar to Button Panel Ratio
            EditorGUILayout.BeginHorizontal();
            tooltip = "The vertical fraction of the dialog panel the main text will take up.  The balance of the vertical space will be allocated to the buttons panel.  Range is 0.0 to 1.0.";
            EditorGUILayout.LabelField(new GUIContent($"Panel Ratio", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.AvatarToButtonPanelRatio = EditorGUILayout.FloatField(_nodeData.AvatarToButtonPanelRatio, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw ButtonsAreaWidth %
            EditorGUILayout.BeginHorizontal();
            tooltip = "Button Wdith as % of panel width.  In VR, this is as a % of the base size of 1m.  Range is 0.0 to 1.0.";
            EditorGUILayout.LabelField(new GUIContent($"Btn Width%", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.ButtonsWidthPct = EditorGUILayout.FloatField(_nodeData.ButtonsWidthPct, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw label and text fields for external function, 
        /// depends on IsExternalFunc boolean field
        /// </summary>
        private void DrawExternalFunctionTextField()
        {
            EditorGUILayout.BeginHorizontal();
            string tooltip = "A string token to be passed to callback functions.  This helps you identify which node invoked the callback.  Set to whatever value you wish.";
            EditorGUILayout.LabelField(new GUIContent($"Token", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.ExternalFunctionToken = EditorGUILayout.TextField(nodeData.ExternalFunctionToken,
                GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Returning external function name
        /// </summary>
        /// <returns></returns>
        public string GetExternalFunctionName() => nodeData.ExternalFunctionToken;

        /// <summary>
        /// Draw choice line
        /// </summary>
        /// <param name="choiceNumber"></param>
        /// <param name="iconPathOrName"></param>
        private void DrawChoiceLine(int choiceNumber, string iconPathOrName)
        {
            GUIContent iconContent = EditorGUIUtility.IconContent(iconPathOrName);

            //Debug.Log("DrawChoiceLine -> ChildNode.Count, ChildConnectionPoint.Count = " + ChildNodes.Count + ", " + ChildConnectionPoint.Count);
            ChildNodeStruct cns = ChildNodes[choiceNumber];

            Rect rect = EditorGUILayout.BeginHorizontal();
            //Debug.Log("->" + this.Rect.center.x +"  "+ rect.center.x + "  "+ this.Rect.center.y + "  " + rect.center.y);
            if (rect.center != Vector2.zero && ChildNodes.Count > choiceNumber)
            {
                cns.ChildConnectionPoint = new Vector2(this.Rect.center.x + rect.center.x, this.Rect.y + rect.center.y);
            }
            EditorGUILayout.LabelField($"{choiceNumber+1}. ", GUILayout.Width(ChoiceLabelFieldSpace));

           cns.ChoiceText = EditorGUILayout.TextField(cns.ChoiceText,
                GUILayout.Width(ChoiceTextFieldWidth));

            ChildNodes[choiceNumber] = cns;

            //draw the red dot after the choice line.  (Is this needed?)
            EditorGUILayout.LabelField(iconContent, GUILayout.Width(ChoiceLabelFieldSpace));

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DrawDialogNodeButtons()
        {
            if (_collapseButtonStyle == null)
            {
                _collapseButtonStyle = new GUIStyle();
                _collapseButtonStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (GUILayout.Button("Add choice"))
                AddChoice();

            if (GUILayout.Button("Remove choice"))
                DeleteLastChoice();

            //if the useCurrentVals flag is set, don't even bother drawing the collapse button
            if (_nodeData.UseCurrentVals) return;

            //draw the collapse button; firts, decide if we need the up or down button icon.
            Texture LogoTex = Resources.Load(_nodeData.Collapsed ? "double-down-arrow" : "double-up-arrow") as Texture;
            if (GUILayout.Button(LogoTex, _collapseButtonStyle, GUILayout.Height(collapseButtonHeight)))
            {
                //toggle our collapsed state
                _nodeData.Collapsed = !_nodeData.Collapsed;
            }

        }

        /// <summary>
        /// Increase number of choices and node height.
        /// Creates an unconnected ChildNode reference.
        /// </summary>
        private void AddChoice(string ChoiceText = "_AUTO_")
        {
            if (IsStartNode()) return;

            ChoiceText = ChoiceText == "_AUTO_" ? "Choice " + (ChildNodes.Count + 1) : ChoiceText;
            ChildNodes.Add(new ChildNodeStruct(ChoiceText));
            SetDialogNodeSize();
        }

        /// <summary>
        /// Increase number of choices and node height.
        /// Attaches the provided node to the ChildNodes list at the
        /// first available space.  If no space is available, it
        /// creates a new spot.
        /// </summary>
        private void AddChoice(DialogNode node)
        {
            if (IsStartNode()) return;

            //do not loop back on ourself
            if (node == this) return;

            int availableNodeIndex = -1;

            //[1] Look for the first emply child node
            for (int i = 0; i < ChildNodes.Count; i++)
            {
                if (ChildNodes[i].ChildNode == null)
                {
                    availableNodeIndex = i;
                    break;
                }
            }
            //[2] Add a node if one is not there
            if (availableNodeIndex == -1)    //did not find a node--> add one
            {
                AddChoice("X");
                availableNodeIndex = ChildNodes.Count - 1;
            }

            //[3] connect the node
            ChildNodeStruct cns = ChildNodes[availableNodeIndex];
            cns.ChildNode = node;
            cns.ChoiceText = node.nodeData.DialogText;
            cns.ChoiceText = "Choice " + (availableNodeIndex+1);
            ChildNodes[availableNodeIndex] = cns;
        }


        /// <summary>
        /// Attaches the provided node to the ChildNodes list
        /// <param name="nodeToAdd"></param>
        /// <returns></returns>
        public override bool AddToChildConnectedNode(Node nodeToAdd)
        {
            //the START node cannot be a child node
            if (((DialogNode)nodeToAdd).IsStartNode()) return false;

            //are we the START node?
            if (IsStartNode())
            {
                ChildNodes.Clear();
                //add the one and only child node!
                var cns = new ChildNodeStruct("X");
                cns.ChildNode = nodeToAdd;
                cns.ChildConnectionPoint = Rect.center;
                ChildNodes.Add(cns);
                return true;
            }

            AddChoice((DialogNode)nodeToAdd);
            return true;
        }


        /// <summary>
        /// Decrease number of choices and node height 
        /// </summary>
        private void DeleteLastChoice()
        {
            //if (IsStartNode()) return;

            if (ChildNodes.Count == 0)
                return;

            //remove the last entry
            ChildNodes.RemoveAt(ChildNodes.Count - 1);
            SetDialogNodeSize();
        }


        /// <summary>
        /// Sets dialog node height based on number of choices and the
        /// presence/absence or an external function
        /// </summary>
        public void SetDialogNodeSize()
        {
            if (IsStartNode())
            {
                Rect.size = new Vector2(StartNodeWidth, StartNodeHeight);
            }
            else
            {
                Rect.width = DialogNodeWidth;
                Rect.height = DialogBaseNodeHeight;
                Rect.height += _nodeData.UseCurrentVals || _nodeData.Collapsed ? 0 : DialogNodeDataHeight;
                Rect.height += ChoiceNodeHeight * ChildNodes.Count;
            }
        }

        /// <summary>
        /// Returns true if the current DialogNode is the start node
        /// </summary>
        /// <returns></returns>
        public bool IsStartNode()
        {
            return (_nodeData.DialogText == StartNodeSentinel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public void CopyNodeData(DialogNode source)
        {
            _nodeData = new NodeData(source.nodeData);
        }
#endif
    }
}