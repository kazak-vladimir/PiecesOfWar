namespace ForestGambit.Core.Time
{
    /// <summary>
    /// Manages game time flow with pause/resume support.
    /// </summary>
    public interface ITimeManager
    {
        /// <summary>Current time scale (0 = paused, 1 = normal speed)</summary>
        float TimeScale { get; }
        
        /// <summary>Is game currently paused?</summary>
        bool IsPaused { get; }
        
        /// <summary>Delta time scaled by TimeScale</summary>
        float DeltaTime { get; }
        
        /// <summary>Pause the game (sets TimeScale to 0)</summary>
        void Pause();
        
        /// <summary>Resume the game (sets TimeScale to 1)</summary>
        void Resume();
        
        /// <summary>Toggle pause state</summary>
        void TogglePause();
    }
}
