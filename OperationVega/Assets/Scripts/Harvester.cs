
namespace Assets.Scripts
{
    using Controllers;
    using Interfaces;

    using UnityEditor;

    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// The harvester class.
    /// </summary>
    public class Harvester : MonoBehaviour, IUnit, IGather, IDamageable
    {
        /// <summary>
        /// Reference to the clean food pefab
        /// </summary>
        public GameObject cleanfood;

        /// <summary>
        /// Reference to the dirty food pefab
        /// </summary>
        public GameObject dirtyfood;

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
        /// The enemy gameobject reference.
        /// </summary>
        public GameObject theEnemy;

        /// <summary>
        /// The recent tree reference that we were farming from.
        /// </summary>
        public GameObject theRecentTree;

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
        public float Healrange;

        /// <summary>
        /// The resource count of the harvester.
        /// </summary>
        [HideInInspector]
        public uint Resourcecount;

        /// <summary>
        /// The navigation agent reference.
        /// </summary>
        private NavMeshAgent navagent;

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
        /// The decontaminate time reference.
        /// How long to take to decontaminate the resource.
        /// </summary>
        private float decontime;

        /// <summary>
        /// The drop off time reference.
        /// How long it takes to drop off the resource at the silo.
        /// </summary>
        private float dropofftime;

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
        /// Instance of the RangeHandler delegate.
        /// Called in changing to the decontamination state.
        /// </summary>
        private RangeHandler decontaminationHandler;

        /// <summary>
        /// The range handler delegate.
        /// The delegate handles setting the attack range upon changing state.
        /// <para></para>
        /// <remarks><paramref name="number"></paramref> -The number to set the attack range to.</remarks>
        /// </summary>
        private delegate void RangeHandler(float number);

