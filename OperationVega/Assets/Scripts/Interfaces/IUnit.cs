
namespace Assets.Scripts.Interfaces
{
    using UnityEngine;

    /// <summary>
    /// The Unit interface to represent unit functionality.
    /// </summary>
    public interface IUnit
    {
        /// <summary>
        /// The harvest function allows a unit to harvest a resource.
        /// </summary>
        void Harvest();

        /// <summary>
        /// The Special Ability function activates a unit(s) special ability.
        /// </summary>
        void SpecialAbility();

        /// <summary>
        /// The set target function.
        /// Sets the object as the target for the unit.
        /// <para></para>
        /// <remarks><paramref name="theTarget"></paramref> -The object that will be set as the target for attacking.</remarks>
        /// </summary>
        void SetTarget(GameObject theTarget);

        /// <summary>
        /// The set target function.
        /// Auto sets the object as the target for the unit.
        /// <para></para>
        /// <remarks><paramref name="theTarget"></paramref> -The object that will be set as the target for attacking.</remarks>
        /// </summary>
        void AutoTarget(GameObject theTarget);

        /// <summary>
        /// The set target resource function.
        /// The function sets the unit with the resource to target.
        /// <para></para>
        /// <remarks><paramref name="theResource"></paramref> -The object that will be set as the target resource.</remarks>
        /// </summary>
        void SetTargetResource(GameObject theResource);

        /// <summary>
        /// The set move position function.
        /// Sets the destination for the unit.
        /// <para></para>
        /// <remarks><paramref name="theClickPosition"></paramref> -The object that will be set as the position to move to.</remarks>
        /// </summary>
        void SetTheMovePosition(Vector3 theClickPosition);

        /// <summary>
        /// The go to pickup function.
        /// Parses and sends the unit to pickup a dropped resource.
        /// <para></para>
        /// <remarks><paramref name="thepickup"></paramref> -The object that will be set as the item to pick up.</remarks>
        /// </summary>
        void GoToPickup(GameObject thepickup);

        /// <summary>
        /// The change states function.
        /// This function changes the state of the unit to the passed in state.
        /// <para></para>
        /// <remarks><paramref name="destinationState"></paramref> -The state to transition to.</remarks>
        /// </summary>
        void ChangeStates(string destinationState);
    }
}
