using UnityEngine;
using UnityEditor;

namespace Wiring.Editor
{
    // Editor representation of node inlet
    public class Inlet
    {
        #region Public members

        public string methodName {
            get { return _methodName; }
        }

        public string displayName {
            get { return _displayName; }
        }

        public Rect buttonRect {
            get { return _buttonRect; }
        }

        public Inlet(string methodName, string displayName)
        {
            _methodName = methodName;
            _displayName = ObjectNames.NicifyVariableName(displayName);
        }

        public bool DrawGUI(bool updateRect)
        {
            EditorGUILayout.BeginHorizontal();

            var result = GUILayout.Button("  ", GUIStyles.button);
            if (updateRect) _buttonRect = GUILayoutUtility.GetLastRect();

            EditorGUILayout.LabelField(_displayName, GUIStyles.labelLeft);

            EditorGUILayout.EndHorizontal();

            return result;
        }

        #endregion

        #region Private fields

        string _methodName;
        string _displayName;
        Rect _buttonRect;

        #endregion
    }
}
