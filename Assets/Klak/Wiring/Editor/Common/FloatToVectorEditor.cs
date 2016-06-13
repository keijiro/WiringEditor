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
    [CustomEditor(typeof(FloatToVector))]
    public class FloatToVectorEditor : Editor
    {
        SerializedProperty _vector0;
        SerializedProperty _vector1;
        SerializedProperty _vectorEvent;

        static GUIContent _textVector0 = new GUIContent("Value at 0");
        static GUIContent _textVector1 = new GUIContent("Value at 1");

        void OnEnable()
        {
            _vector0 = serializedObject.FindProperty("_vector0");
            _vector1 = serializedObject.FindProperty("_vector1");
            _vectorEvent = serializedObject.FindProperty("_vectorEvent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_vector0, _textVector0);
            EditorGUILayout.PropertyField(_vector1, _textVector1);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_vectorEvent);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
