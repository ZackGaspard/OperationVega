﻿
namespace Assets.Scripts
{
    using UnityEngine;

    /// <summary>
    /// The Unit interface to represent what it means to be a Unit.
    /// </summary>
    public interface IUnit
    {
        /// <summary>
        /// The move function that will have the unit move accordingly.
        /// </summary>
        void Move();
    }
}
