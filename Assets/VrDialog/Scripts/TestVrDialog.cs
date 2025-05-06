using UnityEngine;
using cherrydev;

namespace DialogNodeBasedSystem.Demo.Scripts
{
    public class TestVrDialog : MonoBehaviour
    {
        [SerializeField] VrDialog vrDialog;
        private void Start()
        {
            vrDialog.BindExternalFunction("Test", DebugExternal);
            vrDialog.Play();
        }

        void DebugExternal() => Debug.Log("External function works!");
    }
}