using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace cherrydev
{
    public class DialogPanel : MonoBehaviour
    {
        [SerializeField] RectTransform textPanel;
        [SerializeField] RectTransform buttonsPanel;
        //[SerializeField] private TextMeshProUGUI _dialogNameText;
        [SerializeField] private TextMeshProUGUI _dialogText;
        //[SerializeField] private Sprite _dialogBackgroundImage;
        //[SerializeField] private TextMeshProUGUI _dialogButtonText;


        /// <summary>
        /// Setting dialogText max visible characters to zero
        /// </summary>
        public void ResetDialogText()
        {
            _dialogText.maxVisibleCharacters = 0;
        }

        /// <summary>
        /// Set dialog text max visible characters to dialog text length
        /// </summary>
        /// <param name="text"></param>
        public void ShowFullDialogText(string text)
        {
            _dialogText.text = text;
            _dialogText.maxVisibleCharacters = text.Length;
        }

        /// <summary>
        /// Increasing max visible characters
        /// </summary>
        public void IncreaseMaxVisibleCharacters() => _dialogText.maxVisibleCharacters++;


        //============================================================


        [SerializeField] private Button _buttonPrefab;
        [SerializeField] private GridLayoutGroup _buttonsGridLayoutGroup;

        private readonly List<Button> _buttons = new();
        private readonly List<TextMeshProUGUI> _buttonTexts = new();

        /// <summary>
        /// setup the text panel (width, height, etc.)
        /// setup the button panel (width, height, etc.)
        /// setup the background image
        /// setup the panel ratios
        /// </summary>
        /// <param name="dialogNode"></param>
        public void SetupPanel(DialogNode dialogNode)
        {
            Debug.Log("SetupPanel()");

            Debug.Log(buttonsPanel.name + " size= " + buttonsPanel.sizeDelta);
            Debug.Log(buttonsPanel.name + " position= " + buttonsPanel.position);

            //Set text panel width and height (based on the panel ratio)
            float panelRatio = dialogNode.nodeData.PanelRatio;
            textPanel.sizeDelta = new Vector2(dialogNode.nodeData.TextPanelWidthPct - 1.0f, panelRatio - 1.0f);
            //Debug.Log(dialogNode.name + " size= " + textPanel.sizeDelta);

            //Scale the button panel (depends on the panel ratio)
            float buttonPanelWidthPct = dialogNode.nodeData.ButtonsPanelWidthPct;
            int numButtons = dialogNode.ChildNodes.Count;

            //Scale the button grid layout (depends on the panel ratio)
            buttonsPanel.sizeDelta = new Vector2(dialogNode.nodeData.ButtonsPanelWidthPct - 1.0f, dialogNode.nodeData.PanelRatio);
            _buttonsGridLayoutGroup.cellSize = new Vector2(buttonPanelWidthPct, (1f - panelRatio) / (numButtons + 0.5f));
            //Debug.Log(buttonsPanel.name + " size= " + buttonsPanel.sizeDelta);
            //Debug.Log(buttonsPanel.name + " position= " + buttonsPanel.position);

            //Set the background color
            GetComponent<Image>().color = dialogNode.nodeData.BackgroundColor;

            //Set the background image
            GetComponent<Image>().sprite = dialogNode.nodeData.BackgroundImage;

            //Setup the buttons
            SetUpButtons(dialogNode);

        }

        /// <summary>
        /// Returns the total number of buttons
        /// </summary>
        /// <returns>The number of buttons</returns>
        public int GetButtonCount() => _buttons.Count;


        /// <summary>
        /// Instantiate the specified number of buttons.
        /// Stores the reference to each button in the _buttons list.
        /// Stores the TMP text for each button in the _buttonTexts list.
        /// </summary>
        /// <param name="numButtons"></param>
        void SetUpButtons(DialogNode dialogNode)
        {
            DeleteAllExistingButtons();

            int numButtons = dialogNode.ChildNodes.Count;

            for (int i = 0; i < numButtons; i++)
            {
                Button choiceButton = Instantiate(_buttonPrefab, _buttonsGridLayoutGroup.transform);

                var buttonTxt = choiceButton.GetComponentInChildren<TMP_Text>();
                buttonTxt.text = dialogNode.ChildNodes[i].ChoiceText;

                _buttons.Add(choiceButton);
                _buttonTexts.Add(choiceButton.GetComponentInChildren<TextMeshProUGUI>());

            }

        }

        /// <summary>
        /// Returning button by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Button GetButtonByIndex(int index)
        {
            //Debug.Log("_buttons.Count = " + _buttons.Count);
            //Debug.Log("index = " + index);
            return _buttons[index];
        }

        /// <summary>
        /// Returning button text bu index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TextMeshProUGUI GetButtonTextByIndex(int index) => _buttonTexts[index];


        /// <summary>
        /// Removes all existing buttons, used before setup
        /// </summary>
        private void DeleteAllExistingButtons()
        {
            if (_buttons.Count > 0)
            {
                foreach (var button in _buttons) 
                    Destroy(button.gameObject);
                            
                _buttons.Clear();
                _buttonTexts.Clear();
            }
        }


    }
}