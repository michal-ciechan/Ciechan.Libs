using System.Collections.Generic;

namespace Ciechan.Libs.Tests.Timing
{
    public class StringNeverEqualEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return false;
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}