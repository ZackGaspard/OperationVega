

namespace Assets.Scripts
{
	using System.Collections;
	using System.Collections.Generic;

	using Assets.Scripts.Interfaces;

	using UnityEngine;

	/// <summary>
	/// The minerals.
	/// </summary>
	public class Minerals : MonoBehaviour, IResources
	{
		/// <summary>
		/// The i max amount.
		/// </summary>
		private int maxAmount;

		/// <summary>
		/// The amount.
		/// </summary>
		private int amount;

		/// <summary>
		/// The b refill.
		/// </summary>
		private bool refill;

		/// <summary>
		/// The b state.
		/// </summary>
		private bool state;

		/// <summary>
		/// Gets or sets the count.
		/// </summary>
		public int Count
		{
			get
			{
				return this.amount;
			}

			set
			{
				this.amount = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether renewable.
		/// </summary>
		public bool Renewable
		{
			get
			{
				return this.refill;
			}
			set
			{
				this.refill = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether taint.
		/// </summary>
		public bool Taint
		{
			get
			{
				return this.state;
			}

			set
			{
				this.state = value;
			}
		}

		public int Refresh(int i, bool b)
		{
			var timer = 0.0f;
			if (i < this.maxAmount && b == true)
			{
				timer += Time.fixedDeltaTime;
				if (timer >= 15.0f)
				{
					i += 15;
					timer = 0.0f;
				}
			}
			return i;
		}

		public void Reset()
		{
			var timer = 0.0f;
			if (timer >= 60.0f)
			{
				state = false;
			}
		}

		/// <summary>
		/// Use this for initialization
		/// </summary>
		private void Start()
		{
			this.maxAmount = 250;
			this.amount = this.maxAmount;
			this.refill = false;
			this.state = false;
		}

		// Update is called once per frame
		private void Update()
		{

		}
	}
}