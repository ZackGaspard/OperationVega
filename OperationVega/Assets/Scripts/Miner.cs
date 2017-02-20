
namespace Assets.Scripts
{
    using Controllers;
    using Interfaces;
    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// The miner class.
    /// </summary>
    public class Miner : MonoBehaviour, IUnit, ICombat
    {
        /// <summary>
        /// Reference to the clean mineral prefab.
        /// </summary>
        public GameObject cleanmineral;

        /// <summary>
        /// Reference to the dirty mineral prefab.
        /// </summary>
        public GameObject dirtymineral;

        /// <summary>
        /// The miner finite state machine.
        /// Used to keep track of the miners states.
        /// </summary>
        public FiniteStateMachine<string> TheMinerFsm = new FiniteStateMachine<string>();

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
        /// The reference to the most recent mineral deposit.
        /// </summary>
        [HideInInspector]
        public GameObject theRecentMineralDeposit;

        /// <summary>
        /// The target resource to harvest from.
        /// </summary>
        public IResources TargetResource;

        /// <summary>
        /// The target click position to move to.
        /// </summary>
        [HideInInspector]
        public Vector3 TargetClickPosition;

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
        public int Resourcecount;

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
        private GameObject objecttopickup;

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
        /// The already stocked count reference.
        /// This holds the count of a resource already stocked to keep track.
        /// </summary>
        private int alreadystockedcount;

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
        /// Instance of the RangeHandler delegate.
        /// Called in changing to the pickup state.
        /// </summary>
        private RangeHandler pickupHandler;

        /// <summary>
        /// The range handler delegate.
        /// The delegate handles setting the stopping distance upon changing state.
        /// <para></para>
        /// <remarks><paramref name="number"></paramref> -The number to set the stopping distance to.</remarks>
        /// </summary>
        private delegate void RangeHandler(float number);

        /// <summary>
        /// The harvest function provides functionality of the miner to harvest a resource.
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

