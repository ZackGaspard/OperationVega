﻿
namespace Assets.Scripts
{
	using System.Collections.Generic;
	using System.Linq;

	using Assets.Scripts.BaseClasses;
	using Assets.Scripts.Interfaces;

	using UnityEngine;

	/// <summary>
	/// The rocket class.
	/// </summary>
	public class Rocket : MonoBehaviour
	{
		/// <summary>
		/// A list of all the parts that have been selected.
		/// </summary>
		private List<IRocketParts> allParts = new List<IRocketParts>();

		/// <summary>
		/// A list of all the part objects that have been instantiated.
		/// </summary>
		[SerializeField]
		private List<GameObject> builtParts = new List<GameObject>();

		/// <summary>
		/// The total quality.
		/// </summary>
		[SerializeField]
		private uint totalQuality;

		/// <summary>
		/// Place holding value that represents the current cockpit in the list.
		/// Used for when the existing cockpit is being replaced and removing it from the list.
		/// </summary>
		[SerializeField]
		private BaseCockpit currentCockpit;

		/// <summary>
		/// Place holding value that represents the current thrusters in the list.
		/// Used for when the existing thrusters is being replaced and removing it from the list.
		/// </summary>
		[SerializeField]
		private BaseThrusters currentThrusters;

		/// <summary>
		/// Place holding value that represents the current wings in the list.
		/// Used for when the existing wings is being replaced and removing it from the list.
		/// </summary>
		[SerializeField]
		private BaseWings currentWings;

		/// <summary>
		/// The cockpit 1.
		/// </summary>
		public GameObject cockpitOne;

		/// <summary>
		/// The cockpit 2.
		/// </summary>
		public GameObject cockpitTwo;

		/// <summary>
		/// The cockpit 3.
		/// </summary>
		public GameObject cockpitThree;
		
		private GameObject builtCockpit;

		public GameObject thrustersOne;
		public GameObject thrustersTwo;
		public GameObject thrustersThree;

		private GameObject builtThrusters;

		/// <summary>
		/// The wings 1.
		/// </summary>
		public GameObject wingsOne;

		/// <summary>
		/// The wings 2.
		/// </summary>
		public GameObject wingsTwo;

		/// <summary>
		/// The wings 3.
		/// </summary>
		public GameObject wingsThree;
		
		private GameObject builtWings;

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
			return this.allParts.OfType<BaseCockpit>().Any() &&
			       this.allParts.OfType<BaseThrusters>().Any() &&
			       this.allParts.OfType<BaseWings>().Any();
		}

