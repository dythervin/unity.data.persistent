using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dythervin.PersistentData
{
    public sealed class Pref<T>
    {
        public event Action<T> OnChanged;
        private T _value;

        [JsonIgnore]

        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value))
                    return;

                _value = value;
                OnChanged?.Invoke(value);
            }
        }

        internal Pref() { }

        internal Pref(T value)
        {
            _value = value;
        }

        public static implicit operator T(Pref<T> value)
        {
            return value._value;
        }
    }
}