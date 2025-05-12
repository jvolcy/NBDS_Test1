using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/*
#if UNITY_LOCALIZATION
using UnityEngine.Localization.Settings;
#endif
*/
namespace cherrydev
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Nodes/Dialog Node", fileName = "New Dialog Node")]
    public class DialogNode : Node
    {
        [Space(7)] [SerializeField] private bool _isExternalFunc;
        [SerializeField] private string _externalFunctionName;

        private string _externalButtonLabel;

        [SerializeField] private NodeData _nodeData;
        public NodeData nodeData => _nodeData;
        
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
        
        [SerializeField] public List<ChildNodeStruct> ChildNodes;
        
        private const float LabelFieldSpace = 70f;
        private const float TextFieldWidth = 100f;

        private const float ChoiceLabelFieldSpace = 20f;
        private const float ChoiceTextFieldWidth = 130f;

        private const float DialogNodeWidth = 210f;
        private const float DialogBaseNodeHeight = 340f;

        private const float ExternalNodeHeight = 20f;

        private const float ChoiceNodeHeight = 20f;

        private const float StartNodeWidth = 120f;
        private const float StartNodeHeight = 60f;

        public string GetChoiceText(int index)
        {
            if (index < 0 || index >= ChildNodes.Count)
                return string.Empty;

            return ChildNodes[index].ChoiceText;
        }

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

        
        public void CreateStartNode()
        {
            _nodeData.DialogText = StartNodeSentinel;
        }


        /// <summary>
        /// Draw Dialog Node method
        /// </summary>
        /// <param name = "nodeStyle" ></param>
        /// < param name="labelStyle"></param>
        public override void Draw(GUIStyle nodeStyle, GUIStyle labelStyle)
        {
            base.Draw(nodeStyle, labelStyle);


            if (IsStartNode())
            {
                //modify the labelStyle to have yellow and centered text
                GUIStyle startLabelStyle = new GUIStyle(labelStyle);
                startLabelStyle.alignment = TextAnchor.MiddleCenter;
                startLabelStyle.normal.textColor = Color.yellow;
                startLabelStyle.normal.background = null;

                GUILayout.BeginArea(Rect, nodeStyle);
                EditorGUILayout.LabelField("START", startLabelStyle);

                if (ChildNodes.Count > 0)
                {
                    ChildNodeStruct cns = ChildNodes[0];
                    cns.ChildConnectionPoint = Rect.center;
                    ChildNodes[0] = cns;
                }

            }
            else
            {
                float additionalHeight = DialogNodeGraph.ShowLocalizationKeys ? ChildNodes.Count * 20f : 0;

                GUILayout.BeginArea(Rect, nodeStyle);
                EditorGUILayout.LabelField(_nodeData.DialogText, labelStyle);

                DrawNodeData();
                DrawExternalFunctionTextField();

                if (GUILayout.Button(_externalButtonLabel))
                {
                    _isExternalFunc = !_isExternalFunc;
                    SetDialogNodeSize();
                }

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
        public string DialogText;
        public Sprite BackgroundImage;
        bool AutoDismissButton;
        float TextAreaWidthPct;
        float TextAreaHeightPct;
        float TextAreaYPos;
        float ButtonsAreaWidthPct;
        float ButtonAreaHeightPct;
        float ButtonAreaYPos;
        public Sprite ButtonImage;
        */
        void DrawNodeData()
        {
            string tooltip;

            //draw DialogText
            EditorGUILayout.BeginHorizontal();
            tooltip = "XXX";
            EditorGUILayout.LabelField(new GUIContent($"Text ", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.DialogText = EditorGUILayout.TextField(_nodeData.DialogText, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();           

            //draw backgroud sprite
            EditorGUILayout.BeginHorizontal();
            tooltip = "XXX";
            EditorGUILayout.LabelField(new GUIContent($"BackImg ", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.BackgroundImage = (Sprite)EditorGUILayout.ObjectField(_nodeData.BackgroundImage,
                typeof(Sprite), false, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw dismiss btn checkbox
            EditorGUILayout.BeginHorizontal();
            tooltip = "The dismiss button is automatically added when there are no buttons in the dialog area.  Uncheck this box to disable this behavior.";
            EditorGUILayout.LabelField(new GUIContent($"AutoXBtn ", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.AutoDismissButton = EditorGUILayout.Toggle(_nodeData.AutoDismissButton, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw TextAreaWidthPct
            EditorGUILayout.BeginHorizontal();
            tooltip = "XXX";
            EditorGUILayout.LabelField(new GUIContent($"TxtWidth% ", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.TextAreaWidthPct = EditorGUILayout.FloatField(_nodeData.TextAreaWidthPct, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw TextAreaHeightPct
            EditorGUILayout.BeginHorizontal();
            tooltip = "XXX";
            EditorGUILayout.LabelField(new GUIContent($"TxtHeight% ", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.TextAreaHeightPct = EditorGUILayout.FloatField(_nodeData.TextAreaHeightPct, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw TextAreaYPosPct
            EditorGUILayout.BeginHorizontal();
            tooltip = "XXX";
            EditorGUILayout.LabelField(new GUIContent($"TxtYPos% ", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.TextAreaYPos = EditorGUILayout.FloatField(_nodeData.TextAreaYPos, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw ButtonsAreaWidth %
            EditorGUILayout.BeginHorizontal();
            tooltip = "XXX";
            EditorGUILayout.LabelField(new GUIContent($"BtnWidth% ", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.ButtonsAreaWidthPct = EditorGUILayout.FloatField(_nodeData.ButtonsAreaWidthPct, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw ButtonsAreaHeight %
            EditorGUILayout.BeginHorizontal();
            tooltip = "XXX";
            EditorGUILayout.LabelField(new GUIContent($"BtnHeight% ", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.ButtonAreaHeightPct = EditorGUILayout.FloatField(_nodeData.ButtonAreaHeightPct, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw ButtonsAreaWidth YPos %
            EditorGUILayout.BeginHorizontal();
            tooltip = "XXX";
            EditorGUILayout.LabelField(new GUIContent($"BtnYPos% ", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.ButtonAreaYPos = EditorGUILayout.FloatField(_nodeData.ButtonAreaYPos, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw button sprite
            EditorGUILayout.BeginHorizontal();
            tooltip = "XXX";
            EditorGUILayout.LabelField(new GUIContent($"BtnImg ", tooltip), GUILayout.Width(LabelFieldSpace));
            _nodeData.ButtonImage = (Sprite)EditorGUILayout.ObjectField(_nodeData.ButtonImage,
                typeof(Sprite), false, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw label and text fields for external function, 
        /// depends on IsExternalFunc boolean field
        /// </summary>
        private void DrawExternalFunctionTextField()
        {
            if (_isExternalFunc)
            {
                _externalButtonLabel = "Remove external func";

                EditorGUILayout.BeginHorizontal();
                //Rect.height = ExternalNodeHeight;
                EditorGUILayout.LabelField($"Func Name ", GUILayout.Width(LabelFieldSpace));
                _externalFunctionName = EditorGUILayout.TextField(_externalFunctionName,
                    GUILayout.Width(TextFieldWidth));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                _externalButtonLabel = "Add external func";
                //Rect.height = StandardHeight;
            }
        }


        /// <summary>
        /// Draw choice line
        /// </summary>
        /// <param name="choiceNumber"></param>
        /// <param name="iconPathOrName"></param>
        private void DrawChoiceLine(int choiceNumber, string iconPathOrName)
        {
            GUIContent iconContent = EditorGUIUtility.IconContent(iconPathOrName);
            Texture2D fallbackTexture = Resources.Load<Texture2D>("Dot");

            //Debug.Log("DrawChoiceLine -> ChildNode.Count, ChildConnectionPoint.Count = " + ChildNodes.Count + ", " + ChildConnectionPoint.Count);

            Rect rect = EditorGUILayout.BeginHorizontal();
            //Debug.Log("->" + this.Rect.center.x +"  "+ rect.center.x + "  "+ this.Rect.center.y + "  " + rect.center.y);
            if (rect.center != Vector2.zero && ChildNodes.Count > choiceNumber)
            {
                ChildNodeStruct cns = ChildNodes[choiceNumber];
                cns.ChildConnectionPoint = new Vector2(this.Rect.center.x + rect.center.x, this.Rect.y + rect.center.y);
                ChildNodes[choiceNumber] = cns;

                //ChildConnectionPoint.Add(new Vector2(this.Rect.center.x + rect.center.x, this.Rect.center.y + rect.center.y));
                //ChildConnectionPoint.Add(rect.center);
            }
            EditorGUILayout.LabelField($"{choiceNumber+1}. ", GUILayout.Width(ChoiceLabelFieldSpace));

            EditorGUILayout.TextField(ChildNodes[choiceNumber].ChoiceText,
                GUILayout.Width(ChoiceTextFieldWidth));

            if (fallbackTexture == null)
            {
                //Debug.Log("fallbackTexture == null");
                EditorGUILayout.LabelField(iconContent, GUILayout.Width(ChoiceLabelFieldSpace));
            }
            else
            {
                GUILayout.Label(fallbackTexture, GUILayout.Width(ChoiceLabelFieldSpace), GUILayout.Height(ChoiceLabelFieldSpace));
            }

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
        private void AddChoice(string ChoiceText = "Choice")
        {
            if (IsStartNode()) return;

            ChildNodes.Add(new ChildNodeStruct(ChoiceText));
            Rect.height += ChoiceNodeHeight;
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

            ChildNodes.RemoveAt(ChildNodes.Count - 1);

            Rect.height -= ChoiceNodeHeight;
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

                for (int i = 0; i < ChildNodes.Count - 1; i++)
                    Rect.height += ChoiceNodeHeight;

                if (_isExternalFunc)
                    Rect.height += ExternalNodeHeight;
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