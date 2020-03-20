using System.Diagnostics;
using Ciechan.Libs.Timing;
using FluentAssertions;
using Xunit;

namespace Ciechan.Libs.Tests.Timing
{
    public class StaticStopwatchTests
    {
        [Fact]
        public void Instance_ShouldNotBeSameAsInternalStopwatchInstance()
        {
            StaticStopwatch.Instance.Should().NotBeSameAs(StaticStopwatch.StopwatchInstance);
        }

        [Fact]
        public void MillisecondsFrequency_ShouldReturnReadonlyFrequency()
        {
            StaticStopwatch.MillisecondsFrequency.Should().Be((double)Stopwatch.Frequency / 1000);
        }
    }
}