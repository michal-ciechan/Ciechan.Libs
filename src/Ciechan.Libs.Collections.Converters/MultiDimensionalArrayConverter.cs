using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Ciechan.Libs.Collections.Converters.Attributes;
using Ciechan.Libs.Collections.Converters.Interfaces;
using FastMember;

namespace Ciechan.Libs.Collections.Converters
{
    public static class MultiDimensionalArrayConverter
    {
        public static IEnumerable<T> Deserialize<T>(this IEnumerable<IReadOnlyList<object>>? array, IReadOnlyList<string> columns) where T : new()
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
                .Select(x =>
                {
                    var member = typeMembers[x.Name];

                    var converter = GetConverter(member);
                        
                    
                    return (x.Name, x.Ordinal, member, converter);
                })
                .ToList();

            foreach (var row in array)
            {
                var res = new T();

                foreach (var (_, ordinal, member, converter) in colIndexes)
                {
                    if (row.Count <= ordinal) 
                        continue;

                    var value = row[ordinal];

                    if (converter != null)
                        value = converter.Convert(row, value, member.Type);

                    accessor[res, member.Name] = value;
                }

                yield return res;
            }
        }

        private static IColumnConverter? GetConverter(Member member)
        {
            var converterAttribute = member.GetAttribute(typeof(ColumnConverterAttribute), true)
                as ColumnConverterAttribute;

            if (converterAttribute == null)
                return null;

            var type = converterAttribute.Type;

            if (type == null)
                throw new InvalidOperationException($"Property '{member.Name}' must have a {nameof(ColumnConverterAttribute)} Type specified.");

            if(!typeof(IColumnConverter).IsAssignableFrom(type))
                throw new InvalidOperationException($"Property '{member.Name}' must have a {nameof(ColumnConverterAttribute)} Type specified which inherits from {nameof(IColumnConverter)}.");
            
            return (IColumnConverter) TypeAccessor.Create(type).CreateNew();
        }

        private static string GetColumnNameFromPropertyMember(Member x)
        {
            if (x.GetAttribute(typeof(ColumnNameAttribute), true) is ColumnNameAttribute attribute)
                return attribute.Name;
            
            return x.Name;
        }
    }
}