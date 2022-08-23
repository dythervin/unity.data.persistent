using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Dythervin.PersistentData
{
    public static class Prefs
    {
        public static event Action OnSave;
        public static readonly PrefContainer Default;
        private static readonly HashSet<PrefContainer> AllPrefs = new HashSet<PrefContainer>();

        static Prefs()
        {
            Default = GetAt($"{Application.persistentDataPath}/prefs.json");
        }

        [RuntimeInitializeOnLoadMethod]
        private static void _Init()
        {
            Application.quitting += SaveAll;
            Application.focusChanged += b =>
            {
                if (!b)
                    SaveAll();
            };
        }

        public static PrefContainer GetAt(string path, bool autoSaving = true)
        {
            PrefContainer pref = PrefContainer.GetAt(path);
            if (autoSaving)
                AllPrefs.Add(pref);
            return pref;
        }

#if UNITY_EDITOR
        [MenuItem("Tools/Prefs/Delete All")]
        public static void DeleteAll()
        {
            Default.Clear();
            Default.Save();
        }

        [MenuItem("Tools/Prefs/Open Folder")]
        public static void OpenFolder()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

        public static PrefContainer GetAtProject(string path, bool autoSaving = true)
        {
            PrefContainer pref = PrefContainer.GetAtProject(path);
            if (autoSaving)
                AllPrefs.Add(pref);
            return pref;
        }
#endif

        private static void SaveAll()
        {
            foreach (PrefContainer pref in AllPrefs)
                pref.Save();

            OnSave?.Invoke();
            Debug.Log("Prefs saved");
        }
    }
}