using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Text = UnityEngine.UI.Text;
using Assets.Scripts.Managers;
using Assets.Scripts;
using Assets.Scripts.Controllers;
using Assets.Scripts.Interfaces;

public class UIMenu : MonoBehaviour {

    private RectTransform m_OptionsUI;
    private RectTransform m_SettingsUI;
    private RectTransform m_CustomizeUI;
    private RectTransform m_ActionsTAB;
    private RectTransform m_CraftingTAB;
    private RectTransform m_Workshop;
    private RectTransform m_ObjectiveUI;
    private RectTransform m_MainUI;

    private bool objectiveinview;

    void Awake()
    {
        Assets.Scripts.Managers.EventManager.Subscribe("NewGame", this.NewGame);
        Assets.Scripts.Managers.EventManager.Subscribe("Options Menu", this.OnOptions);
        Assets.Scripts.Managers.EventManager.Subscribe("Instructions", this.OnInstructions);
        Assets.Scripts.Managers.EventManager.Subscribe("QuitGame", this.OnQuitGame);
        Assets.Scripts.Managers.EventManager.Subscribe("Close Options", this.CloseOptions);
        Assets.Scripts.Managers.EventManager.Subscribe("Settings", this.OnSettings);
        Assets.Scripts.Managers.EventManager.Subscribe("SettingsClose", this.OnSettingsClose);
        Assets.Scripts.Managers.EventManager.Subscribe("Customize", this.OnCustomize);
        Assets.Scripts.Managers.EventManager.Subscribe("QuitToMenu", this.OnQuitToMenu);
        Assets.Scripts.Managers.EventManager.Subscribe("VolumeSlider", this.OnVolumeSlider);
        Assets.Scripts.Managers.EventManager.Subscribe("CameraSpeedSlider", this.OnCameraSpeedSlider);
        Assets.Scripts.Managers.EventManager.Subscribe("CustomizeClose", this.OnCustomizeClose);
        Assets.Scripts.Managers.EventManager.Subscribe("CustomizeRestore", this.OnCustomRestore);
        Assets.Scripts.Managers.EventManager.Subscribe("ObjectiveClick", this.OnObjective);
    }

    void Start()
    {

    }

    private void NewGame()
    {
        SceneManager.LoadScene(1);
        //Function will begin game from main menu
        Debug.Log("New Game");
    }

    private void OnOptions()
    {
        m_OptionsUI.gameObject.SetActive(true);
        m_SettingsUI.gameObject.SetActive(false);
        Debug.Log("Options Menu");
    }

    private void OnInstructions()
    {
        //Function will bring up the instructions.
        Debug.Log("Instructions");
    }

    private void OnQuitGame()
    {
        Application.Quit();
        //Function will quit game upon click.
        Debug.Log("Quit Game");
    }

    private void OnSettingsClose()
    {
        m_SettingsUI.gameObject.SetActive(false);
        Debug.Log("Settings Close");
    }

    private void OnCustomizeClose()
    {
        //Used to set all UI to active when the customize menu is open.
        m_CustomizeUI.gameObject.SetActive(false);
        m_OptionsUI.gameObject.SetActive(true);
        m_MainUI.gameObject.SetActive(true);
        m_ActionsTAB.gameObject.SetActive(true);
        m_CraftingTAB.gameObject.SetActive(true);
        m_Workshop.gameObject.SetActive(true);
        m_ObjectiveUI.gameObject.SetActive(true);
        Debug.Log("Customize closed");
    }


    private void OnVolumeSlider()
    {
        //Changes the volume number text on the slider
        m_OptionsUI.GetComponentsInChildren<Text>()[2].text = "Audio Volume";
        Debug.Log("Volume Slider");
    }

    private void OnCameraSpeedSlider()
    {
        //Changes the camera speed text on the slider
        Assets.Scripts.Controllers.CameraController.MoveSpeed = (uint)m_OptionsUI.GetComponentsInChildren<Slider>()[1].value;
        m_OptionsUI.GetComponentsInChildren<Text>()[2].text = "Camera Speed: " + Assets.Scripts.Controllers.CameraController.MoveSpeed;
        Debug.Log("CameraSpeed Slider");
    }

    private void OnSettings()
    {
        //Enables the settings panel to pop up when button is clicked.
        m_SettingsUI.gameObject.SetActive(true);
        Debug.Log("Settings Menu");
    }

    private void CloseOptions()
    {
        //Sets the options panel to false when the back button is clicked.
        m_OptionsUI.gameObject.SetActive(false);
        m_SettingsUI.gameObject.SetActive(true);
        Debug.Log("Close Options");
    }

    private void OnQuitToMenu()
    {
        SceneManager.LoadScene(0);
        Debug.Log("Quit to Menu");
    }

    private void OnCustomize()
    {
        //Used to set all the UI to inactive when the customize menu is open.
        m_CustomizeUI.gameObject.SetActive(true);
        m_OptionsUI.gameObject.SetActive(false);
        m_MainUI.gameObject.SetActive(false);
        m_ActionsTAB.gameObject.SetActive(false);
        m_CraftingTAB.gameObject.SetActive(false);
        m_Workshop.gameObject.SetActive(false);
        m_ObjectiveUI.gameObject.SetActive(false);

        Debug.Log("Customize Menu");
    }

    private void OnObjective()
    {
        this.objectiveinview = !objectiveinview;
    }

    private void OnCustomRestore()
    {
        KeyBind.Self.RestoreToDefault(m_CustomizeUI.GetComponentsInChildren<Button>());
    }
}
