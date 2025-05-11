using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_LOCALIZATION
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
#endif

namespace cherrydev
{
    public class DialogBehaviour : MonoBehaviour
    {
        [SerializeField] private Node StartNode;
        [SerializeField] private float _dialogCharDelay;
        [SerializeField] private List<KeyCode> _nextSentenceKeyCodes;
        [SerializeField] private bool _isCanSkippingText = true;
#if UNITY_LOCALIZATION
        [SerializeField] private bool _reloadTextOnLanguageChange = true;
#endif

        //[SerializeField] DialogNodeGraph dialogNodeGraph;

        [Space(10)]
        [SerializeField] private UnityEvent _onDialogStarted;
        [SerializeField] private UnityEvent _onDialogFinished;

        //boolean used to force us to the next node.  This value is set to true by the UI sentence button.
        bool _GoToNextNode = false;   //JV

        private DialogNodeGraph _currentNodeGraph;
        private Node _currentNode;
        
        public DialogNode CurrentAnswerNode { get; private set; }
        //public SentenceNode CurrentSentenceNode { get; private set; }
        
#if UNITY_LOCALIZATION
        public event Action LanguageChanged;
#endif
        
        private int _maxNumberOfChoiceButtons;

        private bool _isDialogStarted;
        private bool _isCurrentSentenceSkipped;
        private bool _isCurrentSentenceTyping;

        public bool IsCanSkippingText
        {
            get => _isCanSkippingText;
            set => _isCanSkippingText = value;
        }

        public event Action SentenceStarted;
        public event Action SentenceEnded;
        public event Action SentenceNodeActivated;
        public event Action<string, string, Sprite, string> SentenceNodeActivatedWithParameter;
//        public event Action<string, string, Sprite> SentenceNodeActivatedWithParameter;
        public event Action AnswerNodeActivated;
        public event Action<int, DialogNode> AnswerButtonSetUp;
        public event Action<int> MaxNumberOfChoiceButtonsCalculated;
        public event Action<int> AnswerNodeActivatedWithParameter;
        public event Action<int, string> AnswerNodeSetUp;
        public event Action DialogTextCharWrote;
        public event Action<string> DialogTextSkipped;

        public DialogExternalFunctionsHandler ExternalFunctionsHandler { get; private set; }

        private void Awake() => ExternalFunctionsHandler = new DialogExternalFunctionsHandler();

        private void OnEnable()
        {
#if UNITY_LOCALIZATION
            if (_reloadTextOnLanguageChange)
                LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
#endif
        }

#if UNITY_LOCALIZATION
        private void OnSelectedLocaleChanged(Locale obj)
        {
            if (_isDialogStarted && _currentNode != null)
            {
                LanguageChanged?.Invoke();

                if (_currentNode is SentenceNode sentenceNode)
                {
                    string updatedText = sentenceNode.GetText();
                    string updatedCharName = sentenceNode.GetCharacterName();

                    SentenceNodeActivatedWithParameter?.Invoke(updatedCharName, updatedText,
                        sentenceNode.GetCharacterSprite());

                    if (_isCurrentSentenceTyping)
                    {
                        StopAllCoroutines();
                        WriteDialogText(updatedText);
                    }
                    else
                        DialogTextSkipped?.Invoke(updatedText);
                }
                else if (_currentNode is AnswerNode)
                    HandleAnswerNode(_currentNode);
            }
        }
#endif

        private void OnDestroy()
        {
#if UNITY_LOCALIZATION
            if (_reloadTextOnLanguageChange)
                LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
#endif
        }
        
        private void Update() => HandleSentenceSkipping();

        /// <summary>
        /// Setting dialogCharDelay float parameter
        /// </summary>
        /// <param name="value"></param>
        public void SetCharDelay(float value) => _dialogCharDelay = value;

        /// <summary>
        /// Setting nextSentenceKeyCodes
        /// </summary>
        /// <param name="keyCodes"></param>
        public void SetNextSentenceKeyCodes(List<KeyCode> keyCodes) => _nextSentenceKeyCodes = keyCodes;

