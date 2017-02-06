
namespace Assets.Scripts
{
	using Assets.Scripts.Interfaces;

	using UnityEngine;

	/// <summary>
	/// The gas.
	/// </summary>
	public class Gas : MonoBehaviour, IResources
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

		/// <summary>
		/// The refresh.
		/// </summary>
		/// <param name="i">
		/// The i.
		/// </param>
		/// <param name="b">
		/// The b.
		/// </param>
		/// <returns>
		/// The <see cref="int"/>.
		/// </returns>
		public int Refresh(int i, bool b)
		{
			var timer = 0.0f;

			if (i < 100 && b == true)
			{
				timer += Time.fixedDeltaTime;

				if (timer >= 15.0f)
				{
					i = this.maxAmount;
					timer = 0.0f;
				}
			}

			return i;
		}

		/// <summary>
		/// The reset.
		/// </summary>
		public void Reset()
		{
			const float Timer = 0.0f;
			if (Timer >= 60.0f)
			{
				this.state = false;
			}
		}

		/// <summary>
		/// Use this for initialization
		/// </summary>
		private void Start()
		{
			this.maxAmount = 999;
			this.amount = this.maxAmount;
			this.refill = true;
			this.state = false;
		}

		/// <summary>
		/// Update is called once per frame
		/// </summary>
		private void Update()
		{

		}

	}
}