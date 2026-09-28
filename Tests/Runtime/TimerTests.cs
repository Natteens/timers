using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Timers.Tests {
    public sealed class TimerTests {
        sealed class TestFrequencyTimer : FrequencyTimer {
            public TestFrequencyTimer(int ticksPerSecond) : base(ticksPerSecond) { }
            public void SetCurrentTime(float value) => CurrentTime = value;
        }

        [Test]
        public void InvalidIntervalsAndFrequenciesAreRejected() {
            Assert.Throws<ArgumentOutOfRangeException>(() => new FrequencyTimer(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new IntervalTimer(1f, 0f));
            Assert.Throws<ArgumentOutOfRangeException>(() => new CountdownTimer(-1f));
        }

        [Test]
        public void FrequencyDoesNotFireAtZeroTimeScale() {
            float originalTimeScale = Time.timeScale;
            var timer = new TestFrequencyTimer(10);
            int ticks = 0;
            try {
                timer.OnTick += () => ticks++;
                timer.Start();
                timer.SetCurrentTime(0.1f);
                Time.timeScale = 0f;
                timer.Tick();
                Assert.Zero(ticks);
            }
            finally {
                Time.timeScale = originalTimeScale;
                timer.Dispose();
            }
        }

        [UnityTest]
        public IEnumerator CountdownPauseResumeAndStop() {
            using var timer = new CountdownTimer(0.12f);
            timer.Start();
            timer.Pause();
            float pausedTime = timer.CurrentTime;
            yield return new WaitForSecondsRealtime(0.03f);
            Assert.That(timer.CurrentTime, Is.EqualTo(pausedTime));
            timer.Resume();
            yield return new WaitForSecondsRealtime(0.2f);
            Assert.IsTrue(timer.IsFinished);
            Assert.IsFalse(timer.IsRunning);
            timer.Resume();
            Assert.IsFalse(timer.IsRunning);
        }

        [UnityTest]
        public IEnumerator StopwatchAndIntervalTickAutomatically() {
            using var stopwatch = new StopwatchTimer();
            using var interval = new IntervalTimer(0.15f, 0.05f);
            int intervals = 0;
            interval.OnInterval += () => intervals++;
            stopwatch.Start();
            interval.Start();
            yield return new WaitForSecondsRealtime(0.25f);
            Assert.Greater(stopwatch.CurrentTime, 0f);
            Assert.GreaterOrEqual(intervals, 2);
            Assert.IsTrue(interval.IsFinished);
        }

        [Test]
        public void PlayerLoopInstallationIsIdempotent() {
            var bootstrapper = typeof(Timer).Assembly.GetType("Timers.Internal.TimerBootstrapper");
            var initialize = bootstrapper.GetMethod("Initialize", BindingFlags.Static | BindingFlags.NonPublic);
            initialize.Invoke(null, null);
            initialize.Invoke(null, null);

            var loop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            var update = Array.Find(loop.subSystemList, part => part.type == typeof(UnityEngine.PlayerLoop.Update));
            int count = 0;
            foreach (var part in update.subSystemList)
                if (part.type?.FullName == "Timers.Internal.TimerManager") count++;
            Assert.AreEqual(1, count);
        }
    }
}
