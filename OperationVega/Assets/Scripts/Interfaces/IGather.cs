
namespace Assets.Scripts.Interfaces
{
    using UnityEngine;

    /// <summary>
    /// The Gather interface to allow an object to implement gathering.
    /// </summary>
    public interface IGather
    {
        /// <summary>
        /// The harvest function allows an object to harvest a resource.
        /// </summary>
        void Harvest();

        /// <summary>
        /// The decontaminate function allows decontamination of a resource.
        /// </summary>
        void Decontaminate();

        /// <summary>
        /// The set the target position.
        /// </summary>
        /// <param name="targetPos">
        /// The target position to go to when clicked.
        /// </param>
        void SetTheTargetPosition(Vector3 targetPos);
    }
}
