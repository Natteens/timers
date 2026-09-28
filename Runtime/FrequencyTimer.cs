using System;
using UnityEngine;

namespace Timers {
    /// <summary>
    /// Timer that ticks at a specific frequency. (N times per second)
    /// </summary>
    public class FrequencyTimer : Timer {
        public int TicksPerSecond { get; private set; }

        public Action OnTick = delegate { };

        float timeThreshold;

        public FrequencyTimer(int ticksPerSecond) : base(0) {
            CalculateTimeThreshold(ticksPerSecond);
        }

        public override void Tick() {
            float deltaTime = Time.deltaTime;
            if (!IsRunning || Time.timeScale <= 0f || deltaTime <= 0f) return;

            CurrentTime += deltaTime;
            int ticks = 0;
            while (IsRunning && CurrentTime >= timeThreshold && ticks++ < 64) {
                CurrentTime -= timeThreshold;
                OnTick.Invoke();
            }
            if (IsRunning && CurrentTime >= timeThreshold)
                CurrentTime %= timeThreshold;
        }

        public override bool IsFinished => !IsRunning;

        public override void Reset() {
            CurrentTime = 0;
        }

        public void Reset(int newTicksPerSecond) {
            CalculateTimeThreshold(newTicksPerSecond);
            Reset();
        }

        void CalculateTimeThreshold(int ticksPerSecond) {
            if (ticksPerSecond <= 0) throw new ArgumentOutOfRangeException(nameof(ticksPerSecond));
            TicksPerSecond = ticksPerSecond;
            timeThreshold = 1f / TicksPerSecond;
        }
    }
}
