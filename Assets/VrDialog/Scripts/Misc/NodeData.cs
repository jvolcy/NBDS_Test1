using UnityEngine;

namespace cherrydev
{
    [System.Serializable]
    public struct NodeData
    {
        public Sprite AvatarImage;
        //public string AvatarName;
        [TextArea(3, 10)]
        public string DialogText;

        public bool UseCurrentVals;

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

        public NodeData(string dialogText, string avatarName = "", Sprite avatarImage = null)
        {
            AvatarImage = avatarImage;
            //AvatarName = avatarName;
            DialogText = dialogText;

            UseCurrentVals = false;

            PanelHorzSizePct = 1;
            PanelVertSizePct = 1;
            BackgroundColor = new Color32(29, 29, 29, 150);
            BackgroundImage = null;
            AvatarImgToTxtRatio = 0.3f;
            AvatarToButtonPanelRatio = 0.5f;
            ButtonsWidthPct = 0.8f;

            ExternalFunctionToken = "";
        }

    }
}