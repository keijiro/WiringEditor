using UnityEngine;
using UnityEditor;

namespace Wiring.Editor
{
    // Editor representation of node inlet
    public class Inlet
    {
        #region Public members

        public string name {
            get { return _name; }
        }

        public Rect buttonRect {
            get { return _buttonRect; }
        }

        public Inlet(string name)
        {
            _name = name;
        }

        public void DrawGUI(bool updateRect)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Button("*");
            if (updateRect) _buttonRect = GUILayoutUtility.GetLastRect();

            EditorGUILayout.LabelField("in: " + _name);

            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Private fields

        string _name;
        Rect _buttonRect;

        #endregion
    }
}
