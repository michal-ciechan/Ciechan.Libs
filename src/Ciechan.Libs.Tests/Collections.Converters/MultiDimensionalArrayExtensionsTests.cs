﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Ciechan.Libs.Collections;
using Ciechan.Libs.Collections.Converters;
using FluentAssertions;
using Snapshooter.Xunit;
using Xunit;

namespace Ciechan.Libs.Tests.Collections.Converters
{
    public class MultiDimensionalArrayExtensionsTests
    {
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
            [DisplayName("Column.1")]
            public T1 First { get; set; }
            
            [DisplayName("Column.2")]
            public T2 Second { get; set; }
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
    }
}
