using System;
using UnityEngine;

namespace Dythervin.PersistentData
{
    public abstract class PlayerPref
    {
        public string key;

        protected PlayerPref(string key)
        {
            this.key = key;
        }
    }

    public abstract class PlayerPref<T> : PlayerPref
    {
        private T _value;
        private bool _isLoaded;

        protected PlayerPref(string key, T defaultValue = default) : base(key)
        {
            _value = defaultValue;
        }

        public T Value
        {
            get
            {
                if (!_isLoaded)
                {
                    if (IsSaved)
                    {
                        _value = PersistentValue;
                    }

                    _isLoaded = true;
                }

                return _value;
            }

            set
            {
                PersistentValue = value;
                _value = value;
                _isLoaded = true;
            }
        }

        public bool IsSaved => PlayerPrefs.HasKey(key);

        protected abstract T PersistentValue { get; set; }
    }

    public class PlayerPrefInt : PlayerPref<int>
    {
        public PlayerPrefInt(string key, int defaultValue = default) : base(key, defaultValue) { }

        protected override int PersistentValue
        {
            get => PlayerPrefs.GetInt(key);
            set => PlayerPrefs.SetInt(key, value);
        }
    }

    public class PlayerPrefFloat : PlayerPref<float>
    {
        public PlayerPrefFloat(string key, float defaultValue = default) : base(key, defaultValue) { }

        protected override float PersistentValue
        {
            get => PlayerPrefs.GetFloat(key);
            set => PlayerPrefs.SetFloat(key, value);
        }
    }

    public class PlayerPrefString : PlayerPref<string>
    {
        public PlayerPrefString(string key, string defaultValue = default) : base(key, defaultValue) { }

        protected override string PersistentValue
        {
            get => PlayerPrefs.GetString(key);
            set => PlayerPrefs.SetString(key, value);
        }
    }

    public class PlayerPrefBool : PlayerPref<bool>
    {
        public PlayerPrefBool(string key, bool defaultValue = default) : base(key, defaultValue) { }

        protected override bool PersistentValue
        {
            get => PlayerPrefs.GetInt(key) == 1;
            set => PlayerPrefs.SetInt(key, value ? 1 : 0);
        }
    }
}