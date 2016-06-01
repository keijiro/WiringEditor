using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

namespace Wiring.Editor
{
    // Editor representation of node outlet
    public class Outlet
    {
        #region Public members

        public string memberName {
            get { return _memberName; }
        }

        public string displayname {
            get { return _displayName; }
        }

        public Rect buttonRect {
            get { return _buttonRect; }
        }

        public UnityEventBase boundEvent {
            get { return _event; }
        }

        public Outlet(string memberName, UnityEventBase boundEvent)
        {
            _memberName = memberName;
            _displayName = ObjectNames.NicifyVariableName(memberName);
            _event = boundEvent;
        }

        public void DrawGUI(bool updateRect)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("out: " + _displayName);

            GUILayout.Button("*");
            if (updateRect) _buttonRect = GUILayoutUtility.GetLastRect();

            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Private fields

        string _memberName;
        string _displayName;
        UnityEventBase _event;
        Rect _buttonRect;

        #endregion
    }
}
