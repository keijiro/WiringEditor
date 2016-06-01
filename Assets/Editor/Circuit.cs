using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Wiring
{
    // Circuit class used to manage connection between nodes
    public class Circuit
    {
        #region Public methods

        // Constructor
        public Circuit()
        {
        }

        // Prepare for scanning.
        public void BeginScan(ReadOnlyCollection<NodeHandler> nodes)
        {
            _nodeHandlers = nodes;
            _connections = new List<Connection>();
        }

        // Register a single connection to the circuit.
        public void RegisterConnection(
            NodeHandler fromNode, NodeOutlet outlet,
            NodeBase targetNode, string inletName
        )
        {
            var targetNodeHandler = FindNodeHandler(targetNode);
            if (targetNodeHandler == null) return;

            var inlet = targetNodeHandler.GetInletWithName(inletName);
            if (inlet == null) return;

            _connections.Add(
                new Connection(fromNode, outlet, targetNodeHandler, inlet)
            );
        }

        // End scanning and clean up.
        public void EndScan()
        {
            _nodeHandlers = null;
        }

        // Draw all the connection lines.
        public void DrawConnectionLines()
        {
            foreach (var c in _connections) DrawConnection(c);
        }

        #endregion

        #region Private members

        // Single connection structure
        struct Connection
        {
            public NodeHandler fromNode;
            public NodeOutlet fromOutlet;

            public NodeHandler toNode;
            public NodeInlet toInlet;

            public Connection(
                NodeHandler fromNode, NodeOutlet fromOutlet,
                NodeHandler toNode, NodeInlet toInlet
            )
            {
                this.fromNode = fromNode;
                this.fromOutlet = fromOutlet;
                this.toNode = toNode;
                this.toInlet = toInlet;
            }
        }

        ReadOnlyCollection<NodeHandler> _nodeHandlers;
        List<Connection> _connections;

        // Look for the node handler with a given node.
        NodeHandler FindNodeHandler(NodeBase node)
        {
            foreach (var h in _nodeHandlers)
                if (h.node == node) return h;
            return null;
        }

        // Draw a connection line between a pair of node.
        void DrawConnection(Connection c)
        {
            var p1 = (Vector3)c.fromNode.windowPosition;
            var p2 = (Vector3)c.toNode.windowPosition;

            p1 += (Vector3)c.fromOutlet.buttonRect.center;
            p2 += (Vector3)c.toInlet.buttonRect.center;

            var t1 = new Vector3(p2.x, p1.y, 0);
            var t2 = new Vector3(p1.x, p2.y, 0);

            Handles.DrawBezier(p1, p2, t1, t2, Color.black, null, 2);
        }

        #endregion
    }
}
