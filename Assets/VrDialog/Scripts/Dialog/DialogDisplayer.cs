using System;
using UnityEngine;
using UnityEngine.UI;

namespace cherrydev
{
    public class DialogDisplayer : MonoBehaviour
    {
        [Header("MAIN COMPONENT")]
        [SerializeField] private DialogBehaviour _dialogBehaviour;

        [Header("NODE PANELS")]
        //[SerializeField] private TextPanel _dialogTextPanel;
        //[SerializeField] private ButtonsPanel _dialogButtonsPanel;
        [SerializeField] private DialogPanel _dialogPanel;

        private void OnEnable()
        {
            _dialogBehaviour.AddListenerToDialogFinishedEvent(DisableDialogPanel);

            //_dialogBehaviour.AnswerButtonSetUp += SetUpAnswerButtonsClickEvent;

            _dialogBehaviour.DialogTextCharWrote += _dialogPanel.IncreaseMaxVisibleCharacters;
            _dialogBehaviour.DialogTextSkipped += _dialogPanel.ShowFullDialogText;

            /*_dialogBehaviour.SentenceNodeActivated += EnableDialogSentencePanel;
            _dialogBehaviour.SentenceNodeActivated += DisableDialogAnswerPanel;
            _dialogBehaviour.SentenceNodeActivated += _dialogSentencePanel.ResetDialogText;
            _dialogBehaviour.SentenceNodeActivatedWithParameter += _dialogSentencePanel.Setup;*/

            _dialogBehaviour.DialogNodeActivated += EnableDialogPanel;
            _dialogBehaviour.DialogNodeDeActivated += DisableDialogPanel;

            //_dialogBehaviour.DialogNodeActivatedWithParameter += _dialogPanel.EnableCertainAmountOfButtons;
            //_dialogBehaviour.MaxNumberOfChoiceButtonsCalculated += _dialogAnswerPanel.SetUpButtons;

            _dialogBehaviour.DialogNodeSetUp += SetUpDialogPanel;
#if UNITY_LOCALIZATION
            _dialogBehaviour.LanguageChanged += HandleLanguageChanged;
#endif
        }

        private void OnDisable()
        {
            //_dialogBehaviour.AnswerButtonSetUp -= SetUpAnswerButtonsClickEvent;

            _dialogBehaviour.DialogTextCharWrote -= _dialogPanel.IncreaseMaxVisibleCharacters;
            _dialogBehaviour.DialogTextSkipped -= _dialogPanel.ShowFullDialogText;

            /*_dialogBehaviour.SentenceNodeActivated -= EnableDialogSentencePanel;
            _dialogBehaviour.SentenceNodeActivated -= DisableDialogAnswerPanel;
            _dialogBehaviour.SentenceNodeActivated += _dialogSentencePanel.ResetDialogText;
            _dialogBehaviour.SentenceNodeActivatedWithParameter -= _dialogSentencePanel.Setup;
            */

            _dialogBehaviour.DialogNodeActivated -= EnableDialogPanel;
            _dialogBehaviour.DialogNodeDeActivated -= DisableDialogPanel;

            //_dialogBehaviour.DialogNodeActivatedWithParameter -= _dialogPanel.EnableCertainAmountOfButtons;
            //_dialogBehaviour.MaxNumberOfChoiceButtonsCalculated -= _dialogAnswerPanel.SetUpButtons;

            _dialogBehaviour.DialogNodeSetUp -= SetUpDialogPanel;
#if UNITY_LOCALIZATION
            _dialogBehaviour.LanguageChanged -= HandleLanguageChanged;
#endif
        }

        /// <summary>
        /// Disable dialog answer and sentence panel
        /// </summary>
        public void DisableDialogPanel()
        {
            _dialogPanel.GetComponent<Animator>().SetBool("Show", false);

            //DisableDialogPanel();
            //DisableDialogSentencePanel();
        }

        /// <summary>
        /// Enable dialog answer panel
        /// </summary>
        public void EnableDialogPanel()
        {
            _dialogPanel.GetComponent<Animator>().SetBool("Show", true);
            //ActiveGameObject(_dialogAnswerPanel.gameObject, true);
            //_dialogButtonsPanel.DisableAllButtons();
        }


        /// <summary>
        /// Enable or disable game object depends on isActive bool flag
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="isActive"></param>
        public void ActiveGameObject(GameObject gameObject, bool isActive)
        {
            if (gameObject == null)
            {
                Debug.LogWarning("Game object is null");
                return;
            }

            gameObject.SetActive(isActive);
        }

