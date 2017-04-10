
namespace Assets.Scripts
{
    using Controllers;
    using Interfaces;
    using UI;
    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// The extractor class.
    /// </summary>
    [RequireComponent(typeof(Stats))]
    public class Extractor : MonoBehaviour, IUnit, ICombat
    {
        /// <summary>
        /// The extractor finite state machine.
        /// Used to keep track of the extractors states.
        /// </summary>
        private readonly FiniteStateMachine<string> theExtractorFsm = new FiniteStateMachine<string>();

        /// <summary>
        /// The danger color reference.
        /// Reference to the color change when health is critically low.
        /// </summary>
        private Color dangercolor;

        /// <summary>
        /// The orb reference.
        /// </summary>
        private GameObject theorb;

        /// <summary>
        /// Reference to the clean gas prefab.
        /// </summary>
        [SerializeField]
        private GameObject cleangas;

        /// <summary>
        /// The object to look at reference.
        /// </summary>
        private GameObject theobjecttolookat;

        /// <summary>
        /// The target to attack.
        /// </summary>
        private ICombat target;

        /// <summary>
        /// The enemy game object reference.
        /// </summary>
        private GameObject theEnemy;

        /// <summary>
        /// The recent geyser reference that we were farming from.
        /// </summary>
        private GameObject theRecentGeyser;

        /// <summary>
        /// The resource to harvest from.
        /// </summary>
        private IResources targetResource;

        /// <summary>
        /// The my stats reference.
        /// This reference will contain all this units stats data.
        /// </summary>
        private Stats mystats;

        /// <summary>
        /// The got hit first reference.
        /// Determines how the unit should act upon taking damage.
        /// </summary>
        private bool gothitfirst;

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
        /// The animator controller reference.
        /// This will help transition a unit to another state of the machine.
        /// </summary>
        private Animator animatorcontroller;

        /// <summary>
        /// The reference to the physical item dropped.
        /// </summary>
        private GameObject theitemdropped;

        /// <summary>
        /// The object to pickup.
        /// </summary>
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
        private int alreadystockedcount;

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
                this.targetResource.Count--;
                Debug.Log("Resource left: " + this.targetResource.Count);
                this.mystats.Resourcecount++;
                Debug.Log("My Resource count " + this.mystats.Resourcecount);

