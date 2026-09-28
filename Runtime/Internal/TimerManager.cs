using System.Collections.Generic;

namespace Timers.Internal {
    internal static class TimerManager {
        private static readonly List<Timer> Timers = new();
        private static readonly List<Timer> Sweep = new();

        public static void RegisterTimer(Timer timer) {
            if (!Timers.Contains(timer)) Timers.Add(timer);
        }
        public static void DeregisterTimer(Timer timer) => Timers.Remove(timer);

        public static void UpdateTimers() {
            if (Timers.Count == 0) {
                Sweep.Clear();
                return;
            }

            Sweep.RefreshWith(Timers);
            foreach (var timer in Sweep) {
                if (timer.IsRunning) timer.Tick();
            }
        }

        public static void Clear() {
            Sweep.RefreshWith(Timers);
            foreach (var timer in Sweep) {
                timer.Dispose();
            }

            Timers.Clear();
            Sweep.Clear();
        }
    }
}
