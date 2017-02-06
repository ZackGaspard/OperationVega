
namespace Assets.Scripts
{
    using Controllers;
    using Interfaces;
    using UnityEngine;

    /// <summary>
    /// The harvester class.
    /// </summary>
    public class Harvester : MonoBehaviour, IUnit, IGather, IDamageable
    {
        /// <summary>
        /// The harvester finite state machine.
        /// Used to keep track of the harvesters states.
        /// </summary>
        public FiniteStateMachine<string> TheHarvesterFsm = new FiniteStateMachine<string>();

        /// <summary>
        /// The target to heal/stun.
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
        /// The health of the harvester.
        /// </summary>
        [HideInInspector]
        public uint Health;

        /// <summary>
        /// The max health of the harvester.
        /// </summary>
        [HideInInspector]
        public uint Maxhealth;

        /// <summary>
        /// The strength of the harvester.
        /// </summary>
        [HideInInspector]
        public uint Strength;

        /// <summary>
        /// The defense of the harvester.
        /// </summary>
        [HideInInspector]
        public uint Defense;

        /// <summary>
        /// The speed of the harvester.
        /// </summary>
        [HideInInspector]
        public uint Speed;

        /// <summary>
        /// The attack speed of the harvester.
        /// </summary>
        [HideInInspector]
        public uint Attackspeed;

        /// <summary>
        /// The skill cool down of the harvester.
        /// </summary>
        [HideInInspector]
        public uint Skillcooldown;

        /// <summary>
        /// The heal range of the harvester.
        /// </summary>
        [HideInInspector]
        public uint Healrange;

        /// <summary>
        /// The resource count of the harvester.
        /// </summary>
        [HideInInspector]
        public uint Resourcecount;

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
           if (Vector3.Magnitude(this.transform.position - this.TargetClickPosition) > 0.1)
           {
              this.transform.position += this.TargetDirection * 2 * Time.deltaTime;
           }
        }

        /// <summary>
        /// The harvest function provides functionality of the harvester to harvest a resource.
        /// </summary>
        public void Harvest()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The decontaminate function provides functionality of the harvester to decontaminate a resource.
        /// </summary>
        public void Decontaminate()
        {
            throw new System.NotImplementedException();
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
        /// The take damage function allows a harvester to take damage.
        /// </summary>
        /// <param name="damage">
        /// The amount of damage.
        /// </param>
        public void TakeDamage(uint damage)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The heal stun function gives the harvester functionality to heal units and stun enemies.
        /// </summary>
        public void HealStun()
        {
          
        }

        /// <summary>
        /// The initialize unit function.
        /// This will initialize the unit with the appropriate values for stats.
        /// </summary>
        private void InitUnit()
        {
            Debug.Log("Harvester Initialized");
        }

        /// <summary>
        /// The awake function.
        /// </summary>
        private void Awake()
        {
            this.battleDelegate = this.HealStun;
            this.harvestDelegate = this.Harvest;

            this.TheHarvesterFsm.CreateState("Init", null);
            this.TheHarvesterFsm.CreateState("Idle", null);
            this.TheHarvesterFsm.CreateState("Battle", this.battleDelegate);
            this.TheHarvesterFsm.CreateState("Harvest", this.harvestDelegate);

            this.TheHarvesterFsm.AddTransition("Init", "Idle", "auto");
            this.TheHarvesterFsm.AddTransition("Idle", "Battle", "IdleToBattle");
            this.TheHarvesterFsm.AddTransition("Battle", "Idle", "BattleToIdle");
            this.TheHarvesterFsm.AddTransition("Idle", "Harvest", "IdleToHarvest");
            this.TheHarvesterFsm.AddTransition("Harvest", "Idle", "HarvestToIdle");
        }

        /// <summary>
        /// The start function.
        /// </summary>
        private void Start()
        {
            this.TheHarvesterFsm.Feed("auto");
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
