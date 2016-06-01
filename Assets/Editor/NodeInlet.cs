using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Wiring
{
    public class NodeInlet
    {
        MemberInfo _memberInfo;
        Rect _buttonRect;

        public string name {
            get { return _memberInfo.Name; }
        }

        public Rect buttonRect {
            get { return _buttonRect; }
        }

        public NodeInlet(MemberInfo member)
        {
            _memberInfo = member;
        }

        public void DrawGUI(bool updateRect)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Button("*");
            if (updateRect) _buttonRect = GUILayoutUtility.GetLastRect();

            EditorGUILayout.LabelField("in: " + _memberInfo.Name);

            EditorGUILayout.EndHorizontal();
        }
    }
}
