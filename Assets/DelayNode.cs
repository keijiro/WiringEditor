using UnityEngine;
using UnityEngine.Events;
using System.Collections;
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

public class DelayNode : MonoBehaviour
{
    [System.Serializable]
    public class FloatEvent : UnityEvent<float> {}

    [SerializeField]
    float _delay = 1;

    [SerializeField, Outlet]
    UnityEvent _event;

    [SerializeField, Outlet]
    public FloatEvent _floatEvent;

    [Inlet]
    public float inputValue {
        get; set;
    }

    [SerializeField, HideInInspector]
    Rect _editorRect;

    public void Bang()
    {
        StartCoroutine(StartDelay());
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(_delay);
        _event.Invoke();
    }
}
