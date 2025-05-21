using UnityEngine;
using cherrydev;

namespace DialogNodeBasedSystem.Demo.Scripts
{
    public class TestVrDialog : MonoBehaviour
    {
        [SerializeField] VrDialog vrDialog;
        private void Start()
        {
            vrDialog.Play();
        }

        public void DialogTextTypeOutCompleted(string token)
        {
            Debug.Log("TestVrDialog:DialogTextTypeOutCompleted() :" + token);
        }

        public void DialogNodeOpen(string token)
        {
            Debug.Log("TestVrDialog:DialogNodeOpen() :" + token);
        }

        public void DialogNodeClose(string token)
        {
            Debug.Log("TestVrDialog:DialogNodeClose() :" + token);
        }
    }
}