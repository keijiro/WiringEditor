using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
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

        // Window rect
        public Rect windowRect {
            get { return new Rect(windowPosition, new Vector2(100, 1)); }
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
            ScanAllInletsAndOutlets();

            // Window position
            _serializedObject = new UnityEditor.SerializedObject(_instance);
            _serializedPosition = _serializedObject.FindProperty("_wiringNodePosition");
            ValidatePosition();
        }

        // Draw (sub)window GUI.
        public void DrawWindowGUI()
        {
            var rect = windowRect;
            var newRect = GUILayout.Window(_windowID, rect, OnWindowGUI, windowTitle);
            if (newRect != rect) {
                _serializedPosition.vector2Value = newRect.position;
                _serializedObject.ApplyModifiedProperties();
            }
        }

        // Draw property inspector GUI.
        public void DrawInspectorGUI()
        {
            if (_editor == null)
                _editor = UnityEditor.Editor.CreateEditor(_instance);

            _editor.OnInspectorGUI();
        }

        // Draw connection lines.
        public void DrawConnectionLines(NodeMap map)
        {
            if (_cachedLinks == null) EnumerateAndCacheLinks(map);
            foreach (var l in _cachedLinks) l.DrawLine();
        }

        #endregion

        #region Private fields

        // Runtime instance
        Wiring.NodeBase _instance;

        // Serialized property accessor
        SerializedObject _serializedObject;
        SerializedProperty _serializedPosition;

        // Inlet/outlet list
        List<Inlet> _inlets;
        List<Outlet> _outlets;

        // Cached connection info
        List<NodeLink> _cachedLinks;

        // GUI
        int _windowID;
        UnityEditor.Editor _editor; // FIXME: might be memory intensive.

        // Window ID of currently selected window
        static int _activeWindowID;

        // The total count of windows (used to generate window IDs)
        static int _windowCounter;

        #endregion

        #region Private properties and methods

        // Window title
        string windowTitle {
            get { return _instance.name + " (" + _instance.GetType().Name + ")"; }
        }

        // Window GUI function
        void OnWindowGUI(int id)
        {
            var updateRect = (Event.current.type == EventType.Repaint);

            foreach (var i in _inlets) i.DrawGUI(updateRect);
            foreach (var o in _outlets) o.DrawGUI(updateRect);

            GUI.DragWindow();

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
                var x = (_windowID % 8 + 1) * 50;
                var y = (_windowID + 1) * 40;
                _serializedPosition.vector2Value = new Vector2(x, y);
            }
        }

        // Retrieve all the inlet/outlet mebers.
        void ScanAllInletsAndOutlets()
        {
            // Enumeration flags: all public and non-public members
            const BindingFlags flags =
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance;

            // Inlets
            foreach (var method in _instance.GetType().GetMethods(flags))
            {
                var attrs = method.GetCustomAttributes(typeof(InletAttribute), true);
                if (attrs.Length == 0) continue;
                
                _inlets.Add(new Inlet(method.Name, method.Name));
            }

            foreach (var prop in _instance.GetType().GetProperties(flags))
            {
                var attrs = prop.GetCustomAttributes(typeof(InletAttribute), true);
                if (attrs.Length == 0) continue;

                _inlets.Add(new Inlet(prop.GetSetMethod().Name, prop.Name));
            }


            // Outlets
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

        // Enumerate all outgoing connections and cache them.
        void EnumerateAndCacheLinks(NodeMap map)
        {
            _cachedLinks = new List<NodeLink>();

            foreach (var outlet in _outlets)
            {
                var boundEvent = outlet.boundEvent;
                var targetCount = boundEvent.GetPersistentEventCount();
                for (var i = 0; i < targetCount; i++)
                {
                    var target = boundEvent.GetPersistentTarget(i);
                    if (target == null || !(target is Wiring.NodeBase)) continue;

                    var methodName = boundEvent.GetPersistentMethodName(i);

                    var node = map.Get((NodeBase)target);
                    var inlet = node.GetInletWithName(methodName);

                    if (node != null && inlet != null)
                        _cachedLinks.Add(new NodeLink(this, outlet, node, inlet));
                }
            }
        }

        #endregion
    }
}
