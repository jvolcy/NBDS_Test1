using UnityEngine;

namespace cherrydev
{
    /* JV: Added optional ButtonText to Sentence structure. */
    [System.Serializable]
    public struct Sentence
    {
        public string CharacterName;
        public string Text;
        public Sprite CharacterSprite;
        public string ButtonText;       //JV

        public Sentence(string characterName, string text, string buttonText="")
        {
            CharacterSprite = null;
            CharacterName = characterName;
            Text = text;
            ButtonText = buttonText;        //JV
        }
    }
}