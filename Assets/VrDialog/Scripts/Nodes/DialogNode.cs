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
            /*
#if UNITY_LOCALIZATION
            if (index < AnswerKeys.Count && !string.IsNullOrEmpty(AnswerKeys[index]))
            {
                try
                {
                    string tableName = GetTableNameFromNodeGraph();
                    if (string.IsNullOrEmpty(tableName))
                        return Answers[index];
                
                    string localizedValue = LocalizationSettings.StringDatabase.GetLocalizedString(
                        tableName, AnswerKeys[index]);

                    if (!string.IsNullOrEmpty(localizedValue))
                        return localizedValue;
                    else
                        Debug.LogWarning($"Localized answer was empty for key: {AnswerKeys[index]}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"Failed to get localized answer: {ex.Message}");
                }
            }
#endif
            */
            return Choices[index];
        }

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

            CalculateNumberOfChoices();
            //JV ChildSentenceNodes = new List<SentenceNode>(_amountOfAnswers);
            ChildNodes = new List<Node>(_numberOfChoices);
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

            float additionalHeight = DialogNodeGraph.ShowLocalizationKeys ? _numberOfChoices * 20f : 0;
            Rect.size = new Vector2(DialogNodeWidth, _currentDialogNodeHeight + additionalHeight);

            GUILayout.BeginArea(Rect, nodeStyle);
            EditorGUILayout.LabelField("Dialog Node", labelStyle);


            //------------------------------------------------
            /*
            if (DialogNodeGraph.ShowLocalizationKeys)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Localization Keys", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name Key", GUILayout.Width(LabelFieldSpace));
                CharacterNameKey = EditorGUILayout.TextField(CharacterNameKey, GUILayout.Width(TextFieldWidth));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Text Key", GUILayout.Width(LabelFieldSpace));
                SentenceTextKey = EditorGUILayout.TextField(SentenceTextKey, GUILayout.Width(TextFieldWidth));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                DrawCharacterNameFieldHorizontal();
                DrawSentenceTextFieldHorizontal();
                DrawCharacterSpriteHorizontal();
                DrawButtonTextFieldHorizontal();        //JV
                DrawExternalFunctionTextField();


            }
            */

            DrawNodeData();
            DrawExternalFunctionTextField();

            if (GUILayout.Button(_externalButtonLabel))
                _isExternalFunc = !_isExternalFunc;


            //------------------------------------------------


            for (int i = 0; i < _numberOfChoices; i++)
            {
                DrawChoiceLine(i + 1, StringConstants.GreenDot);

                /*
                if (DialogNodeGraph.ShowLocalizationKeys)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Key: ", GUILayout.Width(25));
                    
                    while (ChoiceKeys.Count <= i)
                        ChoiceKeys.Add(string.Empty);
                
                    ChoiceKeys[i] = EditorGUILayout.TextField(ChoiceKeys[i], GUILayout.Width(TextFieldWidth + 13));
                    EditorGUILayout.EndHorizontal();
                }
                */
            }

            DrawDialogNodeButtons();

            GUILayout.EndArea();
        }

        /// <summary>
        /// Draws the DialogNode
        /// </summary>
        /*
        public string DialogText;
        public Sprite BackgroundImage;
        bool ShowDismissButton;
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
            //draw DialogText
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Text ", GUILayout.Width(LabelFieldSpace));
            _nodeData.DialogText = EditorGUILayout.TextField(_nodeData.DialogText, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();           

            //draw backgroud sprite
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"BackImg ", GUILayout.Width(LabelFieldSpace));
            _nodeData.BackgroundImage = (Sprite)EditorGUILayout.ObjectField(_nodeData.BackgroundImage,
                typeof(Sprite), false, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw dismiss btn checkbox
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"ShowXBtn ", GUILayout.Width(LabelFieldSpace));
            _nodeData.ShowDismissButton = EditorGUILayout.Toggle(_nodeData.ShowDismissButton, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw TextAreaWidthPct
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"TxtWidth% ", GUILayout.Width(LabelFieldSpace));
            _nodeData.TextAreaWidthPct = EditorGUILayout.FloatField(_nodeData.TextAreaWidthPct, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw TextAreaHeightPct
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"TxtHeight% ", GUILayout.Width(LabelFieldSpace));
            _nodeData.TextAreaHeightPct = EditorGUILayout.FloatField(_nodeData.TextAreaHeightPct, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw TextAreaYPosPct
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"TxtYPos% ", GUILayout.Width(LabelFieldSpace));
            _nodeData.TextAreaYPos = EditorGUILayout.FloatField(_nodeData.TextAreaYPos, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw ButtonsAreaWidth %
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"BtnWidth% ", GUILayout.Width(LabelFieldSpace));
            _nodeData.ButtonsAreaWidthPct = EditorGUILayout.FloatField(_nodeData.ButtonsAreaWidthPct, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw ButtonsAreaHeight %
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"BtnHeight% ", GUILayout.Width(LabelFieldSpace));
            _nodeData.ButtonAreaHeightPct = EditorGUILayout.FloatField(_nodeData.ButtonAreaHeightPct, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw ButtonsAreaWidth YPos %
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"BtnYPos% ", GUILayout.Width(LabelFieldSpace));
            _nodeData.ButtonAreaYPos = EditorGUILayout.FloatField(_nodeData.ButtonAreaYPos, GUILayout.Width(TextFieldWidth));
            EditorGUILayout.EndHorizontal();

            //draw button sprite
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"BtnImg ", GUILayout.Width(LabelFieldSpace));
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
                ChildNodes[_numberOfChoices - 1].ParentNode = null;
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
                ParentNode = nodeToAdd;
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
                nodeToAdd.ParentNode = this;

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

            return nodeToAdd.ParentNode == null
                   && ChildNodes.Count < _numberOfChoices
                   && nodeToAdd.ChildNode != this;
        }
#endif
    }
}