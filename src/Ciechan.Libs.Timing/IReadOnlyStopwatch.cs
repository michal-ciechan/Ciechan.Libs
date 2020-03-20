namespace Ciechan.Libs.Timing
{
    public interface IReadOnlyStopwatch
    {
        long ElapsedTicks { get; }

        /// <summary>
        /// "Frequency" stores the frequency of the high-resolution performance counter, 
        /// if one exists. Otherwise it will store TicksPerSecond. 
        /// The frequency cannot change while the system is running,
        /// so we only need to initialize it once. 
        /// </summary>
        double Frequency { get; }

        long TickPerMillisecond { get; }
    }
}