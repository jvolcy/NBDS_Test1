using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cherrydev;
using UnityEngine.Events;

public class XDialog : MonoBehaviour
{
    /// <summary>
    /// VrDialog encapsulates the modified behavior of the NBDS v2.0.1.
    /// Simply instantiate the VrDialog prefab as a child of the Main Camera
    /// (or the Main Camera's root).  Then, specify the Dialog Node Graph
    /// to be played in the VrDialog inspector panel.
    /// Now, access the VrDialog with two functions:
    /// 1) BindExternalFunction() --> this function provides a call back for
    /// every node in the graph that provides one.  Specify the a function ID
    /// (called function name) which is a unique arbitrary string in the
    /// graph node.  This string is used by the BindExternalFunction to map
    /// node callbacks to actual functions.  Provide a reference to a function
    /// of type void Function (void) as the callback.
    /// 2) Play() --> use this function to begin playing the dialog node graph
    /// </summary>

    [Tooltip("The node graph to execute.")]
    [SerializeField] DialogNodeGraph dialogNodeGraph;
    private DialogBehaviour _dialogBehaviour;
    [Space(5)]
    [Header("Callbacks")]
    [Tooltip("A function to be called when a dialog node is opened.  The node's token string is passed to the function.")]
    public UnityEvent<string> DialogNodeOpen;
    [Tooltip("A function to be called when typeout of the dialog text is completed.  The node's token string is passed to the function.")]
    public UnityEvent <string> DialogTextTypeOutCompleted;
    [Tooltip("A function to be called when a dialog node is closed.  The node's token string is passed to the function.")]
    public UnityEvent<string> DialogNodeClose;

    // Awake is called before Start()
    void Awake()
    {
        //because we may need to bind functions in another script's
        //Start() function, we get a reference to the needed DialogBehaviour
        //object here in Awake, before it is needed by any other object's
        //Start() method.
        _dialogBehaviour = GetComponent<DialogBehaviour>();

        _dialogBehaviour.DialogTextTypeOutCompleted += (val) => DialogTextTypeOutCompleted?.Invoke(val);
        _dialogBehaviour.DialogNodeOpen += (val) => DialogNodeOpen?.Invoke(val);
        _dialogBehaviour.DialogNodeClose += (val) => DialogNodeClose?.Invoke(val);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Play(DialogNodeGraph dng = null)
    {
        if (dng)
        {
            dialogNodeGraph = dng;
        }

        _dialogBehaviour.StartDialog(dialogNodeGraph);
    }

}
