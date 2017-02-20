
namespace Assets.Scripts
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	using Assets.Scripts.Interfaces;

	using UnityEngine;

	/// <summary>
	/// The rocket class.
	/// </summary>
	public class Rocket : MonoBehaviour
	{
		/// <summary>
		/// The all parts.
		/// </summary>
		private List<IRocketParts> allParts;

		/// <summary>
		/// The total quality.
		/// </summary>
		private uint totalQuality;

		/// <summary>
		/// The selectable cockpit.
		/// </summary>
		private Cockpit selectableCockpit;

		/// <summary>
		/// The selectable chassis.
		/// </summary>
		private Chassis selectableChassis;

		/// <summary>
		/// The selectable wings.
		/// </summary>
		private Wings selectableWings;

		/// <summary>
		/// The selectable thrusters.
		/// </summary>
		private Thrusters selectableThrusters;

		/// <summary>
		/// Gets or sets the part list.
		/// </summary>
		public List<IRocketParts> PartList
		{
			get
			{
				return this.allParts;
			}

			set
			{
				this.allParts = value;
			}
		}

		/// <summary>
		/// Gets or sets the total quality.
		/// </summary>
		public uint FullQuality
		{
			get
			{
				return this.totalQuality;
			}

			set
			{
				this.totalQuality = value;
			}
		}

		/// <summary>
		/// The ship build.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShipBuild()
		{
			if (this.allParts.OfType<Cockpit>().Any() &&
				this.allParts.OfType<Chassis>().Any() &&
				this.allParts.OfType<Thrusters>().Any() &&
				this.allParts.OfType<Wings>().Any())
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Function for adding a cockpit to the list.
		/// </summary>
		/// <param name="secondaryList">
		/// The secondaryList. A second list of the parts.
		/// Solves error that's thrown when a part is removed from the list.
		/// </param>
		/// <param name="selectedParts">
		/// The selected Parts.
		/// </param>
		public void AddParts(List<IRocketParts> secondaryList, IRocketParts selectedParts)
		{
			if (this.allParts.Contains(selectedParts))
			{
				this.allParts.Remove(selectedParts);
				this.totalQuality -= selectedParts.Quality;
			}
			else if (!this.allParts.Contains(selectedParts))
			{
				if (User.SteelCount >= selectedParts.SteelCost && User.FuelCount >= selectedParts.FuelCost)
				{
					this.allParts.Add(selectedParts);
					this.totalQuality += selectedParts.Quality;
					User.SteelCount -= selectedParts.SteelCost;
					User.FuelCount -= selectedParts.FuelCost;
				}

				if (User.SteelCount < selectedParts.SteelCost)
				{
					Debug.Log("You don't have enough steel.");
				}

				if (User.FuelCount < selectedParts.FuelCost)
				{
					Debug.Log("You don't have enough fuel.");
				}
			}
		}

		/// <summary>
		/// Use this for initialization
		/// </summary>
		private void Start()
		{
			this.allParts = new List<IRocketParts>();
			this.selectableCockpit = FindObjectOfType<Cockpit>();
			this.selectableChassis = FindObjectOfType<Chassis>();
			this.selectableWings = FindObjectOfType<Wings>();
			this.selectableThrusters = FindObjectOfType<Thrusters>();
		}

		/// <summary>
		/// Update is called once per frame
		/// </summary>
		private void Update()
		{
			var spareList = this.allParts.ToList();

			if (Input.GetMouseButtonDown(0))
			{
				this.AddParts(spareList, this.selectableCockpit);
			}

			if (Input.GetMouseButtonDown(1))
			{
				this.AddParts(spareList, this.selectableChassis);
			}

			if (Input.GetKeyDown(KeyCode.Keypad1))
			{
				this.AddParts(spareList, this.selectableWings);
			}

			if (Input.GetKeyDown(KeyCode.Keypad2))
			{
				this.AddParts(spareList, this.selectableThrusters);
			}

			if (Input.GetKeyDown(KeyCode.A))
			{
				User.SteelCount += 100;
			}

			if (Input.GetKeyDown(KeyCode.S))
			{
				User.FuelCount += 100;
			}

			Debug.Log(this.allParts.Count);
			Debug.Log("Total: " + this.totalQuality);
		}
	}
}
