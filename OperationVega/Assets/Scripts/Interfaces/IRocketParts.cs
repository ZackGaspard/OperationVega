// Interface for the rocket parts

namespace Assets.Scripts.Interfaces
{
	using System.Collections;
	using System.Collections.Generic;

	using UnityEngine;

	/// <summary>
	/// The RocketParts interface.
	/// </summary>
	public interface IRocketParts
	{
		/// <summary>
		/// Gets or sets the quality.
		/// </summary>
		int Quality { get; set; }

		/// <summary>
		/// Gets or sets the steel cost.
		/// </summary>
		int SteelCost { get; set; }

		/// <summary>
		/// Gets or sets the fuel cost.
		/// </summary>
		int FuelCost { get; set; }
	}
}