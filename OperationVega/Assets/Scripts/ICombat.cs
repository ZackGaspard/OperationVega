
namespace Assets.Scripts
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
    }
}
