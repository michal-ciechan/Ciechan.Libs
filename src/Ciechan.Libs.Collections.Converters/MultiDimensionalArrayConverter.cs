using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Ciechan.Libs.Collections.Converters.Attributes;
using Ciechan.Libs.Collections.Converters.Interfaces;
using FastMember;

namespace Ciechan.Libs.Collections.Converters
{
    public static class MultiDimensionalArrayConverter
    {
        public static IEnumerable<T> Deserialize<T>(this IEnumerable<IReadOnlyList<object?>>? array, IReadOnlyList<string> columns) where T : new()
        {
            if (array == null)
                return Enumerable.Empty<T>();

            return DeserializeInternal<T>(array, columns);
        }

        private static IEnumerable<T> DeserializeInternal<T>(IEnumerable<IReadOnlyList<object?>> array,
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

            var rowIndex = -1;
            foreach (var row in array)
            {
                rowIndex++;
                
                var res = new T();

                foreach (var (name, ordinal, member, converter) in colIndexes)
                {
                    if (row.Count <= ordinal) 
                        continue;

                    var value = row[ordinal];

                    if (converter != null)
                    {
                        try
                        {
                            value = converter.Convert(row, value, member.Type);
                        }
                        catch (Exception e)
                        {
                            throw new ColumnConversionException($"Failed to convert column '{name}' (Column Index {ordinal}) value of '{value}' " +
                                                                $"on Row Index {rowIndex} using converter '{converter.GetType().Name}'. " +
                                                                $"Please either fix source data, or implement custom converter using " +
                                                                $"{nameof(IColumnConverter)} interface and decorating relevant properties with [{nameof(ColumnConverterAttribute)}]", e);
                        }
                    }

                    accessor[res, member.Name] = value;
                }

                yield return res;
            }
        }

        private static readonly ConcurrentDictionary<Type, IColumnConverter> _converters = new ConcurrentDictionary<Type, IColumnConverter>();
        
        private static IColumnConverter? GetConverter(Member member)
        {
            var converterAttribute = member.GetAttribute(typeof(ColumnConverterAttribute), true)
                as ColumnConverterAttribute;

            if (converterAttribute == null)
                return DefaultColumnConverter.Instance;

            var type = converterAttribute.Type;

            if (type == null)
                throw new InvalidOperationException($"Property '{member.Name}' must have a {nameof(ColumnConverterAttribute)} Type specified.");

            if(!typeof(IColumnConverter).IsAssignableFrom(type))
                throw new InvalidOperationException($"Property '{member.Name}' must have a {nameof(ColumnConverterAttribute)} Type specified which inherits from {nameof(IColumnConverter)}.");
            
            return _converters.GetOrAdd(type, t => (IColumnConverter) TypeAccessor.Create(t).CreateNew());
        }
        
        public class DefaultColumnConverter : IColumnConverter
        {
            public static DefaultColumnConverter Instance = new DefaultColumnConverter();
            public bool IgnoreInvalidNullableColumnValues { get; set; }

            public object? Convert(object row, object? value, Type targetType)
            {
                if (value == null)
                    return value;

                if (value.GetType() == targetType)
                    return value;
                
                var underlyingType = Nullable.GetUnderlyingType(targetType);

                var isNullable = underlyingType != null;
                
                underlyingType ??= targetType;

                try
                {
                    return System.Convert.ChangeType(value, underlyingType);
                }
                catch
                {
                    if (isNullable && IgnoreInvalidNullableColumnValues)
                    {
                        return null;
                    }

                    throw;
                }
            }
        }

        private static string GetColumnNameFromPropertyMember(Member x)
        {
            if (x.GetAttribute(typeof(ColumnNameAttribute), true) is ColumnNameAttribute attribute)
                return attribute.Name;
            
            return x.Name;
        }
    }

    public class ColumnConversionException : Exception
    {
        public ColumnConversionException()
        {
        }

        protected ColumnConversionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ColumnConversionException(string message) : base(message)
        {
        }

        public ColumnConversionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}