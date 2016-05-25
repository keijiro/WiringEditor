using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class NodeHandler
{
    DelayNode _node;

    SerializedObject _serialized;
    SerializedProperty _rect;

    int _windowID;
    Editor _editor;

    static int _activeWindowID;

    static int _windowCounter;

    public bool isActive {
        get { return _activeWindowID == _windowID; }
    }

    public DelayNode node {
        get { return _node; }
    }

    public SerializedObject serializedObject {
        get { return _serialized; }
    }

    public Editor editor {
        get {
            if (_editor == null)
                _editor = Editor.CreateEditor(_node);
            return _editor;
        }
    }

    public NodeHandler(DelayNode node)
    {
        _node = node;

        _serialized = new UnityEditor.SerializedObject(node); 
        _rect = _serialized.FindProperty("_editorRect");

        FixRect();

        _windowID = _windowCounter++;
    }

    void FixRect()
    {
        var rect = _rect.rectValue;
        rect.width = Mathf.Max(100, rect.width);
        rect.height = Mathf.Max(50, rect.height);
        _rect.rectValue = rect;
    }

    public void MakeWindow()
    {
        var rect0 = _rect.rectValue;
        var rect1 = GUI.Window(_windowID, rect0, OnWindow, _node.name);
        if (rect0 != rect1) {
            _rect.rectValue = rect1;
            _serialized.ApplyModifiedProperties();
        }
    }

    void OnWindow(int id)
    {
        GUI.DragWindow();

        if (Event.current.type == EventType.Used)
            _activeWindowID = id;
    }
}

public class WiringView : EditorWindow
{
    List<NodeHandler> _handlers;
    Vector2 _scroll;
    Vector2 _scroll2;
    int _chosenNode;

    [MenuItem("Window/Wiring")]
    static void Init()
    {
        EditorWindow.GetWindow<WiringView>("Wiring").Show();
    }

    void OnEnable()
    {
        _handlers = new List<NodeHandler>();

        foreach (var n in Object.FindObjectsOfType<DelayNode>())
            _handlers.Add(new NodeHandler(n));
    }

    SerializedObject GetProperty(string nodeName)
    {
        var go = GameObject.Find(nodeName);
        var comp = go.GetComponent<DelayNode>();
        return new UnityEditor.SerializedObject(comp);
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        _scroll = EditorGUILayout.BeginScrollView(_scroll, true, true);
        GUILayout.Box("", GUILayout.Width(1000), GUILayout.Height(1000));
        BeginWindows();
        foreach (var h in _handlers) h.MakeWindow();
        EndWindows();
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginVertical(GUILayout.MinWidth(304));
        _scroll2 = EditorGUILayout.BeginScrollView(_scroll2);

        foreach (var h in _handlers) {
            if (h.isActive) {
                h.editor.OnInspectorGUI();
                break;
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }
}
