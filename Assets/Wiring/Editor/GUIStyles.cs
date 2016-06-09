using UnityEngine;
using UnityEditor;

namespace Wiring.Editor
{
    // GUI styles used in the editor window
    public static class GUIStyles
    {
        #region Public properties

        static public GUIStyle background {
            get {
                if (!_initialized) Initialize();
                return _background;
            }
        }

        static public GUIStyle node {
            get {
                if (!_initialized) Initialize();
                return _node;
            }
        }

        static public GUIStyle activeNode {
            get {
                if (!_initialized) Initialize();
                return _activeNode;
            }
        }

        static public GUIStyle labelLeft {
            get {
                if (!_initialized) Initialize();
                return _labelLeft;
            }
        }

        static public GUIStyle labelRight {
            get {
                if (!_initialized) Initialize();
                return _labelRight;
            }
        }

        static public GUIStyle button {
            get {
                if (!_initialized) Initialize();
                return _button;
            }
        }

        static public GUIStyle horizontalScrollbar {
            get {
                if (!_initialized) Initialize();
                return _horizontalScrollbar;
            }
        }

        static public GUIStyle verticalScrollbar {
            get {
                if (!_initialized) Initialize();
                return _verticalScrollbar;
            }
        }

        #endregion

        #region Private members

        static bool _initialized;

        static GUIStyle _background;
        static GUIStyle _node;
        static GUIStyle _activeNode;
        static GUIStyle _labelLeft;
        static GUIStyle _labelRight;
        static GUIStyle _button;
        static GUIStyle _horizontalScrollbar;
        static GUIStyle _verticalScrollbar;

        public static void Initialize()
        {
            _background = new GUIStyle("flow background");
            _node = new GUIStyle("flow node 0");
            _activeNode = new GUIStyle("flow node 0 on");

            _labelLeft = new GUIStyle("Label");
            _labelRight = new GUIStyle("RightLabel");
            _button = new GUIStyle("miniButton");

            var skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
            _horizontalScrollbar = skin.horizontalScrollbar;
            _verticalScrollbar = skin.verticalScrollbar;

            _initialized = true;
        }

        #endregion
    }
}
