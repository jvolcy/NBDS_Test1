using UnityEngine;

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
            _dialogBehaviour.DialogNodeActivated += DisableDialogPanel;

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
            _dialogBehaviour.DialogNodeActivated -= DisableDialogPanel;

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
        
        /// <summary>
        /// Removing all listeners and Setting up answer button onClick event
        /// </summary>
        /// <param name="index"></param>
        /// <param name="answerNode"></param>
        public void SetUpAnswerButtonsClickEvent(int index, DialogNode answerNode)
        {
            _dialogPanel.GetButtonByIndex(index).onClick.RemoveAllListeners();
            _dialogPanel.AddButtonOnClickListener(index, 
                () => _dialogBehaviour.SetCurrentNodeAndHandleDialogGraph(answerNode.ChildNodes[index].ChildNode));
        }

        /// <summary>
        /// Setting up answer dialog panel
        /// </summary>
        /// <param name="index"></param>
        /// <param name="answerText"></param>
        public void SetUpDialogPanel(DialogNode dialogNode)
        {
            //setup the text panel (width, height, etc.)
            //setup the button panel (width, height, etc.)
            //setup the background image
            //setup the panel ratios

            _dialogPanel.SetupPanel(dialogNode);

            //setup the buttons
            _dialogPanel.SetUpButtons(dialogNode.ChildNodes.Count);

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