using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

// View controller class
public class WiringView : EditorWindow
{
    #region Private fields

    List<NodeHandler> _handlers;
    int _chosenNode;

    Vector2 _scrollMain;
    Vector2 _scrollSide;

    #endregion

    #region Window functions

    [MenuItem("Window/Wiring")]
    static void Init()
    {
        EditorWindow.GetWindow<WiringView>("Wiring").Show();
    }

    void OnEnable()
    {
        _handlers = new List<NodeHandler>();

        // Enumerate all the nodes.
        foreach (var n in Object.FindObjectsOfType<NodeBase>())
            _handlers.Add(new NodeHandler(n));
    }

    void OnDisable()
    {
        _handlers = null;
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        DrawMainView();
        DrawSideBar();
        EditorGUILayout.EndHorizontal();
    }

    #endregion

    #region Private methods

    // Returns the handler of the active node.
    NodeHandler activeNodeHandler {
        get {
            foreach (var h in _handlers)
                if (h.isActive) return h;
            return null;
        }
    }

    // Returns the handler that holds the given node.
    NodeHandler FindHandlerOfNode(NodeBase node)
    {
        foreach (var h in _handlers)
            if (h.node == node) return h;
        return null;
    }

    // Draw a connection line between a pair of node.
    void DrawConnection(NodeHandler from, NodeHandler to)
    {
        var p1 = (Vector3)from.windowRect.center;
        var p2 = (Vector3)to.windowRect.center;
        var t1 = new Vector3(p2.x, p1.y, 0);
        var t2 = new Vector3(p1.x, p2.y, 0);
        Handles.DrawBezier(p1, p2, t1, t2, Color.black, null, 2);
    }

    // Draw all connection lines from a given node.
    void DrawAllConnectionsFrom(NodeHandler handler)
    {
        for (var i = 0; i < handler.outletCount; i++)
        {
            foreach (var target in handler.EnumerateTargetsOfOutlet(i))
            {
                if (target == null || !(target is NodeBase)) continue;
                DrawConnection(handler, FindHandlerOfNode((NodeBase)target));
            }
        }
    }

    // GUI function for the main view
    void DrawMainView()
    {
        _scrollMain = EditorGUILayout.BeginScrollView(_scrollMain, true, true);

        // Dummy box for expanding the scroll view
        // FIXME: this is not acommon approach.
        GUILayout.Box("", GUILayout.Width(1000), GUILayout.Height(1000));

        // Draw connection lines.
        foreach (var h in _handlers) DrawAllConnectionsFrom(h);

        // Draw all the nodes.
        BeginWindows();
        foreach (var h in _handlers) h.MakeWindow();
        EndWindows();

        EditorGUILayout.EndScrollView();
    }

    // GUI function for the side bar
    void DrawSideBar()
    {
        EditorGUILayout.BeginVertical(GUILayout.MinWidth(304));
        _scrollSide = EditorGUILayout.BeginScrollView(_scrollSide);
        var active = activeNodeHandler;
        if (active != null) active.propertyEditor.OnInspectorGUI();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    #endregion
}
