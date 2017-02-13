using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;


namespace Assets.Scripts
{
    using Interfaces;
    


    public class Craft 
    {
        //Will be used to desribe resources as Items
        IResources Items;
        //Integer that will hold the amount.
        int Amount;
        
        
        //Public Craft constructor that has item / amount in the parameters
        public Craft(IResources item, int amount)
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
        public bool CheckNeededItems(User user)
        {
           
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
            Ingredients.Add(new Craft(new Minerals(), 1));
            Ingredients.Add(new Craft(new Gas(), 1));
            CraftItem = false;
            
            
        }
    }

    class Steel : CanCraft
    {
        //Ingridients for Steel
        public Steel()
        {
            //List that contains ingridients for Steel
            Ingredients = new List<Craft>();
            Ingredients.Add(new Craft(new Minerals(), 1));
            //Ingridients.Add(new Craft(new Fuels(), 1));
            CraftItem = false;
            
        }
    }

    class CookedFood : CanCraft
    {
        //Ingridients for CookedFood
        public CookedFood()
        {
            //Ingridients needed for CookedFood
            Ingredients = new List<Craft>();
            Ingredients.Add(new Craft(new Food(), 1));
            Ingredients.Add(new Craft(new Gas(), 1));
            CraftItem = false;
        }
        
    }
}
