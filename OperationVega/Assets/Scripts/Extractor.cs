
namespace Assets.Scripts
{
    using Controllers;
    using Interfaces;
    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// The extractor class.
    /// </summary>
    public class Extractor : MonoBehaviour, IUnit, ICombat
    {
        /// <summary>
        /// Reference to the clean gas prefab.
        /// </summary>
        public GameObject cleangas;

        /// <summary>
        /// The target to attack.
        /// </summary>
        public ICombat Target;

        /// <summary>
        /// The enemy gameobject reference.
        /// </summary>
        [HideInInspector]
        public GameObject theEnemy;

        /// <summary>
        /// The recent geyser reference that we were farming from.
        /// </summary>
        [HideInInspector]
        public GameObject theRecentGeyser;

        /// <summary>
        /// The resource to harvest from.
        /// </summary>
        public IResources TargetResource;

        /// <summary>
        /// The target click position to move to.
        /// </summary>
        [HideInInspector]
        public Vector3 TargetClickPosition;

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
        /// Stores the reference to the timer between attacks.
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
        /// The reference to the physical item dropped.
        /// </summary>
        private GameObject theitemdropped;

        /// <summary>
        /// The object to pickup.
        /// </summary>
        [SerializeField]
        private GameObject objecttopickup;

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
        /// Called in changing to the pickup state.
        /// </summary>
        private RangeHandler pickupHandler;

        /// <summary>
        /// The already stocked count reference.
        /// This holds the count of a resource already stocked to keep track.
        /// </summary>
        private uint alreadystockedcount;

        /// <summary>
        /// The range handler delegate.
        /// The delegate handles setting the stopping distance upon changing state.
        /// <para></para>
        /// <remarks><paramref name="number"></paramref> -The number to set the stopping distance to.</remarks>
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
                    clone.name = "GasTank";
                    this.Resourcecount = 0;
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

        /// <summary>
        /// The take damage function allows a miner to take damage.
        /// <para></para>
        /// <remarks><paramref name="damage"></paramref> -The amount to be calculated when the object takes damage.</remarks>
        /// </summary>
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
        /// The set move position function.
        /// Sets the destination for the unit.
        /// <para></para>
        /// <remarks><paramref name="theClickPosition"></paramref> -The object that will be set as the position to move to.</remarks>
        /// </summary>
        public void SetTheMovePosition(Vector3 targetPos)
        {
            this.navagent.SetDestination(targetPos);
        }

