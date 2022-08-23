using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Dythervin.PersistentData
{
    public partial class PrefContainer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

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
            PrefContainer pref = File.Exists(path)
                ? JsonConvert.DeserializeObject<PrefContainer>(File.ReadAllText(path), Settings)
                  ?? new PrefContainer()
                : new PrefContainer();
            pref.Path = path;
            return pref;
        }

        private static void Save(PrefContainer prefs)
        {
            File.WriteAllText(prefs.Path, JsonConvert.SerializeObject(prefs, Formatting.Indented, Settings));
#if UNITY_EDITOR
            string dirPath = System.IO.Path.GetDirectoryName(prefs.Path);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
#endif
        }
    }
}