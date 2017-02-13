
namespace Assets.Scripts
{
    using Controllers;
    using Interfaces;
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
            Debug.Log("Enemy took damage");
            this.Health -= damage;
        }

        /// <summary>
        /// The change states function.
        /// This function changes the state to the passed in state.
        /// <para></para>
        /// <remarks><paramref name="destinationState"></paramref> -The state to transition to.</remarks>
        /// </summary>
        public void ChangeStates(string destinationState)
        {
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
            this.Health = 100;
            Debug.Log(this.Health);

            MeshCollider mc = this.GetComponent<MeshCollider>();
            mc.sharedMesh = this.GetComponentsInChildren<MeshFilter>()[1].mesh;
        }
    }
}
