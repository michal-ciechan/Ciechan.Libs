using System.Diagnostics;

namespace Ciechan.Libs.Timing
{
    public static class StaticStopwatch
    {
        public static readonly Stopwatch Instance = Stopwatch.StartNew();

        internal static readonly Stopwatch StopwatchInstance = Stopwatch.StartNew();

        public static IReadOnlyStopwatch ReadOnly { get; } = new StopwatchWrapper(StopwatchInstance);

        public static double MillisecondsFrequency = ReadOnly.Frequency / 1000;
    }
}
