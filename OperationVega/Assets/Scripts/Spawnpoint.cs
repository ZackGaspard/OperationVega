
namespace Assets.Scripts
{
    using UnityEngine;

    /// <summary>
    /// The spawn point class.
    /// It requires a rigidbody to perform collision detection.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class Spawnpoint : MonoBehaviour
    {
        /// <summary>
        /// The the enemy to spawn.
        /// </summary>
        public GameObject TheEnemyToSpawn;

        /// <summary>
        /// The start function.
        /// It sets up the rigidbody characteristics.
        /// </summary>
        private void Start()
        {
            Rigidbody theRigidbody = this.GetComponent<Rigidbody>();

            theRigidbody = this.GetComponent<Rigidbody>();
            theRigidbody.useGravity = false;
            theRigidbody.isKinematic = true;
            theRigidbody.constraints = RigidbodyConstraints.FreezeAll;

            BoxCollider theBoxCollider = this.GetComponent<BoxCollider>();
            theBoxCollider.isTrigger = true;
        }

        /// <summary>
        /// The on collision enter.
        /// </summary>
        /// <param name="other">
        /// The other collider that enters.
        /// </param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent(typeof(IGather)))
            {
                // Spawn
            }
        }
    }
}
