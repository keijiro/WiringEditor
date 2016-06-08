using UnityEngine;
using UnityEditor;

namespace Wiring.Editor
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
