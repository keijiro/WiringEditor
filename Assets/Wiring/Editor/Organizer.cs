using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Wiring.Editor
{
    public static class Organizer
    {
        static List<Patch> _patchList = new List<Patch>();

        public static int patchCount {
            get { return _patchList.Count; }
        }

        public static void ScanPatches()
        {
            foreach (var patch in Object.FindObjectsOfType<Patch>())
                _patchList.Add(patch);
        }

        public static Patch GetPatch(int index)
        {
            return _patchList[index];
        }

        public static string[] GetPatchNames()
        {
            var temp = new string[_patchList.Count];
            for (var i = 0; i < _patchList.Count; i++)
                temp[i] = _patchList[i].name;
            return temp;
        }

        public static int GetPatchIndex(Patch patch)
        {
            for (var i = 0; i < _patchList.Count; i++)
                if (_patchList[i] == patch) return i;
            return -1;
        }
    }
}
