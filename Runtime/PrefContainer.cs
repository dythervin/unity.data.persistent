using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Dythervin.PersistentData
{
    [Serializable]
    public partial class PrefContainer : ISerializable
    {
        [JsonProperty] private readonly Dictionary<Type, PersistentContainerBase> _containers;
        [JsonIgnore] public string Path { get; private set; }

        internal PrefContainer(SerializationInfo info, StreamingContext context)
        {
            _containers = (Dictionary<Type, PersistentContainerBase>)info.GetValue(nameof(_containers), typeof(Dictionary<Type, PersistentContainerBase>));
        }

        internal PrefContainer()
        {
            _containers = new Dictionary<Type, PersistentContainerBase>();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(_containers), _containers, typeof(Dictionary<Type, PersistentContainerBase>));
        }

        public void Save()
        {
            Save(this);
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

        public void Clear()
        {
            _containers.Clear();
        }

        private PersistentContainer<T> GetContainer<T>()
        {
            Type type = typeof(T);
            PersistentContainer<T> container;
            if (_containers.TryGetValue(type, out PersistentContainerBase containerBase))
            {
                container = (PersistentContainer<T>)containerBase;
            }
            else
            {
                _containers[type] = container = new PersistentContainer<T>();
                container.Init();
            }

            return container;
        }
    }
}