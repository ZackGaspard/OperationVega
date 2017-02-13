
namespace Assets.Scripts
{
	using System.Collections.Generic;
	using System.Linq;


	using Assets.Scripts.Interfaces;

	using UnityEngine;

	/// <summary>

	/// The rocket.

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
		private int totalQuality;

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
		public int FullQuality
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
		/// Use this for initialization
		/// </summary>
		private void Start()
		{
			this.allParts = new List<IRocketParts>();
		}

		/// <summary>
		/// Update is called once per frame
		/// </summary>
		private void Update()
		{
			Debug.Log(this.ShipBuild());
			Debug.Log(this.allParts.Count);
		}
	}
}