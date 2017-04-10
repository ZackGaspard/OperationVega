

namespace Assets.Scripts.BaseClasses
{
	using System.Collections;
	using System.Collections.Generic;

	using Assets.Scripts.Interfaces;

	using UnityEngine;

	/// <summary>
	/// The base thrusters.
	/// </summary>
	[System.Serializable]
	public class BaseThrusters : IRocketParts
	{
		/// <summary>
		/// The steel.
		/// The amount of steel required to build the part.
		/// Accessible through the SteelCost property.
		/// </summary>
		[SerializeField]
		private uint steel;

		/// <summary>
		/// The fuel.
		/// The amount of Fuel required to build the part.
		/// Accessible through the FuelCost property.
		/// </summary>
		[SerializeField]
		private uint fuel;

		/// <summary>
		/// The quality.
		/// </summary>
		[SerializeField]
		private uint quality;

		[SerializeField]
		private string name;

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseThrusters"/> class.
		/// </summary>
		public BaseThrusters()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseThrusters"/> class.
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
		public BaseThrusters(uint steelcost, uint fuelcost, uint qualities, string identity)
		{
			this.steel = steelcost;
			this.fuel = fuelcost;
			this.quality = qualities;
			this.name = identity;
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

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}
	}
}