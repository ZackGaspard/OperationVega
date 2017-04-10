using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIPublishButtonClick : MonoBehaviour {
    public Button button;
    public string message;
    
    void Awake()
    {
        button = GetComponent<Button>();        
    }

    void Start()
    {        
        button.onClick.AddListener(Broadcast);
    }

    void Broadcast()
    {
        Assets.Scripts.Managers.EventManager.Publish(message);
    }
}
