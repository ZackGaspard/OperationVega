using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Text = UnityEngine.UI.Text;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Assets.Scripts.Managers;



namespace UI
{
    using Assets.Scripts;
    
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
        private Button m_Minerals;
        [SerializeField]
        private Button m_Food;
        [SerializeField]
        private Button m_CookedFood;
        [SerializeField]
        private Button m_Gas;
        [SerializeField]
        private Button m_Fuel;
        [SerializeField]
        private Canvas m_BackgroundUI;
        [SerializeField]
        private RectTransform m_ActionsTAB;
        [SerializeField]
        private RectTransform m_CraftingTAB;
        [SerializeField]
        private RectTransform m_WorkshopUI;
        [SerializeField]
        private RectTransform m_OptionsUI;

        [SerializeField]
        private Text m_MineralsT;
        [SerializeField]
        private Text m_FoodT;
        [SerializeField]
        private Text m_CookedFoodT;
        [SerializeField]
        private Text m_GasT;
        [SerializeField]
        private Text m_FuelT;
        [SerializeField]
       


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
            EventManager.Subscribe("Crafting", this.OnCrafting);
            #endregion

            #region -- Main Menu Subscribers --
            EventManager.Subscribe("NewGame", this.NewGameClick);
            EventManager.Subscribe("Options", this.OnOptionsClick);
            EventManager.Subscribe("Instructions", this.OnInstructions);
            EventManager.Subscribe("QuitGame", this.OnQuitGame);
            #endregion

            #region -- Crafting Subscribers --
            EventManager.Subscribe("Minerals", this.Minerals);
            EventManager.Subscribe("Food", this.Food);
            EventManager.Subscribe("CookedFood", this.CookedFood);
            EventManager.Subscribe("Gas", this.Gas);
            EventManager.Subscribe("Fuel", this.Fuel);
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
            EventManager.UnSubscribe("Crafting", this.OnCrafting);
            #endregion

            #region -- Main Menu Unsubscribers --
            EventManager.UnSubscribe("NewGame", this.NewGame);
            EventManager.UnSubscribe("Options", this.OnOptions);
            EventManager.UnSubscribe("Instructions", this.OnInstructions);
            EventManager.UnSubscribe("QuitGame", this.OnQuitGame);
            #endregion

            #region -- Crafting Unsubscribers --
            EventManager.UnSubscribe("Minerals", this.OnMinerals);
            EventManager.UnSubscribe("Food", this.OnFood);
            EventManager.UnSubscribe("CookedFood", this.OnCookedFood);
            EventManager.UnSubscribe("Gas", this.OnGas);
            EventManager.UnSubscribe("Fuel", this.OnFuel);
            #endregion
        }
        #region -- VOID FUNCTIONS --

        void Update()
        {
            //Updates the amount of resources the player has.
            m_MineralsT.text = " " + User.MineralsCount;
            m_FoodT.text = " " + User.FoodCount;
            m_CookedFoodT.text = "" + User.CookedFoodCount;
            m_GasT.text = " " + User.GasCount;
            m_FuelT.text = "" + User.FuelCount;


            
        }


        public void OnActionsClick()
        {
            EventManager.Publish("Actions");
        }

        public void OnActions()
        {
            m_ActionsTAB.offsetMax = new Vector2(m_CraftingTAB.offsetMax.x, 0);
            m_ActionsTAB.offsetMin = new Vector2(m_CraftingTAB.offsetMin.x, 0);
            Debug.Log("Move Actions Tab down");
        }

        public void OnCraftingClick()
        {
           
            EventManager.Publish("Crafting");
        }

        public void OnCrafting()
        {     
            m_CraftingTAB.offsetMax = new Vector2(m_CraftingTAB.offsetMax.x, 0);
            m_CraftingTAB.offsetMin = new Vector2(m_CraftingTAB.offsetMin.x, 0);

            Debug.Log("Move Crafting Tab down");
        }
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
            m_WorkshopUI.gameObject.SetActive(true);
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
        public void OnOptionsClick()
        {
            EventManager.Publish("Options Menu");
        }
        public void OnOptions()
        {
            m_OptionsUI.gameObject.SetActive(true);
            Debug.Log("Options Menu");
        }
        public void CloseOptionsClick()
        {
            EventManager.Publish("Close Options");
        }

        public void CloseOptions()
        {
            m_OptionsUI.gameObject.SetActive(false);
            Debug.Log("Close Options");
        }

        public void CloseWorkShopClick()
        {
            EventManager.Publish("Close WorkShop");
        }
        public void CloseWorkShop()
        {
           //This function will close work shop menu
            m_WorkshopUI.gameObject.SetActive(false);
            Debug.Log("Close Workshop Menu");

        }
        public void NewGameClick()
        {
            EventManager.Publish("NewGame");
        }
        public void NewGame()
        {
            SceneManager.LoadScene(1);
            //Function will begin game from main menu
            Debug.Log("New Game");
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
            Application.Quit();
            //Function will quit game upon click.
            Debug.Log("Quit Game");
        }
        #endregion

        #region -- Crafting -- 
        public void Minerals()
        {
            EventManager.Publish("Minerals");
        }
        public void OnMinerals()
        {
            //Will Change the source image to the first craft slot
            //Second Slot if first one is selected.
            Debug.Log("Minerals");
        }
        
        public void Food()
        {

            EventManager.Publish("Food");
        }
        public void OnFood()
        {
            //Will Change the source image to the first craft slot
            //Second Slot if first one is selected.
            User.FoodCount++;
            Debug.Log("Food");
        }
        
        public void CookedFood()
        {
            EventManager.Publish("CookedFood");
        }

        public void OnCookedFood()
        {
            //Will Change the source image to the first craft slot
            //Second Slot if first one is selected.
            Debug.Log("Cooked Food");
        }

        public void Gas()
        {
            EventManager.Publish("Gas");
        }

        public void OnGas()
        {
            //Will Change the source image to the first craft slot
            //Second Slot if first one is selected.
            User.GasCount++;
            Debug.Log("Gas");
        }

        public void Fuel()
        {
            EventManager.Publish("Fuel");
        }

        public void OnFuel()
        {
            //Will Change the source image to the first craft slot
            //Second Slot if first one is selected.
            User.FuelCount++;
            
            Debug.Log("Fuel");
        }

        public void OnThrustersClick()
        {
            EventManager.Publish("Thursters");
        }
        public void OnThrusters()
        {

        }

        #endregion
        #endregion
     
    }
}
