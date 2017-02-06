using UnityEngine;

namespace Resource
{
	public interface IResources
	{
		int Count { get; set; }
		bool Renewable { get; set; }
		bool Taint { get; set; }

		int Refresh(int iValue, bool bCheck);
		void Reset(); 
	}
}