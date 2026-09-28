#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Timers.Internal {
    internal static class TimerBootstrapper {
        static PlayerLoopSystem _timerSystem;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() {
            Uninstall();
            TimerManager.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Initialize() {
            Uninstall();
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

            if (!InsertTimerManager<Update>(ref currentPlayerLoop, 0)) {
                Debug.LogWarning("Timers could not register with the Unity PlayerLoop.");
                return;
            }
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeState;
            EditorApplication.playModeStateChanged += OnPlayModeState;

            static void OnPlayModeState(PlayModeStateChange state) {
                if (state == PlayModeStateChange.ExitingPlayMode) {
                    Uninstall();
                    TimerManager.Clear();
                }
            }
#endif
        }

        static void Uninstall() {
            _timerSystem = CreateTimerSystem();
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            RemoveTimerManager<Update>(ref currentPlayerLoop);
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
        }

        static void RemoveTimerManager<T>(ref PlayerLoopSystem loop) {
            PlayerLoopUtils.RemoveSystem<T>(ref loop, in _timerSystem);
        }

        static bool InsertTimerManager<T>(ref PlayerLoopSystem loop, int index) {
            _timerSystem = CreateTimerSystem();
            return PlayerLoopUtils.InsertSystem<T>(ref loop, in _timerSystem, index);
        }

        static PlayerLoopSystem CreateTimerSystem() {
            return new PlayerLoopSystem() {
                type = typeof(TimerManager),
                updateDelegate = TimerManager.UpdateTimers,
                subSystemList = null
            };
        }
    }
}
