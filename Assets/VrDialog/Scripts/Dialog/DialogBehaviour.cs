using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace cherrydev
{
    public class DialogBehaviour : MonoBehaviour
    {
        //[SerializeField] private Node StartNode;
        [SerializeField] private float _dialogCharDelay;
        [SerializeField] private List<KeyCode> _nextSentenceKeyCodes;
        [SerializeField] private bool _isCanSkippingText = true;

        [Space(10)]
        [SerializeField] private UnityEvent _onDialogStarted;
        [SerializeField] private UnityEvent _onDialogFinished;

        //boolean used to force us to the next node.  This value is set to true by the UI sentence button.
        bool _GoToNextNode = false;   //JV

        //private DialogNodeGraph _currentNodeGraph;
        private Node _firstNode;
        
        public DialogNode CurrentDialogNode { get; private set; }

        private bool _isDialogStarted;
        private bool _isCurrentSentenceSkipped;
        //private bool _isCurrentSentenceTyping;

        public bool IsCanSkippingText
        {
            get => _isCanSkippingText;
            set => _isCanSkippingText = value;
        }

        public event Action SentenceStarted;
        public event Action SentenceEnded;
        //public event Action SentenceNodeActivated;
        //public event Action<string, string, Sprite, string> SentenceNodeActivatedWithParameter;
//        public event Action<string, string, Sprite> SentenceNodeActivatedWithParameter;
        public event Action DialogNodeActivated;
        public event Action DialogNodeDeActivated;
        //public event Action<int, DialogNode> AnswerButtonSetUp;
        //public event Action<int> MaxNumberOfChoiceButtonsCalculated;
        public event Action<DialogNode> DialogNodeActivatedWithParameter;
        public event Action<DialogNode> DialogNodeSetUp;
        public event Action DialogTextCharWrote;
        public event Action<string> DialogTextSkipped;

        public DialogExternalFunctionsHandler ExternalFunctionsHandler { get; private set; }

        private void Awake() => ExternalFunctionsHandler = new DialogExternalFunctionsHandler();

        
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
            //_currentNodeGraph = dialogNodeGraph;

            FindFirstNode(dialogNodeGraph);
            HandleDialogGraphCurrentNode(_firstNode);
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

        /*
        /// <summary>
        /// Setting currentNode field to Node and call HandleDialogGraphCurrentNode method
        /// </summary>
        /// <param name="node"></param>
        public void SetCurrentNodeAndHandleDialogGraph(Node node)
        {
            Debug.Log("SetCurrentNodeAndHandleDialogGraph()");
            HandleDialogGraphCurrentNode(node);
        }
        */

        /// <summary>
        /// Processing dialog current node
        /// </summary>
        /// <param name="currentNode"></param>
        public void HandleDialogGraphCurrentNode(Node currentNode)
        {
            DialogNode dialogNode = currentNode as DialogNode;
            Debug.Log("HandleDialogGraphCurrentNode() on node " + dialogNode.name);

            StopAllCoroutines();

            //if (currentNode.GetType() == typeof(SentenceNode))
            //    HandleSentenceNode(currentNode);
            //else if (currentNode.GetType() == typeof(DialogNode))
            HandleDialogNode(currentNode);
        }


        /// <summary>
        /// Processing answer node
        /// </summary>
        /// <param name="currentNode"></param>
        private void HandleDialogNode(Node currentNode)
        {
            DialogNode dialogNode = (DialogNode)currentNode;
            CurrentDialogNode = dialogNode;
        
            //int numberOfConnectedChildNodes = 0;

            DialogNodeActivated?.Invoke();

            /*
            for (int i = 0; i < dialogNode.ChildNodes.Count; i++)
            {
                if (dialogNode.ChildNodes[i].ChildNode)
                {
                    DialogNodeSetUp?.Invoke(i, dialogNode.ChildNodes[i].ChoiceText);
                    AnswerButtonSetUp?.Invoke(i, dialogNode);

                    numberOfConnectedChildNodes++;
                }
                else
                    break;
            }

            //if there are no child nodes, we are at the end of the dialog chain
            if (numberOfConnectedChildNodes == 0)
            {
                _isDialogStarted = false;

                _onDialogFinished?.Invoke();
                return;
            }
            */
            DialogNodeSetUp?.Invoke(dialogNode);

            //enable only connected buttons
            //DialogNodeActivatedWithParameter?.Invoke(dialogNode);



            string localizedText = dialogNode.nodeData.DialogText;

           // SentenceNodeActivatedWithParameter?.Invoke(localizedCharName, localizedText,
           //     sentenceNode.GetCharacterSprite(), sentenceNode.GetButtonText());

            if (dialogNode.nodeData.ExternalFunctionName != "")
                ExternalFunctionsHandler.CallExternalFunction(dialogNode.nodeData.ExternalFunctionName);

            WriteDialogText(localizedText);
        }

        /// <summary>
        /// Finds the start node
        /// </summary>
        /// <param name="dialogNodeGraph"></param>
        private void FindFirstNode(DialogNodeGraph dialogNodeGraph)
        {
            _firstNode = null;

            DialogNode dn;

            Debug.Log("DialogBehaviour:There are " + dialogNodeGraph.NodesList.Count + " nodes.");
            for (int i = 0; i < dialogNodeGraph.NodesList.Count; i++)
            {
                dn = (DialogNode)dialogNodeGraph.NodesList[i];

                if (dn != null)
                {
                    if (dn.IsStartNode())
                    {
                        _firstNode = dn.ChildNodes[0].ChildNode;
                        Debug.Log("Start node is " + ((DialogNode)_firstNode).name);
                        return;
                    }
                }
            }

            Debug.Log("WARNING: No START node found.");

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
            SentenceStarted?.Invoke();
            
            foreach (char textChar in text)
            {
                if (_isCurrentSentenceSkipped)
                {
                    DialogTextSkipped?.Invoke(text);
                    break;
                }

                DialogTextCharWrote?.Invoke();

                yield return new WaitForSeconds(_dialogCharDelay);
            }

            SentenceEnded?.Invoke();
            
            yield return new WaitUntil(CheckNextSentenceKeyCodes);

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