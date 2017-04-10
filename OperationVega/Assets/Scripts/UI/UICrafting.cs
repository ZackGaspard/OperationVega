using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UICrafting : MonoBehaviour
{
    UI.UIManager uiManager;
    bool revertcraftingtab;
    RectTransform m_CraftingTAB;
    float Scalefactor;
   
    void Awake()
    {
        revertcraftingtab = true;
        uiManager = FindObjectOfType<UI.UIManager>();
        Assets.Scripts.Managers.EventManager.Subscribe("Craft", OnCrafting);
    }
    // Use this for initialization
    void Start()
    {

        m_CraftingTAB = uiManager.ActionsTab;
        Scalefactor = uiManager.UIScaleFactor;
        revertcraftingtab = uiManager.RevertCraftingTab;



    }


    private void OnCrafting()
    {
        //Moves the crafting tab up
        if (revertcraftingtab)
        {
            m_CraftingTAB.offsetMax = new Vector2(m_CraftingTAB.offsetMax.x, 0);
            m_CraftingTAB.offsetMin = new Vector2(m_CraftingTAB.offsetMin.x, 0);

            revertcraftingtab = false;

        }

        //Sets the crafting tab to its original state
        else if (!revertcraftingtab)
        {
            revertcraftingtab = true;

            m_CraftingTAB.offsetMax = new Vector2(m_CraftingTAB.offsetMax.x, Scalefactor);
            m_CraftingTAB.offsetMin = new Vector2(m_CraftingTAB.offsetMin.x, -115);
        }


        Debug.Log("Changes Position of the Crafting Tab.");
    }

}
