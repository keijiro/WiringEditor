using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class DelayNode : MonoBehaviour
{
    [SerializeField]
    float _delay = 1;

    [SerializeField]
    UnityEvent _event;

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
