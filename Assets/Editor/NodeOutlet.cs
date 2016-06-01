using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Reflection;

namespace Wiring
{
    public class NodeOutlet
    {
        string _name;
        UnityEventBase _event;
        Rect _buttonRect;

        public Rect buttonRect {
            get { return _buttonRect; }
        }

        public UnityEventBase boundEvent {
            get { return _event; }
        }

        public NodeOutlet(string name, UnityEventBase evt)
        {
            _name = name;
            _event = evt;
        }

        public void DrawGUI(bool updateRect)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("out: " + _name);

            GUILayout.Button("*");
            if (updateRect) _buttonRect = GUILayoutUtility.GetLastRect();

            EditorGUILayout.EndHorizontal();
        }
    }
}
