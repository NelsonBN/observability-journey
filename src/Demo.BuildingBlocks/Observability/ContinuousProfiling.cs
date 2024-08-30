using Pyroscope;

namespace Common.Observability;

public static class ContinuousProfiling
{
    public static void Setup()
    {
        // Enables or disables CPU/wall profiling dynamically.
        // This function works in conjunction with the PYROSCOPE_PROFILING_CPU_ENABLED and
        // PYROSCOPE_PROFILING_WALLTIME_ENABLED environment variables. If CPU/wall profiling is not
        // configured, this function will have no effect.
        Profiler.Instance.SetCPUTrackingEnabled(true);

        // Enables or disables allocation profiling dynamically.
        // This function works in conjunction with the PYROSCOPE_PROFILING_ALLOCATION_ENABLED environment variable.
        // If allocation profiling is not configured, this function will have no effect.
        Profiler.Instance.SetAllocationTrackingEnabled(true);

        // Enables or disables contention profiling dynamically.
        // This function works in conjunction with the PYROSCOPE_PROFILING_LOCK_ENABLED environment variable.
        // If contention profiling is not configured, this function will have no effect.
        Profiler.Instance.SetContentionTrackingEnabled(true);

        // Enables or disables exception profiling dynamically.
        // This function works in conjunction with the PYROSCOPE_PROFILING_EXCEPTION_ENABLED environment variable.
        // If exception profiling is not configured, this function will have no effect.
        Profiler.Instance.SetExceptionTrackingEnabled(true);
    }
}
