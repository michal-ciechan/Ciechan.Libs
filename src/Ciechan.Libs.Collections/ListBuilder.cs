using System;
using System.Collections.Generic;
using System.Text;

namespace Ciechan.Libs.Collections
{
    public class ListBuilder<T>
    {
        public bool PreferEmpty { get; set; }

        public List<T> ToList()
        {
            if(PreferEmpty)
                return new List<T>();

            return null;
        }
    }
}
