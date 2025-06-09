using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using spelmanXR;
using UnityEngine.Events;

public class XDialog : MonoBehaviour
{
    /// <summary>
    /// XDialog is an enhancement of the Node Based Dialog System by
    /// cherrydev (NBDS v2.0.1.)
    /// This class encapsulates the base functions of this enhanced dialog
    /// system through a pair of prefabs: XDialog_Desktop and XDialog_VR.
    /// As the names suggests, the XDialog_Desktop prefab is intended for
    /// desktop and mobile applications (applications with a screen).  Here,
    /// the dialog system is implemented as a screen overaly.  XDialog_VR is
    /// intended for VR applications (where there is not screen).  Here, the
    /// dialog system is implemented as a world space object that is a child
    /// of the VR camera.  Here are the steps to using the XDialog system:
    /// 1) Instantiate the XDialog object
    /// For desktop/mobile apps, instantiate the XDialog_Desktop prefab
    /// somewhere in your scene.  For VR applications, instantiate the
    /// XDialog_VR prefab as a child of the Main Camera.
    /// 2) Run a Dailog Node Graph
    /// 2a - You can specify the Dialog Node Graph (DNG) to be played in the XDialog
    /// inspector panel.  Check the "Play on Awake" checkbox to auto-play on
    /// startup.
    /// 2b - You can programmatically run a DNG by calling the
    /// XDialog Play() function, specifying the DNG to play.
    /// 3) Respond to events
    /// XDialog publishes 3 events to which a program may choose to respond:
    /// 3a - DialogNodeOpen - this event fires before each node of the DNG
    /// being processed is executed.
    /// 3b - DialogTextTypeOutCompleted - this even fires after the task of
    /// typing out the main lines of text in the dialog is completed.
    /// 3c - DialogNodeClose - this event fires after each node of the running
    /// DNG is executed.
    ///
 
    /// Now, access the XDialog with two functions:
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
    [SerializeField] bool PlayOnAwake = false;
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

        if (dialogNodeGraph)
        {
            _dialogBehaviour.StartDialog(dialogNodeGraph);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        if (PlayOnAwake)
        {
            Play();
        }
    }
}
