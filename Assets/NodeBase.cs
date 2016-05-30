using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
public class InletAttribute : Attribute
{
    public InletAttribute() {}
}

[AttributeUsage(AttributeTargets.Field)]
public class OutletAttribute : Attribute
{
    public OutletAttribute() {}
}

public class NodeBase : MonoBehaviour
{
    [SerializeField, HideInInspector]
    Rect _editorRect;
}
