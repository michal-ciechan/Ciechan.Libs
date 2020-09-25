using Ciechan.Libs.Collections;
using FluentAssertions;
using Xunit;

namespace Ciechan.Libs.Tests.Collections
{
    public class ListBuilderTests
    {
        [Fact]
        public void Default_ShouldReturnNull()
        {
            var builder = new ListBuilder<int>();

            var res = builder.ToList();

            res.Should().BeNull();
        }
        
        [Fact]
        public void Default_PreferEmpty_ShouldReturnEmptyList()
        {
            var builder = new ListBuilder<int>
            {
                PreferEmpty = true,
            };

            var res = builder.ToList();

            res.Should().BeEmpty();
        }
    }
}
