using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Storage.Tree
{
    public interface IIndex<K, V>
    {
        public Task<bool> Delete(K key, V value, IComparer<V> valueComparer = null);
        public Task<bool> Delete(K key);
        public Task Insert(K key, V value);
        public Task<Tuple<K, V>> Get(K key);
    }
}