                this.harvesttime = 0;
                if (this.mystats.Resourcecount >= 5)
                { // Create the clean gas object and parent it to the front of the extractor
                    var clone = Instantiate(this.cleangas, this.transform.position + (this.transform.forward * 0.6f), this.transform.rotation);
                    clone.transform.SetParent(this.transform);
                    clone.name = "GasTank";
                    this.mystats.Resourcecount = 0;
                    this.ChangeStates("Stock");
                    GameObject thesilo = GameObject.Find("Silo");
                    Vector3 destination = new Vector3(thesilo.transform.position.x + (this.transform.forward.x * 2), 0.5f, thesilo.transform.position.z + (this.transform.forward.z * 2));
                    this.navagent.SetDestination(destination);
                    this.animatorcontroller.SetBool("IsWalking", true);
                }
            }
        }

        /// <summary>
        /// The special ability for the extractor.
        /// </summary>
        public void SpecialAbility()
        {
            // If able to use ability
            if (this.mystats.CurrentSkillCooldown >= this.mystats.MaxSkillCooldown)
            {
                Collider[] validtargets = Physics.OverlapSphere(this.transform.position, 5);

                // If nothing hit by cast then return
                if (validtargets.Length < 1) return;

                foreach (Collider c in validtargets)
                {
                    // If its an enemy - Stop them
                    if (c.gameObject.GetComponent<Enemy>())
                    {
                        c.gameObject.GetComponent<Enemy>().Currenttarget = null;
                        c.gameObject.transform.rotation = Quaternion.Euler(0f, c.transform.forward.y, 0f);
                        c.gameObject.GetComponent<Enemy>().ChangeStates("Idle");
                        c.gameObject.GetComponent<EnemyAI>().scared = true;
                    }
                }

                UIManager.Self.currentcooldown = 0;
                this.mystats.CurrentSkillCooldown = 0;
                Debug.Log("Extractor Special Ability Activated");
            }
        }

        /// <summary>
        /// The attack function gives the extractor functionality to attack.
        /// </summary>
        public void Attack()
        {
            if (this.theEnemy == null)
            {
                this.gothitfirst = true;
                this.target = null;
                this.ChangeStates("Idle");
            }

            if (this.timebetweenattacks >= this.mystats.Attackspeed)
            {
                Vector3 thedisplacement = (this.transform.position - this.theEnemy.transform.position).normalized;
                if (Vector3.Dot(thedisplacement, this.theEnemy.transform.forward) < 0)
                {
                    Debug.Log("Extractor crit hit!");
                    this.target.TakeDamage(10);
                    this.timebetweenattacks = 0;
                }
                else
                {
                    Debug.Log("Extractor Attacked for normal damage");
                    this.target.TakeDamage(5);
                    this.timebetweenattacks = 0;
                }
            }
        }

        /// <summary>
        /// The take damage function allows a miner to take damage.
        /// <para></para>
        /// <remarks><paramref name="damage"></paramref> -The amount to be calculated when the object takes damage.</remarks>
        /// </summary>
        public void TakeDamage(int damage)
        {
            this.mystats.Health -= damage;

            this.UpdateOrb();

            // Check if unit dies
            if (this.mystats.Health <= 0)
            {
                Destroy(this.gameObject);
            }

            // If unit is not dead
            if (this.theEnemy != null && this.gothitfirst)
            {
                this.gothitfirst = false;
                this.ChangeStates("Battle");
            }
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
            this.animatorcontroller.SetBool("IsWalking", true);
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
                this.theobjecttolookat = this.objecttopickup;
                this.navagent.SetDestination(thepickup.transform.position);
                this.animatorcontroller.SetBool("IsWalking", true);
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
            string thecurrentstate = this.theExtractorFsm.CurrentState.Statename;

            switch (destinationState)
            {
                case "Battle":
                    this.DropItems();
                    this.navagent.updateRotation = false;
                    this.theExtractorFsm.Feed(thecurrentstate + "To" + destinationState, this.mystats.Attackrange);
                    break;
                case "Idle":
                    this.navagent.updateRotation = true;
                    this.theExtractorFsm.Feed(thecurrentstate + "To" + destinationState, 1.0f);
                    break;
                case "Harvest":
                    this.navagent.updateRotation = false;
                    this.theExtractorFsm.Feed(thecurrentstate + "To" + destinationState, 1.5f);
                    break;
                case "Stock":
                    this.theobjecttolookat = GameObject.Find("Silo");
                    this.navagent.updateRotation = false;
                    this.theExtractorFsm.Feed(thecurrentstate + "To" + destinationState, 1.5f);
                    break;
                case "PickUp":
                    this.navagent.updateRotation = false;
                    this.theExtractorFsm.Feed(thecurrentstate + "To" + destinationState, 1.0f);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// The set target function.
        /// Auto sets the object as the target for the unit.
        /// <para></para>
        /// <remarks><paramref name="theTarget"></paramref> -The object that will be set as the target for attacking.</remarks>
        /// </summary>
        public void AutoTarget(GameObject theTarget)
        {
            if (this.theEnemy == null && theTarget != null)
            {
                this.theEnemy = theTarget;

                if (this.gothitfirst)
                {
                    this.theobjecttolookat = this.theEnemy;
                }

                this.target = (ICombat)theTarget.GetComponent(typeof(ICombat));
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
                this.theobjecttolookat = this.theEnemy;
                this.target = (ICombat)theTarget.GetComponent(typeof(ICombat));
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
                this.theobjecttolookat = theResource;
                this.targetResource = (IResources)theResource.GetComponent(typeof(IResources));
                this.navagent.SetDestination(theResource.transform.position);
                this.animatorcontroller.SetBool("IsWalking", true);
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
            this.mystats.CurrentSkillCooldown += 1.0f * Time.deltaTime;
            this.timebetweenattacks += 1 * Time.deltaTime;
            this.harvesttime += 1 * Time.deltaTime;

            this.UpdateRotation();

            // If the navagent isnt looking for a current path - this helps prevent any lag when the unit is already stopped then starting to move,
            // if the navagent is within stopping distance and its currently using the walk animation...
            if (!this.navagent.pathPending && this.navagent.remainingDistance <= this.navagent.stoppingDistance && this.animatorcontroller.GetBool("IsWalking"))
            {
                this.animatorcontroller.SetBool("IsWalking", false);
            }

            switch (this.theExtractorFsm.CurrentState.Statename)
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
            this.theorb = this.transform.GetChild(2).GetChild(2).GetChild(0).gameObject;
            this.dangercolor = Color.black;

            this.mystats = this.GetComponent<Stats>();
            this.mystats.Health = 100;
            this.mystats.Maxhealth = 100;
            this.mystats.Strength = 4;
            this.mystats.Defense = 7;
            this.mystats.Speed = 3;
            this.mystats.Attackspeed = 3;
            this.mystats.MaxSkillCooldown = 15;
            this.mystats.CurrentSkillCooldown = this.mystats.MaxSkillCooldown;
            this.mystats.Attackrange = 5.0f;
            this.mystats.Resourcecount = 0;

            this.gothitfirst = true;
            this.harvesttime = 1.0f;

            this.theorb.GetComponent<SkinnedMeshRenderer>().material.color = Color.green;

            this.timebetweenattacks = this.mystats.Attackspeed;
            this.navagent = this.GetComponent<NavMeshAgent>();
            this.navagent.speed = this.mystats.Speed;
            this.animatorcontroller = this.GetComponent<Animator>();
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

            this.mystats.Resourcecount = 0;

            foreach (Transform t in this.transform)
            {
                if (t.name == "GasTank")
                {
                    this.mystats.Resourcecount += 5;
                }
            }

            this.mystats.Resourcecount -= this.alreadystockedcount;

            Debug.Log("Total to stock" + this.mystats.Resourcecount);
        }

        /// <summary>
        /// The update rotation.
        /// </summary>
        private void UpdateRotation()
        {
            if (!this.navagent.updateRotation && this.theobjecttolookat != null)
            {
                Vector3 dir = this.theobjecttolookat.transform.position - this.transform.position;
                Quaternion lookrotation = Quaternion.LookRotation(dir);
                Vector3 rotation = Quaternion.Lerp(this.transform.rotation, lookrotation, Time.deltaTime * 5).eulerAngles;
                this.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            }
        }

        /// <summary>
        /// The drop items function.
        /// </summary>
        private void DropItems()
        {
            Transform[] children = this.transform.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < children.Length; i++)
            {
                float angle = i * (2 * 3.14159f / children.Length);
                float x = Mathf.Cos(angle) * 1.5f;
                float z = Mathf.Sin(angle) * 1.5f;

                children[i].gameObject.SetActive(true);

                if (children[i].name == "GasTank")
                {
                    children[i].position = new Vector3(this.transform.position.x + x, 0.25f, this.transform.position.z + z);
                    children[i].tag = "PickUp";
                    children[i].parent = null;
                    this.mystats.Resourcecount = 0;
                }
            }
        }

        /// <summary>
        /// The idle state function.
        /// Has the functionality of checking for dropped items.
        /// </summary>
        private void IdleState()
        {
            if (this.theEnemy != null)
            {
                if (Vector3.Distance(this.theEnemy.transform.position, this.transform.position) > this.theEnemy.GetComponent<EnemyAI>().Radius)
                {
                    this.gothitfirst = true;
                    this.theEnemy = null;
                }
            }
        }

        /// <summary>
        /// The battle state function.
        /// The function called while in the battle state.
        /// </summary>
        private void BattleState()
        {
            if (this.target != null)
            {
                if (this.navagent.remainingDistance <= this.mystats.Attackrange && !this.navagent.pathPending)
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
            if (this.targetResource != null && this.targetResource.Count > 0 && !this.transform.Find("GasTank"))
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
                if (this.mystats.Resourcecount <= 0)
                {
                    this.alreadystockedcount = 0;

                    for (int i = 0; i < this.transform.childCount; i++)
                    {
                        if (this.transform.GetChild(i).name == "GasTank")
                        {
                            Destroy(this.transform.GetChild(i).gameObject);
                        }
                    }

                    if (this.targetResource != null && this.targetResource.Count > 0)
                    {
                        this.theobjecttolookat = this.theRecentGeyser;
                        this.navagent.SetDestination(this.theRecentGeyser.transform.position);
                        this.animatorcontroller.SetBool("IsWalking", true);
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
                        this.mystats.Resourcecount--;
                        this.alreadystockedcount++;
                        Debug.Log("My resource count " + this.mystats.Resourcecount);
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

                if (gastank != null && gastank.gameObject.activeInHierarchy)
                {
                    this.objecttopickup.gameObject.SetActive(false);
                }

                this.ChangeStates("Idle");
            }
        }

        /// <summary>
        /// The update orb function.
        /// This function updates the color of the orb upon taking damage.
        /// </summary>
        private void UpdateOrb()
        {
            int halfhealth = this.mystats.Maxhealth / 2;
            int quarterhealth = this.mystats.Maxhealth / 4;

            if (this.mystats.Health > quarterhealth && this.mystats.Health <= halfhealth)
            {
                this.theorb.GetComponent<SkinnedMeshRenderer>().material.color = Color.yellow;
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

            this.theExtractorFsm.CreateState("Init", null);
            this.theExtractorFsm.CreateState("Idle", this.idleHandler);
            this.theExtractorFsm.CreateState("Battle", this.battleHandler);
            this.theExtractorFsm.CreateState("Harvest", this.harvestHandler);
            this.theExtractorFsm.CreateState("Stock", this.stockHandler);
            this.theExtractorFsm.CreateState("PickUp", this.pickupHandler);

            this.theExtractorFsm.AddTransition("Init", "Idle", "auto");
            this.theExtractorFsm.AddTransition("Idle", "Battle", "IdleToBattle");
            this.theExtractorFsm.AddTransition("Battle", "Idle", "BattleToIdle");
            this.theExtractorFsm.AddTransition("Idle", "Harvest", "IdleToHarvest");
            this.theExtractorFsm.AddTransition("Harvest", "Idle", "HarvestToIdle");
            this.theExtractorFsm.AddTransition("Battle", "Harvest", "BattleToHarvest");
            this.theExtractorFsm.AddTransition("Harvest", "Battle", "HarvestToBattle");
            this.theExtractorFsm.AddTransition("Harvest", "Stock", "HarvestToStock");
            this.theExtractorFsm.AddTransition("Battle", "Stock", "BattleToStock");
            this.theExtractorFsm.AddTransition("Stock", "Battle", "StockToBattle");
            this.theExtractorFsm.AddTransition("Stock", "Harvest", "StockToHarvest");
            this.theExtractorFsm.AddTransition("Idle", "Stock", "IdleToStock");
            this.theExtractorFsm.AddTransition("Stock", "Idle", "StockToIdle");
            this.theExtractorFsm.AddTransition("PickUp", "Idle", "PickUpToIdle");
            this.theExtractorFsm.AddTransition("PickUp", "Battle", "PickUpToBattle");
            this.theExtractorFsm.AddTransition("PickUp", "Harvest", "PickUpToHarvest");
            this.theExtractorFsm.AddTransition("PickUp", "Stock", "PickUpToStock");
            this.theExtractorFsm.AddTransition("Idle", "PickUp", "IdleToPickUp");
            this.theExtractorFsm.AddTransition("Battle", "PickUp", "BattleToPickUp");
            this.theExtractorFsm.AddTransition("Harvest", "PickUp", "HarvestToPickUp");
            this.theExtractorFsm.AddTransition("Stock", "PickUp", "StockToPickUp");
        }

        /// <summary>
        /// The start function.
        /// </summary>
        private void Start()
        {
            this.InitUnit();
            this.theExtractorFsm.Feed("auto", 0.1f);
            User.ExtractorCount++;
        }

        /// <summary>
        /// The update function.
        /// </summary>
        private void Update()
        {
            UnitController.Self.CheckIfSelected(this.gameObject);
            this.UpdateUnit();

            if (this.mystats.Health <= this.mystats.Maxhealth / 4)
            {
                this.theorb.GetComponent<SkinnedMeshRenderer>().material.color = Color.Lerp(this.theorb.GetComponent<SkinnedMeshRenderer>().material.color, this.dangercolor, Time.deltaTime * 20);
                if (this.theorb.GetComponent<SkinnedMeshRenderer>().material.color == this.dangercolor && this.dangercolor == Color.black)
                {
                    this.dangercolor = Color.red;
                }
                else if (this.theorb.GetComponent<SkinnedMeshRenderer>().material.color == this.dangercolor && this.dangercolor == Color.red)
                {
                    this.dangercolor = Color.black;
                }
            }
        }

        /// <summary>
        /// The on destroy function.
        /// </summary>
        private void OnDestroy()
        {
            User.ExtractorCount--;
        }
    }
}