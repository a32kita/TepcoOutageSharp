using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TepcoOutageSharp
{
    public class TepcoAreaOutageInfoCollection : IReadOnlyCollection<TepcoAreaOutageInfo>
    {
        private List<TepcoAreaOutageInfo> _list;

        public int Count
        {
            get => ((IReadOnlyCollection<TepcoAreaOutageInfo>)_list).Count;
        }

        public TepcoAreaOutageInfoCollection(IEnumerable<TepcoAreaOutageInfo> _infos)
        {
            this._list = new List<TepcoAreaOutageInfo>(_infos);
        }

        public IEnumerator<TepcoAreaOutageInfo> GetEnumerator()
        {
            return ((IEnumerable<TepcoAreaOutageInfo>)_list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }
    }
}
