

namespace Assets.Scripts.BaseClasses
{
	using System.Collections;
	using System.Collections.Generic;

	using Assets.Scripts.Interfaces;

	using UnityEngine;

	/// <summary>
	/// The base wings.
	/// </summary>
	public class BaseWings : IRocketParts
	{
		/// <summary>
		/// The steel.
		/// The amount of steel required to build the part.
		/// Accessible through the SteelCost property.
		/// </summary>
		private uint steel;

		/// <summary>
		/// The fuel.
		/// The amount of Fuel required to build the part.
		/// Accessible through the FuelCost property.
		/// </summary>
		private uint fuel;

		/// <summary>
		/// The quality.
		/// </summary>
		private uint quality;

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseWings"/> class.
		/// </summary>
		public BaseWings()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseWings"/> class.
		/// </summary>
		/// <param name="steelcost">
		/// The steel cost.
		/// </param>
		/// <param name="fuelcost">
		/// The fuel cost.
		/// </param>
		/// <param name="qualities">
		/// The quality.
		/// </param>
		public BaseWings(uint steelcost, uint fuelcost, uint qualities)
		{
			this.steel = steelcost;
			this.fuel = fuelcost;
			this.quality = qualities;
		}

		/// <summary>
		/// Gets or sets the quality.
		/// </summary>
		public uint Quality
		{
			get
			{
				return this.quality;
			}

			set
			{
				this.quality = value;
			}
		}

		/// <summary>
		/// Gets or sets the steel cost.
		/// </summary>
		public uint SteelCost
		{
			get
			{
				return this.steel;
			}

			set
			{
				this.steel = value;
			}
		}

		/// <summary>
		/// Gets or sets the fuel cost.
		/// </summary>
		public uint FuelCost
		{
			get
			{
				return this.fuel;
			}

			set
			{
				this.fuel = value;
			}
		}
	}
}