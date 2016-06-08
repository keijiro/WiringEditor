using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.Events;
using System.Collections.Generic;
using System.Reflection;

namespace Wiring.Editor
{
    // Editor representation of node
    public class Node
    {
        #region Public properties

        // Is this window selected in the editor?
        public bool isActive {
            get { return _activeWindowID == _windowID; }
        }

        // Window position
        public Vector2 windowPosition {
            get { return _serializedPosition.vector2Value; }
        }

        #endregion

        #region Public methods

        // Constructor
        public Node(NodeBase instance)
        {
            _instance = instance;
            _windowID = _windowCounter++;

            // Inlets and outlets
            _inlets = new List<Inlet>();
            _outlets = new List<Outlet>();
            InitializeInletsAndOutlets();

            // Window position
            _serializedObject = new UnityEditor.SerializedObject(_instance);
            _serializedPosition = _serializedObject.FindProperty("_wiringNodePosition");
            ValidatePosition();
        }

        // Try to make a link from the outlet to a given node/inlet.
        public void TryLinkTo(Outlet outlet, Node targetNode, Inlet inlet)
        {
            Undo.RecordObject(_instance, "Link To Node");

            // Retrieve the target method (inlet) information.
            var targetMethod = targetNode._instance.GetType().GetMethod(inlet.methodName);

            // Try to create a link.
            var result = LinkUtility.TryLinkNodes(
                _instance, outlet.boundEvent,
                targetNode._instance, targetMethod
            );

            // Clear the cache and update information.
            if (result) {
                _cachedLinks = null;
                _serializedObject.Update();
            }
        }

        // Draw (sub)window GUI.
        public void DrawWindowGUI()
        {
            // Make a rect at the window position. The size is not in use.
            var rect = new Rect(windowPosition, Vector2.one);

            // Show the window.
            var title = _instance.name + " (" + _instance.GetType().Name + ")";
            var newRect = GUILayout.Window(_windowID, rect, OnWindowGUI, title);

            // Update the serialized info if the position was changed.
            if (newRect.position != rect.position) {
                _serializedPosition.vector2Value = newRect.position;
                _serializedObject.ApplyModifiedProperties();
            }
        }

        // Draw property inspector GUI.
        public void DrawInspectorGUI()
        {
            if (_editor == null) _editor = UnityEditor.Editor.CreateEditor(_instance);
            _editor.OnInspectorGUI();
        }

        // Draw lines of the links from this node.
        public void DrawLinkLines(NodeMap map)
        {
            if (_cachedLinks == null) CacheLinks(map);
            foreach (var link in _cachedLinks) link.DrawLine();
        }

        #endregion

        #region Private fields

        // Runtime instance
        Wiring.NodeBase _instance;

        // Inlet/outlet list
        List<Inlet> _inlets;
        List<Outlet> _outlets;

        // Cached connection info
        List<NodeLink> _cachedLinks;

        // Serialized property accessor
        SerializedObject _serializedObject;
        SerializedProperty _serializedPosition;

        // GUI
        int _windowID;
        UnityEditor.Editor _editor; // FIXME: should be handled in WiringEditorWindow

        // Window ID of currently selected window
        static int _activeWindowID;

        // The total count of windows (used to generate window IDs)
        static int _windowCounter;

        #endregion

        #region Private properties and methods

        // Window GUI function
        void OnWindowGUI(int id)
        {
            // It can update the button position info on a repaint event.
            var rectUpdate = (Event.current.type == EventType.Repaint);

            // Draw the inlet labels and buttons.
            foreach (var inlet in _inlets)
                if (inlet.DrawGUI(rectUpdate))
                    // The inlet button was pressed; nofity via FeedbackQueue.
                    FeedbackQueue.EnqueueButtonPress(this, inlet);

            // Draw the outlet labels and buttons.
            foreach (var outlet in _outlets)
                if (outlet.DrawGUI(rectUpdate))
                    // The outlet button was pressed; nofity via FeedbackQueue.
                    FeedbackQueue.EnqueueButtonPress(this, outlet);

            // The standard GUI behavior.
            GUI.DragWindow();

            // Is this window clicked? Then assume it's active one.
            if (Event.current.type == EventType.Used)
                _activeWindowID = id;
        }

        // Validate the window position.
        void ValidatePosition()
        {
            // Initialize the node position if not yet.
            var position = _serializedPosition.vector2Value;
            if (position == Wiring.NodeBase.uninitializedNodePosition)
            {
                // Calculate the initial window position with the window ID.
                var x = (_windowID % 8 + 1) * 50;
                var y = (_windowID + 1) * 40;
                _serializedPosition.vector2Value = new Vector2(x, y);
            }
        }

        // Initialize all inlets/outlets from the node instance with using reflection.
        void InitializeInletsAndOutlets()
        {
            // Enumeration flags: all public and non-public members
            const BindingFlags flags =
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance;

            // Inlets (method)
            foreach (var method in _instance.GetType().GetMethods(flags))
            {
                var attrs = method.GetCustomAttributes(typeof(InletAttribute), true);
                if (attrs.Length == 0) continue;
                
                _inlets.Add(new Inlet(method.Name, method.Name));
            }

            // Inlets (property)
            foreach (var prop in _instance.GetType().GetProperties(flags))
            {
                var attrs = prop.GetCustomAttributes(typeof(InletAttribute), true);
                if (attrs.Length == 0) continue;

                _inlets.Add(new Inlet(prop.GetSetMethod().Name, prop.Name));
            }

            // Outlets (UnityEvent members)
            foreach (var field in _instance.GetType().GetFields(flags))
            {
                var attrs = field.GetCustomAttributes(typeof(OutletAttribute), true);
                if (attrs.Length == 0) continue;

                var evt = (UnityEventBase)field.GetValue(_instance);
                _outlets.Add(new Outlet(field.Name, evt));
            }
        }

        // Get an inlet with a given name.
        Inlet GetInletWithName(string name)
        {
            foreach (var inlet in _inlets)
                if (inlet.methodName == name) return inlet;
            return null;
        }

        // Enumerate all links from the outlets and cache them.
        void CacheLinks(NodeMap map)
        {
            _cachedLinks = new List<NodeLink>();

            foreach (var outlet in _outlets)
            {
                // Scan all the events from the outlet.
                var boundEvent = outlet.boundEvent;
                var targetCount = boundEvent.GetPersistentEventCount();
                for (var i = 0; i < targetCount; i++)
                {
                    var target = boundEvent.GetPersistentTarget(i);

                    // Ignore it if it's a null event or the target is not a node.
                    if (target == null || !(target is Wiring.NodeBase)) continue;

                    // Try to retrieve the linked inlet.
                    var targetNode = map.Get((NodeBase)target);
                    var methodName = boundEvent.GetPersistentMethodName(i);
                    var inlet = targetNode.GetInletWithName(methodName);

                    // Cache it if it's a valid link.
                    if (targetNode != null && inlet != null)
                        _cachedLinks.Add(new NodeLink(this, outlet, targetNode, inlet));
                }
            }
        }

        #endregion
    }
}
