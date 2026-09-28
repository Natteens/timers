using System;
using UnityEngine;

namespace Timers {
    /// <summary>
    /// Countdown timer that fires an event every interval until completion.
    /// </summary>
    public class IntervalTimer : Timer {
        readonly float interval;
        float nextInterval;

        public Action OnInterval = delegate { };

        public IntervalTimer(float totalTime, float intervalSeconds) : base(totalTime) {
            if (intervalSeconds <= 0f || float.IsNaN(intervalSeconds) || float.IsInfinity(intervalSeconds))
                throw new ArgumentOutOfRangeException(nameof(intervalSeconds));
            interval = intervalSeconds;
            nextInterval = totalTime - interval;
        }

        public override void Reset() {
            base.Reset();
            nextInterval = initialTime - interval;
        }

        public override void Tick() {
            if (IsRunning && CurrentTime > 0) {
                CurrentTime -= Time.deltaTime;

                // Fire interval events as long as thresholds are crossed
                while (IsRunning && CurrentTime <= nextInterval && nextInterval >= 0) {
                    OnInterval.Invoke();
                    nextInterval -= interval;
                }
            }

            if (IsRunning && CurrentTime <= 0) {
                CurrentTime = 0;
                Stop();
            }
        }

        public override bool IsFinished => CurrentTime <= 0;
    }
}
