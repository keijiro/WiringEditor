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
using UnityEditor;

namespace Klak.Wiring
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ValueMixer))]
    public class ValueMixerEditor : Editor
    {
        SerializedProperty _inputCurve;
        SerializedProperty _modulationType;
        SerializedProperty _modulationCurve;
        SerializedProperty _interpolator;
        SerializedProperty _value0;
        SerializedProperty _value1;
        SerializedProperty _valueEvent;

        static GUIContent _textModulation = new GUIContent("Modulation");
        static GUIContent _textValue0 = new GUIContent("Value at 0");
        static GUIContent _textValue1 = new GUIContent("Value at 1");

        void OnEnable()
        {
            _inputCurve = serializedObject.FindProperty("_inputCurve");
            _modulationType = serializedObject.FindProperty("_modulationType");
            _modulationCurve = serializedObject.FindProperty("_modulationCurve");
            _interpolator = serializedObject.FindProperty("_interpolator");
            _value0 = serializedObject.FindProperty("_value0");
            _value1 = serializedObject.FindProperty("_value1");
            _valueEvent = serializedObject.FindProperty("_valueEvent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_inputCurve);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_modulationType, _textModulation);
            if (_modulationType.hasMultipleDifferentValues ||
                _modulationType.enumValueIndex != 0)
            {
                EditorGUILayout.PropertyField(_modulationCurve);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_interpolator);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_value0, _textValue0);
            EditorGUILayout.PropertyField(_value1, _textValue1);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_valueEvent);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
