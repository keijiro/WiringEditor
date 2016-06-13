//
// Klak - Utilities for creative coding with Unity
//
// Copyright (C) 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using UnityEngine;
using UnityEngine.Events;
using Klak.Math;
using System;

namespace Klak.Wiring
{
    [AddComponentMenu("Klak/Wiring/Value Mixer")]
    public class ValueMixer : NodeBase
    {
        #region Nested Public Classes

        public enum ModulationType {
            Off, Add, Subtract, Multiply, Divide, Minimum, Maximum
        }

        #endregion

        #region Editable Properties

        [SerializeField]
        AnimationCurve _inputCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField]
        ModulationType _modulationType = ModulationType.Add;

        [SerializeField]
        AnimationCurve _modulationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField]
        FloatInterpolator.Config _interpolator = FloatInterpolator.Config.Direct;

        [SerializeField]
        float _value0 = 0.0f;

        [SerializeField]
        float _value1 = 1.0f;

        [SerializeField, Outlet]
        FloatEvent _valueEvent = new FloatEvent();

        #endregion

        #region Public Properties

        [Inlet]
        public float inputValue {
            set {
                _inputValue = value;
                if (_interpolator.enabled)
                    _floatValue.targetValue = CalculateTargetValue();
                else
                    InvokeEvent(CalculateTargetValue());
            }
        }

        [Inlet]
        public float modulationValue {
            set {
                _modulationValue = value;
                if (_interpolator.enabled)
                    _floatValue.targetValue = CalculateTargetValue();
                else
                    InvokeEvent(CalculateTargetValue());
            }
        }

        #endregion

        #region Private Variables And Methods

        float _inputValue;
        float _modulationValue;
        FloatInterpolator _floatValue;

        float EvalInputCurve()
        {
            return _inputCurve.Evaluate(_inputValue);
        }

        float EvalModulationCurve()
        {
            return _modulationCurve.Evaluate(_modulationValue);
        }

        float CalculateTargetValue()
        {
            var x = EvalInputCurve();

            switch (_modulationType)
            {
                case ModulationType.Add:
                    x += EvalModulationCurve();
                    break;
                case ModulationType.Subtract:
                    x -= EvalModulationCurve();
                    break;
                case ModulationType.Multiply:
                    x *= EvalModulationCurve();
                    break;
                case ModulationType.Divide:
                    x /= EvalModulationCurve();
                    break;
                case ModulationType.Minimum:
                    x = Mathf.Min(x, EvalModulationCurve());
                    break;
                case ModulationType.Maximum:
                    x = Mathf.Max(x, EvalModulationCurve());
                    break;
            }

            return x;
        }

        void InvokeEvent(float p)
        {
            var f = BasicMath.Lerp(_value0, _value1, p);
            _valueEvent.Invoke(f);
        }

        #endregion

        #region MonoBehaviour Functions

        void Start()
        {
            _floatValue = new FloatInterpolator(0, _interpolator);
        }

        void Update()
        {
            if (_interpolator.enabled)
                InvokeEvent(_floatValue.Step());
        }

        #endregion
    }
}