                if (this.Resourcecount == 5 && !this.TargetResource.Taint)
                {
                    // Create the clean mineral object and parent it to the front of the miner
                    var clone = Instantiate(this.cleanmineral, this.transform.position + (this.transform.forward * 0.6f), this.transform.rotation);
                    clone.transform.SetParent(this.transform);
                    clone.name = "Minerals";
                    this.Resourcecount = 0;
                    this.ChangeStates("Stock");
                    GameObject thesilo = GameObject.Find("Silo");
                    Vector3 destination = new Vector3(thesilo.transform.position.x + (this.transform.forward.x * 2), 0.5f, thesilo.transform.position.z + (this.transform.forward.z * 2));
                    this.navagent.SetDestination(destination);
                }
                else if (this.Resourcecount == 5 && this.TargetResource.Taint)
                {
                    // The resource is tainted go to decontamination center
                    // Create the dirty mineral object and parent it to the front of the miner
                    var clone = Instantiate(this.dirtymineral, this.transform.position + (this.transform.forward * 0.6f), this.transform.rotation);
                    clone.transform.SetParent(this.transform);
                    clone.name = "MineralsTainted";
                    this.ChangeStates("Decontaminate");
                    GameObject thedecontaminationbuilding = GameObject.Find("Decontamination");
                    Transform thedoor = thedecontaminationbuilding.transform.GetChild(1);
                    this.navagent.SetDestination(thedoor.position);
                }
            }
        }

        /// <summary>
        /// The decontaminate function provides functionality of the miner to decontaminate a resource.
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
                    this.Resourcecount = 0;
                    this.alreadystockedcount = 0;
                    int counter = 0;

                    for (int i = 0; i < this.transform.childCount; i++)
                    {
                        if (this.transform.GetChild(i).name == "MineralsTainted")
                        {
                            Destroy(this.transform.GetChild(i).gameObject);
                            counter++;
                        }
                    }

                    for (int i = 0; i < counter; i++)
                    {
                        var clone = Instantiate(
                        this.cleanmineral,
                        this.transform.position + (this.transform.forward * 0.6f),
                        this.transform.rotation);
                        clone.transform.SetParent(this.transform);
                        clone.name = "Minerals";
                        if (i > 0)
                        {
                            clone.transform.gameObject.SetActive(false);
                        }
                    }

                    this.ChangeStates("Stock");
                    GameObject thesilo = GameObject.Find("Silo");
                    Vector3 destination = new Vector3(
                        thesilo.transform.position.x + (this.transform.forward.x * 2),
                        0.5f,
                        thesilo.transform.position.z + (this.transform.forward.z * 2));
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
                    Debug.Log("Miner crit hit!");
                    this.Target.TakeDamage(10);
                    Enemy e = this.Target as Enemy;
                    Debug.Log(e.Health);
                    this.timebetweenattacks = 0;
                }
                else
                {
                    Debug.Log("Miner Attacking for normal damage");
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
        /// The taunt ability for the miner.
        /// </summary>
        public void Taunt()
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
        /// The change states function.
        /// This function changes the state to the passed in state.
        /// <para></para>
        /// <remarks><paramref name="destinationState"></paramref> -The state to transition to.</remarks>
        /// </summary>
        public void ChangeStates(string destinationState)
        {
            string thecurrentstate = this.TheMinerFsm.CurrentState.Statename;
            switch (destinationState)
            {
                case "Battle":
                    this.TheMinerFsm.Feed(thecurrentstate + "To" + destinationState, 5.0f);
                    break;
                case "Idle":
                    this.TheMinerFsm.Feed(thecurrentstate + "To" + destinationState, 1.0f);
                    break;
                case "Harvest":
                    this.TheMinerFsm.Feed(thecurrentstate + "To" + destinationState, 1.5f);
                    break;
                case "Stock":
                    this.TheMinerFsm.Feed(thecurrentstate + "To" + destinationState, 1.5f);
                    break;
                case "Decontaminate":
                    this.TheMinerFsm.Feed(thecurrentstate + "To" + destinationState, 1.0f);
                    break;
                case "PickUp":
                    this.TheMinerFsm.Feed(thecurrentstate + "To" + destinationState, 1.0f);
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
            if (theResource.GetComponent<Minerals>())
            {
                this.TargetResource = (IResources)theResource.GetComponent(typeof(IResources));
                this.navagent.SetDestination(theResource.transform.position);
                this.theRecentMineralDeposit = theResource;
                this.ChangeStates("Harvest");
            }
        }

        /// <summary>
        /// The go to pickup function.
        /// Parses and sends the unit to pickup a dropped resource.
        /// <para></para>
        /// <remarks><paramref name="thepickup"></paramref> -The object that will be set as the item to pick up.</remarks>
        /// </summary>
        public void GoToPickup(GameObject thepickup)
        {
            if (thepickup.name == "Minerals" || thepickup.name == "MineralsTainted")
            {
                this.objecttopickup = thepickup;
                this.navagent.SetDestination(thepickup.transform.position);
                this.ChangeStates("PickUp");
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

            switch (this.TheMinerFsm.CurrentState.Statename)
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
                case "Decontaminate":
                    this.DecontaminationState();
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
            this.decontime = 1.0f;

            this.timebetweenattacks = this.Attackspeed;
            this.navagent = this.GetComponent<NavMeshAgent>();
            this.navagent.speed = this.Speed;
            Debug.Log("Miner Initialized");
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
                if (t.name == "Minerals")
                {
                    this.Resourcecount += 5;
                }
            }

            this.Resourcecount -= this.alreadystockedcount;

            Debug.Log("Total to stock" + this.Resourcecount);
        }

        /// <summary>
        /// The idle state function.
        /// Has the funtionality of checking for dropped items.
        /// </summary>
        private void IdleState()
        {
            if (this.theitemdropped != null && this.objecttopickup == null)
            {

                if (this.transform.Find("Minerals") && this.theitemdropped.name == "MineralsTainted")
                {
                    return;
                }
                else if (this.transform.Find("MineralsTainted") && this.theitemdropped.name == "Minerals")
                {
                    return;
                }

                this.navagent.SetDestination(this.theitemdropped.transform.position);

                if (this.navagent.remainingDistance <= this.navagent.stoppingDistance && !this.navagent.pathPending)
                {
                    Debug.Log("Found my mineral");
                    this.theitemdropped.transform.SetParent(this.transform);
                    this.theitemdropped.transform.position = this.transform.position + (this.transform.forward * 0.6f);
                    this.theitemdropped = null;

                    if (this.transform.Find("MineralsTainted"))
                    {
                        this.ChangeStates("Decontaminate");
                        GameObject thedecontaminationbuilding = GameObject.Find("Decontamination");
                        Transform thedoor = thedecontaminationbuilding.transform.GetChild(1);
                        this.navagent.SetDestination(thedoor.position);
                    }
                    else
                    {
                        this.ChangeStates("Stock");
                        GameObject thesilo = GameObject.Find("Silo");
                        Vector3 destination = new Vector3(thesilo.transform.position.x + (this.transform.forward.x * 2), 0.5f, thesilo.transform.position.z + (this.transform.forward.z * 2));
                        this.navagent.SetDestination(destination);
                    }
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
                Transform cleanminerals = this.transform.Find("Minerals");
                Transform dirtyminerals = this.transform.Find("MineralsTainted");

                if (cleanminerals != null)
                {
                    cleanminerals.position = new Vector3(cleanminerals.position.x, 0f, cleanminerals.position.z);
                    this.theitemdropped = cleanminerals.gameObject;
                    this.theitemdropped.tag = "PickUp";
                    cleanminerals.gameObject.SetActive(true);
                    cleanminerals.transform.parent = null;
                    this.Resourcecount = 0;
                }
                else if (dirtyminerals != null)
                {
                    dirtyminerals.position = new Vector3(dirtyminerals.position.x, 0f, dirtyminerals.position.z);
                    this.theitemdropped = dirtyminerals.gameObject;
                    this.theitemdropped.tag = "PickUp";
                    dirtyminerals.gameObject.SetActive(true);
                    dirtyminerals.transform.parent = null;
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
            if (this.TargetResource != null && this.TargetResource.Count > 0)
            {
                if (!this.transform.Find("Minerals") && !this.transform.Find("MineralsTainted"))
                {
                    if (this.navagent.remainingDistance <= this.navagent.stoppingDistance && !this.navagent.pathPending)
                    {
                        this.Harvest();
                    }
                }
            }
        }

        /// <summary>
        /// The stock state function.
        /// Handles the exchange of resources to the user from the unit.
        /// </summary>
        private void StockState()
        {
            if (this.transform.Find("Minerals"))
            {
                if (this.Resourcecount <= 0)
                {
                    this.Resourcecount = 0;
                    this.alreadystockedcount = 0;

                    for (int i = 0; i < this.transform.childCount; i++)
                    {
                        if (this.transform.GetChild(i).name == "Minerals")
                        {
                            Destroy(this.transform.GetChild(i).gameObject);
                        }
                    }

                    if (this.TargetResource != null && this.TargetResource.Count > 0)
                    {
                        this.navagent.SetDestination(this.theRecentMineralDeposit.transform.position);
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
                        User.MineralsCount++;
                        Debug.Log("I have now stocked " + User.MineralsCount + " minerals");
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
                Transform mineraltainted = this.transform.Find("MineralsTainted");
                Transform mineral = this.transform.Find("Minerals");

                if (mineral == null && mineraltainted == null)
                {
                    this.objecttopickup.transform.position = this.transform.position + (this.transform.forward * 0.6f);
                    this.objecttopickup.transform.SetParent(this.transform);
                    if (this.objecttopickup.name == "MineralsTainted")
                    {
                        this.Resourcecount = 5;
                    }
                }
                else if (this.objecttopickup.name == "Minerals")
                {
                    if (mineral != null && mineraltainted == null)
                    {
                        this.objecttopickup.transform.position = this.transform.position + (this.transform.forward * 0.6f);
                        this.objecttopickup.transform.SetParent(this.transform);
                        this.objecttopickup.gameObject.SetActive(false);
                    }
                }
                else if (this.objecttopickup.name == "MineralsTainted")
                {
                    if (mineraltainted != null && mineral == null)
                    {
                        this.objecttopickup.transform.position = this.transform.position + (this.transform.forward * 0.6f);
                        this.objecttopickup.transform.SetParent(this.transform);
                        this.objecttopickup.gameObject.SetActive(false);
                        this.Resourcecount = 5;
                    }
                }

                this.ChangeStates("Idle");
            }
        }

        /// <summary>
        /// The decontamination state function.
        /// Handles the decontamination of resources at the decontamination building.
        /// </summary>
        private void DecontaminationState()
        {
            if (this.transform.Find("MineralsTainted"))
            {
                if (this.navagent.remainingDistance <= this.navagent.stoppingDistance && !this.navagent.pathPending)
                {
                    this.Decontaminate();
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
            this.stockHandler = this.TallyResources;
            this.decontaminationHandler = this.ResetStoppingDistance;
            this.pickupHandler = this.ResetStoppingDistance;

            this.TheMinerFsm.CreateState("Init", null);
            this.TheMinerFsm.CreateState("Idle", this.idleHandler);
            this.TheMinerFsm.CreateState("Battle", this.battleHandler);
            this.TheMinerFsm.CreateState("Harvest", this.harvestHandler);
            this.TheMinerFsm.CreateState("Stock", this.stockHandler);
            this.TheMinerFsm.CreateState("Decontaminate", this.decontaminationHandler);
            this.TheMinerFsm.CreateState("PickUp", this.pickupHandler);

            this.TheMinerFsm.AddTransition("Init", "Idle", "auto");
            this.TheMinerFsm.AddTransition("Idle", "Battle", "IdleToBattle");
            this.TheMinerFsm.AddTransition("Battle", "Idle", "BattleToIdle");
            this.TheMinerFsm.AddTransition("Idle", "Harvest", "IdleToHarvest");
            this.TheMinerFsm.AddTransition("Harvest", "Idle", "HarvestToIdle");
            this.TheMinerFsm.AddTransition("Battle", "Harvest", "BattleToHarvest");
            this.TheMinerFsm.AddTransition("Harvest", "Battle", "HarvestToBattle");
            this.TheMinerFsm.AddTransition("Harvest", "Stock", "HarvestToStock");
            this.TheMinerFsm.AddTransition("Battle", "Stock", "BattleToStock");
            this.TheMinerFsm.AddTransition("Idle", "Stock", "IdleToStock");
            this.TheMinerFsm.AddTransition("Stock", "Idle", "StockToIdle");
            this.TheMinerFsm.AddTransition("Stock", "Battle", "StockToBattle");
            this.TheMinerFsm.AddTransition("Stock", "Harvest", "StockToHarvest");
            this.TheMinerFsm.AddTransition("Harvest", "Decontaminate", "HarvestToDecontaminate");
            this.TheMinerFsm.AddTransition("Stock", "Decontaminate", "StockToDecontaminate");
            this.TheMinerFsm.AddTransition("Decontaminate", "Stock", "DecontaminateToStock");
            this.TheMinerFsm.AddTransition("Decontaminate", "Harvest", "DecontaminateToHarvest");
            this.TheMinerFsm.AddTransition("Decontaminate", "Idle", "DecontaminateToIdle");
            this.TheMinerFsm.AddTransition("Idle", "Decontaminate", "IdleToDecontaminate");
            this.TheMinerFsm.AddTransition("Decontaminate", "Battle", "DecontaminateToBattle");
            this.TheMinerFsm.AddTransition("Battle", "Decontaminate", "BattleToDecontaminate");
            this.TheMinerFsm.AddTransition("PickUp", "Idle", "PickUpToIdle");
            this.TheMinerFsm.AddTransition("PickUp", "Battle", "PickUpToBattle");
            this.TheMinerFsm.AddTransition("PickUp", "Harvest", "PickUpToHarvest");
            this.TheMinerFsm.AddTransition("PickUp", "Decontaminate", "PickUpToDecontaminate");
            this.TheMinerFsm.AddTransition("PickUp", "Stock", "PickUpToStock");
            this.TheMinerFsm.AddTransition("Idle", "PickUp", "IdleToPickUp");
            this.TheMinerFsm.AddTransition("Battle", "PickUp", "BattleToPickUp");
            this.TheMinerFsm.AddTransition("Harvest", "PickUp", "HarvestToPickUp");
            this.TheMinerFsm.AddTransition("Stock", "PickUp", "StockToPickUp");
            this.TheMinerFsm.AddTransition("Decontaminate", "PickUp", "DecontaminateToPickUp");
        }

        /// <summary>
        /// The start function.
        /// </summary>
        private void Start()
        {
            this.InitUnit();
            this.TheMinerFsm.Feed("auto", 0.1f);
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
