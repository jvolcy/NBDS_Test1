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
        [SerializeField] private TextMeshProUGUI _dialogText;

        [SerializeField] RectTransform AvatarImage;
        [SerializeField] TextMeshProUGUI AvatarName;

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

            /*



PanelHorzSizePct;
public float PanelVertSizePct
public Color BackgroundColor;
public Sprite BackgroundImage;
public float AvatarImgToTxtRatio;
public float AvatarToButtonPanelRatio;
public float ButtonsWidthPct;
 */
            if (!dialogNode.nodeData.UseCurrentVals)
            {
                //Set dialog panel width and height (based on the panel ratio)
                float panelHorzSizePct = dialogNode.nodeData.PanelHorzSizePct;
                float panelVertSizePct = dialogNode.nodeData.PanelVertSizePct;
                GetComponent<RectTransform>().sizeDelta = new Vector2(panelHorzSizePct - 1.0f, panelVertSizePct - 1.0f);

                //Set text panel width and height (based on the avatar image ratio and panel ratio)
                float avatarToButtonPanelRatio = dialogNode.nodeData.AvatarToButtonPanelRatio;
                float txtPanelVertSize = (avatarToButtonPanelRatio - 1.0f) * panelVertSizePct;
                textPanel.sizeDelta = new Vector2(/*panelHorzSizePct - 1.0f*/0f, txtPanelVertSize);
                //Debug.Log(dialogNode.name + " size= " + textPanel.sizeDelta);

                //avatar image and dialog text (side by side and horizontally connected through AvatarImgToTxtRatio)
                //both have the same height: txtPanelVertSize
                //AvatarImage.GetComponent<RectTransform>().sizeDelta = new Vector2(dialogNode.nodeData.AvatarImgToTxtRatio, -txtPanelVertSize);
                var rect = AvatarImage.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(dialogNode.nodeData.AvatarImgToTxtRatio * panelHorzSizePct, 0);

                _dialogText.GetComponent<RectTransform>().sizeDelta = new Vector2((1f-dialogNode.nodeData.AvatarImgToTxtRatio) * panelHorzSizePct, 0);

                //Scale the button panel (depends on the panel ratio)
                float buttonPanelWidthPct = dialogNode.nodeData.ButtonsWidthPct;
                int numButtons = dialogNode.ChildNodes.Count;

                //Scale the button grid layout (depends on the panel ratio)
                buttonsPanel.sizeDelta = new Vector2( 0/*(dialogNode.nodeData.ButtonsWidthPct - 1.0f)*/, dialogNode.nodeData.AvatarToButtonPanelRatio * panelVertSizePct);
                _buttonsGridLayoutGroup.cellSize = new Vector2(buttonPanelWidthPct * panelHorzSizePct, (1f - avatarToButtonPanelRatio) * panelVertSizePct / (numButtons + 0.5f));
                //Debug.Log(buttonsPanel.name + " size= " + buttonsPanel.sizeDelta);
                //Debug.Log(buttonsPanel.name + " position= " + buttonsPanel.position);

                //Set the background color
                GetComponent<Image>().color = dialogNode.nodeData.BackgroundColor;

                //Set the background image
                GetComponent<Image>().sprite = dialogNode.nodeData.BackgroundImage;
            }

            //Setup the avatar image and name
            //AvatarImage.GetComponent<Image>().sprite = dialogNode.nodeData.AvatarImage;
            AvatarName.text = dialogNode.nodeData.AvatarName;

            //Setup the Dialog's main text
            ResetDialogText();
            _dialogText.text = dialogNode.nodeData.DialogText;

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