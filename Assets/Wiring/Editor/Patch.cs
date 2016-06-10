using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wiring.Editor
{
    // Editor representation of patch
    // It also manages mapping information between
    // node instances (NodeBase) and those editor representations.
    public class Patch
    {
        #region Public properties and methods

        // Validity check
        public bool isValid {
            get { return _instance != null; }
        }

        // Read-only node list
        public ReadOnlyCollection<Node> nodeList {
            get { return new ReadOnlyCollection<Node>(_nodeList); }
        }

        // Constructor
        public Patch(Wiring.Patch instance)
        {
            _instance = instance;
            _nodeList = new List<Node>();
            _instanceIDToNodeMap = new Dictionary<int, Node>();

            Rescan();
        }

        // Check if this is a representation of the given patch instance.
        public bool IsRepresentationOf(Wiring.Patch instance)
        {
            return _instance == instance;
        }

        // Get an editor representation of the given node.
        public Node GetNodeOfInstance(Wiring.NodeBase instance)
        {
            return _instanceIDToNodeMap[instance.GetInstanceID()];
        }

        // Rescan the patch.
        public void Rescan()
        {
            _nodeList.Clear();
            _instanceIDToNodeMap.Clear();

            // Enumerate all the node instances.
            foreach (var i in _instance.GetComponentsInChildren<Wiring.NodeBase>())
            {
                var node = new Node(i);
                _nodeList.Add(node);
                _instanceIDToNodeMap.Add(i.GetInstanceID(), node);
            }
        }

        // Add a node instance to the patch.
        public void AddNodeInstance(Wiring.NodeBase nodeInstance)
        {
            // Append to the hierarchy.
            nodeInstance.transform.parent = _instance.transform;

            // Register to this patch representation.
            var node = new Node(nodeInstance);
            _nodeList.Add(node);
            _instanceIDToNodeMap.Add(nodeInstance.GetInstanceID(), node);
        }

        #endregion

        #region Private members

        Wiring.Patch _instance;
        List<Node> _nodeList;
        Dictionary<int, Node> _instanceIDToNodeMap;

        #endregion
    }
}
