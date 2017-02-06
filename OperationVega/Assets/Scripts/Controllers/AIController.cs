
namespace Assets.Scripts.Controllers
{
    using System.Collections.Generic;

    using UnityEngine;

    /// <summary>
    /// The ai controller.
    /// </summary>
    public class AIController : MonoBehaviour
    {
        /// <summary>
        /// The spawnpoints reference.
        /// </summary>
        public List<Transform> Spawnpoints;

        /// <summary>
        /// The enemy prefab reference.
        /// </summary>
        public GameObject EnemyPrefab;

        /// <summary>
        /// The enemies list reference.
        /// </summary>
        private List<Enemy> enemies;

        /// <summary>
        /// The ai controller instance.
        /// </summary>
        private static AIController aicontroller;

        /// <summary>
        /// Gets the instance of the ai controller.
        /// </summary>
        public static AIController Instance
        {
            get
            {
                aicontroller = aicontroller ?? FindObjectOfType(typeof(AIController)) as AIController;
                return aicontroller;
            }
        }

        /// <summary>
        /// The spawn function.
        /// This will spawn enemies
        /// </summary>
        private void Spawn()
        {
        }
       
        /// <summary>
        /// The find closest target function.
        /// Finds the closest target to the enemy
        /// </summary>
        private void FindClosestTarget()
        {
            
        }

        /// <summary>
        /// The set target function.
        /// Sets the target of the passed in enemy.
        /// </summary>
        /// <param name="e">
        /// The enemy to set the target for.
        /// </param>
        private void SetTarget(Enemy e)
        {
            
        }
    }
}
