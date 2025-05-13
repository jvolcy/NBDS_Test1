using UnityEngine;

namespace cherrydev
{
    [System.Serializable]
    public struct NodeData
    {
        [TextArea(3, 10)]
        public string DialogText;
        public float TextPanelWidthPct;
        //public float TextAreaHeightPct;
        //public float TextAreaYPos;
        //public bool ShowButtons;
        public Sprite ButtonImage;
        public float ButtonsPanelWidthPct;
        //public float ButtonAreaHeightPct;
        public float PanelRatio;
        public Sprite BackgroundImage;

        public NodeData(string dialogText)
        {
            DialogText = dialogText;
            TextPanelWidthPct = 0.8f;
            ButtonImage = null;
            //TextAreaHeightPct = 0.7f;
            //TextAreaYPos = -0.1f;
            //ShowButtons = true;
            ButtonsPanelWidthPct = 0.8f;
            //ButtonAreaHeightPct = 0.2f;
            PanelRatio = 0.5f;
            BackgroundImage = null;
        }

        public NodeData(string dialogText,
                            float textPanelWidthPct,
                            Sprite buttonImage,
                            float buttonsPanelWidthPct,
                            //float textAreaHeightPct,
                            //float textAreaYPos,
                            //bool showButtons,
                            //float buttonAreaHeightPct,
                            float panelRatio,
                            Sprite backgroundImage
                            )
        {
            DialogText = dialogText;
            ButtonImage = buttonImage;
            TextPanelWidthPct = textPanelWidthPct;
            //TextAreaHeightPct = textAreaHeightPct;
            //TextAreaYPos = textAreaYPos;
            //ShowButtons = showButtons;
            ButtonsPanelWidthPct = buttonsPanelWidthPct;
            //ButtonAreaHeightPct = buttonAreaHeightPct;
            PanelRatio = panelRatio;
            BackgroundImage = backgroundImage;
        }
    }
}