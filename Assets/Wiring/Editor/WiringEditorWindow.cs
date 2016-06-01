using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace Wiring.Editor
{
    // Editor window class
    public class WiringEditorWindow : EditorWindow
    {
        #region Private fields

        List<Node> _nodeList;
        NodeMap _nodeMap;

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
            // Enumerate all the nodes in the scene hierarchy.
            _nodeList = new List<Node>();
            _nodeMap = new NodeMap();

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

        #region Private methods

        // Returns the node currently chosen in the window.
        Node activeNode {
            get {
                foreach (var node in _nodeList)
                    if (node.isActive) return node;
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
            foreach (var node in _nodeList) node.DrawWindowGUI();
            EndWindows();

            // Draw connection lines.
            foreach (var node in _nodeList) node.DrawConnectionLines(_nodeMap);

            EditorGUILayout.EndScrollView();
        }

        // GUI function for the side bar
        void DrawSideBarGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.MinWidth(304));
            _scrollSide = EditorGUILayout.BeginScrollView(_scrollSide);
            var active = activeNode;
            if (active != null) active.DrawInspectorGUI();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}
