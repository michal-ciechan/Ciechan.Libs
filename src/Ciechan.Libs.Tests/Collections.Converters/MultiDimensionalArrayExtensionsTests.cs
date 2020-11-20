using System;
using System.Linq;
using Ciechan.Libs.Collections.Converters;
using Ciechan.Libs.Collections.Converters.Attributes;
using Ciechan.Libs.Collections.Converters.Interfaces;
using FluentAssertions;
using Snapshooter.Xunit;
using Xunit;

namespace Ciechan.Libs.Tests.Collections.Converters
{
    public class MultiDimensionalArrayExtensionsTests
    {
        public MultiDimensionalArrayExtensionsTests()
        {
            MultiDimensionalArrayConverter.ColumnConverter.Instance = new MultiDimensionalArrayConverter.ColumnConverter();
        }
        public class Sample<T1,T2>
        {
            public T1 First { get; set; }
            public T2 Second { get; set; }
        }
        public class Sample<T1,T2,T3>
        {
            public T1 First { get; set; }
            public T2 Second { get; set; }
            public T3 Third { get; set; }
        }
        
        public class ColumnNameDecoratedSample<T1,T2>
        {
            [ColumnName("Column.1")]
            public T1 First { get; set; }
            
            [ColumnName("Column.2")]
            public T2 Second { get; set; }
        }
        
        public class StringToStaticIntConverterSample
        {
            [ColumnConverter(typeof(StringToStaticIntConverter))]
            public int First { get; set; }
        }
        
        public class NullableDecimalSample
        {
            public decimal? First { get; set; }
        }

        public class StringToStaticIntConverter : IColumnConverter
        {
            public object? Convert(object row, object? value, Type targetType)
            {
                return 69;
            }
        }

        [Fact]
        public void Deserialize_2Cols_TypeDouble()
        {
            var array = new[]
            {
                new object[]{1.1, 1.2}, 
                new object[]{2.1, 2.2}, 
            };
            
            var cols = new[] {"First", "Second"};

            var res = array.Deserialize<Sample<double, double>>(cols);

            Snapshot.Match(res);
        }
        
        [Fact]
        public void Deserialize_3Cols_BadIndexRow()
        {
            var array = new[]
            {
                new object[]{1.1, 1.2}, 
                new object[]{2.1, 2.2, 2.3}, 
            };
            
            var cols = new [] {"First", "NA", "Third"};

            var res = array.Deserialize<Sample<double, double, double>>(cols);

            Snapshot.Match(res);
        }
        
        [Fact]
        public void Deserialize_2ols_BadIndexRow()
        {
            var array = new[]
            {
                new object[]{1.1, 1.2}, 
                new object[]{2.1, 2.2, 2.3}, 
            };
            
            var cols = new [] {"First", "NA", "Second"};

            var res = array.Deserialize<Sample<double, double>>(cols);

            Snapshot.Match(res);
        }
        
        [Fact]
        public void Deserialize_2Cols_DecoratedSample()
        {
            var array = new[]
            {
                new object[]{11, 12}, 
                new object[]{21, 22}, 
            };
            
            var cols = new[] {"Column.1", "Column.2"};

            var res = array.Deserialize<ColumnNameDecoratedSample<int, int>>(cols);

            Snapshot.Match(res);
        }
        
        [Fact]
        public void Deserialize_StringToStaticIntConverterSample()
        {
            var array = new[]
            {
                new object[]{"11", "12"}, 
                new object[]{"21", "22"}, 
            };
            
            var cols = new[] {"First", "Second"};

            var res = array.Deserialize<StringToStaticIntConverterSample>(cols)
                .ToList();

            Snapshot.Match(res);
        }
        
        [Fact]
        public void Deserialize_DefaultConverter_StringToInt()
        {
            var array = new[]
            {
                new object[]{"11", "12"}, 
                new object[]{"21", "22"}, 
            };
            
            var cols = new[] {"First", "Second"};

            var res = array.Deserialize<Sample<int, int?>>(cols)
                .ToList();

            Snapshot.Match(res);
        }
        
        [Fact]
        public void Deserialize_DefaultConverter_StringToLong()
        {
            var array = new[]
            {
                new object[]{"11", "12"}, 
                new object[]{"21", "22"}, 
            };
            
            var cols = new[] {"First", "Second"};

            var res = array.Deserialize<Sample<long, long?>>(cols)
                .ToList();

            Snapshot.Match(res);
        }
        
        [Fact]
        public void Deserialize_DefaultConverter_StringToULong()
        {
            var array = new[]
            {
                new object[]{"11", "12"}, 
                new object[]{"21", "22"}, 
            };
            
            var cols = new[] {"First", "Second"};

            var res = array.Deserialize<Sample<ulong, ulong?>>(cols)
                .ToList();

            Snapshot.Match(res);
        }
        
        [Fact]
        public void Deserialize_DefaultConverter_StringToByte()
        {
            var array = new[]
            {
                new object[]{"11", "12"}, 
                new object[]{"21", "22"}, 
            };
            
            
            var cols = new[] {"First", "Second"};

            var res = array.Deserialize<Sample<byte, byte?>>(cols)
                .ToList();

            Snapshot.Match(res);
        }
        
        [Fact]
        public void Deserialize_DoubleToNullableDecimal()
        {
            var array = new[]
            {
                new object[]{1.1, 1.2}, 
                new object[]{2.1, 2.2}, 
            };
            
            var cols = new[] {"First", "Second"};

            var res = array.Deserialize<NullableDecimalSample>(cols)
                .ToList();

            Snapshot.Match(res);
        }
        
        [Fact]
        public void Deserialize_StringToNullableDecimal()
        {
            var array = new[]
            {
                new object[]{"1.1", "1.2"}, 
                new object[]{"2.1", "2.2"}, 
            };
            
            var cols = new[] {"First", "Second"};

            var res = array.Deserialize<NullableDecimalSample>(cols)
                .ToList();

            Snapshot.Match(res);
        }
        
        [Fact]
        public void Deserialize_NullObjectToNullableDecimal()
        {
            var array = new[]
            {
                new object?[]{null, "1.2"}, 
            };
            
            var cols = new[] {"First", "Second"};

            var res = array.Deserialize<NullableDecimalSample>(cols)
                .ToList();

            Snapshot.Match(res);
        }
        
        [Fact]
        public void Deserialize_InvalidStringToNullableDecimal_ShouldThrowHelpfulException()
        {
            var array = new[]
            {
                new object?[]{"NA", "1.2"}, 
            };
            
            var cols = new[] {"First", "Second"};

            Action act = () => array.Deserialize<NullableDecimalSample>(cols)
                .ToList();

            var msg = act.Should().Throw<ColumnConversionException>().Which.Message;

            msg.Should().Contain("First");
            msg.Should().Contain("NA");
            msg.Should().Contain(nameof(MultiDimensionalArrayConverter.ColumnConverter));
        }
        
        [Fact]
        public void Deserialize_InvalidStringToNullableDecimal_IgnoreInvalidNullableColumnValues()
        {
            MultiDimensionalArrayConverter.ColumnConverter.Instance.IgnoreInvalidNullableColumnValues = true;
            
            var array = new[]
            {
                new object?[]{"NA", "1.2"}, 
            };
            
            var cols = new[] {"First", "Second"};

            var res = array.Deserialize<Sample<decimal?, decimal?>>(cols)
                .ToList();

            Snapshot.Match(res);
        }
    }
}
