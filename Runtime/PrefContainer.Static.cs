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

        private static PlayerPrefInt _version = new PlayerPrefInt("PersistentEncryptionVer");
#if UNITY_EDITOR
        private const string MenuItemName = "Tools/Prefs/Encryption";

        private static EditorPrefBool _enabled = new EditorPrefBool("PrefsCrypt", true);

        [MenuItem(MenuItemName)]
        private static void Toggle()
        {
            _enabled.Value = !_enabled.Value;
        }

        [MenuItem(MenuItemName, true)]
        private static bool ToggleValidate()
        {
            Menu.SetChecked(MenuItemName, _enabled);
            return true;
        }
#endif

#if UNITY_EDITOR
        [MenuItem("Tools/Prefs/Open Folder")]
        public static void OpenFolder()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

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
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                case 0: return File.ReadAllText(path);
#endif
                case 1: return Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(path)));
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private static void Save(PrefContainer prefs)
        {
            string dirPath = System.IO.Path.GetDirectoryName(prefs.Path);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

#if UNITY_EDITOR
            if (!_enabled.Value)
            {
                File.WriteAllText(prefs.Path, JsonConvert.SerializeObject(prefs, Formatting.Indented, Settings));
                _version.Value = 0;
            }
            else
#endif
            {
                File.WriteAllText(prefs.Path, Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(prefs, Formatting.None, Settings))));
                _version.Value = 1;
            }

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