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
       EventManager.Subscribe("NewGame", this.NewGame);
       EventManager.Subscribe("Options Menu", this.OnOptions);
       EventManager.Subscribe("Instructions", this.OnInstructions);
       EventManager.Subscribe("QuitGame", this.OnQuitGame);
       EventManager.Subscribe("Close Options", this.CloseOptions);
       EventManager.Subscribe("Settings", this.OnSettings);
       EventManager.Subscribe("SettingsClose", this.OnSettingsClose);
       EventManager.Subscribe("Customize", this.OnCustomize);
       EventManager.Subscribe("QuitToMenu", this.OnQuitToMenu);
       EventManager.Subscribe("VolumeSlider", this.OnVolumeSlider);
       EventManager.Subscribe("CameraSpeedSlider", this.OnCameraSpeedSlider);
       EventManager.Subscribe("CustomizeClose", this.OnCustomizeClose);
       EventManager.Subscribe("CustomizeRestore", this.OnCustomRestore);
       EventManager.Subscribe("ObjectiveClick", this.OnObjective);
    }

    void OnDestroy()
    {
       EventManager.UnSubscribe("NewGame", this.NewGame);
       EventManager.UnSubscribe("Options Menu", this.OnOptions);
       EventManager.UnSubscribe("Instructions", this.OnInstructions);
       EventManager.UnSubscribe("QuitGame", this.OnQuitGame);
       EventManager.UnSubscribe("Close Options", this.CloseOptions);
       EventManager.UnSubscribe("Settings", this.OnSettings);
       EventManager.UnSubscribe("SettingsClose", this.OnSettingsClose);
       EventManager.UnSubscribe("Customize", this.OnCustomize);
       EventManager.UnSubscribe("QuitToMenu", this.OnQuitToMenu);
       EventManager.UnSubscribe("VolumeSlider", this.OnVolumeSlider);
       EventManager.UnSubscribe("CameraSpeedSlider", this.OnCameraSpeedSlider);
       EventManager.UnSubscribe("CustomizeClose", this.OnCustomizeClose);
       EventManager.UnSubscribe("CustomizeRestore", this.OnCustomRestore);
       EventManager.UnSubscribe("ObjectiveClick", this.OnObjective);
    }
    void Start()
    {

    }
    public void NewGameClick()
    {
        EventManager.Publish("NewGame");
    }
    private void NewGame()
    {
        SceneManager.LoadScene(1);
        //Function will begin game from main menu
        Debug.Log("New Game");
    }
    public void OnOptionsClick()
    {
        EventManager.Publish("Options Menu");
    }
    private void OnOptions()
    {
        m_OptionsUI.gameObject.SetActive(true);
        m_SettingsUI.gameObject.SetActive(false);
        Debug.Log("Options Menu");
    }
    public void OnInstructionsClick()
    {
        EventManager.Publish("Instructions");
    }

    private void OnInstructions()
    {
        //Function will bring up the instructions.
        Debug.Log("Instructions");
    }
    public void OnQuitGameClick()
    {
        EventManager.Publish("QuitGame");
    }

    private void OnQuitGame()
    {
        Application.Quit();
        //Function will quit game upon click.
        Debug.Log("Quit Game");
    }
    public void OnSettingsCloseClick()
    {
        EventManager.Publish("SettingsClose");
    }

    private void OnSettingsClose()
    {
        m_SettingsUI.gameObject.SetActive(false);
        Debug.Log("Settings Close");
    }
    public void OnCustomizeCloseClick()
    {
        EventManager.Publish("CustomizeClose");
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

    public void OnVolumeSliderClick()
    {
        EventManager.Publish("VolumeSlider");
    }

    private void OnVolumeSlider()
    {
        //Changes the volume number text on the slider
        m_OptionsUI.GetComponentsInChildren<Text>()[2].text = "Audio Volume";
        Debug.Log("Volume Slider");
    }

    public void OnCameraSpeedSliderClick()
    {
        EventManager.Publish("CameraSpeedSlider");
    }

    private void OnCameraSpeedSlider()
    {
        //Changes the camera speed text on the slider
        Assets.Scripts.Controllers.CameraController.MoveSpeed = (uint)m_OptionsUI.GetComponentsInChildren<Slider>()[1].value;
        m_OptionsUI.GetComponentsInChildren<Text>()[2].text = "Camera Speed: " + Assets.Scripts.Controllers.CameraController.MoveSpeed;
        Debug.Log("CameraSpeed Slider");
    }
    public void OnSettingsClick()
    {
        EventManager.Publish("Settings");
    }

    private void OnSettings()
    {
        //Enables the settings panel to pop up when button is clicked.
        m_SettingsUI.gameObject.SetActive(true);
        Debug.Log("Settings Menu");
    }
    public void CloseOptionsClick()
    {
        EventManager.Publish("Close Options");
    }

    private void CloseOptions()
    {
        //Sets the options panel to false when the back button is clicked.
        m_OptionsUI.gameObject.SetActive(false);
        m_SettingsUI.gameObject.SetActive(true);
        Debug.Log("Close Options");
    }
    public void OnQuitToMenuClick()
    {
        EventManager.Publish("QuitToMenu");
    }

    private void OnQuitToMenu()
    {
        SceneManager.LoadScene(0);
        Debug.Log("Quit to Menu");
    }
    public void OnCustomizeClick()
    {
        EventManager.Publish("Customize");
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
    public void OnObjectiveClick()
    {
        EventManager.Publish("ObjectiveClick");

    }

    private void OnObjective()
    {
        this.objectiveinview = !objectiveinview;
    }

    public void OnCustomRestoreClick()
    {
        EventManager.Publish("CustomizeRestore");

    }

    private void OnCustomRestore()
    {
        KeyBind.Self.RestoreToDefault(m_CustomizeUI.GetComponentsInChildren<Button>());
    }
}
