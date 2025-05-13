using UnityEngine;

namespace cherrydev
{
    [System.Serializable]
    public struct NodeData
    {
        [TextArea(3, 10)]
        public string DialogText;
        public Sprite BackgroundImage;
        public bool AutoDismissButton;
        public float TextAreaWidthPct;
        public float TextAreaHeightPct;
        public float TextAreaYPos;
        public float ButtonsAreaWidthPct;
        public float ButtonAreaHeightPct;
        public float ButtonAreaYPos;
        public Sprite ButtonImage;

        public NodeData(string dialogText)
        {
            DialogText = dialogText;
            BackgroundImage = null;
            AutoDismissButton = true;
            TextAreaWidthPct = 0.8f;
            TextAreaHeightPct = 0.7f;
            TextAreaYPos = -0.1f;
            ButtonsAreaWidthPct = 0.8f;
            ButtonAreaHeightPct = 0.2f;
            ButtonAreaYPos = -0.8f;
            ButtonImage = null;
        }

        public NodeData(string dialogText,
                            Sprite backgroundImage,
                            bool autoDismissButton,
                            float textAreaWidthPct,
                            float textAreaHeightPct,
                            float textAreaYPos,
                            float buttonsAreaWidthPct,
                            float buttonAreaHeightPct,
                            float buttonAreaYPos,
                            Sprite buttonImage
                            )
        {
            DialogText = dialogText;
            BackgroundImage = backgroundImage;
            AutoDismissButton = autoDismissButton;
            TextAreaWidthPct = textAreaWidthPct;
            TextAreaHeightPct = textAreaHeightPct;
            TextAreaYPos = textAreaYPos;
            ButtonsAreaWidthPct = buttonsAreaWidthPct;
            ButtonAreaHeightPct = buttonAreaHeightPct;
            ButtonAreaYPos = buttonAreaYPos;
            ButtonImage = buttonImage;
        }
    }
}