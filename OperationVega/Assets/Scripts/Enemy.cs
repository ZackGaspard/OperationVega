
namespace Assets.Scripts
{
    using Controllers;

    using Interfaces;

    using Managers;

    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// The enemy class.
    /// </summary>
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(EnemyAI))]
    [RequireComponent(typeof(Stats))]
    public class Enemy : MonoBehaviour, ICombat
    {
        /// <summary>
        /// The current target reference.
        /// </summary>
        [HideInInspector]
        public GameObject Currenttarget;

        /// <summary>
        /// The enemy finite state machine.
        /// Used to keep track of the enemy states.
        /// </summary>
        private readonly FiniteStateMachine<string> theEnemyFSM = new FiniteStateMachine<string>();

        /// <summary>
        /// The target to attack.
        /// </summary>
        private ICombat target;

        /// <summary>
        /// The resource to taint.
        /// </summary>
        private IResources targetResource;

        /// <summary>
        /// The my stats reference.
        /// This reference will contain all of this enemy stats data.
        /// </summary>
        private Stats mystats;

        /// <summary>
        /// The time to taint reference.
        /// </summary>
        private float timetotaint;

        /// <summary>
        /// The time between attacks reference.
        /// Stores the reference to the timer between attacks.
        /// </summary>
        private float timebetweenattacks;

        /// <summary>
        /// The navigation agent reference.
        /// </summary>
        private NavMeshAgent navagent;

        /// <summary>
        /// The attack function gives the enemy functionality to attack.
        /// </summary>
        public void Attack()
        {
            if (this.timebetweenattacks >= this.mystats.Attackspeed && Vector3.Distance(this.Currenttarget.transform.position, this.transform.position) <= this.mystats.Attackrange)
            {
                Debug.Log("Enemy attacked!");
                IUnit u = this.target as IUnit;
                u.AutoTarget(this.gameObject);
                this.target.TakeDamage(5);

                this.timebetweenattacks = 0;
            }
        }

        /// <summary>
        /// The take damage function allows an enemy to take damage.
        /// <para></para>
        /// <remarks><paramref name="damage"></paramref> -The amount to be calculated when the object takes damage.</remarks>
        /// </summary>
        public void TakeDamage(int damage)
        {
            this.mystats.Health -= damage;

            // Check if unit dies
            if (this.mystats.Health <= 0)
            {
                Destroy(this.gameObject);
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
            string thecurrentstate = this.theEnemyFSM.CurrentState.Statename;

            switch (destinationState)
            {
                case "Battle":
                    this.target = (ICombat)this.Currenttarget.GetComponent(typeof(ICombat));
                    this.theEnemyFSM.Feed(thecurrentstate + "To" + destinationState);
                    break;
                case "Idle":
                    this.theEnemyFSM.Feed(thecurrentstate + "To" + destinationState);
                    break;
                case "TaintResource":
                    this.targetResource = (IResources)this.Currenttarget.GetComponent(typeof(IResources));
                    this.theEnemyFSM.Feed(thecurrentstate + "To" + destinationState);
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// The taint function allows the enemy to taint a resource.
        /// </summary>
        public void Taint()
        {
            if (Vector3.Distance(this.Currenttarget.transform.position, this.transform.position) <= this.mystats.Attackrange && this.timetotaint >= 3)
            {
                Debug.Log("I Tainted it");
                this.targetResource.Taint = true;
                this.targetResource = null;
                this.ChangeStates("Idle");
                this.timetotaint = 0;
            }
        }

        /// <summary>
        /// The awake function.
        /// </summary>
        private void Awake()
        {
            this.theEnemyFSM.CreateState("Init", null);
            this.theEnemyFSM.CreateState("Idle", null);
            this.theEnemyFSM.CreateState("Battle", null);
            this.theEnemyFSM.CreateState("TaintResource", null);

            this.theEnemyFSM.AddTransition("Init", "Idle", "auto");
            this.theEnemyFSM.AddTransition("Idle", "Battle", "IdleToBattle");
            this.theEnemyFSM.AddTransition("Battle", "Idle", "BattleToIdle");
            this.theEnemyFSM.AddTransition("Idle", "TaintResource", "IdleToTaintResource");
            this.theEnemyFSM.AddTransition("TaintResource", "Idle", "TaintResourceToIdle");
            this.theEnemyFSM.AddTransition("Battle", "TaintResource", "BattleToTaintResource");
            this.theEnemyFSM.AddTransition("TaintResource", "Battle", "TaintResourceToBattle");
        }

        /// <summary>
        /// The start function
        /// </summary>
        private void Start()
        {
            this.mystats = this.GetComponent<Stats>();
            this.mystats.Health = 100;
            this.mystats.Maxhealth = 100;
            this.mystats.Strength = 3;
            this.mystats.Defense = 2;
            this.mystats.Speed = 2;
            this.mystats.Attackspeed = 2;
            this.mystats.MaxSkillCooldown = 20;
            this.mystats.CurrentSkillCooldown = this.mystats.MaxSkillCooldown;
            this.mystats.Attackrange = 2.0f;

            this.timetotaint = 0;
            this.timebetweenattacks = this.mystats.Attackspeed;
            this.navagent = this.GetComponent<NavMeshAgent>();
            this.navagent.speed = this.mystats.Speed;

            MeshCollider mc = this.GetComponent<MeshCollider>();
            mc.sharedMesh = this.GetComponentsInChildren<MeshFilter>()[1].mesh;
            this.theEnemyFSM.Feed("auto");
        }

        /// <summary>
        /// The battle state function.
        /// The function called while in the battle state.
        /// </summary>
        private void BattleState()
        {
            if (this.target != null)
            {
               this.Attack();

                // If the unit has died
                if (this.Currenttarget == null)
                {
                    this.target = null;
                    this.ChangeStates("Idle");
                }
                // If unit is alive but out of range
                else if (Vector3.Distance(this.Currenttarget.transform.position, this.transform.position) > this.GetComponent<EnemyAI>().Radius)
                {
                    this.Currenttarget = null;
                    this.target = null;
                    this.ChangeStates("Idle");
                    this.GetComponent<EnemyAI>().taunted = false;
                }
            }
        }

        /// <summary>
        /// The Taint Resource state function.
        /// The function called while in the taint resource state.
        /// </summary>
        private void TaintResourceState()
        {
            if (this.targetResource != null && !this.targetResource.Taint)
            {
                this.Taint();
            }
        }

        /// <summary>
        /// The update enemy function.
        /// This updates the enemy behavior.
        /// </summary>
        private void UpdateEnemy()
        {
            this.timebetweenattacks += 1 * Time.deltaTime;
            this.timetotaint += 1 * Time.deltaTime;

            switch (this.theEnemyFSM.CurrentState.Statename)
            {
                case "Idle":
                    break;
                case "Battle":
                    this.BattleState();
                    break;
                case "TaintResource":
                    this.TaintResourceState();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// The update rotation function.
        /// This function updates the enemy rotation accordingly.
        /// </summary>
        private void UpdateRotation()
        {
            if (this.Currenttarget != null)
            {
                Vector3 dir = this.Currenttarget.transform.position - this.transform.position;
                Quaternion lookrotation = Quaternion.LookRotation(dir);
                Vector3 rotation = Quaternion.Lerp(this.transform.rotation, lookrotation, Time.deltaTime * 5).eulerAngles;
                this.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            }
        }

        /// <summary>
        /// The update function.
        /// </summary>
        private void Update()
        {
            this.UpdateEnemy();
            this.UpdateRotation();
        }

        /// <summary>
        /// The on destroy function.
        /// </summary>
        private void OnDestroy()
        {
           ObjectiveManager.Instance.TheObjectives[ObjectiveType.Kill].Currentvalue++;
        }
    }
}