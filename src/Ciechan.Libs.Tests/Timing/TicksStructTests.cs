using System.Threading;
using Ciechan.Libs.Timing;
using FluentAssertions;
using Xunit;

namespace Ciechan.Libs.Tests.Timing
{
    public class TicksTimerTests
    {
        [Fact]
        public void ElapsedMilliseconds()
        {
            var timer = TicksTimer.Create();

            Thread.Sleep(100);

            var elapsed = timer.ElapsedMilliseconds;

            elapsed.Should().BeApproximately(100, 5);
        }

        [Fact]
        public void Elapsed()
        {
            var timer = TicksTimer.Create();

            Thread.Sleep(100);

            var elapsed = timer.Elapsed;

            var msTicks = Ticks.TicksPerMillisecond;

            elapsed.Should().BeCloseTo(100 * msTicks, 5 * (ulong)msTicks);
        }
    }
}