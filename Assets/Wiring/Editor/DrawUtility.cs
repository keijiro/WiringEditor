using UnityEngine;
using UnityEditor;

namespace Wiring.Editor
{
    public static class DrawUtility
    {
        // Draw bezier line between two nodes.
        public static void Curve(Vector2 p1, Vector2 p2)
        {
            var l = Mathf.Min(Mathf.Abs(p1.y - p2.y), 150);
            var p3 = p1 + new Vector2(l, 0);
            var p4 = p2 - new Vector2(l, 0);
            var c = new Color(0.9f, 0.9f, 0.9f);
            Handles.DrawBezier(p1, p2, p3, p4, c, null, 3);
        }
    }
}
