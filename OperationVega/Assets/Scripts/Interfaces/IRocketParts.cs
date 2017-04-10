
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
		uint Quality { get; set; }

		/// <summary>
		/// Gets or sets the steel cost.
		/// </summary>
		uint SteelCost { get; set; }

		/// <summary>
		/// Gets or sets the fuel cost.
		/// </summary>
		uint FuelCost { get; set; }

		string Name { get; set; }
	}
}