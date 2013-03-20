using System.Collections.Generic;

namespace BabylonToHtml.BabylonReader
{
    public class Set<T> : IEnumerable<T>
    {
        Dictionary<T, byte> _dict = new Dictionary<T, byte>();

        public bool Contains(T entry) { return _dict.ContainsKey(entry); }
        public int Count { get { return _dict.Count; } }
        public void Add(T entry)
        {
            if (Contains(entry)) { return; }
            _dict.Add(entry, 0);
        }
        public bool Remove(T entry) { { return _dict.Remove(entry); } }
        public void Clear() { _dict.Clear(); }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return _dict.Keys.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_dict.Keys).GetEnumerator();
        }

        #endregion
    }
}