		/// <summary>
		/// Function for adding and replacing parts in a list.
		/// Step 1) Checks the integer values that refer to the player's current amount of steel and fuel against the integer values of the object that represent its cost
		/// Step 2) Checks the type of the selected object that uses the interface IRocketParts.
		/// Step 3-1) Checks if an object of that type doesn't exists at any index of the list.
		/// Step 4) If Step 3-1 returns true:
		/// Adds the object to the list
		/// Increments the Rocket class's integer value total quality based on the object's integer value quality
		/// Decrements the values of the player's steel and fuel based on the object's steel and fuel cost values
		/// Sets an object to be equal to the selected object as current
		/// Exits the function
		/// Step 3-2) Else if Step 3-1 returns false, then check if an object of the same type exists in the list at any index and if the list doesn't contain the selected object
		/// Step 5) If Step 3-2 returns true:
		/// Decrement the Rocket's total quality based on the quality value the current object in the list,
		/// Remove the current object from the list,
		/// And repeat from the start until it meets a condition to exit the function
		/// </summary>
		/// <param name="selectedPart">
		/// The selected part.
		/// The object that the player is attempting to add to the list that will be checked.
		/// </param>
		public bool AddPart(IRocketParts selectedPart)
		{
			var spareList = this.allParts;
			if (User.SteelCount >= selectedPart.SteelCost && User.FuelCount >= selectedPart.FuelCost)
			{
				if (selectedPart is BaseCockpit)
				{
					if(selectedPart.Name == currentCockpit.Name)
					{
						return false;
					}
					else if (!spareList.OfType<BaseCockpit>().Any())
					{
						this.allParts.Add(selectedPart);
						this.totalQuality += selectedPart.Quality;
						User.SteelCount -= selectedPart.SteelCost;
						User.FuelCount -= selectedPart.FuelCost;
						this.currentCockpit = selectedPart as BaseCockpit;
					}
					else if (spareList.OfType<BaseCockpit>().Any() && !spareList.Contains(selectedPart))
					{
						this.totalQuality -= this.currentCockpit.Quality;
						this.allParts.Remove(this.currentCockpit);
						this.AddPart(selectedPart);
					}
				}
				else if (selectedPart is BaseThrusters)
				{
					if (selectedPart.Name == currentThrusters.Name)
					{
						return false;
					}
					else if (!spareList.OfType<BaseThrusters>().Any())
					{
						this.allParts.Add(selectedPart);
						this.totalQuality += selectedPart.Quality;
						User.SteelCount -= selectedPart.SteelCost;
						User.FuelCount -= selectedPart.FuelCost;
						this.currentThrusters = selectedPart as BaseThrusters;
					}
					else if (spareList.OfType<BaseThrusters>().Any() && !spareList.Contains(selectedPart))
					{
						this.totalQuality -= this.currentThrusters.Quality;
						this.allParts.Remove(this.currentThrusters);
						this.AddPart(selectedPart);
					}
				}
				else if (selectedPart is BaseWings)
				{
					if(selectedPart.Name == currentWings.Name)
					{
						return false;
					}
					else if (!spareList.OfType<BaseWings>().Any())
					{
						this.allParts.Add(selectedPart);
						this.totalQuality += selectedPart.Quality;
						User.SteelCount -= selectedPart.SteelCost;
						User.FuelCount -= selectedPart.FuelCost;
						this.currentWings = selectedPart as BaseWings;
					}
					else if (spareList.OfType<BaseWings>().Any() && !spareList.Contains(selectedPart))
					{
						this.totalQuality -= this.currentWings.Quality;
						this.allParts.Remove(this.currentWings);
						this.AddPart(selectedPart);
					}
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Use this for initialization
		/// </summary>
		private void Start()
		{
			//User.SteelCount = 9000;
			//User.FuelCount = 9000;
			this.allParts = new List<IRocketParts>();
		}

		/// <summary>
		/// Update is called once per frame
		/// </summary>
		private void TestRocket()
		{
			if (Input.GetMouseButtonDown(0))
			{
				this.CreateCockpit1();
			}

			if (Input.GetMouseButtonDown(1))
			{
				this.CreateCockpit2();
			}

			if (Input.GetKeyDown(KeyCode.Keypad0))
			{
				this.CreateCockpit3();
			}

			if (Input.GetKeyDown(KeyCode.Keypad1))
			{
				this.CreateThrusters1();
			}

			if (Input.GetKeyDown(KeyCode.Keypad2))
			{
				this.CreateThrusters2();
			}

			if (Input.GetKeyDown(KeyCode.Keypad3))
			{
				this.CreateThrusters3();
			}

			if (Input.GetKeyDown(KeyCode.Keypad4))
			{
				this.CreateWings1();
			}

			if (Input.GetKeyDown(KeyCode.Keypad5))
			{
				this.CreateWings2();
			}

			if (Input.GetKeyDown(KeyCode.Keypad6))
			{
				this.CreateWings3();
			}

			if (Input.GetKeyDown(KeyCode.Keypad7))
			{
				this.BuildRocket();
			}
		}
		
		private void BuildParts(IRocketParts thePart, GameObject selectedPart, List<GameObject> building)
		{
			if (thePart is BaseCockpit)
			{
				if (!building.Contains(builtCockpit))
				{
					builtCockpit = (GameObject)Instantiate(selectedPart) as GameObject;
					builtCockpit.transform.parent = transform;
					builtCockpit.transform.localPosition = new Vector3(-2, 0, 5);
					Cockpit behaviour = builtCockpit.AddComponent<Cockpit>();
					behaviour.Create(thePart as BaseCockpit);
					builtCockpit.name = thePart.Name;
					building.Add(builtCockpit);
				}
				else if (building.Contains(builtCockpit))
				{
					building.Remove(builtCockpit);
					Destroy(builtCockpit);
					BuildParts(thePart, selectedPart, building);
				}
			}
			else if (thePart is BaseThrusters)
			{
				if (!building.Contains(builtThrusters))
				{
					builtThrusters = (GameObject)Instantiate(selectedPart) as GameObject;
					builtThrusters.transform.parent = transform;
					builtThrusters.transform.localPosition = new Vector3(-8, 0, 5);
					Thrusters behaviour = builtThrusters.AddComponent<Thrusters>();
					behaviour.Create(thePart as BaseThrusters);
					builtThrusters.name = thePart.Name;
					building.Add(builtThrusters);
				}
				else if (building.Contains(builtThrusters))
				{
					building.Remove(builtThrusters);
					Destroy(builtThrusters);
					BuildParts(thePart, selectedPart, building);
				}
			}
			else if (thePart is BaseWings)
			{
				if (!building.Contains(builtWings))
				{
					builtWings = (GameObject)Instantiate(selectedPart) as GameObject;
					builtWings.transform.parent = transform;
					builtWings.transform.localPosition = new Vector3(5, -4, 4);
					Wings behaviour = builtWings.AddComponent<Wings>();
					behaviour.Create(thePart as BaseWings);
					builtWings.name = thePart.Name;
					building.Add(builtWings);
				}
				else if (building.Contains(builtWings))
				{
					building.Remove(builtWings);
					Destroy(builtWings);
					BuildParts(thePart, selectedPart, building);
				}
			}
		}

		private void AssembleParts(IRocketParts thePart, GameObject selectedPart)
		{
			if (AddPart(thePart) == true)
			{
				BuildParts(thePart, selectedPart, builtParts);
			}
		}

		public void CreateCockpit1()
		{
			var cp = new BaseCockpit(20, 200, 0, 20, "Cockpit Rust");
			AssembleParts(cp, cockpitOne);
		}
		public void CreateCockpit2()
		{
			var cp = new BaseCockpit(30, 200, 0, 50, "Cockpit Color");
			AssembleParts(cp, cockpitTwo);
		}
		public void CreateCockpit3()
		{
			var cp = new BaseCockpit(40, 200, 0, 80, "Cockpit Flame");
			AssembleParts(cp, cockpitThree);
		}
 

		public void CreateThrusters1()
		{
			BaseThrusters thruster = new BaseThrusters(200, 50, 20, "Thrusters Rust");
			AssembleParts(thruster, thrustersOne);
		}
		public void CreateThrusters2()
		{
			BaseThrusters thruster = new BaseThrusters(200, 50, 50, "Thrusters Color");
			AssembleParts(thruster, thrustersTwo);
		}
		public void CreateThrusters3()
		{
			BaseThrusters thruster = new BaseThrusters(200, 50, 80, "Thrusters Flame");
			AssembleParts(thruster, thrustersThree);
		}

		public void CreateWings1()
		{
			BaseWings wing = new BaseWings(200, 0, 20, "Wings Rust");
			AssembleParts(wing, wingsOne);
		}
		public void CreateWings2()
		{
			BaseWings wing = new BaseWings(200, 0, 50, "Wings Color");
			AssembleParts(wing, wingsTwo);
		}
		public void CreateWings3()
		{
			BaseWings wing = new BaseWings(200, 0, 80, "Wings Flame");
			AssembleParts(wing, wingsThree);
		}

		public void BuildRocket()
		{
			if(this.ShipBuild() == true)
			{
				builtWings.transform.parent = builtCockpit.transform;
				builtWings.transform.localPosition = new Vector3(0, 0, 0);
				builtThrusters.transform.parent = builtCockpit.transform;
				if(builtCockpit.name == "Cockpit Rust")
				{
					builtWings.transform.GetChild(0).transform.localPosition = new Vector3(1.6f, -0.15f, 1.9f);
					builtWings.transform.GetChild(1).transform.localPosition = new Vector3(-1.6f, -0.15f, -1.9f);
					builtWings.transform.GetChild(2).transform.localPosition = new Vector3(-1.6f, -0.15f, 1.9f);
					builtWings.transform.GetChild(3).transform.localPosition = new Vector3(1.6f, -0.15f, -1.9f);

					builtWings.transform.GetChild(0).transform.eulerAngles = new Vector3(0, -45, 0);
					builtWings.transform.GetChild(1).transform.eulerAngles = new Vector3(0, 135, 0);
					builtWings.transform.GetChild(2).transform.eulerAngles = new Vector3(0, -135, 0);
					builtWings.transform.GetChild(3).transform.eulerAngles = new Vector3(0, 45, 0);

					if (builtThrusters.name == "Thrusters Rust")
					{
						builtThrusters.transform.localPosition = new Vector3(0, -4.35f, 0);
					}
					else if(builtThrusters.name == "Thrusters Color")
					{
						builtThrusters.transform.localPosition = new Vector3(0, -4.4f, 0);
					}
					else if (builtThrusters.name == "Thrusters Flame")
					{
						builtThrusters.transform.localPosition = new Vector3(0, -4.6f, 0);
					}
				}
				else if(builtCockpit.name == "Cockpit Color")
				{
					builtWings.transform.GetChild(0).transform.localPosition = new Vector3(2.07f, -1.8f, 1);
					builtWings.transform.GetChild(1).transform.localPosition = new Vector3(-2.07f, -1.8f, -1);
					builtWings.transform.GetChild(2).transform.localPosition = new Vector3(-2.07f, -1.8f, 1);
					builtWings.transform.GetChild(3).transform.localPosition = new Vector3(2.07f, -1.8f, -1);

					builtWings.transform.GetChild(0).transform.eulerAngles = new Vector3(0, -25.4f, -9.42f);
					builtWings.transform.GetChild(1).transform.eulerAngles = new Vector3(0, -199, -9.42f);
					builtWings.transform.GetChild(2).transform.eulerAngles = new Vector3(0, 199, -9.42f);
					builtWings.transform.GetChild(3).transform.eulerAngles = new Vector3(0, 25.4f, -9.42f);

					if (builtThrusters.name == "Thrusters Rust")
					{
						builtThrusters.transform.localPosition = new Vector3(0, -4.5f, 0);
					}
					else if (builtThrusters.name == "Thrusters Color")
					{
						builtThrusters.transform.localPosition = new Vector3(0, -4.79f, 0);
					}
					else if (builtThrusters.name == "Thrusters Flame")
					{
						builtThrusters.transform.localPosition = new Vector3(0, -4.79f, 0);
					}
				}
				else if(builtCockpit.name == "Cockpit Flame")
				{
					builtWings.transform.GetChild(0).transform.localPosition = new Vector3(2.5f, -1.7f, 0);
					builtWings.transform.GetChild(1).transform.localPosition = new Vector3(-2.5f, -1.7f, 0);
					builtWings.transform.GetChild(2).transform.localPosition = new Vector3(0, -1.7f, 2.5f);
					builtWings.transform.GetChild(3).transform.localPosition = new Vector3(0, -1.7f, -2.5f);

					builtWings.transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, -14);
					builtWings.transform.GetChild(1).transform.eulerAngles = new Vector3(0, 180, -14);
					builtWings.transform.GetChild(2).transform.eulerAngles = new Vector3(0, -90, -14);
					builtWings.transform.GetChild(3).transform.eulerAngles = new Vector3(0, 90, -14f);

					if (builtThrusters.name == "Thrusters Rust")
					{
						builtThrusters.transform.localPosition = new Vector3(0, -5.1f, 0);
					}
					else if (builtThrusters.name == "Thrusters Color")
					{
						builtThrusters.transform.localPosition = new Vector3(0, -5.15f, 0);
					}
					else if (builtThrusters.name == "Thrusters Flame")
					{
						builtThrusters.transform.localPosition = new Vector3(0, -5.3f, 0);
					}
				}
				//Assets.Scripts.Managers.GameManager.Instance.HasBuiltShip = true;
				//Assets.Scripts.Managers.GameManager.Instance.CheckForWin();
				Debug.Log("You Win!");
			}
			else
			{
				Debug.Log("Nope");
			}
		}
	}
}