        /// <summary>
        /// The go to pickup function.
        /// Parses and sends the unit to pickup a dropped resource.
        /// <para></para>
        /// <remarks><paramref name="thepickup"></paramref> -The object that will be set as the item to pick up.</remarks>
        /// </summary>
        public void GoToPickup(GameObject thepickup)
        {
            if (thepickup.name == "GasTank")
            {
                this.objecttopickup = thepickup;
                this.navagent.SetDestination(thepickup.transform.position);
                this.ChangeStates("PickUp");
            }
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
                case "PickUp":
                    this.TheExtractorFsm.Feed(thecurrentstate + "To" + destinationState, 1.0f);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// The set target function.
        /// Sets the object as the target for the unit.
        /// <para></para>
        /// <remarks><paramref name="theTarget"></paramref> -The object that will be set as the target for attacking.</remarks>
        /// </summary>
        public void SetTarget(GameObject theTarget)
        {
            this.theEnemy = theTarget;
            if (this.theEnemy != null)
            {
                this.Target = (ICombat)theTarget.GetComponent(typeof(ICombat));
            }
        }

        /// <summary>
        /// The set target resource function.
        /// The function sets the unit with the resource.
        /// <para></para>
        /// <remarks><paramref name="theResource"></paramref> -The object that will be set as the target resource.</remarks>
        /// </summary>
        public void SetTargetResource(GameObject theResource)
        {
            if (theResource.GetComponent<Gas>())
            {
                this.TargetResource = (IResources)theResource.GetComponent(typeof(IResources));
                this.navagent.SetDestination(theResource.transform.position);
                this.theRecentGeyser = theResource;
                this.ChangeStates("Harvest");
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

            switch (this.TheExtractorFsm.CurrentState.Statename)
            {
                case "Idle":
                    this.IdleState();
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
                case "PickUp":
                    this.PickUpState();
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
        /// <para></para>
        /// <remarks><paramref name="num"></paramref> -The amount to set the stopping distance to.</remarks>
        /// </summary>
        private void ResetStoppingDistance(float num)
        {
            this.navagent.stoppingDistance = num;
        }

        /// <summary>
        /// The tally resources function.
        /// This function tallies up the resources in hand.
        /// <para></para>
        /// <remarks><paramref name="num"></paramref> -The number to set the stopping distance to.</remarks>
        /// </summary>
        private void TallyResources(float num)
        {
            this.navagent.stoppingDistance = num;

            this.Resourcecount = 0;

            foreach (Transform t in this.transform)
            {
                if (t.name == "GasTank")
                {
                    this.Resourcecount += 5;
                }
            }

            this.Resourcecount -= this.alreadystockedcount;

            Debug.Log("Total to stock" + this.Resourcecount);
        }

        /// <summary>
        /// The idle state function.
        /// Has the functionality of checking for dropped items.
        /// </summary>
        private void IdleState()
        {
            if (this.theitemdropped != null && this.objecttopickup == null)
            {
                this.navagent.SetDestination(this.theitemdropped.transform.position);

                if (this.navagent.remainingDistance <= this.navagent.stoppingDistance && !this.navagent.pathPending)
                {
                    Debug.Log("Found my gas");
                    this.theitemdropped.transform.SetParent(this.transform);
                    this.theitemdropped.transform.position = this.transform.position + (this.transform.forward * 0.6f);
                    this.theitemdropped = null;
                    this.ChangeStates("Stock");
                    GameObject thesilo = GameObject.Find("Silo");
                    Vector3 destination = new Vector3(thesilo.transform.position.x + (this.transform.forward.x * 2), 0.5f, thesilo.transform.position.z + (this.transform.forward.z * 2));
                    this.navagent.SetDestination(destination);
                }
            }
        }

        /// <summary>
        /// The battle state function.
        /// The function called while in the battle state.
        /// </summary>
        private void BattleState()
        {
            if (this.Target != null)
            {
                Transform gastank = this.transform.Find("GasTank");

                if (gastank != null)
                {
                    gastank.position = new Vector3(gastank.position.x, 0.25f, gastank.position.z);
                    this.theitemdropped = gastank.gameObject;
                    this.theitemdropped.tag = "PickUp";
                    gastank.gameObject.SetActive(true);
                    gastank.transform.parent = null;
                    this.Resourcecount = 0;
                }

                if (this.navagent.remainingDistance <= this.Attackrange && !this.navagent.pathPending)
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
            if (this.TargetResource != null && this.TargetResource.Count > 0 && !this.transform.Find("GasTank"))
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
            if (this.transform.Find("GasTank"))
            {
                if (this.Resourcecount <= 0)
                {
                    this.alreadystockedcount = 0;

                    for (int i = 0; i < this.transform.childCount; i++)
                    {
                        if (this.transform.GetChild(i).name == "GasTank")
                        {
                            Destroy(this.transform.GetChild(i).gameObject);
                        }
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

                if (this.navagent.remainingDistance <= this.navagent.stoppingDistance && !this.navagent.pathPending)
                {
                    if (this.dropofftime >= 1.0f)
                    {
                        Debug.Log("Dropping off the goods");
                        this.Resourcecount--;
                        this.alreadystockedcount++;
                        Debug.Log("My resource count " + this.Resourcecount);
                        User.GasCount++;
                        Debug.Log("I have now stocked " + User.GasCount + " gas");
                        this.dropofftime = 0;
                    }
                }
            }
        }

        /// <summary>
        /// The pick up state function.
        /// Regulates game flow while in the pick up state.
        /// </summary>
        private void PickUpState()
        {
            if (this.navagent.remainingDistance <= this.navagent.stoppingDistance && !this.navagent.pathPending)
            {
                Transform gastank = this.transform.Find("GasTank");

                this.objecttopickup.transform.position = this.transform.position + (this.transform.forward * 0.6f);
                this.objecttopickup.transform.SetParent(this.transform);

                if (gastank != null)
                {
                    this.objecttopickup.gameObject.SetActive(false);
                }

                this.ChangeStates("Idle");
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
            this.stockHandler = this.TallyResources;
            this.pickupHandler = this.ResetStoppingDistance;

            this.TheExtractorFsm.CreateState("Init", null);
            this.TheExtractorFsm.CreateState("Idle", this.idleHandler);
            this.TheExtractorFsm.CreateState("Battle", this.battleHandler);
            this.TheExtractorFsm.CreateState("Harvest", this.harvestHandler);
            this.TheExtractorFsm.CreateState("Stock", this.stockHandler);
            this.TheExtractorFsm.CreateState("PickUp", this.pickupHandler);

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
            this.TheExtractorFsm.AddTransition("PickUp", "Idle", "PickUpToIdle");
            this.TheExtractorFsm.AddTransition("PickUp", "Battle", "PickUpToBattle");
            this.TheExtractorFsm.AddTransition("PickUp", "Harvest", "PickUpToHarvest");
            this.TheExtractorFsm.AddTransition("PickUp", "Stock", "PickUpToStock");
            this.TheExtractorFsm.AddTransition("Idle", "PickUp", "IdleToPickUp");
            this.TheExtractorFsm.AddTransition("Battle", "PickUp", "BattleToPickUp");
            this.TheExtractorFsm.AddTransition("Harvest", "PickUp", "HarvestToPickUp");
            this.TheExtractorFsm.AddTransition("Stock", "PickUp", "StockToPickUp");


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
    }
}
