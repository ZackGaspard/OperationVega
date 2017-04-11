
namespace Assets.Scripts
{
    using System.Collections.Generic;

    using Controllers;

    using Interfaces;

    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// The enemy ai class.
    /// Handles enemy ai behavior.
    /// </summary>
    public class EnemyAI : MonoBehaviour
    {
        /// <summary>
        /// The radius reference.
        /// Reference to how big the radius of the overlap sphere is.
        /// </summary>
        [HideInInspector]
        public float Radius = 5.0f;

        /// <summary>
        /// The stunned reference.
        /// Reference if whether the enemy has been stunned.
        /// </summary>
        [HideInInspector]
        public bool stunned;

        /// <summary>
        /// The taunted reference.
        /// Reference if whether the enemy has been taunted.
        /// </summary>
        [HideInInspector]
        public bool taunted;

        /// <summary>
        /// The scared reference.
        /// Reference if whether the enemy is scared.
        /// </summary>
        [HideInInspector]
        public bool scared;

        /// <summary>
        /// The timer reference.
        /// Reference to how long the enemy should run away from the unit.
        /// </summary>
        private float runtimer;

        /// <summary>
        /// The targets reference.
        /// List of the available targets.
        /// </summary>
        private readonly List<GameObject> theTargets = new List<GameObject>();

        /// <summary>
        /// The enemy reference.
        /// Reference to this enemy.
        /// </summary>
        private Enemy enemyreference;

        /// <summary>
        /// The my navigation agent reference.
        /// Reference to the navigation agent.
        /// </summary>
        private NavMeshAgent mynavagent;

        /// <summary>
        /// The enemy controller reference.\
        /// Reference to Animator Controller.
        /// </summary>
        private Animator enemycontroller;

        /// <summary>
        /// The check rate reference.
        /// Reference to how long between casting the overlap sphere.
        /// </summary>
        private float checkrate;

        /// <summary>
        /// The next check reference.
        /// Reference to the next check with sphere.
        /// </summary>
        private float nextcheck;

        /// <summary>
        /// The on unit hit function.
        /// This function is called as an animation event function in the attack animation.
        /// </summary>
        public void OnUnitHit()
        {
            // If unit is not null
            if (UnitController.Self.unithit != null)
            {
                // Queue up a text object
                UnitController.Self.textobjs.Enqueue(UnitController.Self.combattext);
                
                // Start a coroutine to print the text to the screen -
                // It is a coroutine to assist in helping prevent text objects from
                // spawning on top one another.
                this.StartCoroutine(UnitController.Self.CombatText(UnitController.Self.unithit));
            }
        }

        /// <summary>
        /// The start.
        /// </summary>
        private void Start()
        {
            this.enemyreference = this.GetComponent<Enemy>();
            this.mynavagent = this.GetComponent<NavMeshAgent>();
            this.mynavagent.stoppingDistance = 1.2f;
            this.enemycontroller = this.GetComponent<Animator>();
            this.checkrate = Random.Range(0.5f, 1.0f);
            this.stunned = false;
            this.taunted = false;
            this.scared = false;
            this.runtimer = 1;
        }

