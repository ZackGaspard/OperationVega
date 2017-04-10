
namespace Assets.Scripts
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	using Assets.Scripts.BaseClasses;

	using UnityEngine;

	/// <summary>
	/// The thrusters class.
	/// </summary>
	public class Thrusters : MonoBehaviour
	{
		/// <summary>
		/// The accessed.
		/// </summary>
		[SerializeField]
		private BaseThrusters Accessed;
		public void Create(BaseThrusters t)
		{
			Accessed = t;
		}
	}
}