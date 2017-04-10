
namespace Assets.Scripts.Interfaces
{
    /// <summary>
    /// The Combat interface that allows an object to have combat functionality.
    /// </summary>
    public interface ICombat
    {
        /// <summary>
        /// The attack function that is used to attack an IUnit.
        /// </summary>
        void Attack();

        /// <summary>
        /// The take damage function allows an object to take damage.
        /// <para></para>
        /// <remarks><paramref name="damage"></paramref> -The amount to be calculated when the object takes damage.</remarks>
        /// </summary>
        void TakeDamage(int damage);
    }
}
