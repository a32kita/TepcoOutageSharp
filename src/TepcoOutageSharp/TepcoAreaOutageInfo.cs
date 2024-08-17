using System;
using System.Collections.Generic;
using System.Text;

namespace TepcoOutageSharp
{
    public class TepcoAreaOutageInfo
    {
        public string Title
        {
            get;
            private set;
        }

        public TepcoAreaCode AreaCode
        {
            get;
            private set;
        }

        public long OutageCount
        {
            get;
            private set;
        }

        public TepcoAreaOutageInfoCollection ChildAreaOutageInfos
        {
            get;
            private set;
        }


        public TepcoAreaOutageInfo(string title, TepcoAreaCode areaCode, long outageCount, IEnumerable<TepcoAreaOutageInfo> childAreaOutageInfos)
        {
            this.Title = title;
            this.AreaCode = areaCode;
            this.OutageCount = outageCount;
            this.ChildAreaOutageInfos = new TepcoAreaOutageInfoCollection(childAreaOutageInfos);
        }
    }
}
