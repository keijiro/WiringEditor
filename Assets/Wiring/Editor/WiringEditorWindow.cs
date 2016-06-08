using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace Wiring.Editor
{
    // Editor window class
    public class WiringEditorWindow : EditorWindow
    {
        #region Wiring state class

        class WiringState
        {
            public Node node;
            public Inlet inlet;
            public Outlet outlet;

            public WiringState(Node node, Inlet inlet)
            {
                this.node = node;
                this.inlet = inlet;
            }

            public WiringState(Node node, Outlet outlet)
            {
                this.node = node;
                this.outlet = outlet;
            }
        }

        #endregion

        #region Private fields

        // The list of nodes in the system
        List<Node> _nodeList;

        // NodeBase instance to Node map
        NodeMap _nodeMap;

        // Wiring state (null = not wiring now)
        WiringState _wiring;

        // Scroll view positions
        Vector2 _scrollMain;
        Vector2 _scrollSide;

        #endregion

        #region Window functions

        [MenuItem("Window/Wiring")]
        static void Init()
        {
            EditorWindow.GetWindow<WiringEditorWindow>("Wiring").Show();
        }

        void OnEnable()
        {
            _nodeList = new List<Node>();
            _nodeMap = new NodeMap();

            // Enumerate all the nodes in the scene hierarchy.
            foreach (var instance in Object.FindObjectsOfType<NodeBase>())
            {
                var node = new Node(instance);
                _nodeList.Add(node);
                _nodeMap.Add(instance, node);
            }
        }

        void OnDisable()
        {
            _nodeList = null;
            _nodeMap = null;
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            DrawMainViewGUI();
            DrawSideBarGUI();
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Wiring functions

        // Go into the wiring state.
        void BeginWiring(object data)
        {
            _wiring = (WiringState)data;
        }

        // Draw the currently working link.
        void DrawWorkingLink()
        {
            var p1 = (Vector3)_wiring.node.windowPosition;
            var p2 = (Vector3)Event.current.mousePosition;

            if (_wiring.inlet != null)
            {
                // Draw a curve from the inlet button.
                p1 += (Vector3)_wiring.inlet.buttonRect.center;
                DrawUtility.Curve(p2, p1);
            }
            else
            {
                // Draw a curve from the outlet button.
                p1 += (Vector3)_wiring.outlet.buttonRect.center;
                DrawUtility.Curve(p1, p2);
            }

            // Request repaint continuously.
            Repaint();
        }

        #endregion

        #region Private methods

        // Returns the node currently chosen.
        Node activeNode {
            get {
                foreach (var node in _nodeList)
                    if (node.isActive) return node;
                return null;
            }
        }

        // Show the inlet/outlet context menu .
        void ShowNodeButtonMenu(Node node, Inlet inlet, Outlet outlet)
        {
            var menu = new GenericMenu();

            if (inlet != null)
            {
                menu.AddItem(
                    new GUIContent("New Connection"), false,
                    BeginWiring, new WiringState(node, inlet)
                );
            }
            else
            {
                menu.AddItem(
                    new GUIContent("New Connection"), false,
                    BeginWiring, new WiringState(node, outlet)
                );
            }

            menu.ShowAsContext();
        }

        // Process feedback from the leaf UI elemets.
        void ProcessUIFeedback(FeedbackQueue.Record fb)
        {
            if (_wiring == null)
            {
                // Not in wiring: show the context menu.
                ShowNodeButtonMenu(fb.node, fb.inlet, fb.outlet);
            }
            else
            {
                // Currently in wiring: try to make a link.
                if (_wiring.inlet != null)
                    fb.node.TryLinkTo(fb.outlet, _wiring.node, _wiring.inlet);
                else
                    _wiring.node.TryLinkTo(_wiring.outlet, fb.node, fb.inlet);

                // End wiring.
                _wiring =null;
            }
        }

        // GUI function for the main view
        void DrawMainViewGUI()
        {
            FeedbackQueue.Reset();

            _scrollMain = EditorGUILayout.BeginScrollView(_scrollMain);

            // Draw all the nodes and make the bounding box.
            BeginWindows();
            var bound = Vector2.one * 300; // minimum view size
            foreach (var node in _nodeList) {
                node.DrawWindowGUI();
                bound = Vector2.Max(bound, node.windowPosition);
            }
            EndWindows();

            // Place an empty box to expand the scroll view.
            GUILayout.Box(
                "", EditorStyles.label,
                GUILayout.Width(bound.x + 256),
                GUILayout.Height(bound.y + 128)
            );

            // Draw the link lines.
            foreach (var node in _nodeList)
                node.DrawLinkLines(_nodeMap);

            // Draw working link line while wiring.
            if (_wiring != null) DrawWorkingLink();

            EditorGUILayout.EndScrollView();

            // Process all the feedback from the leaf UI elements.
            while (!FeedbackQueue.IsEmpty)
                ProcessUIFeedback(FeedbackQueue.Dequeue());
        }

        // GUI function for the side bar
        void DrawSideBarGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.MinWidth(304));
            _scrollSide = EditorGUILayout.BeginScrollView(_scrollSide);

            // Show the inspector of the active node.
            var active = activeNode;
            if (active != null) active.DrawInspectorGUI();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}
