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
        public float TypeDelay;
        public float PanelHorzSizePct;
        public float PanelVertSizePct;
        public Color BackgroundColor;
        public Sprite BackgroundImage;
        [Range(0f, 1f)]
        public float AvatarImgToTxtRatio;
        [Range(0f, 1f)]
        public float AvatarToButtonPanelRatio;
        [Range(0f, 1f)]
        public float ButtonsWidthPct;

        public string ExternalFunctionToken;
        public bool pinned;

        public NodeData(string dialogText, string avatarName = "", Sprite avatarImage = null)
        {
            AvatarImage = avatarImage;
            DialogText = dialogText;

            UseCurrentVals = false;

            FontSize = 10f;
            TypeDelay = 0.05f;
            PanelHorzSizePct = 1;
            PanelVertSizePct = 1;
            BackgroundColor = new Color32(29, 29, 29, 150);
            BackgroundImage = null;
            AvatarImgToTxtRatio = 0.3f;
            AvatarToButtonPanelRatio = 0.5f;
            ButtonsWidthPct = 0.8f;

            ExternalFunctionToken = "";
            pinned = true;
        }

    }
}