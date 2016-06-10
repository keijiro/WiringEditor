using UnityEngine;

namespace Wiring
{
    [AddComponentMenu("Wiring/Compound/Adder")]
    public class AdderNode : NodeBase
    {
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
        public FloatEvent _floatEvent = new FloatEvent();

        float _lastInputValue;
        float _lastModulationValue;

        void InvokeEvent()
        {
            _floatEvent.Invoke(_lastInputValue + _lastModulationValue);
        }
    }
}
