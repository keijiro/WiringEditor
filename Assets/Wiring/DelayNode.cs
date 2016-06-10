using UnityEngine;
using System.Collections;

namespace Wiring
{
    [AddComponentMenu("Wiring/Timing/Delay")]
    public class DelayNode : NodeBase
    {
        [SerializeField]
        float _delay = 1;

        [SerializeField, Outlet]
        VoidEvent _event = new VoidEvent();

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
