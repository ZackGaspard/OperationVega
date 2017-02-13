
namespace Assets.Scripts.Interfaces
{
    /// <summary>
    /// The Unit interface to represent what it means to be a Unit.
    /// </summary>
    public interface IUnit
    {
        /// <summary>
        /// The change states function.
        /// This function changes the state to the passed in state.
        /// <para></para>
        /// <remarks><paramref name="destinationState"></paramref> -The state to transition to.</remarks>
        /// </summary>
        void ChangeStates(string destinationState);
    }
}
