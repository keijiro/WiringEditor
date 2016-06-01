using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace Wiring
{
    // View controller class
    public class WiringView : EditorWindow
    {
        #region Private fields

        List<NodeHandler> _nodeHandlers;
        Circuit _circuit;
        Vector2 _scrollMain;
        Vector2 _scrollSide;

        #endregion

        #region Window functions

        [MenuItem("Window/Wiring")]
        static void Init()
        {
            EditorWindow.GetWindow<WiringView>("Wiring").Show();
        }

        void OnEnable()
        {
            // Enumerate all the nodes in the scene hierarchy.
            _nodeHandlers = new List<NodeHandler>();
            foreach (var n in Object.FindObjectsOfType<NodeBase>())
                _nodeHandlers.Add(new NodeHandler(n));

            // Enumerate all the connections between nodes.
            _circuit = new Circuit();
            _circuit.BeginScan(_nodeHandlers.AsReadOnly());
            foreach (var h in _nodeHandlers)
                h.EnumerateConnections(_circuit);
            _circuit.EndScan();
        }

        void OnDisable()
        {
            _nodeHandlers = null;
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            DrawMainViewGUI();
            DrawSideBarGUI();
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Private methods

        // Returns the handler of the active node.
        NodeHandler activeNodeHandler {
            get {
                foreach (var h in _nodeHandlers)
                    if (h.isActive) return h;
                return null;
            }
        }

        // GUI function for the main view
        void DrawMainViewGUI()
        {
            _scrollMain = EditorGUILayout.BeginScrollView(_scrollMain, true, true);

            // Dummy box for expanding the scroll view
            // FIXME: this is not acommon approach.
            GUILayout.Box("", GUILayout.Width(1000), GUILayout.Height(1000));

            // Draw all the nodes.
            BeginWindows();
            foreach (var h in _nodeHandlers) h.DrawWindowGUI();
            EndWindows();

            // Draw connection lines.
            _circuit.DrawConnectionLines();

            EditorGUILayout.EndScrollView();
        }

        // GUI function for the side bar
        void DrawSideBarGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.MinWidth(304));
            _scrollSide = EditorGUILayout.BeginScrollView(_scrollSide);
            var active = activeNodeHandler;
            if (active != null) active.DrawInspectorGUI();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}
