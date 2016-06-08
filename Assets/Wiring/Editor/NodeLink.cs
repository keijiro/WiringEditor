using UnityEngine;
using UnityEditor;

namespace Wiring.Editor
{
    // Link between a pair of nodes
    // Mainly used for caching link infromation.
    public class NodeLink
    {
        #region Public methods

        // Constructor
        public NodeLink(
            Node fromNode, Outlet fromOutlet,
            Node toNode, Inlet toInlet
        )
        {
            _fromNode = fromNode;
            _fromOutlet = fromOutlet;
            _toNode = toNode;
            _toInlet = toInlet;
        }

        // Draw a line (curve) between the nodes.
        public void DrawLine()
        {
            var p1 = (Vector3)_fromNode.windowPosition;
            var p2 = (Vector3)_toNode.windowPosition;

            p1 += (Vector3)_fromOutlet.buttonRect.center;
            p2 += (Vector3)_toInlet.buttonRect.center;

            DrawUtility.Curve(p1, p2);
        }

        #endregion

        #region Private fields

        Node _fromNode;
        Outlet _fromOutlet;

        Node _toNode;
        Inlet _toInlet;

        #endregion
    }
}
