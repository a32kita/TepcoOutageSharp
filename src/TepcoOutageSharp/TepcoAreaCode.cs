using System;
using System.Collections.Generic;
using System.Text;

namespace TepcoOutageSharp
{
    public struct TepcoAreaCode
    {
        public string Code
        {
            get;
            set;
        }

        public TepcoAreaCode(string code)
        {
            this.Code = code;
        }

        public override string ToString()
        {
            return this.Code;
        }
    }
}
