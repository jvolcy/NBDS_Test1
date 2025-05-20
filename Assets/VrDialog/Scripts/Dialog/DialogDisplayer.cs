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
            //_dialogBehaviour.AddListenerToDialogFinishedEvent(DisableDialogPanel);
            _dialogBehaviour.DialogTextCharWritten += _dialogPanel.IncreaseMaxVisibleCharacters;
            _dialogBehaviour.DialogTextSkipped += _dialogPanel.ShowFullDialogText;

            _dialogBehaviour.DialogNodeOpen += DialogPanelOpen;
            _dialogBehaviour.DialogNodeClose += DialogPanelClose;

            _dialogBehaviour.DialogNodeSetUp += SetUpDialogPanel;

        }

        private void OnDisable()
        {
            //_dialogBehaviour.AnswerButtonSetUp -= SetUpAnswerButtonsClickEvent;

            _dialogBehaviour.DialogTextCharWritten -= _dialogPanel.IncreaseMaxVisibleCharacters;
            _dialogBehaviour.DialogTextSkipped -= _dialogPanel.ShowFullDialogText;

            _dialogBehaviour.DialogNodeOpen -= DialogPanelOpen;
            _dialogBehaviour.DialogNodeClose -= DialogPanelClose;

            _dialogBehaviour.DialogNodeSetUp -= SetUpDialogPanel;

        }

        /// <summary>
        /// Disable dialog answer and sentence panel
        /// </summary>
        public void DialogPanelClose(string token)
        {
            Debug.Log("DialogDisplayer:DialogPanelClose()...");
            _dialogPanel.GetComponent<Animator>().SetBool("Show", false);
            //DisableDialogPanel();
            //DisableDialogSentencePanel();
        }

        /// <summary>
        /// Enable dialog answer panel
        /// </summary>
        public void DialogPanelOpen(string token)
        {
            _dialogPanel.GetComponent<Animator>().SetBool("Show", true);
            //ActiveGameObject(_dialogAnswerPanel.gameObject, true);
            //_dialogButtonsPanel.DisableAllButtons();
        }


        /// <summary>
        /// Removing all listeners and Setting up answer button onClick event
        /// </summary>
        /// <param name="dialogNode"></param>
        public void SetUpButtonsClickEvent(DialogNode dialogNode)
        {
            //***WARNING*** This might fail if the child node is not connected.

            Debug.Log("SetUpButtonsClickEvent()...");

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

    }
}