using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Dythervin.PersistentData
{
    public partial class Prefs
    {
        public static event Action OnSave;
        public static readonly Prefs Default;
        private static readonly HashSet<Prefs> AllPrefs = new HashSet<Prefs>();
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };

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
                {
#if !UNITY_EDITOR
                    SaveAll();
#endif
                }
            };
        }

        public static Prefs GetAt(string path, bool autoSaving = true)
        {
            Prefs pref = File.Exists(path)
                ? JsonConvert.DeserializeObject<Prefs>(File.ReadAllText(path), Settings)
                  ?? new Prefs()
                : new Prefs();
            pref.Path = path;
            if (autoSaving)
                AllPrefs.Add(pref);
            return pref;
        }

        private static void Save(Prefs prefs, string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(prefs, Formatting.Indented, Settings));
#if UNITY_EDITOR
            string dirPath = System.IO.Path.GetDirectoryName(path);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
#endif
        }

#if UNITY_EDITOR
        [MenuItem("Tools/Delete Prefs")]
        public static void DeleteAll()
        {
            Default.Clear();
            Default.Save();
        }

        public static Prefs GetAtProject(string path, bool autoSaving = true)
        {
            var file = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            Prefs pref = file
                ? JsonConvert.DeserializeObject<Prefs>(file.text) ?? new Prefs()
                : new Prefs();

            pref.Path = path;
            if (autoSaving)
                AllPrefs.Add(pref);
            return pref;
        }
#endif

        private static void SaveAll()
        {
            foreach (Prefs pref in AllPrefs)
                pref.Save();

            OnSave?.Invoke();
        }
    }
}