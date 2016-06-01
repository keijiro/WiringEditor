using UnityEngine;
using UnityEditor;

namespace Wiring.Editor
{
    // Connection between a pair of nodes
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

        // Draw connection line
        public void DrawLine()
        {
            var p1 = (Vector3)_fromNode.windowPosition;
            var p2 = (Vector3)_toNode.windowPosition;

            p1 += (Vector3)_fromOutlet.buttonRect.center;
            p2 += (Vector3)_toInlet.buttonRect.center;

            var t1 = new Vector3(p2.x, p1.y, 0);
            var t2 = new Vector3(p1.x, p2.y, 0);

            Handles.DrawBezier(p1, p2, t1, t2, Color.black, null, 3);
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
