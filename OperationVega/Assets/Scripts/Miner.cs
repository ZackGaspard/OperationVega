
namespace Assets.Scripts
{
    using Controllers;
    using Interfaces;
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
        public FiniteStateMachine<string> TheMinerFsm = new FiniteStateMachine<string>();

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
        public float Attackrange;

        /// <summary>
        /// The resource count of the miner.
        /// </summary>
        [HideInInspector]
        public uint Resourcecount;

        /// <summary>
        /// The idle delegate.
        /// This delegate contains the functions for the idle state.
        /// </summary>
        private Handler idleDelegate;

        /// <summary>
        /// The battle delegate.
        /// This delegate contains the functions for the battle state.
        /// </summary>
        private Handler battleDelegate;

        /// <summary>
        /// The harvest delegate.
        /// This delegate contains the functions for the harvest state.
        /// </summary>
        private Handler harvestDelegate;

        /// <summary>
        /// The handler delegate.
        /// </summary>
        private delegate void Handler();

        /// <summary>
        /// The move function providing movement functionality.
        /// </summary>
        public void Move()
        {
            if (Vector3.Magnitude(this.transform.position - this.TargetClickPosition) > this.Attackrange)
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
            Debug.Log("Attacking");
            this.Target.TakeDamage(5);
            this.Attackrange = 5.0f;
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
        /// The initialize unit function.
        /// This will initialize the unit with the appropriate values for stats.
        /// </summary>
        private void InitUnit()
        {
            this.Attackrange = 0.1f;
            Debug.Log("Miner Initialized");
        }

        /// <summary>
        /// The reset range function.
        /// This resets the range of distance the unit stands from the clicked position.
        /// </summary>
        private void ResetRange()
        {
            Debug.Log("In Idle State");
            this.Attackrange = 0.1f;
        }

        /// <summary>
        /// The awake function.
        /// </summary>
        private void Awake()
        {
            this.idleDelegate = this.ResetRange;
            this.battleDelegate = this.Attack;
            this.harvestDelegate = this.Harvest;

            this.TheMinerFsm.CreateState("Init", null);
            this.TheMinerFsm.CreateState("Idle", this.idleDelegate);
            this.TheMinerFsm.CreateState("Battle", this.battleDelegate);
            this.TheMinerFsm.CreateState("Harvest", this.harvestDelegate);

            this.TheMinerFsm.AddTransition("Init", "Idle", "auto");
            this.TheMinerFsm.AddTransition("Idle", "Battle", "IdleToBattle");
            this.TheMinerFsm.AddTransition("Battle", "Idle", "BattleToIdle");
            this.TheMinerFsm.AddTransition("Idle", "Harvest", "IdleToHarvest");
            this.TheMinerFsm.AddTransition("Harvest", "Idle", "HarvestToIdle");
        }

        /// <summary>
        /// The start function.
        /// </summary>
        private void Start()
        {
            this.TheMinerFsm.Feed("auto");
            this.InitUnit();
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
