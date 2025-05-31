using UnityEngine;

namespace cherrydev
{
    [System.Serializable]
    public struct NodeData
    {
        public Sprite AvatarImage;
        [TextArea(3, 10)]
        public string DialogText;

        public bool UseCurrentVals;

        public float FontSize;
        [Range(0f, 1f)]
        public float TextPaddingPct;
        public float TypeDelay;
        public float HScalePct;
        public float VScalePct;
        public Color BackgroundColor;
        public Sprite BackgroundImage;
        [Range(0f, 1f)]
        public float HorzPanelRatio;
        [Range(0f, 1f)]
        public float VertPanelRatio;
        [Range(0f, 1f)]
        public float ButtonsWidthPct;
        public float ButtonFontSize;
        public float Timeout;
        public int ChildNodeOnTimeout;

        public string ExternalFunctionToken;
        public bool Collapsed;

        public NodeData(string dialogText, Sprite avatarImage = null)
        {
            AvatarImage = avatarImage;
            DialogText = dialogText;

            UseCurrentVals = false;

            FontSize = 10f;
            TextPaddingPct = .05f;
            TypeDelay = 0.05f;
            HScalePct = 1;
            VScalePct = 1;
            BackgroundColor = new Color32(29, 29, 29, 150);
            BackgroundImage = null;
            HorzPanelRatio = 0.3f;
            VertPanelRatio = 0.5f;
            ButtonsWidthPct = 0.8f;
            ButtonFontSize = 10f;
            Timeout = 0f;
            ChildNodeOnTimeout = 0;

            ExternalFunctionToken = "";
            Collapsed = false;
        }

        //create a copy constructor
        public NodeData(NodeData src)
        {
            AvatarImage = src.AvatarImage;
            DialogText = src.DialogText;

            UseCurrentVals = src.UseCurrentVals;

            FontSize = src.FontSize;
            TextPaddingPct = src.TextPaddingPct;
            TypeDelay = src.TypeDelay;
            HScalePct = src.HScalePct;
            VScalePct = src.VScalePct;
            BackgroundColor = src.BackgroundColor;
            BackgroundImage = src.BackgroundImage;
            HorzPanelRatio = src.HorzPanelRatio;
            VertPanelRatio = src.VertPanelRatio;
            ButtonsWidthPct = src.ButtonsWidthPct;
            ButtonFontSize = src.ButtonFontSize;
            Timeout = src.Timeout;
            ChildNodeOnTimeout = src.ChildNodeOnTimeout;

            ExternalFunctionToken = src.ExternalFunctionToken;
            Collapsed = src.Collapsed;
        }

    }
}