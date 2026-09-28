# Timers

`Timer` exposes `CurrentTime`, `IsRunning`, `Progress`, `Start`, `Pause`, `Resume`, `Stop`, `Reset`, and `Dispose`.

- `CountdownTimer(duration)` stops at zero.
- `StopwatchTimer()` counts upward.
- `IntervalTimer(totalTime, intervalSeconds)` invokes `OnInterval` for crossed intervals.
- `FrequencyTimer(ticksPerSecond)` invokes `OnTick` at the requested frequency.

`Start()` resets to the initial value. `Pause()` and `Resume()` retain elapsed time. `Stop()` deregisters; use `Start()` to run again. Durations must be finite and non-negative; intervals and frequencies must be positive.

Frequency timers catch up missed ticks, with at most 64 callbacks per frame to avoid a long stall after a large delta-time spike.
