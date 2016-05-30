using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

public class NodeHandler
{
    #region Public properties

    // Target node accessor.
    public NodeBase node {
        get { return _node; }
    }

    // Is this window selected on the editor?
    public bool isActive {
        get { return _activeWindowID == _windowID; }
    }

    // Provides property editor object.
    // FIXME: might be memory intensive.
    // Should be cached in the parent editor side.
    public Editor propertyEditor {
        get {
            if (_editor == null)
                _editor = Editor.CreateEditor(_node);
            return _editor;
        }
    }

    #endregion

    #region Public methods

    // Constructor
    public NodeHandler(NodeBase node)
    {
        _node = node;
        _title = node.GetType().Name + ":" + node.name;

        _inlets = new List<MemberInfo>();
        _outlets = new List<MemberInfo>();

        EnumerateInletsAndOutlets(node);

        _serialized = new UnityEditor.SerializedObject(node); 
        _rect = _serialized.FindProperty("_editorRect");

        FixRect();

        _windowID = _windowCounter++;
    }

    // Make a subwindow of the node.
    public void MakeWindow()
    {
        var rect0 = _rect.rectValue;
        var rect1 = GUILayout.Window(_windowID, rect0, OnWindow, _title);
        if (rect0 != rect1) {
            _rect.rectValue = rect1;
            _serialized.ApplyModifiedProperties();
        }
    }

    #endregion

    #region Internal fields

    // Target node.
    NodeBase _node;
    string _title;

    // Members for handling the serialized properties.
    SerializedObject _serialized;
    SerializedProperty _rect;

    // Inlet/outlet list.
    List<MemberInfo> _inlets;
    List<MemberInfo> _outlets;

    // Members for handling UI.
    int _windowID;
    Editor _editor;

    #endregion

    #region Static class members

    // ID of currently selected window.
    static int _activeWindowID;

    // The total count of windows; used for giving each window ID.
    static int _windowCounter;

    #endregion

    #region Private methods

    // Enumerate all inlet/outlet members.
    void EnumerateInletsAndOutlets(NodeBase node)
    {
        // Enumeration flags: all public and non-public members.
        const BindingFlags flags =
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance;

        foreach (var m in node.GetType().GetMembers(flags))
        {
            var ia = m.GetCustomAttributes(typeof(InletAttribute), true);
            var oa = m.GetCustomAttributes(typeof(OutletAttribute), true);
            if (ia.Length > 0) _inlets.Add(m);
            if (oa.Length > 0) _outlets.Add(m);
        }
    }

    // Fix the window rect if there is something wrong.
    void FixRect()
    {
        var rect = _rect.rectValue;
        rect.width = Mathf.Max(100, rect.width);
        rect.height = 50;
        _rect.rectValue = rect;
    }

    #endregion

    #region Editor UI functions

    void OnWindow(int id)
    {
        foreach (var m in _inlets)
            EditorGUILayout.LabelField("in: " + m.Name);

        foreach (var m in _outlets)
            EditorGUILayout.LabelField("out: " + m.Name);

        GUI.DragWindow();

        if (Event.current.type == EventType.Used)
            _activeWindowID = id;
    }

    #endregion
}
