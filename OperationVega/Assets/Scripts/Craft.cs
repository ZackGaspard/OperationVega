using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;


namespace Assets.Scripts
{
    public class Craft : MonoBehaviour
    {
        //Will be used to desribe resources as Items
        string Items;
        //Integer that will hold the amount.
        int Amount;
        
        
        //Public Craft constructor that has item / amount in the parameters
        public Craft(string item, int amount)
        {
            this.Items = item;
            this.Amount = amount;
        }

       

       
    }

    public class CanCraft 
    {
        //Public List that holds on the ingredients.
        public List<Craft> Ingredients;
        //Bool that checks to see if the item can be crafted.
        public bool CraftItem;
        Craft output;

        //Boolean that checks the items needed 
        public bool CheckNeededItems()
        {
            User.CookedFoodCount++;
            User.FoodCount++;
            User.MineralsCount++;
            User.SteelCount++;
            User.FuelCount++;

            //Checks through craft list to see if it has required ingredients.
            foreach (Craft item in Ingredients)
            {
                if (Ingredients.Contains(item))
                    CraftItem = true;
            }
            return false;
        }
    }

    public class Fuel : CanCraft 
    {
        //Ingridients for Fuel
        public Fuel()
        {
            // List that contains ingridients for Fuel
            Ingredients = new List<Craft>();
            Ingredients.Add(new Craft("Minerals", 1));
            Ingredients.Add(new Craft("Gas", 1));
            CraftItem = false;

            //Will say that Fuel has been created.
            Debug.Log("Fuel Created");
            
            
        }
    }

    class Steel : CanCraft
    {
        //Ingridients for Steel
        public Steel()
        {
            //List that contains ingridients for Steel
            Ingredients = new List<Craft>();
            Ingredients.Add(new Craft("Minerals", 1));
            Ingredients.Add(new Craft("Fuel", 1));
            CraftItem = false;

            //Will say that Steel has been created.
            Debug.Log("Steel Created");
        }
    }

    class CookedFood : CanCraft
    {
        //Ingridients for CookedFood
        public CookedFood()
        {
            //Ingridients needed for CookedFood
            Ingredients = new List<Craft>();
            Ingredients.Add(new Craft("Food", 1));
            Ingredients.Add(new Craft("Gas", 1));
            CraftItem = false;

            //Will dipslay when the food is cooked.
            Debug.Log("CookedFood Created");
        }
        
    }


    
}
