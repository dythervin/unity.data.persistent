using System;
using System.Collections.Generic;

namespace Dythervin.PersistentData
{
    [Serializable]
    internal class PersistentContainer<T> : PersistentContainerBase, IPersistentContainer<T>
    {
        private Dictionary<string, Pref<T>> _values;

        public void Clear()
        {
            _values.Clear();
        }

        public bool ContainsKey(string key)
        {
            return _values.ContainsKey(key);
        }

        public bool Delete(string key)
        {
            if (!_values.Remove(key))
                return false;

            return true;
        }

        public Pref<T> Get(string key)
        {
            if (!_values.TryGetValue(key, out var value))
                _values[key] = value = new Pref<T>();

            return value;
        }

        public Pref<T> Get(string key, T defaultValue)
        {
            if (!_values.TryGetValue(key, out var value))
            {
                _values[key] = value = new Pref<T>(defaultValue);
            }

            return value;
        }

        internal void Init()
        {
            _values = _values ?? new Dictionary<string, Pref<T>>();
        }
    }
}