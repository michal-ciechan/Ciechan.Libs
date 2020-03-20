using System;
using System.Diagnostics;

namespace Ciechan.Libs.Timing
{
    public class StopwatchWrapper : IStopwatch
    {
        private readonly Stopwatch _stopwatch;

        public StopwatchWrapper(Stopwatch stopwatch)
        {
            _stopwatch = stopwatch;
        }

        public long ElapsedTicks => _stopwatch.ElapsedTicks;

        /// <summary>
        /// "Frequency" stores the frequency of the high-resolution performance counter, 
        /// if one exists. Otherwise it will store TicksPerSecond. 
        /// The frequency cannot change while the system is running,
        /// so we only need to initialize it once. 
        /// </summary>
        public double Frequency => Stopwatch.Frequency;

        public long TickPerMillisecond { get; } = Stopwatch.Frequency / 1000;
        public void Reset() => _stopwatch.Reset();

        public void Restart() => _stopwatch.Restart();

        public void Start() => _stopwatch.Start();

        public void Stop() => _stopwatch.Stop();

        public TimeSpan Elapsed => _stopwatch.Elapsed;

        public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;

        public bool IsRunning => _stopwatch.IsRunning;
    }
}