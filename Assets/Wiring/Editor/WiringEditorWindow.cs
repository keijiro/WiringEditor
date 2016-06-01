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

        enum State { Ready, LinkMaking }

        State _state;

        List<Node> _nodeList;
        NodeMap _nodeMap;

        Node _linkNode;
        Inlet _linkInlet;
        Outlet _linkOutlet;

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

        void ShowConnectionMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("New Connection"), false, BeginLinking);
            menu.ShowAsContext();
        }

        void BeginLinking()
        {
            _state = State.LinkMaking;
        }

        // GUI function for the main view
        void DrawMainViewGUI()
        {
            FeedbackQueue.Reset();

            _scrollMain = EditorGUILayout.BeginScrollView(_scrollMain);

            // Draw all the nodes.
            BeginWindows();
            var bound = Vector2.one * 300;
            foreach (var node in _nodeList)
            {
                node.DrawWindowGUI();
                bound = Vector2.Max(bound, node.windowPosition);
            }
            EndWindows();

            // Place a empty box to expand the scroll view.
            GUILayout.Box(
                "", EditorStyles.label,
                GUILayout.Width(bound.x + 256),
                GUILayout.Height(bound.y + 128)
            );

            // Draw connection lines.
            foreach (var node in _nodeList) node.DrawConnectionLines(_nodeMap);

            if (_state == State.LinkMaking) DrawWorkingLink();

            EditorGUILayout.EndScrollView();

            while (true)
            {
                var r = FeedbackQueue.Dequeue();
                if (r == null) break;

                if (_state == State.Ready)
                {
                    _linkNode = r.node;
                    _linkInlet = r.inlet;
                    _linkOutlet = r.outlet;
                    ShowConnectionMenu();
                }
                else
                {
                    if (_linkInlet != null)
                        r.node.TryConnect(r.outlet, _linkNode, _linkInlet);
                    else
                        _linkNode.TryConnect(_linkOutlet, r.node, r.inlet);

                    _state = State.Ready;
                    _linkNode = null;
                    _linkInlet = null;
                    _linkOutlet = null;
                }
            }
        }

        void DrawWorkingLink()
        {
            var p1 = (Vector3)_linkNode.windowPosition;
            var p2 = (Vector3)Event.current.mousePosition;

            if (_linkInlet == null)
            {
                p1 += (Vector3)_linkOutlet.buttonRect.center;
                DrawUtility.Curve(p1, p2);
            }
            else
            {
                p1 += (Vector3)_linkInlet.buttonRect.center;
                DrawUtility.Curve(p2, p1);
            }

            Repaint();
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
