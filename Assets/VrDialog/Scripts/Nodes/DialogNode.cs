using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_LOCALIZATION
using UnityEngine.Localization.Settings;
#endif

namespace cherrydev
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Nodes/Dialog Node", fileName = "New Dialog Node")]
    public class DialogNode : Node
    {
        [SerializeField] private NodeData _nodeData;

        private int _numberOfChoices = 1;

        public List<string> Choices = new();
        public List<string> ChoiceKeys = new();

        //public Node ParentSentenceNode;
        public List<Node> ChildNodes = new();

        private const float LabelFieldSpace = 18f;
        private const float TextFieldWidth = 120f;

        private const float DialogNodeWidth = 190f;
        private const float DialogNodeHeight = 115f;

        private float _currentDialogNodeHeight = 115f;
        private const float AdditionalDialogNodeHeight = 20f;

        public string GetChoiceText(int index)
        {
            if (index < 0 || index >= Choices.Count)
                return string.Empty;

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

            for (int i = 0; i < _numberOfChoices; i++)
            {
                DrawChoiceLine(i + 1, StringConstants.GreenDot);
        
                if (DialogNodeGraph.ShowLocalizationKeys)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Key: ", GUILayout.Width(25));
                    
                    while (ChoiceKeys.Count <= i)
                        ChoiceKeys.Add(string.Empty);
                
                    ChoiceKeys[i] = EditorGUILayout.TextField(ChoiceKeys[i], GUILayout.Width(TextFieldWidth + 13));
                    EditorGUILayout.EndHorizontal();
                }
            }

            DrawDialogNodeButtons();

            GUILayout.EndArea();
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
            EditorGUILayout.LabelField($"{choiceNumber}. ", GUILayout.Width(LabelFieldSpace));

            Choices[choiceNumber - 1] = EditorGUILayout.TextField(Choices[choiceNumber - 1],
                GUILayout.Width(TextFieldWidth));

            if (fallbackTexture == null)
                EditorGUILayout.LabelField(iconContent, GUILayout.Width(LabelFieldSpace));
            else
                GUILayout.Label(fallbackTexture, GUILayout.Width(LabelFieldSpace), GUILayout.Height(LabelFieldSpace));
            
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