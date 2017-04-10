using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UnityEngine.AI;
using UnityEngine.UI;
using Assets.Scripts.Managers;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Controllers;



namespace Assets.Scripts
{
    //Currently need to figure out how exactly I want crafting to be set up. 
    public class Craft2 : MonoBehaviour
    {
        
        #region --Variables--
        public bool HasNeededItems1;
        public bool HasNeededItems2;
        public bool HasNeededItems3;
       
        public bool CraftItem;
        #endregion


        #region --Void Functions--
        protected void Awake()
        {
            HasNeededItems1 = true;
            HasNeededItems2 = true;
            HasNeededItems3 = true;

            CraftItem = false;
        }


        //Function will be used to create steel
        public void Steel()
        {
            
            if(HasNeededItems1)
            {
                User.SteelCount++;
                HasNeededItems1 = false;
            }
            else if(!HasNeededItems1)
            {
                User.SteelCount = 0;
                HasNeededItems1 = true;
            }
        }

        //Function will be used to create fuel.
        public void Fuel()
        {
            if(HasNeededItems2)
            {
                User.FuelCount++;
                HasNeededItems2 = false;
            }
            else if(!HasNeededItems2)
            {
                User.FuelCount = 0;
                HasNeededItems2 = true;
            }

        }

        //Function will be used to create CookedFood.
        public void CookedFood()
        {
            if(HasNeededItems3)
            {
                User.CookedFoodCount++;
                HasNeededItems3 = false;
            }
            else if (!HasNeededItems3)
            {
                User.CookedFoodCount = 0;
                HasNeededItems3 = true;
             }
        }
        #endregion
    }
}
