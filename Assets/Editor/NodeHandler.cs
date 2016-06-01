using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

// Class for handling data of each node
public class NodeHandler
{
    #region Public properties

    // Target node
    public NodeBase node {
        get { return _node; }
    }

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
    public NodeHandler(NodeBase node)
    {
        _node = node;
        _windowID = _windowCounter++;

        // Inlets and outlets
        _inlets = new List<NodeInlet>();
        _outlets = new List<NodeOutlet>();
        ScanAllInletsAndOutlets();

        // Window position
        _serializedObject = new UnityEditor.SerializedObject(node); 
        _serializedPosition = _serializedObject.FindProperty("_wiringNodePosition");
        ValidatePosition();
    }

    // Draw the (sub)window GUI.
    public void DrawWindowGUI()
    {
        var rect = windowRect;
        var newRect = GUILayout.Window(_windowID, rect, OnWindowGUI, windowTitle);
        if (newRect != rect) {
            _serializedPosition.vector2Value = newRect.position;
            _serializedObject.ApplyModifiedProperties();
        }
    }

    // Draw the property inspector GUI.
    public void DrawInspectorGUI()
    {
        if (_editor == null) _editor = Editor.CreateEditor(_node);
        _editor.OnInspectorGUI();
    }

    // Enumerate all outgoing connections.
    public void EnumerateConnections(ConnectionDrawer drawer)
    {
        foreach (var outlet in _outlets)
        {
            var boundEvent = outlet.boundEvent;
            var targetCount = boundEvent.GetPersistentEventCount();
            for (var i = 0; i < targetCount; i++)
            {
                var target = boundEvent.GetPersistentTarget(i);
                if (target == null || !(target is NodeBase)) continue;
                var methodName = boundEvent.GetPersistentMethodName(i);
                drawer.AddConnection(this, outlet, (NodeBase)target, methodName);
            }
        }
    }

    // Get an inlet with a given name.
    public NodeInlet GetInletWithName(string name)
    {
        foreach (var inlet in _inlets)
            if (inlet.name == name) return inlet;
        return null;
    }

    #endregion

    #region Private fields

    // Target node
    NodeBase _node;

    // Serialized property accessor
    SerializedObject _serializedObject;
    SerializedProperty _serializedPosition;

    // Inlet/outlet list
    List<NodeInlet> _inlets;
    List<NodeOutlet> _outlets;

    // GUI
    int _windowID;
    Editor _editor; // FIXME: might be memory intensive.
                    // Should be cached in the parent editor side.

    // Window ID of currently selected window
    static int _activeWindowID;

    // The total count of windows (used to generate window IDs)
    static int _windowCounter;

    #endregion

    #region Private properties and methods

    // Window title
    string windowTitle {
        get { return _node.name + " (" + _node.GetType().Name + ")"; }
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

    // Retrieve all the inlet/outlet mebers.
    void ScanAllInletsAndOutlets()
    {
        // Enumeration flags: all public and non-public members
        const BindingFlags flags =
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance;

        // Inlets
        foreach (var member in _node.GetType().GetMembers(flags))
        {
            var attrs = member.GetCustomAttributes(typeof(InletAttribute), true);
            if (attrs.Length > 0) _inlets.Add(new NodeInlet(member));
        }

        // Outlets
        foreach (var field in _node.GetType().GetFields(flags))
        {
            var attrs = field.GetCustomAttributes(typeof(OutletAttribute), true);
            if (attrs.Length == 0) continue;

            var evt = (UnityEventBase)field.GetValue(_node);
            _outlets.Add(new NodeOutlet(field.Name, evt));
        }
    }

    // Validate the window position.
    void ValidatePosition()
    {
        // Initialize the node position if not yet.
        var position = _serializedPosition.vector2Value;
        if (position == NodeBase.uninitializedNodePosition)
        {
            position = new Vector2((_windowID % 8) * 50, _windowID * 10);
            _serializedPosition.vector2Value = position;
        }
    }

    #endregion
}
