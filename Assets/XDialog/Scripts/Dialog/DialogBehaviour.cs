using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace cherrydev
{
    public class DialogBehaviour : MonoBehaviour
    {
        [SerializeField] private List<KeyCode> _nextSentenceKeyCodes;

        //There are 3 ways to trigger loaddig the next node.
        //1) programmatically, by calling the GotoNextNode() function.
        //pass the index of the child node to load.
        //2) click one of the dialog buttons on the curently displayed node.
        // This also calls the GotoNextNode() function, passing the index of
        // the button that was clicked.
        //3) pressing select keys on the keyboard.  With this option, the
        //first child node will be loaded.  You cannot specify which child.
        //The following booleans are used to trigger transitions to the child node.
        public bool ProgramTrigNodeTransition = false;      //program or button click
        public bool KeyboardTrigNodeTransition = false;     //keyboard 

        //The child node to load next.  Use in conjunction with the trigger
        //booleans above.
        int _ChildNodeToLoadIndex = 0;

        //the first node to load.  This is the child of the START node.
        private Node _firstNode;

        //are referene to the dialog node currently displayed
        public DialogNode CurrentDialogNode { get; private set; }

        public event Action DialogStarted;
        public event Action DialogFinished;
        public event Action <string>DialogTextTypeOutCompleted;
        public event Action <string>DialogNodeOpen;
        public event Action <string>DialogNodeClose;
        public event Action<DialogNode> DialogNodeSetUp;
        public event Action DialogTextCharWritten;
        public event Action<string> DialogTextSkipped;


        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            //Check if there is a keyboard request to transition to the next node.
            //In that case, always transition to the child node that corresponds to the
            //position of the ky in the _nextSentenceKeyCodes list.
            for (int i = 0; i < _nextSentenceKeyCodes.Count; i++)
            {
                //avoid a childnode index error... we have more defined transition keys than we have child nodes.
                if (i >= CurrentDialogNode.ChildNodes.Count) return;

                if (Input.GetKeyDown(_nextSentenceKeyCodes[i]))
                {
                    _ChildNodeToLoadIndex = i;       //load the ith child node
                    KeyboardTrigNodeTransition = true;
                }
            }
        }

        /// <summary>
        /// Start a dialog
        /// </summary>
        /// <param name="dialogNodeGraph"></param>
        public void StartDialog(DialogNodeGraph dialogNodeGraph)
        {
            if (dialogNodeGraph.NodesList == null)
            {
                Debug.LogWarning("Dialog Graph's node list is empty");
                return;
            }

            DialogStarted?.Invoke();

            FindFirstNode(dialogNodeGraph);

            //if the START node is disconnected, exit immediately
            if (!_firstNode) return;

            HandleDialogGraphCurrentNode(_firstNode);
        }

        public void GoToNextNode(int childNodeIndex)
        {
            _ChildNodeToLoadIndex = childNodeIndex;
            ProgramTrigNodeTransition = true;
        }



        /// <summary>
        /// Processing dialog current node
        /// </summary>
        /// <param name="currentNode"></param>
        public void HandleDialogGraphCurrentNode(Node currentNode)
        {
            DialogNode dialogNode = currentNode as DialogNode;
            //Debug.Log("HandleDialogGraphCurrentNode() on node " + dialogNode.name);

            StopAllCoroutines();

            /*** START ***/
            DialogNodeOpen?.Invoke(dialogNode.nodeData.ExternalFunctionToken);

            HandleDialogNode(currentNode);
        }


        /// <summary>
        /// Processing answer node
        /// </summary>
        /// <param name="currentNode"></param>
        private void HandleDialogNode(Node currentNode)
        {
            DialogNode dialogNode = (DialogNode)currentNode;
            CurrentDialogNode = dialogNode;

            DialogNodeSetUp?.Invoke(dialogNode);

            string dialogText = dialogNode.nodeData.DialogText;
            WriteDialogText(dialogText);
        }

        /// <summary>
        /// Finds the start node
        /// </summary>
        /// <param name="dialogNodeGraph"></param>
        private void FindFirstNode(DialogNodeGraph dialogNodeGraph)
        {
            _firstNode = null;

            DialogNode dn;

            //Debug.Log("DialogBehaviour:There are " + dialogNodeGraph.NodesList.Count + " nodes.");
            for (int i = 0; i < dialogNodeGraph.NodesList.Count; i++)
            {
                dn = (DialogNode)dialogNodeGraph.NodesList[i];

                if (dn != null)
                {
                    if (dn.IsStartNode())
                    {
                        //set _firstNode to the START node's first (and only) child
                        _firstNode = dn.ChildNodes[0].ChildNode;
                        //Debug.Log("Start node is " + ((DialogNode)_firstNode).name);
                        return;
                    }
                }
            }

            Debug.LogWarning("WARNING: No START node found.");

        }

        /// <summary>
        /// Writing dialog text
        /// </summary>
        /// <param name="text"></param>
        private void WriteDialogText(string text) => StartCoroutine(WriteDialogTextRoutine(text));

        /// <summary>
        /// Writing dialog text coroutine
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private IEnumerator WriteDialogTextRoutine(string text)
        {
            if (CurrentDialogNode.nodeData.TypeDelay == 0)
            {
                //do not type out the dialog text: just display it
                DialogTextSkipped?.Invoke(text);
            }
            else
            {
                //here, we will type out the dialot text character by character
                foreach (char textChar in text)
                {
                    DialogTextCharWritten?.Invoke();

                    //delay between characters
                    yield return new WaitForSeconds(CurrentDialogNode.nodeData.TypeDelay);

                    //if we have a request to transition, exit the typing loop.
                    if (ProgramTrigNodeTransition || KeyboardTrigNodeTransition) break;
                }
            }

            //invoke the TypeOutCompleted event
            DialogTextTypeOutCompleted?.Invoke(CurrentDialogNode.nodeData.ExternalFunctionToken);
            
            yield return new WaitUntil(CheckTransitionToNextNode);

        }


        /// <summary>
        /// There are 3 ways we can advance to the next node.
        ///     1) One of the option buttons on the current node has been clicked, so
        ///     we need to load the appropriate child node.
        ///     2) The keyboard was used to signal that we need to load the next node.
        ///     3) We received a programmatic GotoNextNode() call.  Some code is
        ///     requesting that we load a specific child node.
        ///     
        /// Checking whether at least one key from the nextSentenceKeyCodes was pressed
        /// or one of the choice buttons was pushed or we got a command to advance to
        /// the next node.
        /// </summary>
        /// <returns></returns>
        private bool CheckTransitionToNextNode()
        {
            //Either a choice button was clicked or the GotoNextNode() function
            //was called or the keyboard trigger key was hit
            if (ProgramTrigNodeTransition || KeyboardTrigNodeTransition)
            {
                ProgramTrigNodeTransition = false;
                KeyboardTrigNodeTransition = false;

                PrepareNextPanel();
                return true;
            }

            return false;
        }


        /// <summary>
        /// Prepares the next panel to load
        /// </summary>
        /// <param name="childIndex"></param>
        void PrepareNextPanel()
        {
            //signal that the current node is closing
            DialogNodeClose?.Invoke(CurrentDialogNode.nodeData.ExternalFunctionToken);

            Node childNodeToLoad = null;

            //get the target node to load if the index is in range
            if (_ChildNodeToLoadIndex < CurrentDialogNode.ChildNodes.Count)
            {
                childNodeToLoad = CurrentDialogNode.ChildNodes[_ChildNodeToLoadIndex].ChildNode;
            }

            if(childNodeToLoad)
            { 
                //Debug.Log("DialogBehaviour:LoadNextPanel() ... loading child " + _ChildNodeToLoadIndex);
                HandleDialogGraphCurrentNode(childNodeToLoad);  //load next node
                return;
            }

            //Here, either the requested child index was out of range or not
            //connected. Either way, terminate the dialog.
            DialogFinished?.Invoke();

        }
    }
}