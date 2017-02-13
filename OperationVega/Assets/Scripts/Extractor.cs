
namespace Assets.Scripts
{
    using System.Collections;

    using Controllers;
    using Interfaces;
    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// The extractor class.
    /// </summary>
    public class Extractor : MonoBehaviour, IUnit, ICombat, IGather, IDamageable
    {
        /// <summary>
        /// Reference to the clean gas pefab
        /// </summary>
        public GameObject cleangas;

        /// <summary>
        /// The target to attack.
        /// </summary>
        [HideInInspector]
        public IDamageable Target;

        /// <summary>
        /// The enemy gameobject reference.
        /// </summary>
        public GameObject theEnemy;

        /// <summary>
        /// The recent geyser reference that we were farming from.
        /// </summary>
        public GameObject theRecentGeyser;

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
        /// The health of the extractor.
        /// </summary>
        [HideInInspector]
        public uint Health;

        /// <summary>
        /// The max health of the extractor.
        /// </summary>
        [HideInInspector]
        public uint Maxhealth;

        /// <summary>
        /// The strength of the extractor.
        /// </summary>
        [HideInInspector]
        public uint Strength;

        /// <summary>
        /// The defense of the extractor.
        /// </summary>
        [HideInInspector]
        public uint Defense;

        /// <summary>
        /// The speed of the extractor.
        /// </summary>
        [HideInInspector]
        public uint Speed;

        /// <summary>
        /// The attack speed of the extractor.
        /// </summary>
        [HideInInspector]
        public uint Attackspeed;

        /// <summary>
        /// The skill cool down of the extractor.
        /// </summary>
        [HideInInspector]
        public uint Skillcooldown;

        /// <summary>
        /// The attack range of the extractor.
        /// </summary>
        [HideInInspector]
        public float Attackrange;

        /// <summary>
        /// The resource count of the extractor.
        /// </summary>
        [HideInInspector]
        public uint Resourcecount;

        /// <summary>
        /// The extractor finite state machine.
        /// Used to keep track of the extractors states.
        /// </summary>
        public FiniteStateMachine<string> TheExtractorFsm = new FiniteStateMachine<string>();

        /// <summary>
        /// The time between attacks reference.
        /// Stores the reference to the timer between attacks
        /// </summary>
        private float timebetweenattacks;

        /// <summary>
        /// The harvest time reference.
        /// How long between each gathering of the resource.
        /// </summary>
        private float harvesttime;

        /// <summary>
        /// The drop off time reference.
        /// How long it takes to drop off the resource at the silo.
        /// </summary>
        private float dropofftime;

        /// <summary>
        /// The navigation agent reference.
        /// </summary>
        private NavMeshAgent navagent;

        /// <summary>
        /// Instance of the RangeHandler delegate.
        /// Called in changing to the idle state.
        /// </summary>
        private RangeHandler idleHandler;

        /// <summary>
        /// Instance of the RangeHandler delegate.
        /// Called in changing to the battle state.
        /// </summary>
        private RangeHandler battleHandler;

        /// <summary>
        /// Instance of the RangeHandler delegate.
        /// Called in changing to the harvest state.
        /// </summary>
        private RangeHandler harvestHandler;

        /// <summary>
        /// Instance of the RangeHandler delegate.
        /// Called in changing to the stock state.
        /// </summary>
        private RangeHandler stockHandler;

        /// <summary>
        /// The range handler delegate.
        /// The delegate handles setting the attack range upon changing state.
        /// <para></para>
        /// <remarks><paramref name="number"></paramref> -The number to set the attack range to.</remarks>
        /// </summary>
        private delegate void RangeHandler(float number);

