using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Wiring
{
    public class ConnectionDrawer
    {
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

        // Returns the handler that holds the given node.
        NodeHandler FindHandlerOfNode(NodeBase node)
        {
            foreach (var h in _nodeHandlers)
                if (h.node == node) return h;
            return null;
        }

        public ConnectionDrawer()
        {
        }

        public void BeginCaching(ReadOnlyCollection<NodeHandler> nodeHandlers)
        {
            _nodeHandlers = nodeHandlers;
            _connections = new List<Connection>();
        }

        public void AddConnection(
            NodeHandler fromNode, NodeOutlet outlet,
            NodeBase target, string methodName
        )
        {
            var targetHandler = FindHandlerOfNode(target);
            if (targetHandler == null) return;

            var inlet = targetHandler.GetInletWithName(methodName);
            if (inlet == null) return;

            _connections.Add(new Connection(fromNode, outlet, targetHandler, inlet));
        }

        public void EndCaching()
        {
            _nodeHandlers = null;
        }

        public void DrawLines()
        {
            foreach (var c in _connections) DrawConnection(c);
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
    }
}
