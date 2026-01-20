using UnityEngine;

namespace ForestGambit.Core.Bootstrap
{
    /// <summary>
    /// Entry point for the game. Loads in Bootstrap scene.
    /// Initializes core systems before loading main menu.
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        private void Awake()
        {
            // Ensure this persists across scenes
            DontDestroyOnLoad(gameObject);
            
            // TODO: Initialize core systems here
            Debug.Log("Game Bootstrapper: Initializing...");
            
            // TODO: Load Main Menu scene
        }
    }
}
