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

        // Currently editing patch.
        Patch _patch;

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
            // Get the first patch.
            Organizer.Reset();
            if (Organizer.patchCount > 0)
                _patch = Organizer.RetrievePatch(0);
        }

        void OnDisable()
        {
            _patch = null;
        }

        void OnGUI()
        {
            // Tool bar
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            // - Patch selector
            var patchIndex = Organizer.GetPatchIndex(_patch);
            var newPatchIndex = EditorGUILayout.Popup(
                patchIndex, Organizer.GetPatchNameList(), EditorStyles.toolbarDropDown
            );

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // View area
            EditorGUILayout.BeginHorizontal();

            // - Main view
            DrawMainViewGUI();

            // - Property editor
            DrawSideBarGUI();

            EditorGUILayout.EndHorizontal();

            // Re-initialize the editor if the patch selection was changed.
            if (patchIndex != newPatchIndex) {
                _patch = Organizer.RetrievePatch(newPatchIndex);
                Repaint();
            }
        }

        #endregion

        #region Wiring functions

        // Go into the wiring state.
        void BeginWiring(object data)
        {
            _wiring = (WiringState)data;
        }

        // Remove a link between a pair of nodes.
        void RemoveLink(object data)
        {
            var link = (NodeLink)data;
            link.fromNode.RemoveLink(link.fromOutlet, link.toNode, link.toInlet);
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
                foreach (var node in _patch.nodeList)
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
                // "New Connection"
                menu.AddItem(
                    new GUIContent("New Connection"), false,
                    BeginWiring, new WiringState(node, inlet)
                );

                // Disconnection items
                foreach (var targetNode in _patch.nodeList)
                {
                    var link = targetNode.TryGetLinkTo(node, inlet, _patch);
                    if (link == null) continue;

                    var label = "Disconnect/" + targetNode.displayName;
                    menu.AddItem(new GUIContent(label), false, RemoveLink, link);
                }
            }
            else
            {
                // "New Connection"
                menu.AddItem(
                    new GUIContent("New Connection"), false,
                    BeginWiring, new WiringState(node, outlet)
                );

                // Disconnection items
                foreach (var link in node.EnumerateLinksFrom(outlet, _patch))
                {
                    var label = "Disconnect/" + link.toNode.displayName;
                    menu.AddItem(new GUIContent(label), false, RemoveLink, link);
                }
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
            foreach (var node in _patch.nodeList) {
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
            foreach (var node in _patch.nodeList)
                node.DrawLinkLines(_patch);

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
