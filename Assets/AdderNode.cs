using UnityEngine;
using UnityEngine.Events;

namespace Wiring
{
    public class AdderNode : NodeBase
    {
        [System.Serializable]
        public class FloatEvent : UnityEvent<float> {}

        [Inlet]
        public float inputValue {
            set {
                _lastInputValue = value;
                InvokeEvent();
            }
        }

        [Inlet]
        public float modulationValue {
            set {
                _lastModulationValue = value;
                InvokeEvent();
            }
        }

        [SerializeField, Outlet]
        public FloatEvent _floatEvent;

        float _lastInputValue;
        float _lastModulationValue;

        void InvokeEvent()
        {
            _floatEvent.Invoke(_lastInputValue + _lastModulationValue);
        }
    }
}
