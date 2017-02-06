
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// The Damageable interface.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// The take damage function allows an object to take damage.
        /// </summary>
        /// <param name="damage">
        /// The amount of damage.
        /// </param>
        void TakeDamage(uint damage);
    }
}
