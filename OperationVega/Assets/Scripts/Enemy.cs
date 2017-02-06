
namespace Assets.Scripts
{
    // Namespace to use for the Resource script
    using Resource;

    using UnityEngine;

    /// <summary>
    /// The enemy class.
    /// </summary>
    [RequireComponent(typeof(MeshCollider))]
    public class Enemy : MonoBehaviour, IUnit, ICombat, IDamageable
    {
        /// <summary>
        /// The enemy finite state machine.
        /// Used to keep track of the enemy states.
        /// </summary>
        public FiniteStateMachine<string> TheEnemyFSM = new FiniteStateMachine<string>();

        /// <summary>
        /// The target to attack.
        /// </summary>
        [HideInInspector]
        public IDamageable Target;

        /// <summary>
        /// The resource to taint.
        /// </summary>
        [HideInInspector]
        public IResources TargetResource;

        /// <summary>
        /// The health of the enemy.
        /// </summary>
        [HideInInspector]
        public uint Health;

        /// <summary>
        /// The max health of the enemy.
        /// </summary>
        [HideInInspector]
        public uint Maxhealth;

        /// <summary>
        /// The strength of the enemy.
        /// </summary>
        [HideInInspector]
        public uint Strength;

        /// <summary>
        /// The defense of the enemy.
        /// </summary>
        [HideInInspector]
        public uint Defense;

        /// <summary>
        /// The speed of the enemy.
        /// </summary>
        [HideInInspector]
        public uint Speed;

        /// <summary>
        /// The attack speed of the enemy.
        /// </summary>
        [HideInInspector]
        public uint Attackspeed;

        /// <summary>
        /// The skill cool down of the enemy.
        /// </summary>
        [HideInInspector]
        public uint Skillcooldown;

        /// <summary>
        /// The attack range of the enemy.
        /// </summary>
        [HideInInspector]
        public uint Attackrange;

        /// <summary>
        /// The move function providing move functionality to the enemy.
        /// </summary>
        public void Move()
        {
            Debug.Log("I Am Moving");
        }

        /// <summary>
        /// The attack function gives the enemy functionality to attack.
        /// </summary>
        public void Attack()
        {
            Debug.Log("I Am Attacking");
        }

        /// <summary>
        /// The take damage function allows an enemy to take damage.
        /// </summary>
        /// <param name="damage">
        /// The amount of damage.
        /// </param>
        public void TakeDamage(uint damage)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The taint function allows the enemy to taint a resource.
        /// </summary>
        public void Taint()
        {

        }

        /// <summary>
        /// The start function
        /// </summary>
        private void Start()
        {
            MeshCollider mc = this.GetComponent<MeshCollider>();
            mc.sharedMesh = this.GetComponentsInChildren<MeshFilter>()[1].mesh;
        }
    }
}
