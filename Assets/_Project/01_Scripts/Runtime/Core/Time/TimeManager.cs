using UnityEngine;

namespace ForestGambit.Core.Time
{
    /// <summary>
    /// Default implementation of ITimeManager.
    /// Manages Unity's Time.timeScale for pause functionality.
    /// </summary>
    public class TimeManager : ITimeManager
    {
        public float TimeScale => UnityEngine.Time.timeScale;
        public bool IsPaused => TimeScale == 0f;
        public float DeltaTime => UnityEngine.Time.deltaTime;

        public void Pause()
        {
            UnityEngine.Time.timeScale = 0f;
            Debug.Log("Game Paused");
        }

        public void Resume()
        {
            UnityEngine.Time.timeScale = 1f;
            Debug.Log("Game Resumed");
        }

        public void TogglePause()
        {
            if (IsPaused)
                Resume();
            else
                Pause();
        }
    }
}
