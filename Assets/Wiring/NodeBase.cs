// Suppress "unused variable" warning messages.
#pragma warning disable 0414

using UnityEngine;
using System;

namespace Wiring
{
    // Attribute for marking inlets
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class InletAttribute : Attribute
    {
        public InletAttribute() {}
    }

    // Attribute for marking outlets
    [AttributeUsage(AttributeTargets.Field)]
    public class OutletAttribute : Attribute
    {
        public OutletAttribute() {}
    }

    // Base class of wiring node classes
    public class NodeBase : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        Vector2 _wiringNodePosition = uninitializedNodePosition;

        static public Vector2 uninitializedNodePosition {
            get { return new Vector2(-1000, -1000); }
        }
    }
}
