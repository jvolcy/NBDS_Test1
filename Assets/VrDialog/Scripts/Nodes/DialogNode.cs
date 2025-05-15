using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace cherrydev
{
    //[CreateAssetMenu(menuName = "Scriptable Objects/Nodes/Dialog Node", fileName = "New Dialog Node")]
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

        //External Function
        [Space(7)] //[SerializeField] private bool _invokeExternalFunc;
        [SerializeField] private string _externalFunctionName;
        //private string _externalButtonLabel;

        //constants
        private const float LabelFieldSpace = 70f;
        private const float TextFieldWidth = 100f;

        private const float ChoiceLabelFieldSpace = 20f;
        private const float ChoiceTextFieldWidth = 130f;

        private const float DialogNodeWidth = 210f;
        private const float DialogBaseNodeHeight = 245f;

        //private const float ExternalNodeHeight = 20f;

        private const float ChoiceNodeHeight = 20f;

        private const float StartNodeWidth = 120f;
        private const float StartNodeHeight = 60f;

        public const string StartNodeSentinel = "!START!";

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

            SetDialogNodeSize();
        }


        /// <summary>
        /// Draw Dialog Node method
        /// </summary>
        /// <param name = "nodeStyle" ></param>
        /// < param name="labelStyle"></param>
        public override void Draw(GUIStyle nodeStyle, GUIStyle labelStyle)
        {
            base.Draw(nodeStyle, labelStyle);
            GUILayout.BeginArea(Rect, nodeStyle);


            if (IsStartNode())
            {

                EditorGUILayout.LabelField("START", labelStyle);

                if (ChildNodes.Count > 0)
                {
                    ChildNodeStruct cns = ChildNodes[0];
                    cns.ChildConnectionPoint = Rect.center;
                    ChildNodes[0] = cns;
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
                /*
                if (GUILayout.Button(_externalButtonLabel))
                {
                    _invokeExternalFunc = !_invokeExternalFunc;
                    SetDialogNodeSize();
                }*/

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
        /*
            DialogText = dialogText;
            TextPanelWidthPct = 0.8f;
            ButtonImage = null;
            ButtonsPanelWidthPct = 0.8f;
            PanelRatio = 0.5f;
            BackgroundImage = null;
        */
        void DrawNodeData()
        {
            string tooltip;

            //draw DialogText
            EditorGUILayout.BeginHorizontal();
            tooltip = "XXX";
            EditorGUILayout.LabelField(new GUIContent($"Dialog Txt", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.DialogText = EditorGUILayout.TextField(_nodeData.DialogText, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();           

            //draw TextAreaWidthPct
            EditorGUILayout.BeginHorizontal();
            tooltip = "XXX";
            EditorGUILayout.LabelField(new GUIContent($"Txt Width%", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.TextPanelWidthPct = EditorGUILayout.FloatField(_nodeData.TextPanelWidthPct, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw button sprite
            EditorGUILayout.BeginHorizontal();
            tooltip = "XXX";
            EditorGUILayout.LabelField(new GUIContent($"Btn Img", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.ButtonImage = (Sprite)EditorGUILayout.ObjectField(_nodeData.ButtonImage,
                typeof(Sprite), false, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw ButtonsAreaWidth %
            EditorGUILayout.BeginHorizontal();
            tooltip = "XXX";
            EditorGUILayout.LabelField(new GUIContent($"Btn Width%", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.ButtonsPanelWidthPct = EditorGUILayout.FloatField(_nodeData.ButtonsPanelWidthPct, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw Panel Ratio
            EditorGUILayout.BeginHorizontal();
            tooltip = "Ratio of the Text Panel Height to the Buttons Panel Height.  Range is 0 to 1.";
            EditorGUILayout.LabelField(new GUIContent($"Panel Ratio", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.PanelRatio = EditorGUILayout.FloatField(_nodeData.PanelRatio, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw backgroud sprite
            EditorGUILayout.BeginHorizontal();
            tooltip = "XXX";
            EditorGUILayout.LabelField(new GUIContent($"Back Img", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.BackgroundImage = (Sprite)EditorGUILayout.ObjectField(_nodeData.BackgroundImage,
                typeof(Sprite), false, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Returning external function name
        /// </summary>
        /// <returns></returns>
        public string GetExternalFunctionName() => _externalFunctionName;


        /// <summary>
        /// Draw label and text fields for external function, 
        /// depends on IsExternalFunc boolean field
        /// </summary>
        private void DrawExternalFunctionTextField()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Ext Func", GUILayout.Width(LabelFieldSpace));
            _externalFunctionName = EditorGUILayout.TextField(_externalFunctionName,
                GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();
        }


        /// <summary>
        /// Draw choice line
        /// </summary>
        /// <param name="choiceNumber"></param>
        /// <param name="iconPathOrName"></param>
        private void DrawChoiceLine(int choiceNumber, string iconPathOrName)
        {
            GUIContent iconContent = EditorGUIUtility.IconContent(iconPathOrName);
            //Texture2D fallbackTexture = Resources.Load<Texture2D>("Dot");

            //Debug.Log("DrawChoiceLine -> ChildNode.Count, ChildConnectionPoint.Count = " + ChildNodes.Count + ", " + ChildConnectionPoint.Count);
            ChildNodeStruct cns = ChildNodes[choiceNumber];

            Rect rect = EditorGUILayout.BeginHorizontal();
            //Debug.Log("->" + this.Rect.center.x +"  "+ rect.center.x + "  "+ this.Rect.center.y + "  " + rect.center.y);
            if (rect.center != Vector2.zero && ChildNodes.Count > choiceNumber)
            {
                cns.ChildConnectionPoint = new Vector2(this.Rect.center.x + rect.center.x, this.Rect.y + rect.center.y);

                //ChildConnectionPoint.Add(new Vector2(this.Rect.center.x + rect.center.x, this.Rect.center.y + rect.center.y));
                //ChildConnectionPoint.Add(rect.center);
            }
            EditorGUILayout.LabelField($"{choiceNumber+1}. ", GUILayout.Width(ChoiceLabelFieldSpace));

           cns.ChoiceText = EditorGUILayout.TextField(cns.ChoiceText,
                GUILayout.Width(ChoiceTextFieldWidth));

            ChildNodes[choiceNumber] = cns;


            //draw the red dot after the choice line.  (Is this needed?)
            EditorGUILayout.LabelField(iconContent, GUILayout.Width(ChoiceLabelFieldSpace));

            /*
            if (fallbackTexture == null)
            {
                //Debug.Log("fallbackTexture == null");
                EditorGUILayout.LabelField(iconContent, GUILayout.Width(ChoiceLabelFieldSpace));
            }
            else
            {
                GUILayout.Label(fallbackTexture, GUILayout.Width(ChoiceLabelFieldSpace), GUILayout.Height(ChoiceLabelFieldSpace));
            }
            */

            EditorGUILayout.EndHorizontal();
        }

        private void DrawDialogNodeButtons()
        {
            if (GUILayout.Button("Add choice"))
                AddChoice();

            if (GUILayout.Button("Remove choice"))
                DeleteLastChoice();
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
#endif
    }
}