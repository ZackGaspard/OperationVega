
namespace Assets.Scripts.Managers
{
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// The game manager class. This class is responsible for 
    /// managing the win/loss conditions of the game.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// The the harvesters list.
        /// This stores all the harvesters currently in play.
        /// </summary>
        [HideInInspector]
        public List<Harvester> TheHarvesters;

        /// <summary>
        /// The has built ship variable.
        /// This determines whether the ship has been built or not.
        /// </summary>
        [HideInInspector]
        public bool HasBuiltShip;

        /// <summary>
        /// The game manager variable.
        /// </summary>
        private static GameManager gameManager;

        /// <summary>
        /// Prevents a default instance of the <see cref="GameManager"/> class from being created.
        /// </summary>
        private GameManager()
        {
            this.TheHarvesters = new List<Harvester>();
            this.HasBuiltShip = false;
        }

        /// <summary>
        /// Gets the instance.
        /// Returns the GameManager.
        /// </summary>
        public static GameManager Instance
        {
            get
            {
                // Set gameManager to the value of gameManager if gameManager is NOT null; otherwise,
                // if gameManager = null, set gameManager to new gameManager().
                gameManager = gameManager ?? FindObjectOfType(typeof(GameManager)) as GameManager;
                return gameManager;
            }
        }

        /// <summary>
        /// The check for win function.
        /// Checks if the user has met the win condition.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckForWin()
        {
            if (this.HasBuiltShip)
            {
                ObjectiveManager.Instance.TheObjectives[ObjectiveType.Main].Currentvalue++;

                SceneManager.LoadScene(0);
                return true;
            }
            return false;
        }

        /// <summary>
        /// The check for loss function.
        /// Checks if the user has met the loss condition.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckForLoss()
        {
            // If there are no harvesters left on the map, cant purchase anymore, the ship hasnt been built, and you dont have the rigth parts to build the ship.
            if (this.TheHarvesters.Count == 0 && User.FoodCount < 5 && !this.HasBuiltShip)
            {
                Debug.Log("Game Over - No more food to purchase harvesters, no more harvesters left on the map and the ship hasnt been built");
                return true;
            }
            return false;
        }
    }
}
