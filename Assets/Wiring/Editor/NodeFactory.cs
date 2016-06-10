using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace Wiring.Editor
{
    // Class for creating new nodes
    public class NodeFactory
    {
        #region Public methods

        // Constructor
        public NodeFactory(Patch patch)
        {
            _patch = patch;
            EnumerateNodeTypes();
        }

        // "Create New Node" menu GUI function
        public void CreateNodeMenuGUI()
        {
            if (GUILayout.Button(_buttonText, EditorStyles.toolbarDropDown))
            {
                var menu = new GenericMenu();

                foreach (var nodeType in _nodeTypes) {
                    menu.AddItem(
                        new GUIContent(nodeType.label), false,
                        OnCreateNodeMenuItem, nodeType
                    );
                }

                var oy = EditorStyles.toolbar.fixedHeight - 2;
                menu.DropDown(new Rect(1, oy, 1, 1));
            }
        }

        #endregion

        #region Node type list

        class NodeType
        {
            public string label;
            public System.Type type;

            public NodeType(string label, System.Type type)
            {
                this.label = label;
                this.type = type;
            }
        }

        List<NodeType> _nodeTypes;

        // Enumerate all the node types.
        void EnumerateNodeTypes()
        {
            _nodeTypes = new List<NodeType>();

            // Scan all assemblies in the current domain.
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                // Scan all types in the assembly.
                foreach(var type in assembly.GetTypes())
                {
                    // Retrieve AddComponentMenu attributes.
                    var attr = type.GetCustomAttributes(typeof(AddComponentMenu), true);
                    if (attr.Length == 0) continue;

                    // Retrieve the menu label.
                    var label = ((AddComponentMenu)attr[0]).componentMenu;

                    // Chech if it's in the Wiring menu.
                    if (!label.StartsWith("Wiring/")) continue;

                    // Add this to the node type list.
                    label = label.Substring(7);
                    _nodeTypes.Add(new NodeType(label, type));
                }
            }
        }

        #endregion

        #region Other private members

        static GUIContent _buttonText = new GUIContent("Create New Node");

        Patch _patch;

        // Menu item execution
        void OnCreateNodeMenuItem(object data)
        {
            var nodeType = (NodeType)data;

            // Create a game object.
            var go = new GameObject("New " + nodeType.type.Name);
            var instance = go.AddComponent(nodeType.type);

            // Add it to the patch.
            _patch.AddNodeInstance((Wiring.NodeBase)instance);

            // Make it undo-able.
            Undo.RegisterCreatedObjectUndo(go, "New Node");
        }

        #endregion
    }
}
