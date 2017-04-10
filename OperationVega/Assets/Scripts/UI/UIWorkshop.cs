using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;

public class UIWorkshop : MonoBehaviour {


    private RectTransform m_WorkshopUI;

    [SerializeField]
    private RectTransform m_ThrusterChoice;
    [SerializeField]
    private RectTransform m_CockpitChoice;
    [SerializeField]
    private RectTransform m_WingChoice;


    bool undo1;
    bool undo2;
    bool undo3;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnBuildClick()
    {
        EventManager.Publish("Build Rocket");
    }
    private void OnBuild()
    {
        //Function that will craft the ship when all parts are obtained.
        Debug.Log("Build Rocket");
    }
    public void OnThrustersClick()
    {
        EventManager.Publish("Thrusters");
    }
    private void OnThrusters()
    {
        if (undo1)
        {
            m_ThrusterChoice.gameObject.SetActive(true);

            undo1 = false;
        }
        else if (!undo1)
        {
            m_ThrusterChoice.gameObject.SetActive(false);
            undo1 = true;
        }

        //Function that will apply the selected thruster on the ship
        Debug.Log("Apply Thrusters");
    }
    public void OnChassisClick()
    {
        EventManager.Publish("Chassis");
    }
    private void OnChassis()
    {

        //Function that will apply the selected chassis on the ship
        Debug.Log("Apply Chassis");
    }
    public void OnCockpitClick()
    {
        EventManager.Publish("Cockpit");
    }
    private void OnCockpit()
    {
        if (undo2)
        {
            m_CockpitChoice.gameObject.SetActive(true);

            undo2 = false;
        }
        else if (!undo2)
        {
            m_CockpitChoice.gameObject.SetActive(false);

            undo2 = true;
        }
        //Function that will apply the selected cockpit on the ship
        Debug.Log("Apply Cockpit");
    }
    public void OnWingsClick()
    {

        EventManager.Publish("Apply Wings");
    }
    private void OnWings()
    {
        if (undo3)
        {
            m_WingChoice.gameObject.SetActive(true);

            undo3 = false;
        }
        else if (!undo3)
        {
            m_WingChoice.gameObject.SetActive(false);

            undo3 = true;
        }
        //Function that will apply the selected wing on the ship
        Debug.Log("Apply Wings");
    }
    public void OnWC1Click()
    {
        EventManager.Publish("Player chose WC1");
    }
    private void OnWC1()
    {
        Debug.Log("Player chose WC1");
    }
    public void OnWC2Click()
    {
        EventManager.Publish("Player chose WC2");

    }
    private void OnWC2()
    {
        Debug.Log("Player chose WC2");
    }
    public void OnWC3Click()
    {
        EventManager.Publish("Player chose WC3");
    }
    private void OnWC3()
    {
        Debug.Log("Player chose WC3");
    }
    public void OnCP1Click()
    {
        EventManager.Publish("Player chose CP1");
    }
    private void OnCP1()
    {
        Debug.Log("Player chose Cockpit1");
    }
    public void OnCP2Click()
    {
        EventManager.Publish("Player chose CP2");
    }
    private void OnCP2()
    {
        Debug.Log("Player chose Cockpit2");
    }
    public void OnCP3Click()
    {
        EventManager.Publish("Player chose CP3");
    }
    private void OnCP3()
    {
        Debug.Log("Player chose Cockpit3");
    }
    public void OnTC1Click()
    {
        EventManager.Publish("Player chose TC1");
    }
    private void OnTC1()
    {
        Debug.Log("Player chose Thrust1");
    }
    public void OnTC2Click()
    {
        EventManager.Publish("Player chose TC2");
    }
    private void OnTC2()
    {
        Debug.Log("Player chose Thrust2");
    }
    public void OnTC3Click()
    {
        EventManager.Publish("Player chose TC3");
    }
    private void OnTC3()
    {
        Debug.Log("Player chose Thrust3");
    }
}