        /// <summary>
        /// The harvest function provides functionality of the extractor to harvest a resource.
        /// </summary>
        public void Harvest()
        {
            if (this.harvesttime >= 1.0f)
            {
                Debug.Log("I am harvesting");
                this.TargetResource.Count--;
                Debug.Log("Resource left: " + this.TargetResource.Count);
                this.Resourcecount++;
                Debug.Log("My Resource count " + this.Resourcecount);

                this.harvesttime = 0;
                if (this.Resourcecount >= 5)
                { // Create the clean gas object and parent it to the front of the extractor
                    var clone = Instantiate(this.cleangas, this.transform.position + (this.transform.forward * 0.6f), this.transform.rotation);
                    clone.transform.SetParent(this.transform);
                    this.ChangeStates("Stock");
                    GameObject thesilo = GameObject.Find("Silo");
                    Vector3 destination = new Vector3(thesilo.transform.position.x + (this.transform.forward.x * 2), 0.5f, thesilo.transform.position.z + (this.transform.forward.z * 2));
                    this.navagent.SetDestination(destination);
                }
            }
        }

        /// <summary>
        /// The attack function gives the extractor functionality to attack.
        /// </summary>
        public void Attack()
        {
            if (this.timebetweenattacks >= this.Attackspeed)
            {
                Vector3 thedisplacement = (this.transform.position - this.theEnemy.transform.position).normalized;
                if (Vector3.Dot(thedisplacement, this.theEnemy.transform.forward) < 0)
                {
                    Debug.Log("Extractor crit hit!");
                    this.Target.TakeDamage(10);
                    Enemy e = this.Target as Enemy;
                    Debug.Log(e.Health);
                    this.timebetweenattacks = 0;
                }
                else
                {
                    Debug.Log("Extractor Attacked for normal damage");
                    this.Target.TakeDamage(5);
                    Enemy e = this.Target as Enemy;
                    Debug.Log(e.Health);
                    this.timebetweenattacks = 0;
                }
            }
        }

        public void Decontaminate()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The take damage function allows an extractor to take damage.
        /// </summary>
        /// <param name="damage">
        /// The amount of damage.
        /// </param>
        public void TakeDamage(uint damage)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The fear factor ability for the extractor.
        /// </summary>
        public void FearFactor()
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
            this.navagent.SetDestination(targetPos);
        }

