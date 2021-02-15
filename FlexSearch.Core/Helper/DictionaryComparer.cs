using System;
using System.Collections.Generic;

namespace Core.Helper
{
    public class DictionaryComparer : IComparer<Dictionary<string, Guid>>
    {
        public int Compare(Dictionary<string, Guid> x, Dictionary<string, Guid> y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            return x.Count.CompareTo(y.Count);
        }
    }
}