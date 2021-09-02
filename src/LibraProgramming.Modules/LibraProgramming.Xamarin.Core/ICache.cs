namespace LibraProgramming.Xamarin.Core
{
    public interface ICache<in TKey, TValue>
    {
        void Put(TKey key, TValue value);

        TValue Get(TKey key);

        bool Has(TKey key);

        void Invalidate(TKey key);
    }
}