        /// <summary>
        /// The harvest function provides functionality of the harvester to harvest a resource.
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
                if (this.Resourcecount >= 5 && !this.TargetResource.Taint)
                { // Create the clean food object and parent it to the front of the harvester
                    var clone = Instantiate(this.cleanfood, this.transform.position + (this.transform.forward * 0.6f), this.transform.rotation);
                    clone.transform.SetParent(this.transform);
                    this.ChangeStates("Stock");
                    GameObject thesilo = GameObject.Find("Silo");
                    Vector3 destination = new Vector3(thesilo.transform.position.x + (this.transform.forward.x * 2), 0.5f, thesilo.transform.position.z + (this.transform.forward.z * 2));
                    this.navagent.SetDestination(destination);
                }
                else if (this.Resourcecount >= 5 && this.TargetResource.Taint)
                {
                    // The resource is tainted go to decontamination center
                    // Create the dirty food object and parent it to the front of the harvester
                    var clone = Instantiate(this.dirtyfood, this.transform.position + (this.transform.forward * 0.6f), this.transform.rotation);
                    clone.transform.SetParent(this.transform);
                    clone.name = "PickupFoodTaint";
                    this.ChangeStates("Decontaminate");
                    GameObject thedecontaminationbuilding = GameObject.Find("Decontamination");
                    Transform thedoor = thedecontaminationbuilding.transform.GetChild(1);
                    Vector3 destination = new Vector3(thedoor.position.x, 0.5f, thedoor.position.z);
                    this.navagent.SetDestination(destination);
                }
            }
        }

        /// <summary>
        /// The decontaminate function provides functionality of the harvester to decontaminate a resource.
        /// </summary>
        public void Decontaminate()
        {
            if (this.decontime >= 1.0f)
            {
                Debug.Log("Decontaminating");
                this.Resourcecount--;
                this.decontime = 0;

                if (this.Resourcecount <= 0)
                {
                    for (int i = 0; i < this.transform.childCount; i++)
                    {
                        Destroy(this.transform.GetChild(i).gameObject);
                    }

                    this.Resourcecount = 5;
                    var clone = Instantiate(this.cleanfood, this.transform.position + (this.transform.forward * 0.6f), this.transform.rotation);
                    clone.transform.SetParent(this.transform);
                    this.ChangeStates("Stock");
                    GameObject thesilo = GameObject.Find("Silo");
                    Vector3 destination = new Vector3(thesilo.transform.position.x + (this.transform.forward.x * 2), 0.5f, thesilo.transform.position.z + (this.transform.forward.z * 2));
                    this.navagent.SetDestination(destination);
                }
            }
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
        /// The change states function.
        /// This function changes the state to the passed in state.
        /// <para></para>
        /// <remarks><paramref name="destinationState"></paramref> -The state to transition to.</remarks>
        /// </summary>
        public void ChangeStates(string destinationState)
        {
            string thecurrentstate = this.TheHarvesterFsm.CurrentState.Statename;
            switch (destinationState)
            {
                case "Battle":
                    this.TheHarvesterFsm.Feed(thecurrentstate + "To" + destinationState, 5.0f);
                    break;
                case "Idle":
                    this.TheHarvesterFsm.Feed(thecurrentstate + "To" + destinationState, 1.0f);
                    break;
                case "Harvest":
                    this.TheHarvesterFsm.Feed(thecurrentstate + "To" + destinationState, 2.0f);
                    break;
                case "Stock":
                    this.TheHarvesterFsm.Feed(thecurrentstate + "To" + destinationState, 1.5f);
                    break;
                case "Decontaminate":
                    this.TheHarvesterFsm.Feed(thecurrentstate + "To" + destinationState, 1.0f);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// The heal stun function gives the harvester functionality to heal units and stun enemies.
        /// </summary>
        public void HealStun()
        {
            if (this.timebetweenattacks >= this.Attackspeed)
            {
                Vector3 thedisplacement = (this.transform.position - this.theEnemy.transform.position).normalized;
                if (Vector3.Dot(thedisplacement, this.theEnemy.transform.forward) < 0)
                {
                    Debug.Log("Harvester hit crit!");
                    this.Target.TakeDamage(10);
                    Enemy e = this.Target as Enemy;
                    Debug.Log(e.Health);
                    this.timebetweenattacks = 0;
                }
                else
                {
                    Debug.Log("Harvester attacking normal damage");
                    this.Target.TakeDamage(5);
                    Enemy e = this.Target as Enemy;
                    Debug.Log(e.Health);
                    this.timebetweenattacks = 0;
                }
            }

        }

        /// <summary>
        /// The update unit function.
        /// This updates the units behavior.
        /// </summary>
        private void UpdateUnit()
        {
            this.timebetweenattacks += 1 * Time.deltaTime;
            this.harvesttime += 1 * Time.deltaTime;
            this.decontime += 1 * Time.deltaTime;

            switch (this.TheHarvesterFsm.CurrentState.Statename)
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
                case "Decontaminate":
                    this.DecontaminationState();
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
            this.Healrange = 5.0f;
            this.Attackspeed = 3;
            this.Speed = 2;
            this.harvesttime = 1.0f;
            this.decontime = 1.0f;

            this.timebetweenattacks = this.Attackspeed;
            this.navagent = this.GetComponent<NavMeshAgent>();
            this.navagent.speed = this.Speed;
            Debug.Log("Harvester Initialized");
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
                if (this.navagent.remainingDistance <= this.Healrange && !this.navagent.pathPending)
                {
                    this.HealStun();
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
                if (this.navagent.remainingDistance <= this.navagent.stoppingDistance && !this.navagent.pathPending)
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
                    this.navagent.SetDestination(this.theRecentTree.transform.position);
                    this.ChangeStates("Harvest");
                }
                else
                {
                    this.ChangeStates("Idle");
                }
            }

            dropofftime += 1 * Time.deltaTime;

            if (this.navagent.remainingDistance <= this.navagent.stoppingDistance && !this.navagent.pathPending)
            {
                if (this.dropofftime >= 1.0f)
                {
                    Debug.Log("Dropping off the goods");
                    this.Resourcecount--;
                    Debug.Log("My resource count " + this.Resourcecount);
                    User.FoodCount++;
                    Debug.Log("I have now stocked " + User.FoodCount + " food");
                    this.dropofftime = 0;
                }
            }
        }

        /// <summary>
        /// The decontamination state function.
        /// Handles the decontamination of resources at the decontamination building.
        /// </summary>
        private void DecontaminationState()
        {
            if (this.navagent.remainingDistance <= this.navagent.stoppingDistance && !this.navagent.pathPending)
            {
                this.Decontaminate();
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
            this.decontaminationHandler = this.ResetStoppingDistance;

            this.TheHarvesterFsm.CreateState("Init", null);
            this.TheHarvesterFsm.CreateState("Idle", this.idleHandler);
            this.TheHarvesterFsm.CreateState("Battle", this.battleHandler);
            this.TheHarvesterFsm.CreateState("Harvest", this.harvestHandler);
            this.TheHarvesterFsm.CreateState("Stock", this.stockHandler);
            this.TheHarvesterFsm.CreateState("Decontaminate", this.decontaminationHandler);

            this.TheHarvesterFsm.AddTransition("Init", "Idle", "auto");
            this.TheHarvesterFsm.AddTransition("Idle", "Battle", "IdleToBattle");
            this.TheHarvesterFsm.AddTransition("Battle", "Idle", "BattleToIdle");
            this.TheHarvesterFsm.AddTransition("Idle", "Harvest", "IdleToHarvest");
            this.TheHarvesterFsm.AddTransition("Harvest", "Idle", "HarvestToIdle");
            this.TheHarvesterFsm.AddTransition("Battle", "Harvest", "BattleToHarvest");
            this.TheHarvesterFsm.AddTransition("Harvest", "Battle", "HarvestToBattle");
            this.TheHarvesterFsm.AddTransition("Harvest", "Stock", "HarvestToStock");
            this.TheHarvesterFsm.AddTransition("Battle", "Stock", "BattleToStock");
            this.TheHarvesterFsm.AddTransition("Idle", "Stock", "IdleToStock");
            this.TheHarvesterFsm.AddTransition("Stock", "Idle", "StockToIdle");
            this.TheHarvesterFsm.AddTransition("Stock", "Battle", "StockToBattle");
            this.TheHarvesterFsm.AddTransition("Stock", "Harvest", "StockToHarvest");
            this.TheHarvesterFsm.AddTransition("Harvest", "Decontaminate", "HarvestToDecontaminate");
            this.TheHarvesterFsm.AddTransition("Decontaminate", "Stock", "DecontaminateToStock");
            this.TheHarvesterFsm.AddTransition("Decontaminate", "Idle", "DecontaminateToIdle");
            this.TheHarvesterFsm.AddTransition("Idle", "Decontaminate", "IdleToDecontaminate");
        }

        /// <summary>
        /// The start function.
        /// </summary>
        private void Start()
        {
            this.InitUnit();
            this.TheHarvesterFsm.Feed("auto", 0.1f);
        }

        /// <summary>
        /// The update function.
        /// </summary>
        private void Update()
        {
            UnitController.Self.CheckIfSelected(this.gameObject);
            this.UpdateUnit();
        }
    }
}
