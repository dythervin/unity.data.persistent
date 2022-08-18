using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Dythervin.PersistentData
{
    [Serializable]
    public partial class Prefs : ISerializable
    {
        internal readonly Dictionary<Type, PersistentContainerBase> containers;
        [JsonIgnore] public string Path { get; private set; }

        public Prefs(SerializationInfo info, StreamingContext context)
        {
            containers = (Dictionary<Type, PersistentContainerBase>)info.GetValue(nameof(containers), typeof(Dictionary<Type, PersistentContainerBase>));
        }

        private Prefs()
        {
            containers = new Dictionary<Type, PersistentContainerBase>();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(containers), containers, typeof(Dictionary<Type, PersistentContainerBase>));
        }

        public void Save()
        {
            Save(this, Path);
        }


        public Pref<T> Get<T>(string key)
        {
            return GetContainer<T>().Get(key);
        }

        public void Get<T>(string key, out Pref<T> value)
        {
            value = GetContainer<T>().Get(key);
        }

        public Pref<T> Get<T>(string key, T defaultValue)
        {
            return GetContainer<T>().Get(key, defaultValue);
        }

        private PersistentContainer<T> GetContainer<T>()
        {
            Type type = typeof(T);
            PersistentContainer<T> container;
            if (containers.TryGetValue(type, out PersistentContainerBase containerBase))
            {
                container = (PersistentContainer<T>)containerBase;
            }
            else
            {
                containers[type] = container = new PersistentContainer<T>();
                container.Init();
            }

            return container;
        }

    }
}