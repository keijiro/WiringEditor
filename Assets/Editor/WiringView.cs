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

        foreach (var n in Object.FindObjectsOfType<NodeBase>())
            _handlers.Add(new NodeHandler(n));
    }

    SerializedObject GetProperty(string nodeName)
    {
        var go = GameObject.Find(nodeName);
        var comp = go.GetComponent<NodeBase>();
        return new UnityEditor.SerializedObject(comp);
    }

    NodeHandler FindHandlerOfNode(Object o)
    {
        foreach (var h in _handlers)
            if (h.node == o) return h;
        return null;
    }

    void DrawConnection(NodeHandler from, NodeHandler to)
    {
        var p1 = (Vector3)from.windowRect.center;
        var p2 = (Vector3)to.windowRect.center;
        var t1 = new Vector3(p2.x, p1.y, 0);
        var t2 = new Vector3(p1.x, p2.y, 0);
        Handles.DrawBezier(p1, p2, t1, t2, Color.black, null, 2);
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        _scroll = EditorGUILayout.BeginScrollView(_scroll, true, true);

        GUILayout.Box("", GUILayout.Width(1000), GUILayout.Height(1000));

        foreach (var h in _handlers) DrawConnection(h);

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

    void DrawConnection(NodeHandler handler)
    {
        for (var i = 0; i < handler.outletCount; i++)
        {
            foreach (var target in handler.EnumerateTargetsOfOutlet(i))
            {
                if (target == null) continue;
                var targetNode = FindHandlerOfNode(target);
                if (targetNode != null)
                    DrawConnection(handler, targetNode);
            }
        }
    }
}
