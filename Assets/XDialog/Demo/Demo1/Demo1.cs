using UnityEngine;
using cherrydev;

namespace DialogNodeBasedSystem.Demo.Scripts
{
    public class Demo1 : MonoBehaviour
    {
        [SerializeField] XDialog xDialog;

        [Tooltip("The node graph to execute.")]
        [SerializeField] DialogNodeGraph dialogNodeGraph;

        bool play = true;
        private void Start()
        {
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

        private void Update()
        {
            if (play)
            {
                play = false;
                //xDialog.Play(dialogNodeGraph);

            }
        }

        //demo the NodeOpen callback
        public void DialogNodeOpen(string token)
        {
            Debug.Log("Dialog Node Opened:" + token);
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