        /// <summary>
        /// The change states function.
        /// This function changes the state to the passed in state.
        /// <para></para>
        /// <remarks><paramref name="destinationState"></paramref> -The state to transition to.</remarks>
        /// </summary>
        public void ChangeStates(string destinationState)
        {
            string thecurrentstate = this.TheExtractorFsm.CurrentState.Statename;

            switch (destinationState)
            {
                case "Battle":
                    this.TheExtractorFsm.Feed(thecurrentstate + "To" + destinationState, 5.0f);
                    break;
                case "Idle":
                    this.TheExtractorFsm.Feed(thecurrentstate + "To" + destinationState, 1.0f);
                    break;
                case "Harvest":
                    this.TheExtractorFsm.Feed(thecurrentstate + "To" + destinationState, 2.0f);
                    break;
                case "Stock":
                    this.TheExtractorFsm.Feed(thecurrentstate + "To" + destinationState, 1.5f);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// The initialize unit function.
        /// This will initialize the unit with the appropriate values for stats.
        /// </summary>
        private void InitUnit()
        {
            this.Attackrange = 5.0f;
            this.Attackspeed = 3;
            this.Speed = 2;
            this.harvesttime = 1.0f;

            this.timebetweenattacks = this.Attackspeed;
            this.navagent = this.GetComponent<NavMeshAgent>();
            this.navagent.speed = this.Speed;
            Debug.Log("Extractor Initialized");
        }

        /// <summary>
        /// The reset range function.
        /// This resets the range of distance the unit stands from the clicked position.
        /// </summary>
        /// <param name="num">
        /// The number to set the attack range to.
        /// </param>
        private void ResetStoppingDistance(float num)
        {
            this.navagent.stoppingDistance = num;
        }

        /// <summary>
        /// The battle state function.
        /// The function called while in the battle state.
        /// </summary>
        private void BattleState()
        {
            if (this.Target != null)
            {
                if (this.navagent.remainingDistance <= this.Attackrange && this.navagent.remainingDistance >= 1.5f)
                {
                    this.Attack();
                }
            }
        }

        /// <summary>
        /// The harvest state function.
        /// The function called while in the harvest state.
        /// </summary>
        private void HarvestState()
        {
            if (this.TargetResource != null && this.TargetResource.Count > 0)
            {
                if (this.navagent.remainingDistance <= this.navagent.stoppingDistance && this.navagent.remainingDistance >= 1.4f)
                {
                    this.Harvest();
                }
            }
        }

        /// <summary>
        /// The stock state function.
        /// Handles the exchange of resources to the user from the unit.
        /// </summary>
        private void StockState()
        {
            if (this.Resourcecount <= 0)
            {
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    Destroy(this.transform.GetChild(i).gameObject);
                }

                if (this.TargetResource != null && this.TargetResource.Count > 0)
                {
                    this.navagent.SetDestination(this.theRecentGeyser.transform.position);
                    this.ChangeStates("Harvest");
                }
                else
                {
                    this.ChangeStates("Idle");
                }
            }

            dropofftime += 1 * Time.deltaTime;

            if (this.navagent.remainingDistance <= this.navagent.stoppingDistance)
            {
                if (this.dropofftime >= 1.0f)
                {
                    Debug.Log("Dropping off the goods");
                    this.Resourcecount--;
                    Debug.Log("My resource count " + this.Resourcecount);
                    User.GasCount++;
                    Debug.Log("I have now stocked " + User.GasCount + " gas");
                    this.dropofftime = 0;
                }
            }
        }

        /// <summary>
        /// The awake function.
        /// </summary>
        private void Awake()
        {
            this.idleHandler = this.ResetStoppingDistance;
            this.battleHandler = this.ResetStoppingDistance;
            this.harvestHandler = this.ResetStoppingDistance;
            this.stockHandler = this.ResetStoppingDistance;

            this.TheExtractorFsm.CreateState("Init", null);
            this.TheExtractorFsm.CreateState("Idle", this.idleHandler);
            this.TheExtractorFsm.CreateState("Battle", this.battleHandler);
            this.TheExtractorFsm.CreateState("Harvest", this.harvestHandler);
            this.TheExtractorFsm.CreateState("Stock", this.stockHandler);

            this.TheExtractorFsm.AddTransition("Init", "Idle", "auto");
            this.TheExtractorFsm.AddTransition("Idle", "Battle", "IdleToBattle");
            this.TheExtractorFsm.AddTransition("Battle", "Idle", "BattleToIdle");
            this.TheExtractorFsm.AddTransition("Idle", "Harvest", "IdleToHarvest");
            this.TheExtractorFsm.AddTransition("Harvest", "Idle", "HarvestToIdle");
            this.TheExtractorFsm.AddTransition("Battle", "Harvest", "BattleToHarvest");
            this.TheExtractorFsm.AddTransition("Harvest", "Battle", "HarvestToBattle");
            this.TheExtractorFsm.AddTransition("Harvest", "Stock", "HarvestToStock");
            this.TheExtractorFsm.AddTransition("Battle", "Stock", "BattleToStock");
            this.TheExtractorFsm.AddTransition("Stock", "Battle", "StockToBattle");
            this.TheExtractorFsm.AddTransition("Stock", "Harvest", "StockToHarvest");
            this.TheExtractorFsm.AddTransition("Idle", "Stock", "IdleToStock");
            this.TheExtractorFsm.AddTransition("Stock", "Idle", "StockToIdle");
        }

        /// <summary>
        /// The start function.
        /// </summary>
        private void Start()
        {
            this.InitUnit();
            this.TheExtractorFsm.Feed("auto", 0.1f);
        }

        /// <summary>
        /// The update function.
        /// </summary>
        private void Update()
        {
            UnitController.Self.CheckIfSelected(this.gameObject);
            this.UpdateUnit();
        }

        /// <summary>
        /// The update unit function.
        /// This updates the units behavior.
        /// </summary>
        private void UpdateUnit()
        {
            this.timebetweenattacks += 1 * Time.deltaTime;
            this.harvesttime += 1 * Time.deltaTime;

            switch (this.TheExtractorFsm.CurrentState.Statename)
            {
                case "Idle":
                    break;
                case "Battle":
                    this.BattleState();
                    break;
                case "Harvest":
                    this.HarvestState();
                    break;
                case "Stock":
                    this.StockState();
                    break;
                default:
                    break;
            }
        }
    }
}
