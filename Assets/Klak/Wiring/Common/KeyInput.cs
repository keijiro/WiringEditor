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
using System;
using Klak.Math;

namespace Klak.Wiring
{
    [AddComponentMenu("Klak/Wiring/Input/Key Input")]
    public class KeyInput : NodeBase
    {
        #region Editable Properties

        [SerializeField]
        KeyCode _keyCode;

        [SerializeField]
        float _offValue = 0.0f;

        [SerializeField]
        float _onValue = 1.0f;

        [SerializeField]
        FloatInterpolator.Config _interpolator;

        [SerializeField, Outlet]
        VoidEvent _keyDownEvent = new VoidEvent();

        [SerializeField, Outlet]
        VoidEvent _keyUpEvent = new VoidEvent();

        [SerializeField, Outlet]
        FloatEvent _valueEvent = new FloatEvent();

        #endregion

        #region Private Properties And Variables

        bool IsKeyDown {
            get { return Input.GetKeyDown(_keyCode); }
        }

        bool IsKeyUp {
            get { return Input.GetKeyUp(_keyCode); }
        }

        FloatInterpolator _floatValue;

        #endregion

        #region MonoBehaviour Functions

        void Start()
        {
            _floatValue = new FloatInterpolator(0, _interpolator);
        }

        void Update()
        {
            if (IsKeyDown)
            {
                _keyDownEvent.Invoke();
                _floatValue.targetValue = _onValue;
            }
            else if (IsKeyUp)
            {
                _keyUpEvent.Invoke();
                _floatValue.targetValue = _offValue;
            }

            _valueEvent.Invoke(_floatValue.Step());
        }

        #endregion
    }
}
