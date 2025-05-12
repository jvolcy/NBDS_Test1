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

        //private int ChildNodes.Count = 1;

        //to do:  Change "Choice" to "Text"
        
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
        

        //public List<string> Choices = new();
        //public List<string> ChoiceKeys = new();

        //public Node ParentSentenceNode;
        //public List<Node> OldChildNodes = new();
        //public List<Vector2> ChildConnectionPoint = new();

        private const float LabelFieldSpace = 70f;
        private const float TextFieldWidth = 100f;

        private const float ChoiceLabelFieldSpace = 20f;
        private const float ChoiceTextFieldWidth = 130f;

        private const float DialogNodeWidth = 210f;
        private const float DialogNodeHeight = 325f;

        private const float ExternalNodeHeight = DialogNodeWidth + 80f;

        private float _currentDialogNodeHeight = DialogNodeHeight;
        private const float AdditionalDialogNodeHeight = 20f;


        public string GetChoiceText(int index)
        {
            if (index < 0 || index >= ChildNodes.Count)
                return string.Empty;

            return ChildNodes[index].ChoiceText;
        }

        public const string StartNodeSentinel = "!START!";

#if UNITY_EDITOR

        /// <summary>
        /// Dialog node initialisation method
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="nodeName"></param>
        /// <param name="nodeGraph"></param>
        public override void Initialize(Rect rect, string nodeName, DialogNodeGraph nodeGraph)
        {
            base.Initialize(rect, nodeName, nodeGraph);

            ChildNodes = new();

            //initialize the node data
            _nodeData = new NodeData(nodeName);

            //CalculateNumberOfChoices();
            //OldChildNodes = new List<Node>(ChildNodes.Count);
            //ChildConnectionPoint = new List<Vector2>(ChildNodes.Count);

            //Debug.Log("Initialize() -> ChildNode.Count, ChildConnectionPoint.Count = " + ChildNodes.Count + ", " + ChildConnectionPoint.Count);

        }
        /*
        public void CreateStartNode()
        {
            _nodeData.DialogText = StartNodeSentinel;
        }
        */

        /// <summary>
        /// Draw Dialog Node method
        /// </summary>
        /// <param name = "nodeStyle" ></param>
        /// < param name="labelStyle"></param>
        public override void Draw(GUIStyle nodeStyle, GUIStyle labelStyle)
        {
            base.Draw(nodeStyle, labelStyle);

            //ChildNodes.RemoveAll(item => item == null);

            /*
            for (int i = 0; i<ChildNodes.Count; i++)
            {
                if (OldChildNodes[i] == null)
                {
                    Debug.Log("Draw(1) -> ChildNode.Count, ChildConnectionPoint.Count = " + ChildNodes.Count + ", " + ChildConnectionPoint.Count);
                    OldChildNodes.RemoveAt(i);
                    ChildConnectionPoint.RemoveAt(i);
                    Debug.Log("Draw(2) -> ChildNode.Count, ChildConnectionPoint.Count = " + ChildNodes.Count + ", " + ChildConnectionPoint.Count);
                }

            }
            */

            if (_nodeData.DialogText == StartNodeSentinel)
            {
                //modify the labelStyle to have yellow and centered text
                GUIStyle startLabelStyle = new GUIStyle(labelStyle);
                startLabelStyle.alignment = TextAnchor.MiddleCenter;
                startLabelStyle.normal.textColor = Color.yellow;
                startLabelStyle.normal.background = null;

                Rect.size = new Vector2(/*DialogNodeWidth*/ 120f, 60f);
                GUILayout.BeginArea(Rect, nodeStyle);
                EditorGUILayout.LabelField("START", startLabelStyle);

                //Rect rect = EditorGUILayout.BeginHorizontal();
                //ChildConnectionPoint.Add(new Vector2(rect.center.x + rect.xMax, rect.center.y));
                //EditorGUILayout.EndHorizontal();
            }
            else
            {
                float additionalHeight = DialogNodeGraph.ShowLocalizationKeys ? ChildNodes.Count * 20f : 0;
                Rect.size = new Vector2(DialogNodeWidth, _currentDialogNodeHeight + additionalHeight);

                GUILayout.BeginArea(Rect, nodeStyle);
                EditorGUILayout.LabelField(_nodeData.DialogText, labelStyle);

                DrawNodeData();
                DrawExternalFunctionTextField();

                if (GUILayout.Button(_externalButtonLabel))
                    _isExternalFunc = !_isExternalFunc;


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
                Rect.height = ExternalNodeHeight;
                EditorGUILayout.LabelField($"Func Name ", GUILayout.Width(LabelFieldSpace));
                _externalFunctionName = EditorGUILayout.TextField(_externalFunctionName,
                    GUILayout.Width(TextFieldWidth));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                _externalButtonLabel = "Add external func";
                Rect.height = StandardHeight;
            }
        }

        /// <summary>
        /// Determines the number of choices depending on choices list count
        /// </summary>
        /*
        public void CalculateNumberOfChoices()
        {
            if (ChildNodes.Count == 0)
            {
                ChildNodes.Count = 1;
                Choices = new List<string> { string.Empty };
            }
            else
                ChildNodes.Count = ChildNodes.Count;
        }
        */

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
        /// Increase number of choices and node height
        /// </summary>
        private void AddChoice(string ChoiceText = "Choice")
        {
            ChildNodes.Add(new ChildNodeStruct(ChoiceText));

            //ChildNodes.Count++;
            //Choices.Add(string.Empty);
            _currentDialogNodeHeight += AdditionalDialogNodeHeight;

        }

        /// <summary>
        /// Decrease number of choices and node height 
        /// </summary>
        private void DeleteLastChoice()
        {
            if (ChildNodes.Count == 0)
                return;

            ChildNodes.RemoveAt(ChildNodes.Count - 1);

            _currentDialogNodeHeight -= AdditionalDialogNodeHeight;
        }

        /// <summary>
        /// Adding nodeToAdd Node to the parentSentenceNode field
        /// </summary>
        /// <param name="nodeToAdd"></param>
        /// <returns></returns>
        public override bool AddToParentConnectedNode(Node nodeToAdd)
        {
            //if (nodeToAdd.GetType() == typeof(SentenceNode))
            //{
                //JV ParentSentenceNode = (SentenceNode)nodeToAdd;
                //ParentNode = nodeToAdd;
                return true;
            //}

            //return false;
        }

        /// <summary>
        /// Adding nodeToAdd Node to the childSentenceNodes array
        /// </summary>
        /// <param name="nodeToAdd"></param>
        /// <returns></returns>
        /*
        public override bool AddToChildConnectedNode(Node nodeToAdd)
        {
            /x* add as a child node and make the current node its parent. *x/
            if (IsCanAddToChildConnectedNode(nodeToAdd))
            {
                OldChildNodes.Add(nodeToAdd);
                ChildConnectionPoint.Add(Vector2.zero);

                Debug.Log("AddToChildConnectedNode() -> ChildNode.Count, ChildConnectionPoint.Count = " + ChildNodes.Count + ", " + ChildConnectionPoint.Count);
                return true;
            }

            return false;
        }
*/
        /// <summary>
        /// Calculate dialog node height based on number of choices
        /// </summary>
        public void CalculateDialogNodeHeight()
        {
            _currentDialogNodeHeight = DialogNodeHeight;

            for (int i = 0; i < ChildNodes.Count - 1; i++)
                _currentDialogNodeHeight += AdditionalDialogNodeHeight;

            if (_isExternalFunc)
                _currentDialogNodeHeight += 40;
        }

        /// <summary>
        /// Checks if sentence node can be added as child of dialog node
        /// </summary>
        /// <param name="sentenceNodeToAdd"></param>
        /// <returns></returns>
        private bool IsCanAddToChildConnectedNode(Node nodeToAdd)
        {
            //check that 1) the candidate node has no current parent;
            //2) we have space in our choices list for this node;
            //3) the candidate node is not ourself.

            return /*nodeToAdd.ParentNode == null
                   && ChildNodes.Count < ChildNodes.Count
                   && */ nodeToAdd != this;
        }
#endif
    }
}