using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Assets.Scripts.Managers;
using Assets.Scripts;

class TestRocketUI : MonoBehaviour
{
	private List<string> cockpitClickEvents;
	private List<string> wingsClickEvents;
	private List<string> thrusterClickEvents;
	[SerializeField]
	public Rocket rocketFactory;

	public string save;

	void Awake()
	{
		rocketFactory = FindObjectOfType<Rocket>();
		if (rocketFactory == null)
			throw new NotImplementedException();
	}
	
	void Start()
	{
		UnityAction ua = delegate { Debug.Log("do it"); };
		cockpitClickEvents = new List<string>() { "Player chose CP1", "Player chose CP2", "Player chose CP3" };
		wingsClickEvents = new List<string>() { "Player chose WC1", "Player chose WC2", "Player chose WC3" };
		thrusterClickEvents = new List<string>() { "Player chose TC1", "Player chose TC2", "Player chose TC3" };

		
		EventManager.Subscribe(cockpitClickEvents[0], rocketFactory.CreateCockpit1);
		EventManager.Subscribe(cockpitClickEvents[1], rocketFactory.CreateCockpit2);
		EventManager.Subscribe(cockpitClickEvents[2], rocketFactory.CreateCockpit3);

		EventManager.Subscribe(wingsClickEvents[0], rocketFactory.CreateWings1);
		EventManager.Subscribe(wingsClickEvents[1], rocketFactory.CreateWings2);
		EventManager.Subscribe(wingsClickEvents[2], rocketFactory.CreateWings3);


		EventManager.Subscribe(thrusterClickEvents[0], rocketFactory.CreateThrusters1);
		EventManager.Subscribe(thrusterClickEvents[1], rocketFactory.CreateThrusters2);
		EventManager.Subscribe(thrusterClickEvents[2], rocketFactory.CreateThrusters3);		

		EventManager.Subscribe("Build Rocket", rocketFactory.BuildRocket);
		//EventManager.Subscribe("Build Rocket", Save);

		User.SteelCount = 9000;
		User.FuelCount = 9000;
		User.GasCount = 9000;
	}

	void Save()
	{
		save = JsonUtility.ToJson(rocketFactory);
	}
}

