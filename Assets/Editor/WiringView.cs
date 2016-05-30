using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

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
                h.propertyEditor.OnInspectorGUI();
                break;
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }
}
