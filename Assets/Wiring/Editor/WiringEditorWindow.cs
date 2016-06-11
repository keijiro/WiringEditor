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

        // Currently editing patch
        Patch _patch;

        // Helper objects
        NodeFactory _factory;

        // Wiring state (null = not wiring now)
        WiringState _wiring;

        // View size and positions
        Vector2 _mainViewSize;
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
            ResetState();
            Undo.undoRedoPerformed += ResetState;
            EditorApplication.playmodeStateChanged += ResetState;
        }

        void OnDisable()
        {
            _patch = null;
            _factory = null;
            Undo.undoRedoPerformed -= ResetState;
            EditorApplication.playmodeStateChanged -= ResetState;
        }

        void OnFocus()
        {
            ResetState();
        }

        void OnGUI()
        {
            if (_patch == null) {
                DrawNoPatchMessage();
                return;
            }

            // Tool bar
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            // - Create node menu
            _factory.CreateNodeMenuGUI();
            GUILayout.Space(100);

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
                _factory = new NodeFactory(_patch);
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
                if (_patch != null)
                    foreach (var node in _patch.nodeList)
                        if (node.isActive) return node;
                return null;
            }
        }

        // Reset the internal state.
        void ResetState()
        {
            if (!EditorApplication.isPlaying &&
                !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Organizer.Reset();

                if (_patch == null || !_patch.isValid)
                    RetrieveDefaultPatch();
                else
                    _patch.Rescan();

                _mainViewSize = Vector2.one * 300; // minimum view size
            }
            else
            {
                // Play mode: disable editing.
                _patch = null;
                _factory = null;
            }

            Repaint();
        }

        // Retrieve the default patch (first available one).
        void RetrieveDefaultPatch()
        {
            if (Organizer.patchCount > 0)
            {
                _patch = Organizer.RetrievePatch(0);
                _factory = new NodeFactory(_patch);
            }
            else
            {
                _patch = null;
                _factory = null;
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
        void ProcessUIFeedback(FeedbackQueue.RecordBase record)
        {
            // Delete request
            if (record is FeedbackQueue.DeleteNodeRecord)
            {
                var node = ((FeedbackQueue.DeleteNodeRecord)record).node;
                node.RemoveFromPatch(_patch);
                ResetState();
            }

            // Inlet button pressed
            if (record is FeedbackQueue.InletButtonRecord)
            {
                var info = (FeedbackQueue.InletButtonRecord)record;
                if (_wiring == null)
                    // Not in wiring: show the context menu.
                    ShowNodeButtonMenu(info.node, info.inlet, null);
                else
                    // Currently in wiring: try to make a link.
                    _wiring.node.TryLinkTo(_wiring.outlet, info.node, info.inlet);
            }

            // Outlet button pressed
            if (record is FeedbackQueue.OutletButtonRecord)
            {
                var info = (FeedbackQueue.OutletButtonRecord)record;
                if (_wiring == null)
                    // Not in wiring: show the context menu.
                    ShowNodeButtonMenu(info.node, null, info.outlet);
                else
                    // Currently in wiring: try to make a link.
                    info.node.TryLinkTo(info.outlet, _wiring.node, _wiring.inlet);
            }

            // Force to end wiring.
            _wiring = null;
        }

        // GUI function for the main view
        void DrawMainViewGUI()
        {
            FeedbackQueue.Reset();

            _scrollMain = EditorGUILayout.BeginScrollView(
                _scrollMain, false, false,
                GUIStyles.horizontalScrollbar,
                GUIStyles.verticalScrollbar,
                GUIStyles.background
            );

            // Draw the link lines.
            if (Event.current.type == EventType.Repaint)
                foreach (var node in _patch.nodeList)
                    if (!node.DrawLinkLines(_patch))
                    {
                        // Request repaint if position info is not ready.
                        Repaint();
                        break;
                    }

            // Draw all the nodes and make the bounding box.
            BeginWindows();
            foreach (var node in _patch.nodeList) {
                node.DrawWindowGUI();
                _mainViewSize = Vector2.Max(_mainViewSize, node.windowPosition);
            }
            EndWindows();

            // Place an empty box to expand the scroll view.
            GUILayout.Box(
                "", EditorStyles.label,
                GUILayout.Width(_mainViewSize.x + 256),
                GUILayout.Height(_mainViewSize.y + 128)
            );

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
            if (active != null) {
                EditorGUILayout.TextField("Node Name", active.displayName);
                EditorGUILayout.Space();
                active.DrawInspectorGUI();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        // "No patch" message
        void DrawNoPatchMessage()
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("No patch available", EditorStyles.largeLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        #endregion
    }
}
