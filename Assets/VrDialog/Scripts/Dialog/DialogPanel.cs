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
        [SerializeField] private Image _dialogBackgroundImage;
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

        /// <summary>
        /// Assigning dialog name text, character image sprite and dialog text
        /// </summary>
        /*
        public void Setup(string characterName, string text, Sprite sprite, string buttonText)
        {
            _dialogNameText.text = characterName;
            _dialogText.text = text;
            _dialogButtonText.text = buttonText;

            if (sprite == null)
            {
                _dialogBackgroundImage.color = new Color(_dialogBackgroundImage.color.r,
                    _dialogBackgroundImage.color.g, _dialogBackgroundImage.color.b, 0);
                return;
            }

            _dialogBackgroundImage.color = new Color(_dialogBackgroundImage.color.r,
                _dialogBackgroundImage.color.g, _dialogBackgroundImage.color.b, 255);
            _dialogBackgroundImage.sprite = sprite;
        }
        */

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
        /*
         RectTransform textPanel;
         RectTransform buttonsPanel;
         TextMeshProUGUI _dialogText;
         Image _dialogBackgroundImage;
         Button _buttonPrefab;
         GridLayoutGroup _buttonsGridLayoutGroup;
        */
        public void SetupPanel(DialogNode dialogNode)
        {
            Debug.Log("SetupPanel()");

            Debug.Log(buttonsPanel.name + " size= " + buttonsPanel.sizeDelta);
            Debug.Log(buttonsPanel.name + " position= " + buttonsPanel.position);

            //Set text panel width and height
            float panelRatio = dialogNode.nodeData.PanelRatio;
            textPanel.sizeDelta = new Vector2(dialogNode.nodeData.TextPanelWidthPct - 1.0f, panelRatio - 1.0f);
            //Debug.Log(dialogNode.name + " size= " + textPanel.sizeDelta);

            //JV: Scale the grid layout group
            float buttonPanelWidthPct = dialogNode.nodeData.ButtonsPanelWidthPct;
            int numButtons = dialogNode.ChildNodes.Count;

            buttonsPanel.sizeDelta = new Vector2(dialogNode.nodeData.ButtonsPanelWidthPct - 1.0f, dialogNode.nodeData.PanelRatio);
            _buttonsGridLayoutGroup.cellSize = new Vector2(buttonPanelWidthPct, (1f - panelRatio) / (numButtons + 0.5f));
            //_buttonsGridLayoutGroup.cellSize = new Vector2(0, 1f / (numButtons + 1));
            Debug.Log(buttonsPanel.name + " size= " + buttonsPanel.sizeDelta);
            Debug.Log(buttonsPanel.name + " position= " + buttonsPanel.position);

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
        public void SetUpButtons(int numButtons)
        {
            DeleteAllExistingButtons();



            for (int i = 0; i < numButtons; i++)
            {
                Button coiceButton = Instantiate(_buttonPrefab, _buttonsGridLayoutGroup.transform);

                _buttons.Add(coiceButton);
                _buttonTexts.Add(coiceButton.GetComponentInChildren<TextMeshProUGUI>());
            }

            Debug.Log("SetupButtons() created " + numButtons + " buttons.");
        }

        /// <summary>
        /// Returning button by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Button GetButtonByIndex(int index) => _buttons[index];

        /// <summary>
        /// Returning button text bu index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TextMeshProUGUI GetButtonTextByIndex(int index) => _buttonTexts[index];

        /// <summary>
        /// Setting UnityAction to button onClick event by index 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="action"></param>
        public void AddButtonOnClickListener(int index, UnityAction action) => _buttons[index].onClick.AddListener(action);

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Enable certain amount of buttons
        /// </summary>
        /// <param name="amount"></param>
        /*
        public void EnableCertainAmountOfButtons(int amount)
        {
            if (_buttons.Count == 0)
            {
                Debug.LogWarning("Please assign button list!");
                return;
            }

            for (int i = 0; i < amount; i++)
                _buttons[i].gameObject.SetActive(true);

            //JV: Scale the grid layout group
            _parentGridLayoutGroup.cellSize = new Vector2(0.8f, 1f / (amount + 1));
        }
        */
        /// <summary>
        /// Disable all buttons
        /// </summary>
        public void DisableAllButtons()
        {
            foreach (Button button in _buttons)
                button.gameObject.SetActive(false);
        }

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