        /*
        /// <summary>
        /// Removing all listeners and Setting up answer button onClick event
        /// </summary>
        /// <param name="index"></param>
        /// <param name="dialogNode"></param>
        void xSetUpButtonsClickEvent(DialogNode dialogNode)
        {
            //***WARNING*** This might fail if the child node is not connected.

            Debug.Log("SetUpButtonsClickEvent()...");
            /x*
            for (int index = 0; index < dialogNode.ChildNodes.Count; index++)
            {
                _dialogPanel.GetButtonByIndex(index).onClick.RemoveAllListeners();
                //                _dialogPanel.AddButtonOnClickListener(index,
                //                    () => _dialogBehaviour.SetCurrentNodeAndHandleDialogGraph(dialogNode.ChildNodes[index].ChildNode));
                _dialogPanel.AddButtonOnClickListener(index, ButtonClicked);

                void xButtonClicked(int x)
                {
                    Debug.Log("ButtonClicked().  Index is " + index);
                    _dialogBehaviour.SetCurrentNodeAndHandleDialogGraph(dialogNode.ChildNodes[index].ChildNode);
                }


                


                //() => _dialogBehaviour.SetCurrentNodeAndHandleDialogGraph(dialogNode.ChildNodes[index].ChildNode));
            }
            *x/

            void someFunc(int i, Button button)
            {
                button.onClick.AddListener(delegate { ButtonClicked(i); });
            }

            for (int index = 0; index < dialogNode.ChildNodes.Count; index++)
            {

                var button = _dialogPanel.GetButtonByIndex(index);
                button.onClick.RemoveAllListeners();
                Debug.Log("Setting OnClick event for button " + index);
                someFunc(index, button);
                //button.onClick.AddListener(delegate { ButtonClicked(index); });

            }

            void ButtonClicked(int index)
            {
                Debug.Log("Button " + index + " clicked.");

            }

        }
        */





        /*void SetUpButtonsClickEvent(int index, DialogNode dialogNode)
        {
            Debug.Log("SetUpButtonsClickEvent()...");

            _dialogPanel.GetButtonByIndex(index).onClick.RemoveAllListeners();
            _dialogPanel.AddButtonOnClickListener(index,
                () => _dialogBehaviour.SetCurrentNodeAndHandleDialogGraph(dialogNode.ChildNodes[index].ChildNode));
        }*/



        /// <summary>
        /// Removing all listeners and Setting up answer button onClick event
        /// </summary>
        /// <param name="dialogNode"></param>
        public void SetUpButtonsClickEvent(DialogNode dialogNode)
        {
            //***WARNING*** This might fail if the child node is not connected.

            Debug.Log("SetUpButtonsClickEvent()...");
            /*
            for (int index = 0; index < dialogNode.ChildNodes.Count; index++)
            {
                _dialogPanel.GetButtonByIndex(index).onClick.RemoveAllListeners();
                //                _dialogPanel.AddButtonOnClickListener(index,
                //                    () => _dialogBehaviour.SetCurrentNodeAndHandleDialogGraph(dialogNode.ChildNodes[index].ChildNode));

            }
            */

            //local function to add a listerner.  When implemneted as a lambda function
            //the index value does not behave properly.  For now, keep _addListener
            //as a separate local function.
            void _addListener(Node i, Button button)
            {
                //create a delegate so that we can pass a value from the OnClick
                //event to our handler.
                button.onClick.AddListener(delegate { _dialogBehaviour.HandleDialogGraphCurrentNode(i); });
            }

            //each button will call the same function, passing to it and integer
            //equal to its index in the _buttons List (which should be the
            //same as the index from the ChildNodes list.
            for (int index = 0; index < dialogNode.ChildNodes.Count; index++)
            {
                var button = _dialogPanel.GetButtonByIndex(index);
                button.onClick.RemoveAllListeners();

                _addListener(dialogNode.ChildNodes[index].ChildNode, button);
            }



        }
        /*
        /// <summary>
        /// Handler for our button OnClick event.  The index parameter is the
        /// button's index in the ChildNodes list.
        /// </summary>
        /// <param name="index"></param>
        void ButtonClickedHandler(int index)
        {
            Debug.Log("Button " + index + " clicked.");
            _dialogBehaviour.HandleDialogGraphCurrentNode()
        }
        */

        /// <summary>
        /// Setting up answer dialog panel
        /// </summary>
        /// <param name="index"></param>
        /// <param name="answerText"></param>
        public void SetUpDialogPanel(DialogNode dialogNode)
        {
            //setup the text panel and buttons dimensions, background image, etc.
            _dialogPanel.SetupPanel(dialogNode);

            //setup button events
            SetUpButtonsClickEvent(dialogNode);
        }

        /*
        public void SetUpDialogPanel(int index, string answerText)
        {

            DialogNode currentDialogNode = _dialogBehaviour.CurrentDialogNode;

            if (currentDialogNode != null)
                _dialogPanel.GetButtonTextByIndex(index).text = currentDialogNode.ChildNodes[index].ChoiceText;
            else
                _dialogPanel.GetButtonTextByIndex(index).text = answerText;
        }
        */

        /*
        private void HandleLanguageChanged()
        {
            if (_dialogBehaviour.CurrentAnswerNode != null)
                RefreshAnswerButtons();
        }
        */
        /// <summary>
        /// Refresh all answer buttons with updated localized text
        /// </summary>
        /*
        private void RefreshAnswerButtons()
        {
            DialogNode currentAnswerNode = _dialogBehaviour.CurrentAnswerNode;
            
            if (currentAnswerNode != null)
            {
                for (int i = 0; i < currentAnswerNode.ChildNodes.Count; i++)
                {
                    if (i < _dialogAnswerPanel.GetButtonCount() &&
                        _dialogAnswerPanel.GetButtonByIndex(i).gameObject.activeSelf)
                        _dialogAnswerPanel.GetButtonTextByIndex(i).text = currentAnswerNode.GetChoiceText(i);
                }
            }
        }
        */
    }
}