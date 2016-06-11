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

namespace Klak.WiringEditor
{
    // Class for organizing the list of patches.
    public static class Organizer
    {
        #region Static public properties and methods

        // The total count of patches.
        public static int patchCount {
            get { return _patchInstances.Length; }
        }

        // Reset and scan the patches.
        public static void Reset()
        {
            _patchInstances = Object.FindObjectsOfType<Wiring.Patch>();
        }

        // Create an editor representation of the patch at the index.
        public static Patch RetrievePatch(int index)
        {
            return new Patch(_patchInstances[index]);
        }

        // Create a list of the names of the patches.
        public static string[] GetPatchNameList()
        {
            return System.Array.ConvertAll(_patchInstances, p => p.name);
        }

        // Determine the index of the given patch.
        public static int GetPatchIndex(Patch patch)
        {
            return System.Array.FindIndex(
                _patchInstances, i => patch.IsRepresentationOf(i)
            );
        }

        #endregion

        #region Static private mebers

        static Wiring.Patch[] _patchInstances;

        #endregion
    }
}