        /// <summary>
        /// The update.
        /// </summary>
        private void Update()
        {
            if (!this.stunned && !this.taunted && !this.scared)
            {
                this.CheckForUnits();
            }
            else if (this.taunted && this.enemyreference.Currenttarget != null)
            {
                this.mynavagent.SetDestination(this.enemyreference.Currenttarget.transform.position);

                if (!this.enemycontroller.GetBool("Walk"))
                {
                    this.enemycontroller.SetBool("Walk", true);
                }
            }
            else if (this.scared && !this.stunned)
            {
                this.runtimer += 1 * Time.deltaTime;

                this.mynavagent.SetDestination(this.transform.position * this.runtimer);

                if (!this.enemycontroller.GetBool("Walk"))
                {
                    this.enemycontroller.SetBool("Walk", true);
                }

                // If the enemy has been scared for 5 seconds then the effect wore off
                if (this.runtimer >= 5.0f)
                {
                    this.scared = false;
                    this.runtimer = 1;
                    this.mynavagent.SetDestination(this.transform.position);

                    if (!this.enemycontroller.GetBool("Walk"))
                    {
                        this.enemycontroller.SetBool("Walk", true);
                    }
                }
            }
            else if (this.stunned)
            {
                if (this.enemycontroller.GetBool("Walk"))
                {
                    this.enemycontroller.SetBool("Walk", false);
                }
            }

            // If the navagent isnt looking for a current path - this helps prevent any lag when the enemy is already stopped then starting to animating,
            // if the navagent is within stopping distance...
            if (!this.mynavagent.pathPending && this.mynavagent.remainingDistance <= this.mynavagent.stoppingDistance)
            {
                // If the enemy is not attacking and the enemy has a target
                if (!this.enemycontroller.GetBool("Attack") && this.enemyreference.Currenttarget != null)
                {
                    // Switch to attack animation
                    this.enemycontroller.SetBool("Attack", true);

                    // Incase the unit was walking before attacking, set it to false
                    this.enemycontroller.SetBool("Walk", false);

                    // Incase the unit was idling before attacking, set it to false
                    this.enemycontroller.SetBool("Idle", false);
                }
                // Else if the enemy is attacking and the target has become null
                else if (this.enemycontroller.GetBool("Attack") && this.enemyreference.Currenttarget == null)
                {
                    // Switch to idle
                    this.enemycontroller.SetBool("Attack", false);
                }
                // Else if the enemy is walking and the target has become null
                else if (this.enemycontroller.GetBool("Walk") && this.enemyreference.Currenttarget == null)
                {
                    // Switch to idle
                    this.enemycontroller.SetBool("Walk", false);
                }
            }
        }

        /// <summary>
        /// The check for units function.
        /// Checks for units in range of the overlap sphere.
        /// </summary>
        private void CheckForUnits()
        {
            if (Time.time > this.nextcheck)
            {
                this.nextcheck = Time.time + this.checkrate;
                this.theTargets.Clear();
                Collider[] validtargets = Physics.OverlapSphere(this.transform.position, this.Radius, 1 << LayerMask.NameToLayer("Targetable"));

                // If nothing hit by cast then return
                if (validtargets.Length < 1) return;

                // Sort the found targets
                this.SortTargets(validtargets);

                // Go to the target
                this.GoToTarget();
            }
        }

        /// <summary>
        /// The find closest target function.
        /// Finds the closest target to the enemy.
        /// </summary>
        private void GoToTarget()
        {
            if (this.enemyreference.Currenttarget != null)
            {
                if (this.enemyreference.Currenttarget.GetComponent(typeof(IUnit)))
                {
                    this.enemyreference.ChangeStates("Battle");
                }
                else if (this.enemyreference.Currenttarget.GetComponent(typeof(IResources)))
                {
                    this.enemyreference.ChangeStates("TaintResource");
                }

                this.mynavagent.SetDestination(this.enemyreference.Currenttarget.transform.position);

                // Only Set the animator to walk if its not in any other state
                if (!this.enemycontroller.GetBool("Walk") && !this.enemycontroller.GetBool("Attack"))
                {
                    this.enemycontroller.SetBool("Walk", true);
                }
            }
        }

        /// <summary>
        /// The sort targets function.
        /// Sorts the list until able to find a unit to attack or resource to taint.
        /// <remarks><paramref name="validtargets"></paramref> -The colliders found from the overlap sphere cast.</remarks>
        /// </summary>
        private void SortTargets(Collider[] validtargets)
        {
            foreach (Collider c in validtargets)
            {
                if (c.gameObject.GetComponent(typeof(IResources)))
                {
                    IResources theResource = (IResources)c.gameObject.GetComponent(typeof(IResources));

                    // Add resource if its not tainted
                    if (!theResource.Taint)
                    {
                        this.theTargets.Add(c.transform.gameObject);
                    }
                }
                else
                {
                    // The only other object in the list is a type of unit so add it.
                    this.theTargets.Add(c.transform.gameObject);
                }
            }

            this.theTargets.Sort(
                        delegate(GameObject a, GameObject b)
                        {
                            float distanceA = Vector3.Distance(a.transform.position, this.transform.position);
                            float distanceB = Vector3.Distance(b.transform.position, this.transform.position);

                            if (distanceA > distanceB) return 1;
                            if (distanceA < distanceB) return -1;

                            return 0;
                        });

            if (this.theTargets.Count > 0)
            {
                this.enemyreference.Currenttarget = this.theTargets[0].gameObject;
            }
        }
    }
}
