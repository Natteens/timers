using System;
using Timers.Internal;
using UnityEngine;

namespace Timers {
    public abstract class Timer : IDisposable {
        public float CurrentTime { get; protected set; }
        public bool IsRunning { get; private set; }

        protected float initialTime;

        public float Progress => initialTime > 0f ? Mathf.Clamp01(CurrentTime / initialTime) : 0f;

        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };

        protected Timer(float value) {
            ValidateDuration(value);
            initialTime = value;
        }

        public void Start() {
            if (disposed) throw new ObjectDisposedException(GetType().Name);
            CurrentTime = initialTime;
            if (!IsRunning) {
                IsRunning = true;
                TimerManager.RegisterTimer(this);
                registered = true;
                OnTimerStart.Invoke();
            }
        }

        public void Stop() {
            if (registered) {
                IsRunning = false;
                TimerManager.DeregisterTimer(this);
                registered = false;
                OnTimerStop.Invoke();
            }
        }

        public abstract void Tick();
        public abstract bool IsFinished { get; }

        public void Resume() {
            if (registered) IsRunning = true;
        }
        public void Pause() => IsRunning = false;

        public virtual void Reset() => CurrentTime = initialTime;

        public virtual void Reset(float newTime) {
            ValidateDuration(newTime);
            initialTime = newTime;
            Reset();
        }

        bool disposed;
        bool registered;

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposed) return;

            if (disposing && registered) {
                TimerManager.DeregisterTimer(this);
            }

            IsRunning = false;
            registered = false;
            disposed = true;
        }

        static void ValidateDuration(float value) {
            if (value < 0f || float.IsNaN(value) || float.IsInfinity(value))
                throw new ArgumentOutOfRangeException(nameof(value));
        }
    }
}