        /*
        public void Play(DialogNodeGraph dialogNodeGraph)
        {
            StartDialog(dialogNodeGraph);
        }
        */

        /// <summary>
        /// Start a dialog
        /// </summary>
        /// <param name="dialogNodeGraph"></param>
        public void StartDialog(DialogNodeGraph dialogNodeGraph)
        {
            _isDialogStarted = true;
            //_GoToNextNode = false;

            if (dialogNodeGraph.NodesList == null)
            {
                Debug.LogWarning("Dialog Graph's node list is empty");
                return;
            }

            _onDialogStarted?.Invoke();
            _currentNodeGraph = dialogNodeGraph;

            DefineFirstNode(dialogNodeGraph);
            CalculateMaxNumberOfChoiceButtons();
            HandleDialogGraphCurrentNode(_currentNode);
        }

        public void GoToNextNode()
        {
            _GoToNextNode = true;
        }

        /// <summary>
        /// This method is designed for ease of use. Calls a method 
        /// BindExternalFunction of the class DialogExternalFunctionsHandler
        /// </summary>
        /// <param name="funcName"></param>
        /// <param name="function"></param>
        public void BindExternalFunction(string funcName, Action function) => 
            ExternalFunctionsHandler.BindExternalFunction(funcName, function);

        /// <summary>
        /// Adding listener to OnDialogFinished UnityEvent
        /// </summary>
        /// <param name="action"></param>
        public void AddListenerToDialogFinishedEvent(UnityAction action) => 
            _onDialogFinished.AddListener(action);

        /// <summary>
        /// Setting currentNode field to Node and call HandleDialogGraphCurrentNode method
        /// </summary>
        /// <param name="node"></param>
        public void SetCurrentNodeAndHandleDialogGraph(Node node)
        {
            _currentNode = node;
            HandleDialogGraphCurrentNode(this._currentNode);
        }

        /// <summary>
        /// Processing dialog current node
        /// </summary>
        /// <param name="currentNode"></param>
        private void HandleDialogGraphCurrentNode(Node currentNode)
        {
            StopAllCoroutines();

            //if (currentNode.GetType() == typeof(SentenceNode))
            //    HandleSentenceNode(currentNode);
            //else if (currentNode.GetType() == typeof(DialogNode))
                HandleDialogNode(currentNode);
        }

        /// <summary>
        /// Processing sentence node
        /// </summary>
        /// <param name="currentNode"></param>
        /*
        private void HandleSentenceNode(Node currentNode)
        {
            SentenceNode sentenceNode = (SentenceNode)currentNode;
            CurrentSentenceNode = sentenceNode;

            _isCurrentSentenceSkipped = false;

            SentenceNodeActivated?.Invoke();
    
            string localizedCharName = sentenceNode.GetCharacterName();
            string localizedText = sentenceNode.GetText();
            
            SentenceNodeActivatedWithParameter?.Invoke(localizedCharName, localizedText,
                sentenceNode.GetCharacterSprite(), sentenceNode.GetButtonText());

            if (sentenceNode.IsExternalFunc())
                ExternalFunctionsHandler.CallExternalFunction(sentenceNode.GetExternalFunctionName());
    
            WriteDialogText(localizedText);
        }
        */
        /// <summary>
        /// Processing answer node
        /// </summary>
        /// <param name="currentNode"></param>
        private void HandleDialogNode(Node currentNode)
        {
            DialogNode answerNode = (DialogNode)currentNode;
            CurrentAnswerNode = answerNode;
        
            int numberOfActiveButtons = 0;

            AnswerNodeActivated?.Invoke();

            for (int i = 0; i < answerNode.ChildNodes.Count; i++)
            {
                if (answerNode.ChildNodes[i])
                {
                    AnswerNodeSetUp?.Invoke(i, answerNode.Choices[i]);
                    AnswerButtonSetUp?.Invoke(i, answerNode);

                    numberOfActiveButtons++;
                }
                else
                    break;
            }

            if (numberOfActiveButtons == 0)
            {
                _isDialogStarted = false;

                _onDialogFinished?.Invoke();
                return;
            }

            AnswerNodeActivatedWithParameter?.Invoke(numberOfActiveButtons);
        }

