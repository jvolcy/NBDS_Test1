using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace cherrydev
{
    public class NodeEditor : EditorWindow
    {
        private static DialogNodeGraph _currentNodeGraph;
        private static NodeEditor _nodeEditor;
        private Node _currentNode;
        private Node _nodeToDragLineFrom;

        private GUIStyle _nodeStyle;
        private GUIStyle _selectedNodeStyle;
        private GUIStyle _selectedStartNodeStyle;

        private readonly Color _headerColor = new(0.235f, 0.235f, 0.235f);
        private readonly Color _backgroundColor = new(0.165f, 0.165f, 0.165f);
        private readonly Color _backgroundLinesColor = new(0.113f, 0.113f, 0.113f);

        private GUIStyle _toolbarButtonStyle;
        private GUIStyle _headerLabelStyle;
        private GUIStyle _dropdownStyle;
        private GUIStyle _searchFieldStyle;

        private GUIStyle _labelStyle;
        private GUIStyle _startLabelStyle;

        private bool toolbarStylesInitialized = false;

        private Rect _selectionRect;
        private Vector2 _mouseScrollClickPosition;

        private Vector2 _graphOffset;
        private Vector2 _graphDrag;

        private GUIStyle _activeToolbarButtonStyle;

        private const float ToolbarHeight = 30f;

        private const float ConnectingLineWidth = 2f;
        private const float ConnectingLineArrowSize = 8f;

        private const int LabelFontSize = 12;

        private const int NodePadding = 20;
        private const int NodeBorder = 10;

        private const float GridLargeLineSpacing = 100f;
        private const float GridSmallLineSpacing = 25;

        private bool _isLeftMouseDragFromEmpty;
        private bool _showLocalizationKeys;
        private bool _isMiddleMouseClickedOnNode;

        // Search functionality
        private string _searchText = "";

        //Creative Random Node Names
        readonly string[] Adjectives = { "Active", "Adaptable", "Adventurous", "Affectionate", "Alert", "Artistic", "Assertive", "Boundless", "Brave", "Broad-minded", "Calm", "Capable", "Careful", "Caring", "Cheerful", "Clever", "Comfortable", "Communicative", "Compassionate", "Conscientious", "Considerate", "Courageous", "Creative", "Curious", "Decisive", "Determined", "Diligent", "Dynamic", "Eager", "Energetic", "Entertaining", "Enthusiastic", "Exuberant", "Expressive", "Fabulous", "Fair-minded", "Fantastic", "Fearless", "Flexible thinker", "Frank", "Friendly", "Funny", "Generous", "Gentle", "Gregarious", "Happy", "Hard working", "Helpful", "Hilarious", "Honest", "Imaginative", "Independent", "Intellectual", "Intelligent", "Intuitive", "Inventive", "Joyous", "Kind", "Kind-hearted", "Knowledgable", "Level-headed", "Lively", "Loving", "Loyal", "Mature", "Modest", "Optimistic", "Outgoing", "Passionate", "Patient", "Persistent", "Philosophical", "Polite", "Practical", "Pro-active", "Productive", "Quick-witted", "Quiet", "Rational", "Receptive", "Reflective", "Reliable", "Resourceful", "Responsible", "Selective", "Self-confident", "Sensible", "Sensitive", "Skillful", "Straightforward", "Successful", "Thoughtful", "Trustworthy", "Understanding", "Versatile", "Vivacious", "Warm-hearted", "Willing", "Witty", "Wonderful" };
        readonly string[] Animals = { "Aardvark", "Alligator", "Antelope", "Badger", "Bat", "Bear", "Bee", "Beetle", "Blue whale", "Bulldog", "Butterfly", "Camel", "Cat", "Caterpillar", "Cheetah", "Chicken", "Chimpanzee", "Clam", "Cow", "Coyote", "Crab", "Crocodile", "Cuttlefish", "Deer", "Dog", "Dolphin", "Donkey", "Duck", "Dugong", "Elephant", "Elk", "Fire Ant", "Fish", "Fox", "Frog", "Gazelle", "Giraffe", "Goat", "Goose", "Gorilla", "Guinea Pig", "Hare", "Hedgehog", "Hen", "Hippopotamus", "Horse", "Jackrabbit", "Jelly Fish", "Kangaroo", "Koala", "Leopard", "Lion", "Lizard", "Lobster", "Manatee", "Meerkat", "Millipede", "Mole", "Monkey", "Mosquito", "Nudibranch", "Octopus", "Otter", "Owl", "Oyster", "Panda", "Pelican", "Pig", "Porcupine", "Rabbit", "Raccoon", "Rat", "Reindeer", "Rhinoceros", "Scorpion", "Sea Lion", "Seahorse", "Seal", "Shark", "Sheep", "Shrimp", "Sidewinder", "Snake", "Spider", "Squid", "Squirrel", "Starfish", "Swordfish", "Tiger", "Toad", "Turkey", "Turtle", "Urchin", "Walrus", "Whale", "Wolf", "Wombat", "Woodpecker", "Yucca Moth", "Zebra" };

        private static int HighestNodeID = 0;

        /// <summary>
        /// Helper function to create a constant color texture
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        private Texture2D MakeTex(Color col)
        {
            Color[] pix = new Color[1];

            pix[0] = col;

            Texture2D result = new Texture2D(1, 1);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }


        /// <summary>
        /// Define nodes and label style parameters on enable
        /// </summary>
        private void OnEnable()
        {
            Debug.Log("NodeEditor:OnEnable()...");

            _nodeEditor = this;

            Selection.selectionChanged += ChangeEditorWindowOnSelection;

            //InitializeToolbarStyles();

            _nodeStyle = new GUIStyle();
            _nodeStyle.normal.background = EditorGUIUtility.Load(StringConstants.Node) as Texture2D;
            _nodeStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
            _nodeStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);

            _selectedNodeStyle = new GUIStyle();
            _selectedNodeStyle.normal.background = MakeTex(new Color32(0x18, 0x3c, 0x79, 0xff));
            _selectedNodeStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
            _selectedNodeStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);


            _selectedStartNodeStyle = new GUIStyle();
            _selectedStartNodeStyle.normal.background = EditorGUIUtility.Load(StringConstants.SelectedNode) as Texture2D;
            _selectedStartNodeStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
            _selectedStartNodeStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);


            _labelStyle = new GUIStyle();
            _labelStyle.alignment = TextAnchor.MiddleLeft;
            _labelStyle.fontSize = LabelFontSize;
            _labelStyle.normal.textColor = Color.white;
            _labelStyle.clipping = TextClipping.Clip;
            _labelStyle.normal.background = MakeTex(new Color32(0x10, 0x7a, 0xfe, 0xff));


            _startLabelStyle = new GUIStyle();
            _startLabelStyle.alignment = TextAnchor.MiddleCenter;
            _startLabelStyle.fontSize = LabelFontSize;
            _startLabelStyle.normal.textColor = Color.yellow;
            _startLabelStyle.clipping = TextClipping.Clip;
            _startLabelStyle.normal.background = MakeTex(new Color32(0x0, 0xa, 0xe, 0x0));


        }


        /// <summary>
        /// Saving all changes and unsubscribing from events
        /// </summary>
        private void OnDisable()
        {
            Debug.Log("NodeEditor:OnDisable()...");

            CleanUpUnusedAssets();
            Selection.selectionChanged -= ChangeEditorWindowOnSelection;
            AssetDatabase.SaveAssets();
            SaveChanges();
        }

        /// <summary>
        /// Open Node Editor Window when Node Graph asset is double clicked in the inspector
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        [OnOpenAsset(0)]
        public static bool OnDoubleClickAsset(int instanceID, int line)
        {
            Debug.Log("NodeEditor:OnDoubleClickAsset()...Loading " + EditorUtility.InstanceIDToObject(instanceID).name);

            //load the DialogNodeGraph scriptable object
            _currentNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as DialogNodeGraph;


            if (_currentNodeGraph != null)
            {
                OpenWindow();
                SetUpNodes();

                //Add a Start Node, if one does not already exist
                _nodeEditor.CreateStartNode(Vector2.zero);

                //Find the highest NodeID
                _nodeEditor.FindHighestNodeID();

                return true;
            }

            Debug.Log("Could not load " + EditorUtility.InstanceIDToObject(instanceID).name);
            return false;
        }

        void FindHighestNodeID()
        {
            foreach (Node node in _currentNodeGraph.NodesList)
            {
                if (node.NodeID > HighestNodeID) { HighestNodeID = node.NodeID; }
            }
        }

        /// <summary>
        /// Function to create a static reference to the current node graph
        /// </summary>
        /// <param name="nodeGraph"></param>
        public static void SetCurrentNodeGraph(DialogNodeGraph nodeGraph) =>
            _currentNodeGraph = nodeGraph;

        /// <summary>
        /// Open Node Editor window
        /// </summary>
        [MenuItem("Dialog Node Based Editor", menuItem = "Window/Dialog Node Based Editor")]
        public static void OpenWindow()
        {
            NodeEditor window = (NodeEditor)GetWindow(typeof(NodeEditor));
            window.titleContent = new GUIContent("Dialog Graph Editor");
            window.Show();
        }

        /// <summary>
        /// Rendering and handling GUI events
        /// </summary>
        private void OnGUI()
        {
            /* Referencing EditorStyles in Enable() by calling 
             * InitializeToolbarStyles() leads to a Null reference error.
             * There is no guaranteed that styles will be initialized when
             * Enable() is called.  Rather, we delay and call the function
             * here in OnGUI().  The toolbarStylesInitialized bool is used
             * to avoid repeated initializations.
            */
            //if (!toolbarStylesInitialized) { InitializeToolbarStyles(); }
            InitializeToolbarStyles();

            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), _backgroundColor);
            DrawToolbar();
            GUI.BeginGroup(new Rect(0, ToolbarHeight, position.width, position.height - ToolbarHeight));

            if (_currentNodeGraph != null)
            {
                Undo.RecordObject(_currentNodeGraph, "Changed Value");
                DrawDraggedLine();
                DrawNodeConnection();
                DrawGridBackground(GridSmallLineSpacing, 0.3f, _backgroundLinesColor);
                DrawGridBackground(GridLargeLineSpacing, 0.3f, _backgroundLinesColor);
                ProcessEvents(Event.current);
                DrawNodes(Event.current);
            }

            GUI.EndGroup();

            if (GUI.changed)
                Repaint();
        }

        /// <summary>
        /// Setting up nodes when opening the editor
        /// </summary>
        private static void SetUpNodes()
        {
            foreach (Node node in _currentNodeGraph.NodesList)
            {
                DialogNode dialogNode = (DialogNode)node;
                dialogNode.SetDialogNodeSize();
            }
        }

        /// <summary>
        /// Initializes toolbar styles
        /// </summary>
        private void InitializeToolbarStyles()
        {
            //Debug.Log("InitializeToolbarStyles()...");
            _toolbarButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
            _toolbarButtonStyle.normal.textColor = Color.white;
            _toolbarButtonStyle.fontSize = 11;
            _toolbarButtonStyle.alignment = TextAnchor.MiddleCenter;
            _toolbarButtonStyle.fixedHeight = ToolbarHeight - 4;
            _toolbarButtonStyle.margin = new RectOffset(2, 2, 2, 2);
            _toolbarButtonStyle.padding = new RectOffset(6, 6, 2, 2);

            _activeToolbarButtonStyle = new GUIStyle(_toolbarButtonStyle);
            _activeToolbarButtonStyle.normal.background =
                EditorGUIUtility.Load("builtin skins/darkskin/images/btn act.png") as Texture2D;
            _activeToolbarButtonStyle.normal.textColor = Color.white;

            _headerLabelStyle = new GUIStyle(EditorStyles.label);
            _headerLabelStyle.normal.textColor = Color.white;
            _headerLabelStyle.fontStyle = FontStyle.Bold;
            _headerLabelStyle.fontSize = 12;
            _headerLabelStyle.alignment = TextAnchor.MiddleLeft;
            _headerLabelStyle.padding = new RectOffset(10, 10, 0, 0);

            _dropdownStyle = new GUIStyle(EditorStyles.popup);
            _dropdownStyle.normal.textColor = Color.white;
            _dropdownStyle.fontSize = 11;
            _dropdownStyle.alignment = TextAnchor.MiddleLeft;
            _dropdownStyle.fixedHeight = ToolbarHeight - 4;
            _dropdownStyle.margin = new RectOffset(2, 2, 2, 2);
            _dropdownStyle.padding = new RectOffset(6, 6, 2, 2);

            _searchFieldStyle = new GUIStyle(EditorStyles.toolbarTextField);
            _searchFieldStyle.normal.textColor = Color.white;
            _searchFieldStyle.fontSize = 11;
            _searchFieldStyle.alignment = TextAnchor.MiddleLeft;
            _searchFieldStyle.fixedHeight = ToolbarHeight - 4;
            _searchFieldStyle.margin = new RectOffset(2, 2, 2, 2);
            _searchFieldStyle.padding = new RectOffset(6, 6, 2, 2);

            toolbarStylesInitialized = true;
        }

        /// <summary>
        /// Clean up orphaned node assets that are sub-assets of the graph but not in the NodesList
        /// </summary>
        private void CleanUpUnusedAssets()
        {
            if (_currentNodeGraph == null)
                return;

            UnityEngine.Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(
                AssetDatabase.GetAssetPath(_currentNodeGraph));

            foreach (UnityEngine.Object subAsset in subAssets)
            {
                Node nodeAsset = subAsset as Node;

                if (nodeAsset == null)
                    continue;

                if (!_currentNodeGraph.NodesList.Contains(nodeAsset))
                    DestroyImmediate(nodeAsset, true);
            }
        }

        /// <summary>
        /// Draws toolbar with helper buttons
        /// </summary>
        private void DrawToolbar()
        {
            //Debug.Log("NodeEditor:DrawToolbar()...");

            EditorGUI.DrawRect(new Rect(0, 0, position.width, ToolbarHeight), _headerColor);
            GUILayout.BeginArea(new Rect(0, 0, position.width, ToolbarHeight));
            GUILayout.BeginHorizontal();

            if (_currentNodeGraph != null && _currentNodeGraph.NodesList != null &&
                _currentNodeGraph.NodesList.Count > 0)
            {
                Rect nodesButtonRect =
                    GUILayoutUtility.GetRect(new GUIContent("Nodes"), _toolbarButtonStyle, GUILayout.Width(100));
                if (GUI.Button(nodesButtonRect, "Nodes", _toolbarButtonStyle))
                    DrawNodesDropdown(nodesButtonRect);

                GUILayout.Space(10f);

                string newSearchText = EditorGUILayout.TextField(_searchText, _searchFieldStyle, GUILayout.Width(200));

                if (newSearchText != _searchText)
                {
                    _searchText = newSearchText;
                    if (!string.IsNullOrEmpty(_searchText))
                        SearchAndSelectNode(_searchText);
                }

                if (GUILayout.Button("Search", _toolbarButtonStyle, GUILayout.Width(60)))
                    SearchAndSelectNode(_searchText);

                if (!string.IsNullOrEmpty(_searchText))
                {
                    if (GUILayout.Button("×", _toolbarButtonStyle, GUILayout.Width(20)))
                        _searchText = "";
                }
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Find My Nodes", _toolbarButtonStyle, GUILayout.Width(100)))
                CenterWindowOnNodes();

            if (GUILayout.Button("Edit Table Keys",
                    _showLocalizationKeys ? _activeToolbarButtonStyle : _toolbarButtonStyle, GUILayout.Width(100)))
            {
                _showLocalizationKeys = !_showLocalizationKeys;
                DialogNodeGraph.ShowLocalizationKeys = _showLocalizationKeys;
                GUI.changed = true;
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Searches for a node containing the search text and selects it
        /// </summary>
        /// <param name="searchText">The text to search for</param>
        private void SearchAndSelectNode(string searchText)
        {
            if (string.IsNullOrEmpty(searchText) || _currentNodeGraph == null || _currentNodeGraph.NodesList == null)
                return;

            searchText = searchText.ToLower();

            foreach (Node node in _currentNodeGraph.NodesList)
            {

                    DialogNode dialogNode = (DialogNode)node;
                    bool found = false;


                    foreach (DialogNode.ChildNodeStruct cns in dialogNode.ChildNodes)
                    {
                        if (!string.IsNullOrEmpty(cns.ChoiceText) && cns.ChoiceText.ToLower().Contains(searchText))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        CenterAndSelectNode(node);
                        return;
                    }
            }

            // If we got here, no node was found
            Debug.Log($"No node containing '{searchText}' was found.");
        }

        /// <summary>
        /// Draws the nodes dropdown in the toolbar
        /// </summary>
        /// <param name="buttonRect">The rectangle of the button that triggered the dropdown</param>
        private void DrawNodesDropdown(Rect buttonRect)
        {
            GenericMenu nodesMenu = new GenericMenu();

            foreach (Node node in _currentNodeGraph.NodesList)
            {
                string prefix;
                string nodeText;

                DialogNode dialogNode = (DialogNode)node;

                prefix = "A";
                /*
                if (dialogNode.name != null && dialogNode.ChildNodes.Count > 0 &&
                    !string.IsNullOrEmpty(dialogNode.ChildNodes[0].ChoiceText))
                    nodeText = dialogNode.ChildNodes[0].ChoiceText;
                else
                    nodeText = "Empty";
                */

                if (dialogNode.name.Length > 20)
                    nodeText = dialogNode.name.Substring(0, 20) + "...";
                else
                    nodeText = dialogNode.name;

                string menuItemName = $"{prefix}: {nodeText}";
                nodesMenu.AddItem(new GUIContent(menuItemName), false, () => CenterAndSelectNode(node));
            }

            Rect dropDownRect = new Rect(buttonRect.x, buttonRect.y + buttonRect.height, 150, 0);
            nodesMenu.DropDown(dropDownRect);
        }

        /// <summary>
        /// Centers the view on a specific node and selects it
        /// </summary>
        /// <param name="nodeToCenter">The node to center on and select</param>
        private void CenterAndSelectNode(Node nodeToCenter)
        {
            if (nodeToCenter == null)
                return;

            Vector2 windowCenter = new Vector2(position.width / 2, (position.height - ToolbarHeight) / 2);
            Vector2 offset = windowCenter - nodeToCenter.Rect.center;

            foreach (Node node in _currentNodeGraph.NodesList)
                node.DragNode(offset);

            foreach (Node node in _currentNodeGraph.NodesList)
                node.IsSelected = false;

            nodeToCenter.IsSelected = true;
            _currentNode = nodeToCenter;

            GUI.changed = true;
        }

        /// <summary>
        /// Draw connection line when we drag it
        /// </summary>
        private void DrawDraggedLine()
        {
            if (_currentNodeGraph.LinePosition != Vector2.zero)
            {
                Handles.DrawBezier(_currentNodeGraph.NodeToDrawLineFrom.Rect.center, _currentNodeGraph.LinePosition,
                    _currentNodeGraph.NodeToDrawLineFrom.Rect.center, _currentNodeGraph.LinePosition,
                    Color.white, null, ConnectingLineWidth);
            }
        }

        /// <summary>
        /// Draw connections in the editor window between nodes
        /// </summary>
        private void DrawNodeConnection()
        {
            if (_currentNodeGraph.NodesList == null)
                return;

            foreach (Node node in _currentNodeGraph.NodesList)
            {
                DialogNode parentNode = (DialogNode)node;
                DialogNode childNode;

                for (int i = 0; i < parentNode.ChildNodes.Count; i++)
                {
                    childNode = parentNode.ChildNodes[i].ChildNode as DialogNode;

                    //Add the child's name to the node's choice text field
                    DialogNode.ChildNodeStruct cns = parentNode.ChildNodes[i];
                    if (childNode)
                    {
                        DrawBezierConnector(parentNode, childNode, i);
                        //cns.ChoiceText = childNode.name;
                    }
                    else
                    {
                        //if no child is connected, set the field to an empty string
                        //cns.ChoiceText = "";
                    }
                    parentNode.ChildNodes[i] = cns;

                }
            }
        }

        /// <summary>
        /// Draw connection Bezier from parent to child node
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="childNode"></param>
        private void DrawBezierConnector(DialogNode parentNode, DialogNode childNode, int index)
        {

            Vector2 startPosition;
            if (parentNode.ChildNodes.Count == 0)
                startPosition = parentNode.Rect.center;
            else
                startPosition = parentNode.ChildNodes[index].ChildConnectionPoint;

            //Debug.Log("DrawBezierConnector() -> ChildNode.Count, ChildConnectionPoint.Count = " + parentNode.ChildNodes.Count + ", " + parentNode.ChildConnectionPoint.Count);

            //Debug.Log("index = " + index);
            //Debug.Log("------" + parentNode.nodeData.DialogText + "------");
            //Debug.Log(parentNode.ChildConnectionPoint.Count + " index = " + index);
            //Debug.Log("startPosition = " + startPosition);
            

            Vector2 endPosition = new Vector2(childNode.Rect.xMin, childNode.Rect.center.y);

            float distance = Vector2.Distance(startPosition, endPosition);

            Vector2 startTangent = startPosition + Vector2.right * (distance * 0.5f);
            Vector2 endTangent = endPosition + Vector2.left * (distance * 0.5f);

            Handles.DrawBezier(
                startPosition,
                endPosition,
                startTangent,
                endTangent,
                Color.white,
                null,
                ConnectingLineWidth
            );

            Vector3[] bezierPoints = Handles.MakeBezierPoints(
                startPosition,
                endPosition,
                startTangent,
                endTangent,
                20
            );

            Vector2 midPosition = bezierPoints[bezierPoints.Length / 2];
            Vector2 direction = (endPosition - startPosition).normalized;

            Handles.color = Color.white;
            Handles.DrawSolidDisc(bezierPoints[bezierPoints.Length-1], Vector3.forward, 6f);


            if (!parentNode.IsStartNode())     //place a number at the center of the Bezier curve
            {
                string indexText = (index + 1).ToString();

                Handles.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                Handles.DrawSolidDisc(midPosition, Vector3.forward, 12f);

                Handles.color = Color.white;

                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 12;
                style.fontStyle = FontStyle.Bold;

                Handles.BeginGUI();
                GUI.Label(new Rect(midPosition.x - 10, midPosition.y - 10, 20, 20), indexText, style);
                Handles.EndGUI();
            }
            else    //draw an error at the center of the Bezier curve
                DrawArrowAtMidpoint(midPosition, direction);

            GUI.changed = true;
        }
        
        /// <summary>
        /// Draw arrow at the midpoint of a connection line
        /// </summary>
        /// <param name="midPosition">Midpoint of the line</param>
        /// <param name="direction">Direction of the line</param>
        private void DrawArrowAtMidpoint(Vector2 midPosition, Vector2 direction)
        {
            Vector2 arrowTail1 = midPosition - new Vector2(-direction.y, direction.x).normalized * ConnectingLineArrowSize;
            Vector2 arrowTail2 = midPosition + new Vector2(-direction.y, direction.x).normalized * ConnectingLineArrowSize;
            Vector2 arrowHead = midPosition + direction * ConnectingLineArrowSize;

            Handles.DrawBezier(arrowHead, arrowTail1, arrowHead, arrowTail1, Color.white, null, ConnectingLineWidth);
            Handles.DrawBezier(arrowHead, arrowTail2, arrowHead, arrowTail2, Color.white, null, ConnectingLineWidth);
        }

        /// <summary>
        /// Draw grid background lines for node editor window
        /// </summary>
        /// <param name="gridSize"></param>
        /// <param name="gridOpacity"></param>
        /// <param name="color"></param>
        /// <param name="lineWidth"></param>
        private void DrawGridBackground(float gridSize, float gridOpacity, Color color, float lineWidth = 4f)
        {
            int verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
            int horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);

            Color finalColor = new(color.r, color.g, color.b, gridOpacity);
            Handles.color = finalColor;

            _graphOffset += _graphDrag * 0.5f;
            Handles.BeginGUI();

            Vector3 gridOffset = new(_graphOffset.x % gridSize, _graphOffset.y % gridSize, 0);
            for (int i = 0; i < verticalLineCount; i++)
            {
                Vector3 p1 = new Vector3(gridSize * i, -gridSize, 0) + gridOffset;
                Vector3 p2 = new Vector3(gridSize * i, position.height + gridSize, 0) + gridOffset;

                Handles.DrawAAPolyLine(lineWidth, p1, p2);
            }

            for (int j = 0; j < horizontalLineCount; j++)
            {
                Vector3 p1 = new Vector3(-gridSize, gridSize * j, 0) + gridOffset;
                Vector3 p2 = new Vector3(position.width + gridSize, gridSize * j, 0f) + gridOffset;

                Handles.DrawAAPolyLine(lineWidth, p1, p2);
            }

            Handles.EndGUI();
            Handles.color = Color.white;
        }

        /// <summary>
        /// Call Draw method from all existing nodes in nodes list
        /// </summary>
        private void DrawNodes(Event currentEvent)
        {
            if (_currentNodeGraph.NodesList.Count == 0)
                return;

            foreach (DialogNode dialogNode in _currentNodeGraph.NodesList)
            {
                if (dialogNode.IsStartNode())
                {
                    dialogNode.Draw(!dialogNode.IsSelected ? _nodeStyle : _selectedStartNodeStyle, _startLabelStyle);
                }
                else
                {
                    dialogNode.Draw(!dialogNode.IsSelected ? _nodeStyle : _selectedNodeStyle, _labelStyle);
                }
            }

            if (_isLeftMouseDragFromEmpty)
                SelectNodesBySelectionRect(currentEvent.mousePosition);

            GUI.changed = true;
        }

        /// <summary>
        /// Process events
        /// </summary>
        /// <param name="currentEvent"></param>
        private void ProcessEvents(Event currentEvent)
        {
            _graphDrag = Vector2.zero;

            if (currentEvent.type == EventType.MouseUp)
                ProcessMouseUpEvent(currentEvent);

            if (_currentNode == null || _currentNodeGraph.NodeToDrawLineFrom != null || currentEvent.button == 2)
                ProcessNodeEditorEvents(currentEvent);
            else
                _currentNode.ProcessNodeEvents(currentEvent);
        }

        /// <summary>
        /// Process mouse up event
        /// </summary>
        /// <param name="currentEvent"></param>
        private void ProcessMouseUpEvent(Event currentEvent)
        {
            //Debug.Log("NodeEditor: ProcessMouseUpEvent("+ currentEvent.button + ")...");
            if (currentEvent.button == 0)
            {
                _currentNode = null;
                _isLeftMouseDragFromEmpty = false;
                _selectionRect = new Rect(0, 0, 0, 0);
            }
            else if (currentEvent.button == 1)
            {
                ProcessRightMouseUpEvent(currentEvent);
            }
            else if (currentEvent.button == 2)
            {
                ProcessMiddleMouseUpEvent(currentEvent);
            }
        }

        private void ProcessMiddleMouseUpEvent(Event currentEvent)
        {
            //Debug.Log("NodeEditor: ProcessMiddleMouseUpEvent()...");

            //Debug.Log("NodeEditor:PMMUE [0]");
            if (_currentNodeGraph.NodeToDrawLineFrom != null)
            {
                //Debug.Log("NodeEditor:PMMUE [1]");
                CheckLineConnection(currentEvent);
                //Debug.Log("NodeEditor:PMMUE [2]");
                ClearDraggedLine();
            }
        }

        /// <summary>
        /// Process all events
        /// </summary>
        /// <param name="currentEvent"></param>
        private void ProcessNodeEditorEvents(Event currentEvent)
        {
            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    //Debug.Log("NodeEditor: ProcessNodeEditorEvents(MouseDown)...");
                    ProcessMouseDownEvent(currentEvent);
                    break;
                case EventType.MouseDrag:
                    //Debug.Log("NodeEditor: ProcessNodeEditorEvents(MouseDrag)...");
                    ProcessMouseDragEvent(currentEvent);
                    break;
            }
        }

        /// <summary>
        /// Process mouse down event
        /// </summary>
        /// <param name="currentEvent"></param>
        private void ProcessMouseDownEvent(Event currentEvent)
        {
            if (currentEvent.button == 1)
                ProcessRightMouseDownEvent(currentEvent);
            else if (currentEvent.button == 0)
                ProcessLeftMouseDownEvent(currentEvent);
            else if (currentEvent.button == 2)
                ProcessMiddleMouseDownEvent(currentEvent);
        }

        /// <summary>
        /// Process middle mouse button down event
        /// </summary>
        /// <param name="currentEvent"></param>
        private void ProcessMiddleMouseDownEvent(Event currentEvent)
        {
            Node node = GetHighlightedNode(currentEvent.mousePosition);

            if (node != null)
            {
                _nodeToDragLineFrom = node;
                _currentNodeGraph.SetNodeToDrawLineFromAndLinePosition(_nodeToDragLineFrom,
                    currentEvent.mousePosition);
            }

            _isMiddleMouseClickedOnNode = node != null;
            _mouseScrollClickPosition = currentEvent.mousePosition;
        }

        /// <summary>
        /// Process right mouse click event
        /// </summary>
        /// <param name="currentEvent"></param>
        private void ProcessRightMouseDownEvent(Event currentEvent)
        {
            Node clickedNode = GetHighlightedNode(currentEvent.mousePosition);

            if (clickedNode != null)
            {
                foreach (Node node in _currentNodeGraph.NodesList)
                    node.IsSelected = false;

                clickedNode.IsSelected = true;
            }
        }

        /// <summary>
        /// Process left mouse click event
        /// </summary>
        /// <param name="currentEvent"></param>
        private void ProcessLeftMouseDownEvent(Event currentEvent)
        {
            if (_isLeftMouseDragFromEmpty)
                return;

            Node clickedNode = GetHighlightedNode(currentEvent.mousePosition);

            if (clickedNode == null)
            {
                _currentNode = null;
                _mouseScrollClickPosition = currentEvent.mousePosition;
                _isLeftMouseDragFromEmpty = true;
            }
            else
            {
                SelectOnlyHighlightedNode(currentEvent.mousePosition);
                ProcessNodeSelection(currentEvent.mousePosition);
            }
        }

        /// <summary>
        /// Process right mouse up event
        /// </summary>
        /// <param name="currentEvent"></param>
        private void ProcessRightMouseUpEvent(Event currentEvent) => ShowContextMenu(currentEvent.mousePosition);

        /// <summary>
        /// Process mouse drag event
        /// </summary>
        /// <param name="currentEvent"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ProcessMouseDragEvent(Event currentEvent)
        {
            if (currentEvent.button == 0)
                ProcessLeftMouseDragEvent(currentEvent);
            else if (currentEvent.button == 1)
                ProcessRightMouseDragEvent(currentEvent);
            else if (currentEvent.button == 2)
                ProcessMiddleMouseDragEvent(currentEvent);
        }

        /// <summary>
        /// Process middle mouse drag event (graph dragging)
        /// </summary>
        /// <param name="currentEvent"></param>
        private void ProcessMiddleMouseDragEvent(Event currentEvent)
        {
            if (!_isMiddleMouseClickedOnNode)
            {
                _graphDrag = currentEvent.delta;

                foreach (var node in _currentNodeGraph.NodesList)
                    node.DragNode(_graphDrag);

                GUI.changed = true;
            }
            else
            {
                if (_currentNodeGraph.NodeToDrawLineFrom != null)
                {
                    DragConnectionLine(currentEvent.delta);
                    GUI.changed = true;
                }
            }
        }

        /// <summary>
        /// Process left mouse drag event
        /// </summary>
        /// <param name="currentEvent"></param>
        private void ProcessLeftMouseDragEvent(Event currentEvent)
        {
            if (_isLeftMouseDragFromEmpty)
            {
                GUI.changed = true;
                return;
            }

            Node node = GetHighlightedNode(currentEvent.mousePosition);

            if (node != null)
                node.DragNode(currentEvent.delta);
        }

        /// <summary>
        /// Process right mouse drag event
        /// </summary>
        /// <param name="currentEvent"></param>
        private void ProcessRightMouseDragEvent(Event currentEvent)
        {
            if (_currentNodeGraph.NodeToDrawLineFrom != null)
            {
                DragConnectionLine(currentEvent.delta);
                GUI.changed = true;
            }
        }

        /// <summary>
        /// Drag connecting line from the node
        /// </summary>
        /// <param name="delta"></param>
        private void DragConnectionLine(Vector2 delta) =>
            _currentNodeGraph.LinePosition += delta;

        /// <summary>
        /// Check line connect when right mouse up
        /// </summary>
        /// <param name="currentEvent"></param>
        private void CheckLineConnection(Event currentEvent)
        {
            //Debug.Log("NodeEditor:CheckLineConnection()...");

            //Debug.Log("NodeEditor:CLC[0]...");
            if (_currentNodeGraph.NodeToDrawLineFrom != null)
            {
                //Debug.Log("NodeEditor:CLC[1]...");
                Node node = GetHighlightedNode(currentEvent.mousePosition);

                //Debug.Log("NodeEditor:CLC[2]...");
                if (node != null)
                {
                    //Debug.Log("NodeEditor:CLC[3]...");
                    _currentNodeGraph.NodeToDrawLineFrom.AddToChildConnectedNode(node);
                }
            }
        }

        /// <summary>
        /// Clear dragged line
        /// </summary>
        private void ClearDraggedLine()
        {
            _currentNodeGraph.NodeToDrawLineFrom = null;
            _currentNodeGraph.LinePosition = Vector2.zero;
            GUI.changed = true;
        }

        /// <summary>
        /// Process node selection, add to selected node list if node is selected
        /// </summary>
        /// <param name="mouseClickPosition"></param>
        private void ProcessNodeSelection(Vector2 mouseClickPosition)
        {
            Node clickedNode = GetHighlightedNode(mouseClickPosition);

            if (clickedNode == null)
            {
                foreach (Node node in _currentNodeGraph.NodesList)
                {
                    if (node.IsSelected)
                        node.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// Draw selection rect and select all node in it
        /// </summary>
        /// <param name="mousePosition"></param>
        private void SelectNodesBySelectionRect(Vector2 mousePosition)
        {
            if (!_isLeftMouseDragFromEmpty)
                return;

            _selectionRect = new Rect(
                Mathf.Min(_mouseScrollClickPosition.x, mousePosition.x),
                Mathf.Min(_mouseScrollClickPosition.y, mousePosition.y),
                Mathf.Abs(mousePosition.x - _mouseScrollClickPosition.x),
                Mathf.Abs(mousePosition.y - _mouseScrollClickPosition.y)
            );

            foreach (Node node in _currentNodeGraph.NodesList)
            {
                if (_selectionRect.Overlaps(node.Rect, true))
                    node.IsSelected = true;
                else
                    node.IsSelected = false;
            }

            GUI.color = new Color(0, 0, 0, 0.5f);
            GUI.DrawTexture(_selectionRect, Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        /// <summary>
        /// Return the node that is at the mouse position
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        private Node GetHighlightedNode(Vector2 mousePosition)
        {
            if (_currentNodeGraph.NodesList.Count == 0)
                return null;

            foreach (Node node in _currentNodeGraph.NodesList)
            {
                if (node.Rect.Contains(mousePosition))
                    return node;
            }

            return null;
        }

        /// <summary>
        /// Show the context menu
        /// </summary>
        /// <param name="mousePosition"></param>
        private void ShowContextMenu(Vector2 mousePosition)
        {
            GenericMenu contextMenu = new GenericMenu();

            contextMenu.AddItem(new GUIContent("Create Dialog Node"), false, CreateDialogNode, mousePosition);
            contextMenu.AddItem(new GUIContent("Create Start Node"), false, CreateStartNode, mousePosition);
            contextMenu.AddSeparator("");
            contextMenu.AddItem(new GUIContent("Select All Nodes"), false, SelectAllNodes, mousePosition);
            contextMenu.AddItem(new GUIContent("Delete Selected Node"), false, RemoveSelectedNodes, mousePosition);
            contextMenu.AddItem(new GUIContent("Remove Selected Node's Child Connectors"), false, RemoveAllConnections, mousePosition);
            contextMenu.ShowAsContext();
        }

        /// <summary>
        /// Create Dialog Node at mouse position and add it to Node Graph asset
        /// </summary>
        /// <param name="mousePositionObject"></param>
        private void CreateDialogNode(object mousePositionObject)
        {
            DialogNode dialogNode = CreateInstance<DialogNode>();
            HighestNodeID++;
            dialogNode.SetNodeID(HighestNodeID);
            System.Random random = new System.Random();

            string PlaceholderText = Adjectives[random.Next(0, Adjectives.Length)] + " " + Animals[random.Next(0, Animals.Length)];
            //InitializeNode((Vector2)mousePositionObject, dialogNode, "Dialog Node [" + _currentNodeGraph.NodesList.Count + "]");
            InitializeNode((Vector2)mousePositionObject, dialogNode, PlaceholderText);
        }

        /// <summary>
        /// Handler for "Create Start Node" menu call
        /// </summary>
        /// <param name="mousePositionObject"></param>
        private void CreateStartNode(object mousePositionObject)
        {
            CreateStartNode((Vector2)mousePositionObject);
        }

        /// <summary>
        /// Create Dialog Node at mouse position and add it to Node Graph asset
        /// </summary>
        /// <param name="mousePositionObject"></param>
        private void CreateStartNode(Vector2 Position)
        {
            //if there is already a start node, do nothing!
            foreach (Node node in _currentNodeGraph.NodesList)
                if (((DialogNode)node).nodeData.DialogText == DialogNode.StartNodeSentinel)
                {
                    Debug.Log("Start node already exists.");
                    return;
                }

            DialogNode dialogNode = CreateInstance<DialogNode>();
            InitializeNode(Position, dialogNode, DialogNode.StartNodeSentinel);
            dialogNode.name = "Start Node";
        }

        /// <summary>
        /// Select all nodes in node editor
        /// </summary>
        /// <param name="userData"></param>
        private void SelectAllNodes(object userData)
        {
            foreach (Node node in _currentNodeGraph.NodesList)
                node.IsSelected = true;

            GUI.changed = true;
        }

        private void SelectOnlyHighlightedNode(Vector2 position)
        {
            Node highlightedNode = GetHighlightedNode(position);

            foreach (Node node in _currentNodeGraph.NodesList)
                node.IsSelected = false;

            highlightedNode.IsSelected = true;
            _currentNode = highlightedNode;
        }

        /// <summary>
        /// Remove all selected nodes
        /// </summary>
        /// <param name="userData"></param>
        private void RemoveSelectedNodes(object userData)
        {
            Queue<Node> nodeDeletionQueue = new Queue<Node>();

            foreach (Node node in _currentNodeGraph.NodesList)
            {
                if (node.IsSelected && !((DialogNode)node).IsStartNode())
                    nodeDeletionQueue.Enqueue(node);
            }

            while (nodeDeletionQueue.Count > 0)
            {
                Node nodeToDelete = nodeDeletionQueue.Dequeue();

                _currentNodeGraph.NodesList.Remove(nodeToDelete);

                DestroyImmediate(nodeToDelete, true);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Create Node at mouse position and add it to Node Graph asset
        /// </summary>
        /// <param name="mousePositionObject"></param>
        /// <param name="dialogNode"></param>
        /// <param name="nodeName"></param>
        private void InitializeNode(Vector2 Position, DialogNode dialogNode, string placeholderText)
        {
            _currentNodeGraph.NodesList.Add(dialogNode);

            dialogNode.Initialize(new Rect(Position, Vector2.zero), placeholderText, _currentNodeGraph);
            dialogNode.name = "[ Node " + dialogNode.NodeID.ToString("D4") + " ]";

            AssetDatabase.AddObjectToAsset(dialogNode, _currentNodeGraph);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Chance current node graph and draw the new one
        /// </summary>
        private void ChangeEditorWindowOnSelection()
        {
            DialogNodeGraph nodeGraph = Selection.activeObject as DialogNodeGraph;

            if (nodeGraph != null)
            {
                _currentNodeGraph = nodeGraph;
                GUI.changed = true;
            }
        }

        /// <summary>
        /// Clears all connections in the selected nodes
        /// </summary>
        /// <param name="userData"></param>
        private void RemoveAllConnections(object userData)
        {
            foreach (Node node in _currentNodeGraph.NodesList)
            {
                if (!node.IsSelected)
                    continue;

                DialogNode dialogNode = (DialogNode)node;
                dialogNode.ChildNodes.Clear();
                //recalculate the size of the dialog node
                dialogNode.SetDialogNodeSize();
            }
        }

        /// <summary>
        /// Center the node editor window on all nodes
        /// </summary>
        private void CenterWindowOnNodes()
        {
            Vector2 nodesCenter = CalculateNodesCenter();
            Vector2 canvasCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

            Vector2 offset = canvasCenter - nodesCenter;

            foreach (var node in _currentNodeGraph.NodesList)
                node.DragNode(offset);

            GUI.changed = true;
        }

        /// <summary>
        /// Calculate the center of all nodes
        /// </summary>
        /// <returns>The center position of all nodes</returns>
        private Vector2 CalculateNodesCenter()
        {
            if (_currentNodeGraph.NodesList == null || _currentNodeGraph.NodesList.Count == 0)
                return Vector2.zero;

            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (var node in _currentNodeGraph.NodesList)
            {
                Rect nodeRect = node.Rect;
                minX = Mathf.Min(minX, nodeRect.xMin);
                maxX = Mathf.Max(maxX, nodeRect.xMax);
                minY = Mathf.Min(minY, nodeRect.yMin);
                maxY = Mathf.Max(maxY, nodeRect.yMax);
            }

            return new Vector2((minX + maxX) / 2, (minY + maxY) / 2);
        }
    }
}