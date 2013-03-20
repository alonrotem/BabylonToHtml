using System.Collections.Generic;

namespace BabylonToHtml.BabylonReader
{
    public class MultiDictionary<Key, Value>
    {
        Dictionary<Key, List<Value>> _dict = new Dictionary<Key, List<Value>>();
        readonly List<Value> EmptyList = new List<Value>();

        public MultiDictionary() { }

        /// <summary>
        /// Add a value mapping to a key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(Key key, Value value)
        {
            if (!_dict.ContainsKey(key))
            {
                _dict.Add(key, new List<Value>());
            }

            // TODO: inefficient for large lists, but hopefully won't be large
            List<Value> vals = _dict[key];
            if (!vals.Contains(value)) { vals.Add(value); }
        }

        /// <summary>
        /// Remove the value mapping to the key.
        /// Removes the key itself as well if zero values remain.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Remove(Key key, Value value)
        {
            if (!_dict.ContainsKey(key)) { return false; }

            List<Value> vals = _dict[key];
            vals.Remove(value);

            if (vals.Count == 0) { _dict.Remove(key); }
            return true;
        }

        /// <summary>
        /// Remove all values mapping to the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveAll(Key key)
        {
            return _dict.Remove(key);
        }

        /// <summary>
        /// Clear all data.
        /// </summary>
        public void Clear()
        {
            _dict.Clear();
        }

        /// <summary>
        /// Get the number of values mapping to the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int ValueCount(Key key)
        {
            return this[key].Count;
        }

        /// <summary>
        /// Get a list of values mapping to the key, empty list if none.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<Value> this[Key key]
        {
            get
            {
                if (!_dict.ContainsKey(key)) { return EmptyList; }
                return _dict[key];
            }
        }

        /// <summary>
        /// Get an enumerable collection of keys.
        /// </summary>
        public Dictionary<Key, List<Value>>.KeyCollection Keys
        {
            get { return _dict.Keys; }
        }
    }
}
