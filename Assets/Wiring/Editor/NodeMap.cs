using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wiring.Editor
{
    // Class for listing nodes in a patch
    // It also stores mapping information between
    // node instances (NodeBase) and editor representations.
    public class NodeMap
    {
        #region Public properties and methods

        // Read-only node list
        public ReadOnlyCollection<Node> nodeList {
            get { return new ReadOnlyCollection<Node>(_nodeList); }
        }

        // Constructor
        public NodeMap(Patch patch)
        {
            var instances = patch.gameObject.GetComponentsInChildren<NodeBase>();

            _nodeList = new Node[instances.Length];
            _instanceIDToNodeMap = new Dictionary<int, Node>();

            for (var i = 0; i < instances.Length; i++)
            {
                var instance = instances[i];
                var node = new Node(instance);

                _nodeList[i] = node;
                _instanceIDToNodeMap.Add(instance.GetInstanceID(), node);
            }
        }

        // Get an editor representation of a given node.
        public Node GetNode(NodeBase instance)
        {
            return _instanceIDToNodeMap[instance.GetInstanceID()];
        }

        #endregion

        #region Private members

        Node[] _nodeList;
        Dictionary<int, Node> _instanceIDToNodeMap;

        #endregion
    }
}
