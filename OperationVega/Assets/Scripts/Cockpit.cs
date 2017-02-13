
namespace Assets.Scripts
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	using Assets.Scripts.Interfaces;

	using UnityEditor;

	using UnityEngine;

	/// <summary>
	/// The cockpit  class.
	/// </summary>
	public class Cockpit : MonoBehaviour, IRocketParts
	{
		/// <summary>
		/// The capacity.
		/// </summary>
		private int capacity;

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
		/// The ship.
		/// </summary>
		private Rocket ship;

		/// <summary>
		/// Gets or sets the carrying.
		/// </summary>
		public int Carrying
		{
			get
			{
				return this.capacity;
			}

			set
			{
				this.capacity = value;
			}
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

	
		/// <param name="secondaryList">
		/// The secondaryList. A second list of the parts.
		/// Solves error that's thrown when a part is removed from the list.
		/// </param>
		public void AddParts(List<IRocketParts> secondaryList)
		{
			if (User.SteelCount >= this.steel && User.FuelCount >= this.fuel)
			{
				if (this.ship.PartList.OfType<Cockpit>().Any())
				{
					foreach (var go in secondaryList)
					{
						if (go as Cockpit)
						{
							this.ship.PartList.Remove(go);
						}
					}
				}
				else if (!this.ship.PartList.OfType<Cockpit>().Any())
				{
					this.ship.PartList.Add(this);
					User.SteelCount -= this.steel;
					User.FuelCount -= this.fuel;
				}
			}

			if (User.SteelCount < this.steel)
			{
				Debug.Log("You don't have enough steel.");
			}

			if (User.FuelCount < this.fuel)
			{
				Debug.Log("You don't have enough fuel.");
			}
		}

		/// <summary>
		/// Use this for initialization
		/// </summary>
		private void Start()
		{
			this.quality = 20;
			this.steel = 200;
			this.fuel = 0;
			this.capacity = 20;
			this.ship = FindObjectOfType<Rocket>();
		}

		/// <summary>
		/// Update is called once per frame
		/// </summary>
		private void Update()
		{
			var cockpitList = this.ship.PartList.ToList();

			if (Input.GetMouseButtonDown(0))
			{

				this.AddParts(cockpitList);
			}

			// User.SteelCount -= this.steel;
			if (Input.GetKeyDown(KeyCode.A))
			{
				User.SteelCount += 100;
			}

			if (Input.GetKeyDown(KeyCode.S))
			{
				User.FuelCount += 100;
			}
			
			Debug.Log("Temp: " + cockpitList.Count);
		}
	}
}