using UnityEngine;
using UnityEditor;

namespace Wiring.Editor
{
    public static class DrawUtility
    {
        public static void Curve(Vector2 p1, Vector2 p2)
        {
            var l = Mathf.Min(Mathf.Abs(p1.y - p2.y), 100);
            var p3 = p1 + new Vector2(l, 0);
            var p4 = p2 - new Vector2(l, 0);
            Handles.DrawBezier(p1, p2, p3, p4, Color.black, null, 2);
        }
    }
}
