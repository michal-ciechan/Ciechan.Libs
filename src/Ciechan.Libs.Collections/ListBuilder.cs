using System.Collections.Generic;

namespace Ciechan.Libs.Collections
{
    public class ListBuilder<T>
    {
        public bool PreferEmpty { get; set; }

        public List<T> ToList()
        {
            return new List<T>();
        }

        public List<T>? ToNullableList()
        {
            return PreferEmpty ? new List<T>() : null;
        }
    }
}
