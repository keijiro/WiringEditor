using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Wiring.Editor
{
    // Class for mapping node instances (NodeBase) to node representations
    public class NodeMap
    {
        Dictionary<int, Node> _map;

        public NodeMap()
        {
            _map = new Dictionary<int, Node>();
        }

        public void Add(NodeBase instance, Node rep)
        {
            _map.Add(instance.GetInstanceID(), rep);
        }

        public Node Get(NodeBase instance)
        {
            return _map[instance.GetInstanceID()];
        }
    }
}
