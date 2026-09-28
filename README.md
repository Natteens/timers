# Timers

Small Unity timers that tick automatically from the PlayerLoop. No timer GameObject or coroutine owner is needed. Requires Unity 6000.3 or newer.

```csharp
using Timers;

var timer = new CountdownTimer(2f);
timer.OnTimerStop += () => Debug.Log("Done");
timer.Start();
```

`CountdownTimer`, `StopwatchTimer`, `IntervalTimer`, and `FrequencyTimer` preserve the GameInit timer API under the new `Timers` namespace. `Pause()` retains the timer's registration; `Resume()` only resumes a paused timer. Call `Start()` to restart a stopped timer, and `Dispose()` when its owner no longer needs it. Timers use scaled `Time.deltaTime`.

See [Documentation](Documentation~/index.md) for details. MIT license.
