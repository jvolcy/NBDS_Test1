using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace cherrydev
{
    public class DialogPanel : MonoBehaviour
    {
        public RectTransform MainPanel;
        public RectTransform DialogSubPanel;
        public RectTransform ButtonsSubPanel;
        public RectTransform AvatarSubPanel;
        public RectTransform TextSubPanel;

        [SerializeField] private TextMeshProUGUI _dialogText;

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

        Rect CanvasRect;    //the dimensions of the canvas

        //create shadow copies of the NodeData and NumButtons used to render the current
        //dialog panel.  We use these values to refresh the panel when the screen
        //size changes.  We also use these when the next node has its UseCurrentVals
        //flag set.
        NodeData? _currentNodeData = null;
        int _currentNumButtons;

        //store the current width and height of the screen and compare to the
        //width and height on the next call to Update().  If they differ, the
        //screen has been resized and we must redraw the dialog.  This is a
        //homemade "OnWindowResized" substitute.
        int ScreenWidth;
        int ScreenHeight;

        [SerializeField] private Button _buttonPrefab;
        [SerializeField] private GridLayoutGroup _buttonsGridLayoutGroup;

        private readonly List<Button> _buttons = new();
        private readonly List<TextMeshProUGUI> _buttonTexts = new();

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            CanvasRect = GetComponent<RectTransform>().rect;
            ScreenHeight = 0;   //force a resize in Update()
            ScreenWidth = 0;    //force a resize in Update()
        }

        /// <summary>
        /// Monitor the size of the dialog panel for changes.  If it changes,
        /// update the display
        /// </summary>
        private void Update()
        {
            if (ScreenHeight != Screen.height || ScreenWidth != Screen.width)
            {
                ScreenHeight = Screen.height;
                ScreenWidth = Screen.width;

                //reset the CanvasRect since the screen has changed size.
                CanvasRect = GetComponent<RectTransform>().rect;
                SetupDialogGeometry((NodeData)_currentNodeData, _currentNumButtons);
            }
        }

        /// <summary>
        /// setup the text panel (width, height, etc.)
        /// setup the button panel (width, height, etc.)
        /// setup the background image
        /// setup the panel ratios
        /// </summary>
        /// <param name="dialogNode"></param>
        public void SetupPanel(DialogNode dialogNode)
        {
            //Debug.Log("SetupPanel()");

            //Setup the panel geometry
            SetupDialogGeometry(dialogNode.nodeData, dialogNode.ChildNodes.Count);

            //Setup the avatar image
            AvatarSubPanel.GetComponentInChildren<Image>().sprite = dialogNode.nodeData.AvatarImage;

            //Setup the Dialog's main text
            ResetDialogText();
            _dialogText.text = dialogNode.nodeData.DialogText;

            //Setup the buttons
            SetUpButtons(dialogNode);
        }

        /// <summary>
        /// Function to setup/refresh the geometric layout of the dialog
        /// </summary>
        /// <param name="nodeData"></param>
        void SetupDialogGeometry(NodeData nodeData, int NumButtons)
        {
            if (!nodeData.UseCurrentVals)
            {
                //Set dialog panel width and height (based on the panel ratio)
                MainPanel.sizeDelta = new Vector2(CanvasRect.width * (nodeData.HScalePct - 1), CanvasRect.height * (nodeData.VScalePct - 1));
                Rect rect = MainPanel.rect;

                //Debug.Log("Screen size = " + Screen.width + "," + Screen.height);
                //Debug.Log("position = " + MainPanel.position);

                //Set the dialog position
                MainPanel.position = new Vector2(Screen.width * nodeData.NormHPos, Screen.height * nodeData.NormVPos);

                //Set the avatar, text and button sub-panel sizes, based on the horz and vert panel ratios
                AvatarSubPanel.sizeDelta = new Vector2(-rect.width * (1 - nodeData.HorzPanelRatio), -rect.height * (1 - nodeData.VertPanelRatio));
                TextSubPanel.sizeDelta = new Vector2(-rect.width * nodeData.HorzPanelRatio, -rect.height * (1 - nodeData.VertPanelRatio));
                ButtonsSubPanel.sizeDelta = new Vector2(0, -rect.height * nodeData.VertPanelRatio);

                //text padding
                _dialogText.GetComponent<RectTransform>().sizeDelta = new Vector2(-rect.width * nodeData.TextPaddingPct, -rect.height * nodeData.TextPaddingPct);

                //set the dialot text font size
                _dialogText.fontSize = nodeData.FontSize;

                //Set the background color
                Image img = MainPanel.GetComponent<Image>();
                img.color = nodeData.BackgroundColor;

                //Set the background image
                img.sprite = nodeData.BackgroundImage;

                //keep a local copy of the nodeData.  We need this copy in case
                //we have to refresh the dialog panel.  Also, if the next node has its
                //"UseCurrentVals" boolean set, this is the data we will need to
                //refresh the dialog.
                _currentNumButtons = NumButtons;
                _currentNodeData = nodeData;        //should be the last thing we do in this function
            }

            //Scale the button grid layout (depends on the panel ratio)
            _buttonsGridLayoutGroup.cellSize = new Vector2(ButtonsSubPanel.rect.width * nodeData.ButtonsWidthPct, ButtonsSubPanel.rect.height / (NumButtons + 1));
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
                buttonTxt.fontSize = ((NodeData)_currentNodeData).ButtonFontSize;

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