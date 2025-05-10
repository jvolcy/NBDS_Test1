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

        private int _numberOfChoices = 1;

        public List<string> Choices = new();
        //public List<string> ChoiceKeys = new();

        //public Node ParentSentenceNode;
        public List<Node> ChildNodes = new();

        private const float LabelFieldSpace = 70f;
        private const float TextFieldWidth = 100f;

        private const float ChoiceLabelFieldSpace = 20f;
        private const float ChoiceTextFieldWidth = 130f;

        private const float DialogNodeWidth = 210f;
        private const float DialogNodeHeight = 360f;

        private const float ExternalNodeHeight = DialogNodeWidth + 80f;

        private float _currentDialogNodeHeight = DialogNodeHeight;
        private const float AdditionalDialogNodeHeight = 20f;

        public string GetChoiceText(int index)
        {
            if (index < 0 || index >= Choices.Count)
                return string.Empty;

            return Choices[index];
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

            //initialize the node data
            _nodeData = new NodeData(nodeName);

            CalculateNumberOfChoices();
            ChildNodes = new List<Node>(_numberOfChoices);
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

            ChildNodes.RemoveAll(item => item == null);



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

            }
            else
            {
                float additionalHeight = DialogNodeGraph.ShowLocalizationKeys ? _numberOfChoices * 20f : 0;
                Rect.size = new Vector2(DialogNodeWidth, _currentDialogNodeHeight + additionalHeight);

                GUILayout.BeginArea(Rect, nodeStyle);
                EditorGUILayout.LabelField(_nodeData.DialogText, labelStyle);

                DrawNodeData();
                DrawExternalFunctionTextField();

                if (GUILayout.Button(_externalButtonLabel))
                    _isExternalFunc = !_isExternalFunc;


                for (int i = 0; i < _numberOfChoices; i++)
                {
                    DrawChoiceLine(i + 1, StringConstants.GreenDot);
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
        public void CalculateNumberOfChoices()
        {
            if (Choices.Count == 0)
            {
                _numberOfChoices = 1;
                Choices = new List<string> { string.Empty };
            }
            else
                _numberOfChoices = Choices.Count;
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
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{choiceNumber}. ", GUILayout.Width(ChoiceLabelFieldSpace));

            Choices[choiceNumber - 1] = EditorGUILayout.TextField(Choices[choiceNumber - 1],
                GUILayout.Width(ChoiceTextFieldWidth));

            if (fallbackTexture == null)
                EditorGUILayout.LabelField(iconContent, GUILayout.Width(ChoiceLabelFieldSpace));
            else
                GUILayout.Label(fallbackTexture, GUILayout.Width(ChoiceLabelFieldSpace), GUILayout.Height(ChoiceLabelFieldSpace));
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawDialogNodeButtons()
        {
            if (GUILayout.Button("Add choice"))
                IncreaseNumberOfChoices();

            if (GUILayout.Button("Remove choice"))
                DecreaseNumberOfChoices();
        }

        /// <summary>
        /// Increase number of choices and node height
        /// </summary>
        private void IncreaseNumberOfChoices()
        {
            _numberOfChoices++;
            Choices.Add(string.Empty);
            _currentDialogNodeHeight += AdditionalDialogNodeHeight;
        }

        /// <summary>
        /// Decrease number of choices and node height 
        /// </summary>
        private void DecreaseNumberOfChoices()
        {
            if (Choices.Count == 1)
                return;

            Choices.RemoveAt(_numberOfChoices - 1);

            if (ChildNodes.Count == _numberOfChoices)
            {
                //ChildNodes[_numberOfChoices - 1].ParentNode = null;
                ChildNodes.RemoveAt(_numberOfChoices - 1);
            }

            _numberOfChoices--;
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
        public override bool AddToChildConnectedNode(Node nodeToAdd)
        {
            //Node sentenceNodeToAdd;

            /* verify we have a sentence node: cast the Node to a
             * SentenceNode. */
            //if (nodeToAdd.GetType() != typeof(AnswerNode))
            //{
                //JV sentenceNodeToAdd = (SentenceNode)nodeToAdd;
                //sentenceNodeToAdd = nodeToAdd;
            //}
            //else
            //    return false;

            /* add as a child node and make the current node its parent. */
            if (IsCanAddToChildConnectedNode(nodeToAdd))
            {
                ChildNodes.Add(nodeToAdd);
                //nodeToAdd.ParentNode = this;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculate dialog node height based on number of choices
        /// </summary>
        public void CalculateDialogNodeHeight()
        {
            _currentDialogNodeHeight = DialogNodeHeight;

            for (int i = 0; i < _numberOfChoices - 1; i++)
                _currentDialogNodeHeight += AdditionalDialogNodeHeight;
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
            //3) the candidate node is not our parent.

            return /*nodeToAdd.ParentNode == null
                   && */ChildNodes.Count < _numberOfChoices
                   && nodeToAdd.ChildNode != this;
        }
#endif
    }
}