        /// <summary>
        /// Finds the first node that does not have a parent node but has a child one
        /// </summary>
        /// <param name="dialogNodeGraph"></param>
        private void DefineFirstNode(DialogNodeGraph dialogNodeGraph)
        {
            if (dialogNodeGraph.NodesList.Count == 0)
            {
                Debug.LogWarning("The list of nodes in the DialogNodeGraph is empty");
                return;
            }

            _currentNode = StartNode;
            /*

            foreach (Node node in dialogNodeGraph.NodesList)
            {
                _currentNode = node;

                //if (node.GetType() == typeof(SentenceNode))
                //{
                    //SentenceNode sentenceNode = (SentenceNode)node;

                    if (node.ParentNode == null && node.ChildNode != null)
                    {
                        _currentNode = node;
                        return;
                    }
                //}
            }

            _currentNode = dialogNodeGraph.NodesList[0];
            */
        }

        /// <summary>
        /// Writing dialog text
        /// </summary>
        /// <param name="text"></param>
        private void WriteDialogText(string text) => StartCoroutine(WriteDialogTextRoutine(text));

        /// <summary>
        /// Writing dialog text coroutine
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private IEnumerator WriteDialogTextRoutine(string text)
        {
            _isCurrentSentenceTyping = true;
            SentenceStarted?.Invoke();
            
            foreach (char textChar in text)
            {
                if (_isCurrentSentenceSkipped)
                {
                    DialogTextSkipped?.Invoke(text);
                    _isCurrentSentenceTyping = false;
                    break;
                }

                DialogTextCharWrote?.Invoke();

                yield return new WaitForSeconds(_dialogCharDelay);
            }

            _isCurrentSentenceTyping = false;
            SentenceEnded?.Invoke();
            
            yield return new WaitUntil(CheckNextSentenceKeyCodes);

            //JV CheckForDialogNextNode();
        }

        /// <summary>
        /// Checking is next dialog node has a child node
        /// </summary>
        /*
        private void CheckForDialogNextNode()
        {
            //if (_currentNode.GetType() == typeof(SentenceNode))
            //{
                //SentenceNode sentenceNode = (SentenceNode)_currentNode;

                if (_currentNode.ChildNode != null)
                {
                    _currentNode = _currentNode.ChildNode;
                    HandleDialogGraphCurrentNode(_currentNode);
                }
                else
                {
                    _isDialogStarted = false;
                    _onDialogFinished?.Invoke();
                }
            //}
        }*/

        /// <summary>
        /// Calculate max number of choice buttons
        /// </summary>
        private void CalculateMaxNumberOfChoiceButtons()
        {
            foreach (Node node in _currentNodeGraph.NodesList)
            {
                if (node.GetType() == typeof(DialogNode))
                {
                    DialogNode answerNode = (DialogNode)node;

                    if (answerNode.Choices.Count > _maxNumberOfChoiceButtons)
                        _maxNumberOfChoiceButtons = answerNode.Choices.Count;
                }
            }

            MaxNumberOfChoiceButtonsCalculated?.Invoke(_maxNumberOfChoiceButtons);
        }

        /// <summary>
        /// Handles text skipping mechanics
        /// </summary>
        private void HandleSentenceSkipping()
        {
            if (!_isDialogStarted || !_isCanSkippingText)
                return;
            
            if (CheckNextSentenceKeyCodes() && !_isCurrentSentenceSkipped)
                _isCurrentSentenceSkipped = true;
        }

        /// <summary>
        /// Checking whether at least one key from the nextSentenceKeyCodes was pressed
        /// </summary>
        /// <returns></returns>
        private bool CheckNextSentenceKeyCodes()
        {
            if (_GoToNextNode)
            {
                _GoToNextNode = false;
                return true;
            }

            for (int i = 0; i < _nextSentenceKeyCodes.Count; i++)
            { 
                if (Input.GetKeyDown(_nextSentenceKeyCodes[i]))
                    return true;
            }

            return false;
        }
    }
}