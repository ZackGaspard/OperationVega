using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Resource;

public class Food : MonoBehaviour, IResources
{
	int iMaxAmount;
	int iAmount;
	bool bRefill;
	bool bState;

	public int Count
	{
		get { return iAmount; }
		set { iAmount = value; }
	}

	public bool Renewable
	{
		get { return bRefill; }
		set { bRefill = value; }
	}

	public bool Taint
	{
		get { return bState; }
		set { bState = value; }
	}

	// Use this for initialization
	void Start ()
	{
		iMaxAmount = 150;
		iAmount = iMaxAmount;
		bRefill = true;
		bState = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public int Refresh(int i, bool b)
	{
		float fTimer = 0.0f;
		if (i < iMaxAmount && b == true)
		{
			fTimer += Time.fixedDeltaTime;
			if (fTimer >= 15.0f)
			{
				i += 15;
				fTimer = 0.0f;
			}
		}
		return i;
	}

	public void Reset()
	{
		float fTimer = 0.0f;
		if(fTimer >= 60.0f)
		{
			bState = false;
		}
	}
}
