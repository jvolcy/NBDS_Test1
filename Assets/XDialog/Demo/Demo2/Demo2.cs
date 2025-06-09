using UnityEngine;
using spelmanXR;

namespace DialogNodeBasedSystem.Demo.Scripts
{
    public class Demo2 : MonoBehaviour
    {
        [SerializeField] XDialog xDialog;

        [Tooltip("The node graph to execute.")]
        [SerializeField] DialogNodeGraph dialogNodeGraph;

        public AudioClip ClipA;
        public AudioClip ClipB;
        public AudioClip ClipC;

        AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();

            //subscribe to the NodeOpen Action (optional)
            xDialog.DialogNodeOpen.AddListener(DialogNodeOpen);

            /*
            //subscribe to the NodeOpen Action (optional)
            xDialog.DialogTextTypeOutCompleted.AddListener(DialogTextTypeOutCompleted);

            //subscribe to the NodeOpen Action (optional)
            xDialog.DialogNodeClose.AddListener(DialogNodeClose);
            */

            //play the dialog node graph
            xDialog.Play(dialogNodeGraph);
        }


        //demo the NodeOpen callback
        public void DialogNodeOpen(string token)
        {
            switch (token)
            {
                case "Clip_A":
                    audioSource.clip = ClipA;
                    audioSource.Play();
                    break;

                case "Clip_B":
                    audioSource.clip = ClipB;
                    audioSource.Play();
                    break;

                case "Clip_C":
                    audioSource.clip = ClipC;
                    audioSource.Play();
                    break;
            }
        }

        /*
        //demo the TypeOutCompleted callback
        public void DialogTextTypeOutCompleted(string token)
        {
            Debug.Log("Dialog TypeOut Completed:" + token);
        }


        //demo the NodeClose callback
        public void DialogNodeClose(string token)
        {
            Debug.Log("Dialog Node Closed:" + token);
        }
        */
    }
}