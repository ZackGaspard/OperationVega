
namespace Assets.Scripts
{
    // Namespace to use for the Resource script
    using Resource;

    using UnityEngine;

    /// <summary>
    /// The miner class.
    /// </summary>
    public class Miner : MonoBehaviour, IUnit, IGather, ICombat, IDamageable
    {
        /// <summary>
        /// The miner finite state machine.
        /// Used to keep track of the miners states.
        /// </summary>
        public FiniteStateMachine<string> TheMinerFSM = new FiniteStateMachine<string>();

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
        /// The target click position to move to.
        /// </summary>
        [HideInInspector]
        public Vector3 TargetClickPosition;

        /// <summary>
        /// The target direction to move at.
        /// </summary>
        [HideInInspector]
        public Vector3 TargetDirection;

        /// <summary>
        /// The health of the miner.
        /// </summary>
        [HideInInspector]
        public uint Health;

        /// <summary>
        /// The max health of the miner.
        /// </summary>
        [HideInInspector]
        public uint Maxhealth;

        /// <summary>
        /// The strength of the miner.
        /// </summary>
        [HideInInspector]
        public uint Strength;

        /// <summary>
        /// The defense of the miner.
        /// </summary>
        [HideInInspector]
        public uint Defense;

        /// <summary>
        /// The speed of the miner.
        /// </summary>
        [HideInInspector]
        public uint Speed;

        /// <summary>
        /// The attack speed of the miner.
        /// </summary>
        [HideInInspector]
        public uint Attackspeed;

        /// <summary>
        /// The skill cool down of the miner.
        /// </summary>
        [HideInInspector]
        public uint Skillcooldown;

        /// <summary>
        /// The attack range of the miner.
        /// </summary>
        [HideInInspector]
        public uint Attackrange;

        /// <summary>
        /// The resource count of the miner.
        /// </summary>
        [HideInInspector]
        public uint Resourcecount;

        /// <summary>
        /// The move function providing movement functionality.
        /// </summary>
        public void Move()
        {
            if (Vector3.Magnitude(this.transform.position - this.TargetClickPosition) > 0.1)
            {
                this.transform.position += this.TargetDirection * 2 * Time.deltaTime;
            }
        }

        /// <summary>
        /// The harvest function provides functionality of the miner to harvest a resource.
        /// </summary>
        public void Harvest()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The decontaminate function provides functionality of the miner to decontaminate a resource.
        /// </summary>
        public void Decontaminate()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The attack function gives the extractor functionality to attack.
        /// </summary>
        public void Attack()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The take damage function allows a miner to take damage.
        /// </summary>
        /// <param name="damage">
        /// The amount of damage.
        /// </param>
        public void TakeDamage(uint damage)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The taunt ability for the miner.
        /// </summary>
        public void Taunt()
        {
            
        }

        /// <summary>
        /// The set the target position function.
        /// </summary>
        /// <param name="targetPos">
        /// The target position to go to when clicked.
        /// </param>
        public void SetTheTargetPosition(Vector3 targetPos)
        {
            this.TargetClickPosition = targetPos;
            this.TargetDirection = (this.TargetClickPosition - this.transform.position).normalized;

        }

        /// <summary>
        /// The update function.
        /// </summary>
        private void Update()
        {
            UnitController.Self.CheckIfSelected(this.gameObject);
            this.Move();
        }
    }
}
