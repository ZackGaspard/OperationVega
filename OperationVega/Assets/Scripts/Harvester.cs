﻿
namespace Assets.Scripts
{
    using System.Collections;
    using Controllers;
    using Interfaces;
    using Managers;
    using UI;
    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// The harvester class.
    /// </summary>
    [RequireComponent(typeof(Stats))]
    public class Harvester : MonoBehaviour, IUnit, ICombat
    {
        /// <summary>
        /// The harvester finite state machine.
        /// Used to keep track of the harvesters states.
        /// </summary>
        private readonly FiniteStateMachine<string> theHarvesterFsm = new FiniteStateMachine<string>();

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
        /// Reference to the clean food prefab.
        /// </summary>
        [SerializeField]
        private GameObject cleanfood;

        /// <summary>
        /// Reference to the dirty food prefab.
        /// </summary>
        [SerializeField]
        private GameObject dirtyfood;

        /// <summary>
        /// The object to look at reference.
        /// </summary>
        private GameObject theobjecttolookat;

        /// <summary>
        /// The target to heal/stun.
        /// </summary>
        private ICombat target;

        /// <summary>
        /// The enemy game object reference.
        /// </summary>
        private GameObject theEnemy;

        /// <summary>
        /// The recent tree reference that we were farming from.
        /// </summary>
        private GameObject theRecentTree;

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
        /// The attack function houses the Heal Stun ability function.
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
                    Debug.Log("Harvester hit crit!");
                    this.target.TakeDamage(10);
                    this.timebetweenattacks = 0;
                }
                else
                {
                    Debug.Log("Harvester attacking normal damage");
                    this.target.TakeDamage(5);
                    this.timebetweenattacks = 0;
                }
            }
        }

        /// <summary>
        /// The harvest function provides functionality of the harvester to harvest a resource.
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
                if (this.mystats.Resourcecount == 5 && !this.targetResource.Taint)
                {
                    // Create the clean food object and parent it to the front of the harvester
                    var clone = Instantiate(this.cleanfood, this.transform.position + (this.transform.forward * 0.6f), this.transform.rotation);
                    clone.transform.SetParent(this.transform);
                    clone.name = "Food";
                    this.mystats.Resourcecount = 0;
                    this.ChangeStates("Stock");
                    GameObject thesilo = GameObject.Find("Silo");
                    Vector3 destination = new Vector3(thesilo.transform.position.x + (this.transform.forward.x * 2), 0.5f, thesilo.transform.position.z + (this.transform.forward.z * 2));
                    this.navagent.SetDestination(destination);
                    this.animatorcontroller.SetBool("IsWalking", true);
                }
                else if (this.mystats.Resourcecount == 5 && this.targetResource.Taint)
                {
                    // The resource is tainted go to decontamination center
                    // Create the dirty food object and parent it to the front of the harvester
                    var clone = Instantiate(this.dirtyfood, this.transform.position + (this.transform.forward * 0.6f), this.transform.rotation);
                    clone.transform.SetParent(this.transform);
                    clone.name = "FoodTainted";
                    this.ChangeStates("Decontaminate");
                    GameObject thedecontaminationbuilding = GameObject.Find("Decontamination");
                    Transform thedoor = thedecontaminationbuilding.transform.Find("FrontDoor");
                    Vector3 destination = new Vector3(thedoor.position.x, 0.5f, thedoor.position.z);
                    this.navagent.SetDestination(destination);
                    this.animatorcontroller.SetBool("IsWalking", true);
                }
            }
        }

        /// <summary>
        /// The special ability for the harvester.
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
                        Debug.Log("Found An enemy");
                        NavMeshAgent navagent = c.gameObject.GetComponent<NavMeshAgent>();
                        EnemyAI enemy = c.gameObject.GetComponent<EnemyAI>();

                        this.StartCoroutine(this.EnemyStopTimer(navagent, enemy));

                    } // If its a unit - Try and heal them
                    else if (c.gameObject.GetComponent(typeof(IUnit)))
                    {
                        Stats unitstats = c.gameObject.GetComponent<Stats>();

                        // Only heal them if health is less than max health
                        if (unitstats.Health < unitstats.Maxhealth)
                        {
                            unitstats.Health += 20;

                            // If health is greater than max health..set health to max health, else just set it to the current health value.
                            unitstats.Health = unitstats.Health > unitstats.Maxhealth ? unitstats.Maxhealth : unitstats.Health;
                        }
                    }
                }

                UIManager.Self.currentcooldown = 0;
                this.mystats.CurrentSkillCooldown = 0;
                Debug.Log("Harvester Special Ability Activated");
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
                this.mystats.Resourcecount--;
                this.decontime = 0;

                if (this.mystats.Resourcecount <= 0)
                {
                    this.mystats.Resourcecount = 0;
                    this.alreadystockedcount = 0;
                    int counter = 0;

                    for (int i = 0; i < this.transform.childCount; i++)
                    {
                        if (this.transform.GetChild(i).name == "FoodTainted")
                        {
                            Destroy(this.transform.GetChild(i).gameObject);
                            counter++;
                        }
                    }

                    for (int i = 0; i < counter; i++)
                    {
                        var clone = Instantiate(
                        this.cleanfood,
                        this.transform.position + (this.transform.forward * 0.6f),
                        this.transform.rotation);
                        clone.transform.SetParent(this.transform);
                        clone.name = "Food";
                        if (i > 0)
                        {
                            clone.transform.gameObject.SetActive(false);
                        }
                    }

                    this.ChangeStates("Stock");
                    GameObject thesilo = GameObject.Find("Silo");
                    Vector3 destination = new Vector3(thesilo.transform.position.x + (this.transform.forward.x * 2), 0.5f, thesilo.transform.position.z + (this.transform.forward.z * 2));
                    this.navagent.SetDestination(destination);
                    this.animatorcontroller.SetBool("IsWalking", true);
                }
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
        /// The change states function.
        /// This function changes the state to the passed in state.
        /// <para></para>
        /// <remarks><paramref name="destinationState"></paramref> -The state to transition to.</remarks>
        /// </summary>
        public void ChangeStates(string destinationState)
        {
            string thecurrentstate = this.theHarvesterFsm.CurrentState.Statename;
            switch (destinationState)
            {
                case "Battle":
                    this.DropItems();
                    this.navagent.updateRotation = false;
                    this.theHarvesterFsm.Feed(thecurrentstate + "To" + destinationState, this.mystats.Attackrange);
                    break;
                case "Idle":
                    this.navagent.updateRotation = true;
                    this.theHarvesterFsm.Feed(thecurrentstate + "To" + destinationState, 1.0f);
                    break;
                case "Harvest":
                    this.navagent.updateRotation = false;
                    this.theHarvesterFsm.Feed(thecurrentstate + "To" + destinationState, 1.5f);
                    break;
                case "Stock":
                    this.theobjecttolookat = GameObject.Find("Silo");
                    this.navagent.updateRotation = false;
                    this.theHarvesterFsm.Feed(thecurrentstate + "To" + destinationState, 1.5f);
                    break;
                case "Decontaminate":
                    this.theobjecttolookat = GameObject.Find("Decontamination");
                    this.navagent.updateRotation = false;
                    this.theHarvesterFsm.Feed(thecurrentstate + "To" + destinationState, 1.0f);
                    break;
                case "PickUp":
                    this.navagent.updateRotation = false;
                    this.theHarvesterFsm.Feed(thecurrentstate + "To" + destinationState, 1.0f);
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
            if (theResource.GetComponent<Food>())
            {
                this.theobjecttolookat = theResource;
                this.targetResource = (IResources)theResource.GetComponent(typeof(IResources));
                this.navagent.SetDestination(theResource.transform.position);
                this.animatorcontroller.SetBool("IsWalking", true);
                this.theRecentTree = theResource;
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
            if (thepickup.name == "Food" || thepickup.name == "FoodTainted")
            {
                this.objecttopickup = thepickup;
                this.theobjecttolookat = this.objecttopickup;
                this.navagent.SetDestination(thepickup.transform.position);
                this.animatorcontroller.SetBool("IsWalking", true);
                this.ChangeStates("PickUp");
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
            this.decontime += 1 * Time.deltaTime;

            this.UpdateRotation();

            // If the navagent isnt looking for a current path - this helps prevent any lag when the unit is already stopped then starting to move,
            // if the navagent is within stopping distance and its currently using the walk animation...
            if (!this.navagent.pathPending && this.navagent.remainingDistance <= this.navagent.stoppingDistance && this.animatorcontroller.GetBool("IsWalking"))
            {
                this.animatorcontroller.SetBool("IsWalking", false);
            }

            switch (this.theHarvesterFsm.CurrentState.Statename)
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
            this.theorb = this.transform.GetChild(2).GetChild(2).GetChild(0).gameObject;
            this.dangercolor = Color.black;

            this.mystats = this.GetComponent<Stats>();
            this.mystats.Health = 100;
            this.mystats.Maxhealth = 100;
            this.mystats.Strength = 2;
            this.mystats.Defense = 5;
            this.mystats.Speed = 3;
            this.mystats.Attackspeed = 3;
            this.mystats.MaxSkillCooldown = 15;
            this.mystats.CurrentSkillCooldown = this.mystats.MaxSkillCooldown;
            this.mystats.Attackrange = 5.0f;
            this.mystats.Resourcecount = 0;

            this.gothitfirst = true;
            this.harvesttime = 1.0f;
            this.decontime = 1.0f;

            this.theorb.GetComponent<SkinnedMeshRenderer>().material.color = Color.green;
            this.timebetweenattacks = this.mystats.Attackspeed;
            this.navagent = this.GetComponent<NavMeshAgent>();
            this.navagent.speed = this.mystats.Speed;
            this.animatorcontroller = this.GetComponent<Animator>();
            Debug.Log("Harvester Initialized");
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
                if (t.name == "Food")
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

                if (children[i].name == "Food" || children[i].name == "FoodTainted")
                {
                    children[i].position = new Vector3(this.transform.position.x + x, 0f, this.transform.position.z + z);
                    children[i].tag = "PickUp";
                    children[i].parent = null;
                    this.mystats.Resourcecount = 0;
                }
            }
        }

        /// <summary>
        /// The idle state function.
        /// Has the functionality of checking if out of the range of the enemy.
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
            if (this.targetResource != null && this.targetResource.Count > 0)
            {
                if (!this.transform.Find("Food") && !this.transform.Find("FoodTainted"))
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
            if (this.transform.Find("Food"))
            {
                if (this.mystats.Resourcecount <= 0)
                {
                    this.mystats.Resourcecount = 0;
                    this.alreadystockedcount = 0;

                    for (int i = 0; i < this.transform.childCount; i++)
                    {
                        if (this.transform.GetChild(i).name == "Food")
                        {
                            Destroy(this.transform.GetChild(i).gameObject);
                        }
                    }

                    if (this.targetResource != null && this.targetResource.Count > 0)
                    {
                        this.theobjecttolookat = this.theRecentTree;
                        this.navagent.SetDestination(this.theRecentTree.transform.position);
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
                        User.FoodCount++;
                        Debug.Log("I have now stocked " + User.FoodCount + " food");
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
                Transform foodtainted = this.transform.Find("FoodTainted");
                Transform food = this.transform.Find("Food");

                if (food == null && foodtainted == null)
                {
                    this.objecttopickup.transform.position = this.transform.position + (this.transform.forward * 0.6f);
                    this.objecttopickup.transform.SetParent(this.transform);
                    if (this.objecttopickup.name == "FoodTainted")
                    {
                        this.mystats.Resourcecount = 5;
                    }
                }
                else if (this.objecttopickup.name == "Food")
                {
                    if (food != null && foodtainted == null)
                    {
                        this.objecttopickup.transform.position = this.transform.position + (this.transform.forward * 0.6f);
                        this.objecttopickup.transform.SetParent(this.transform);
                        this.objecttopickup.gameObject.SetActive(false);
                    }
                }
                else if (this.objecttopickup.name == "FoodTainted")
                {
                    if (foodtainted != null && food == null)
                    {
                        this.objecttopickup.transform.position = this.transform.position + (this.transform.forward * 0.6f);
                        this.objecttopickup.transform.SetParent(this.transform);
                        this.objecttopickup.gameObject.SetActive(false);
                        this.mystats.Resourcecount = 5;
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
            if (this.transform.Find("FoodTainted"))
            {
                if (this.navagent.remainingDistance <= this.navagent.stoppingDistance && !this.navagent.pathPending)
                {
                    this.Decontaminate();
                }
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
        /// The enemy stop timer function.
        /// The enemy takes the stun effect .
        /// <para></para>
        /// <remarks><paramref name="nav"></paramref> -The nav agent to stop and resume.</remarks>
        /// <para></para>
        /// <remarks><paramref name="enemy"></paramref> -The enemy ai to set its stunned value.</remarks>
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        private IEnumerator EnemyStopTimer(NavMeshAgent nav, EnemyAI enemy)
        {
            enemy.stunned = true;
            nav.Stop();
            Debug.Log("Starting to wait");
            yield return new WaitForSeconds(3);
            enemy.stunned = false;
            nav.Resume();
            Debug.Log("Done waiting.");
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

            this.theHarvesterFsm.CreateState("Init", null);
            this.theHarvesterFsm.CreateState("Idle", this.idleHandler);
            this.theHarvesterFsm.CreateState("Battle", this.battleHandler);
            this.theHarvesterFsm.CreateState("Harvest", this.harvestHandler);
            this.theHarvesterFsm.CreateState("Stock", this.stockHandler);
            this.theHarvesterFsm.CreateState("Decontaminate", this.decontaminationHandler);
            this.theHarvesterFsm.CreateState("PickUp", this.pickupHandler);

            this.theHarvesterFsm.AddTransition("Init", "Idle", "auto");
            this.theHarvesterFsm.AddTransition("Idle", "Battle", "IdleToBattle");
            this.theHarvesterFsm.AddTransition("Battle", "Idle", "BattleToIdle");
            this.theHarvesterFsm.AddTransition("Idle", "Harvest", "IdleToHarvest");
            this.theHarvesterFsm.AddTransition("Harvest", "Idle", "HarvestToIdle");
            this.theHarvesterFsm.AddTransition("Battle", "Harvest", "BattleToHarvest");
            this.theHarvesterFsm.AddTransition("Harvest", "Battle", "HarvestToBattle");
            this.theHarvesterFsm.AddTransition("Harvest", "Stock", "HarvestToStock");
            this.theHarvesterFsm.AddTransition("Battle", "Stock", "BattleToStock");
            this.theHarvesterFsm.AddTransition("Idle", "Stock", "IdleToStock");
            this.theHarvesterFsm.AddTransition("Stock", "Idle", "StockToIdle");
            this.theHarvesterFsm.AddTransition("Stock", "Battle", "StockToBattle");
            this.theHarvesterFsm.AddTransition("Stock", "Harvest", "StockToHarvest");
            this.theHarvesterFsm.AddTransition("Harvest", "Decontaminate", "HarvestToDecontaminate");
            this.theHarvesterFsm.AddTransition("Stock", "Decontaminate", "StockToDecontaminate");
            this.theHarvesterFsm.AddTransition("Decontaminate", "Stock", "DecontaminateToStock");
            this.theHarvesterFsm.AddTransition("Decontaminate", "Idle", "DecontaminateToIdle");
            this.theHarvesterFsm.AddTransition("Idle", "Decontaminate", "IdleToDecontaminate");
            this.theHarvesterFsm.AddTransition("Decontaminate", "Battle", "DecontaminateToBattle");
            this.theHarvesterFsm.AddTransition("Battle", "Decontaminate", "BattleToDecontaminate");
            this.theHarvesterFsm.AddTransition("PickUp", "Idle", "PickUpToIdle");
            this.theHarvesterFsm.AddTransition("PickUp", "Battle", "PickUpToBattle");
            this.theHarvesterFsm.AddTransition("PickUp", "Harvest", "PickUpToHarvest");
            this.theHarvesterFsm.AddTransition("PickUp", "Decontaminate", "PickUpToDecontaminate");
            this.theHarvesterFsm.AddTransition("PickUp", "Stock", "PickUpToStock");
            this.theHarvesterFsm.AddTransition("Idle", "PickUp", "IdleToPickUp");
            this.theHarvesterFsm.AddTransition("Battle", "PickUp", "BattleToPickUp");
            this.theHarvesterFsm.AddTransition("Harvest", "PickUp", "HarvestToPickUp");
            this.theHarvesterFsm.AddTransition("Stock", "PickUp", "StockToPickUp");
            this.theHarvesterFsm.AddTransition("Decontaminate", "PickUp", "DecontaminateToPickUp");
        }

        /// <summary>
        /// The start function.
        /// </summary>
        private void Start()
        {
            this.InitUnit();
            this.theHarvesterFsm.Feed("auto", 0.1f);
            GameManager.Instance.TheHarvesters.Add(this);
            User.HarvesterCount++;
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
            User.HarvesterCount--;
            GameManager.Instance.TheHarvesters.Remove(this);
            GameManager.Instance.CheckForLoss();
        }

    }
}