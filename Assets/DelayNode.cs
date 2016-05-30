using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class DelayNode : NodeBase
{
    [System.Serializable]
    public class FloatEvent : UnityEvent<float> {}

    [SerializeField]
    float _delay = 1;

    [Inlet]
    public float inputValue {
        get; set;
    }

    [SerializeField, Outlet]
    UnityEvent _event;

    [SerializeField, Outlet]
    public FloatEvent _floatEvent;

    public void Bang()
    {
        StartCoroutine(StartDelay());
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(_delay);
        _event.Invoke();
        _floatEvent.Invoke(1);
    }
}
