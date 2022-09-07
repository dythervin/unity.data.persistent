using System;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using Dythervin.Core.Editor;
#endif
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Dythervin.PersistentData
{
    public partial class PrefContainer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

        private static PlayerPrefInt _version = new PlayerPrefInt("CryptVer");
#if UNITY_EDITOR
        private static EditorPrefBool _enabled = new EditorPrefBool("PrefsCrypt");
#endif

#if UNITY_EDITOR
        internal static PrefContainer GetAtProject(string path)
        {
            var file = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            PrefContainer pref = file
                ? JsonConvert.DeserializeObject<PrefContainer>(file.text) ?? new PrefContainer()
                : new PrefContainer();

            pref.Path = path;
            return pref;
        }
#endif

        internal static PrefContainer GetAt(string path)
        {
            PrefContainer pref = File.Exists(path) ? Deserialize(path) : new PrefContainer();

            pref.Path = path;
            return pref;
        }

        private static string GetString(string path)
        {
            switch (_version.Value)
            {
                case 0: return File.ReadAllText(path);
                case 1: return Encoding.UTF8.GetString(File.ReadAllBytes(path));
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private static void Save(PrefContainer prefs)
        {
            string dirPath = System.IO.Path.GetDirectoryName(prefs.Path);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            string json = JsonConvert.SerializeObject(prefs, Formatting.Indented, Settings);
#if UNITY_EDITOR
            if (!_enabled.Value)
            {
                File.WriteAllText(prefs.Path, json);
            }
#endif
            File.WriteAllBytes(prefs.Path, Encoding.UTF8.GetBytes(json));
            _version.Value = 1;
            //To ensure
            PlayerPrefs.Save();
        }

        private static PrefContainer Deserialize(string path)
        {
            return JsonConvert.DeserializeObject<PrefContainer>(GetString(path), Settings)
                   ?? new PrefContainer();
        }
    }
}