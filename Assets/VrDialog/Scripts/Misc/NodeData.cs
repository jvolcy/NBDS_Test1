using UnityEngine;

namespace cherrydev
{
    [System.Serializable]
    public struct NodeData
    {
        [TextArea(3, 10)]
        public string DialogText;
        public bool UseCurrentVals;
        public float TextPanelWidthPct;
        //public float TextAreaHeightPct;
        //public float TextAreaYPos;
        //public Sprite ButtonImage;
        public float ButtonsPanelWidthPct;
        //public float ButtonAreaHeightPct;
        public float PanelRatio;
        public Color BackgroundColor;
        public Sprite BackgroundImage;
        public string ExternalFunctionToken;

        public NodeData(string dialogText)
        {
            DialogText = dialogText;
            UseCurrentVals = false;
            TextPanelWidthPct = 0.8f;
            //ButtonImage = null;
            //TextAreaHeightPct = 0.7f;
            //TextAreaYPos = -0.1f;
            ButtonsPanelWidthPct = 0.8f;
            //ButtonAreaHeightPct = 0.2f;
            PanelRatio = 0.5f;
            BackgroundColor = new Color32(29, 29, 29, 150);
            BackgroundImage = null;
            ExternalFunctionToken = "";
        }

        public NodeData(string dialogText,
                            bool useCurrentVals,
                            float textPanelWidthPct,
                            Sprite buttonImage,
                            float buttonsPanelWidthPct,
                            //float textAreaHeightPct,
                            //float textAreaYPos,
                            //float buttonAreaHeightPct,
                            float panelRatio,
                            Color backgroundColor,
                            Sprite backgroundImage,
                            string externalFunctionToken
                            )
        {
            DialogText = dialogText;
            UseCurrentVals = useCurrentVals;
            //ButtonImage = buttonImage;
            TextPanelWidthPct = textPanelWidthPct;
            //TextAreaHeightPct = textAreaHeightPct;
            //TextAreaYPos = textAreaYPos;
            ButtonsPanelWidthPct = buttonsPanelWidthPct;
            //ButtonAreaHeightPct = buttonAreaHeightPct;
            PanelRatio = panelRatio;
            BackgroundColor = backgroundColor;
            BackgroundImage = backgroundImage;
            ExternalFunctionToken = externalFunctionToken;
        }

    }
}