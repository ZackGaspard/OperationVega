
namespace Assets.Scripts
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	using Assets.Scripts.BaseClasses;

	using UnityEngine;

	/// <summary>
	/// The cockpit  class.
	/// </summary>
	public class Cockpit : MonoBehaviour
	{
		/// <summary>
		/// The accessed.
		/// </summary>
		[SerializeField]
		private BaseCockpit Accessed;
		public void Create(BaseCockpit c)
		{
			Accessed = c;
		}
	}
}