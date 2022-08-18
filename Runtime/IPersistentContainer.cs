namespace Dythervin.PersistentData
{
    public interface IPersistentContainer<T>
    {
        Pref<T> Get(string key);
        bool ContainsKey(string key);
        bool Delete(string key);
        void Clear();
    }
}