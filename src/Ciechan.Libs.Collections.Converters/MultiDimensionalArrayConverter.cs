using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FastMember;

namespace Ciechan.Libs.Collections.Converters
{
    public static class MultiDimensionalArrayConverter
    {
        public static IEnumerable<T> Deserialize<T>(this IEnumerable<IReadOnlyList<object>> array, IReadOnlyList<string> columns) where T : new()
        {
            if (array == null)
                return Enumerable.Empty<T>();

            return DeserializeInternal<T>(array, columns);
        }

        private static IEnumerable<T> DeserializeInternal<T>(IEnumerable<IReadOnlyList<object>> array,
            IReadOnlyList<string> cols, IEqualityComparer<string>? columnComparer = null) where T : new()
        {
            columnComparer ??= EqualityComparer<string>.Default;
            
            var accessor = TypeAccessor.Create(typeof(T));

            var typeMembers = accessor.GetMembers().ToDictionary(GetColumnNameFromPropertyMember, m => m, columnComparer);

            var colIndexes = cols.Select((s, i) => (Name: s, Ordinal: i))
                .Where(x => typeMembers.ContainsKey(x.Name))
                .Select(x => (x.Name, x.Ordinal, typeMembers[x.Name]))
                .ToList();

            foreach (var row in array)
            {
                var res = new T();

                foreach (var (name, ordinal, member) in colIndexes)
                {
                    if(row.Count > ordinal)
                        accessor[res, member.Name] = row[ordinal];
                }

                yield return res;
            }
        }

        private static string GetColumnNameFromPropertyMember(Member x)
        {
            if (x.GetAttribute(typeof(DisplayNameAttribute), true) is DisplayNameAttribute attribute)
                return attribute.DisplayName;
            
            return x.Name;
        }
    }
}