
namespace Assets.Scripts.Managers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    using Assets.Scripts.Controllers;

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
        public List<Harvester> TheHarvesters = new List<Harvester>();

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
        /// The start function.
        /// </summary>
        private void Start()
        {
            gameManager = this;
        }

        /// <summary>
        /// Gets the instance.
        /// Returns the GameManager.
        /// </summary>
        public static GameManager Instance
        {
            get
            {
                // If gameManager is null, set gameManager to new gameManager().
                if(gameManager == null)
                    gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;

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

                // Start a coroutine to print the text to the screen -
                // It is a coroutine to assist in helping prevent text objects from
                // spawning on top one another.
                this.StartCoroutine(UnitController.Self.CombatText(null, Color.white, "You win! Thank you for playing!"));
                this.StartCoroutine(this.GameEnd());
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
                // Start a coroutine to print the text to the screen -
                // It is a coroutine to assist in helping prevent text objects from
                // spawning on top one another.
                this.StartCoroutine(UnitController.Self.CombatText(null, Color.white, "Game Over!"));
                this.StartCoroutine(this.GameEnd());
                return true;
            }
            return false;
        }

        /// <summary>
        /// The set up new game function.
        /// This function is used to set up all the data for the user when the game starts new.
        /// </summary>
        public void SetUpNewGame()
        {
            User.FoodCount = 0;
            User.FuelCount = 0;
            User.SteelCount = 0;
            User.CookedFoodCount = 0;
            User.GasCount = 0;
            User.MineralsCount = 0;
            User.UpgradePoints = 0;
            this.HasBuiltShip = false;
            this.TheHarvesters = new List<Harvester>();
        }

        /// <summary>
        /// The game end function.
        /// Gives a delay before switching to the credits scene.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        private IEnumerator GameEnd()
        {
            yield return new WaitForSeconds(5.0f);
            SceneManager.LoadScene(2);
        }
    }
}
