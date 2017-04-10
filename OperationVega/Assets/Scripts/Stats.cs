
namespace Assets.Scripts
{
    using UnityEngine;

    /// <summary>
    /// The stats class.
    /// This class will hold the stats for each unit.
    /// </summary>
    public class Stats : MonoBehaviour
    {
        /// <summary>
        /// The health of the unit.
        /// </summary>
        [HideInInspector]
        public int Health;

        /// <summary>
        /// The max health of the unit.
        /// </summary>
        [HideInInspector]
        public int Maxhealth;

        /// <summary>
        /// The strength of the unit.
        /// </summary>
        [HideInInspector]
        public int Strength;

        /// <summary>
        /// The defense of the unit.
        /// </summary>
        [HideInInspector]
        public int Defense;

        /// <summary>
        /// The speed of the unit.
        /// </summary>
        [HideInInspector]
        public int Speed;

        /// <summary>
        /// The attack speed of the unit.
        /// </summary>
        [HideInInspector]
        public int Attackspeed;

        /// <summary>
        /// The current skill cool down of the unit.
        /// </summary>
        [HideInInspector]
        public float CurrentSkillCooldown;

        /// <summary>
        /// The max skill cool down of the unit.
        /// </summary>
        [HideInInspector]
        public float MaxSkillCooldown;

        /// <summary>
        /// The attack range of the unit.
        /// </summary>
        [HideInInspector]
        public float Attackrange;

        /// <summary>
        /// The resource count of the unit.
        /// </summary>
        [HideInInspector]
        public int Resourcecount;
    }
}
