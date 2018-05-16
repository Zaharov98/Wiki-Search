using System;
using System.Collections.Generic;
using System.Linq;

namespace WikiCrawler.SizedCache
{
    internal class SizedCache<T> : ICache<T> 
    {
        private int size;
        private IDictionary<T, int> _cacheRows;

        private const int _additionPower = 1;  // if item in cache is requested, it's power will encrease
        private const int _newItemPower = 2;   // initial power of new item in cache

        public SizedCache(int size)
        {
            this.size = size;
            _cacheRows = new Dictionary<T, int>(size);
        }


        public void Add(T item)
        {
            if (_cacheRows.ContainsKey(item)) {
                _cacheRows[item] += _additionPower;
            }
            else {
                DecreceCacheItemsPower();
                TryFitCache();

                _cacheRows.Add(item, _newItemPower);
            }
        }


        public bool Contains(T item)
        {
            return _cacheRows.ContainsKey(item);
        }


        private void DecreceCacheItemsPower()
        {
            foreach (var key in _cacheRows.Keys.ToList())
            {
                _cacheRows[key] -= 1;
            }
        }


        private void TryFitCache()
        {
            if (_cacheRows.Count > size)
            {
                IEnumerable<T> lowPowerRows = (from key in _cacheRows.Keys
                                              where _cacheRows[key] < 1
                                              select key).ToList();
                foreach (var row in lowPowerRows) {
                    _cacheRows.Remove(row);
                }                    
            }
        }
    }
}