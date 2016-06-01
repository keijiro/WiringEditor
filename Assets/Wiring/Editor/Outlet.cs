using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

namespace Wiring.Editor
{
    // Editor representation of node outlet
    public class Outlet
    {
        #region Public members

        public string name {
            get { return _name; }
        }

        public Rect buttonRect {
            get { return _buttonRect; }
        }

        public UnityEventBase boundEvent {
            get { return _event; }
        }

        public Outlet(string name, UnityEventBase boundEvent)
        {
            _name = name;
            _event = boundEvent;
        }

        public void DrawGUI(bool updateRect)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("out: " + _name);

            GUILayout.Button("*");
            if (updateRect) _buttonRect = GUILayoutUtility.GetLastRect();

            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Private fields

        string _name;
        UnityEventBase _event;
        Rect _buttonRect;

        #endregion
    }
}
