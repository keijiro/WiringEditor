using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Wiring
{
    public class DelayNode : NodeBase
    {
        [SerializeField]
        float _delay = 1;

        [SerializeField, Outlet]
        UnityEvent _event;

        [Inlet]
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
}
