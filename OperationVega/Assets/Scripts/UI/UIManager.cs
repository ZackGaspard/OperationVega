using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using UnityEngine.Events;
using Assets.Scripts.Managers;

namespace UI
{
    public class UIManager : MonoBehaviour
    {     
        #region -- VARIABLES --
        [SerializeField]
        private Button m_NewGame;
        [SerializeField]
        private Button m_LoadGame;
        [SerializeField]
        private Button m_QuitGame;
        [SerializeField]
        private Button m_Instructions;
        [SerializeField]
        private Button m_Options;
        [SerializeField]
        private Button m_Workshop;
        [SerializeField]
        private Button m_Craft;
        [SerializeField]
        private Button m_Clear;
        [SerializeField]
        private Button m_CancelAction;
        [SerializeField]
        private Canvas m_BackgroundUI;
        #endregion

        #region -- PROPERTIES --
        public Canvas BackgroundUI
        {
            get { return m_BackgroundUI; }
        }
        #endregion

        protected void Awake()
        {
            #region -- Ingame Subscribers --
            EventManager.Subscribe("Rally", this.OnRally);
            EventManager.Subscribe("Harvest", this.OnHarvest);
            EventManager.Subscribe("Recall", this.OnRecall);
            EventManager.Subscribe("CancelAction", this.OnCancelAction);
            EventManager.Subscribe("Workshop", this.OnWorkShop);
            EventManager.Subscribe("Craft", this.OnCraft);
            EventManager.Subscribe("Clear", this.OnClear);
            EventManager.Subscribe("Mine", this.OnMine);
            EventManager.Subscribe("Extract", this.OnExtract);
            #endregion

            #region -- Main Menu Subscribers --
            EventManager.Subscribe("NewGame", this.NewGame);
            EventManager.Subscribe("Options", this.OnOptions);
            EventManager.Subscribe("Instructions", this.OnInstructions);
            EventManager.Subscribe("QuitGame", this.OnQuitGame);
            #endregion

        }

        protected void OnDestroy()
        {
            #region -- Ingame Unsubscribers --
            EventManager.UnSubscribe("Rally", this.OnRally);
            EventManager.UnSubscribe("Harvest", this.OnHarvest);
            EventManager.UnSubscribe("Recall", this.OnRecall);
            EventManager.UnSubscribe("CancelAction", this.OnCancelAction);
            EventManager.UnSubscribe("Workshop", this.OnWorkShop);
            EventManager.UnSubscribe("Craft", this.OnCraft);
            EventManager.UnSubscribe("Clear", this.OnClear);
            EventManager.UnSubscribe("Mine", this.OnMine);
            EventManager.UnSubscribe("Extract", this.OnExtract);
            #endregion

            #region -- Main Menu Unsubscribers --
            EventManager.UnSubscribe("NewGame", this.NewGame);
            EventManager.UnSubscribe("Options", this.OnOptions);
            EventManager.UnSubscribe("Instructions", this.OnInstructions);
            EventManager.UnSubscribe("QuitGame", this.OnQuitGame);
            #endregion 
        }
        #region -- VOID FUNCTIONS --
        public void OnRallyClick()
        {
            EventManager.Publish("Rally");
        }
        public void OnRally()
        {            
            //Function will be use to rally upon click.
            Debug.Log("Begin Rallying ");
        }
        public void OnHarvestClick()
        {
            EventManager.Publish("Harvest");
        }
        public void OnHarvest()
        {           
            //Function will be use to harvest upon click.
            Debug.Log("Begin Harvesting");
        }
        public void OnRecallClick()
        {
            EventManager.Publish("Recall");
        }

        public void OnRecall()
        {            
            //Function will be use to recall upon click.
            Debug.Log("Recall to barracks.");
        }
        public void OnCancelActionClick()
        {
            EventManager.Publish("CancelAction");
        }
        public void OnCancelAction()
        {          
            //Function will cancel previous action upon click.
            Debug.Log("Cancel Previous Action");
        }

        public void OnCraftClick()
        {
            //This function will run the craft function
            EventManager.Publish("Craft");
        }
        void OnCraft()
        {
            Debug.Log("Craft");
        }
        public void OnClearClick()
        {
            EventManager.Publish("Clear");
        }
        public void OnClear()
        {
            //This function will clear items in the craft.
            Debug.Log("Clear");
        }
        public void OnWorkShopClick()
        {
            EventManager.Publish("Workshop");
        }
        public void OnWorkShop()
        {          
            //This function will bring up the workshop within the game.
            Debug.Log("Workshop");
        }
        public void OnMineClick()
        {
            EventManager.Publish("Mine");
        }
        public void OnMine()
        {
            //This function will prompt the mining function
            Debug.Log("Begin Mining");
        }
        public void OnExtractClick()
        {
            EventManager.Publish("Extract");
        }
        public void OnExtract()
        {
            Debug.Log("Begin Extracting");
        }
        #region -- Main Menu Functions --

        public void NewGameClick()
        {
            EventManager.Publish("NewGame");
        }
        public void NewGame()
        {            
            //Function will begin game from main menu
            Debug.Log("New Game");
        }

        public void OnOptionsClick()
        {
            EventManager.Publish("Options");
        }
        public void OnOptions()
        {         
            //Function will acess the options menu
            Debug.Log("Options");
        }

        public void OnInstructionsClick()
        {
            EventManager.Publish("Instructions");
        }

        public void OnInstructions()
        {          
            //Function will bring up the instructions.
            Debug.Log("Instructions");
        }

        public void OnQuitGameClick()
        {
            EventManager.Publish("QuitGame");
        }
        public void OnQuitGame()
        {
            //Function will quit game upon click.
            Debug.Log("Quit Game");
        }
        #endregion
        #endregion
    }
}
