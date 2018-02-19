using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IceCreamShop.Models
{
    public abstract class Item
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Decsription { get; set; }
        public MenuCategory Category { get; set; }
        public int Weight { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderedItem : Item
    {
        public List<SelectedIngredient> AdditionalIngredients { get; set; }        
        public decimal Amount { get; set; }

        public decimal CountAmount()
        {
            decimal amount = 0;

            for (int i = 0; i < Quantity; i++)
                amount += Price;

            return amount;
        }

        public decimal CountPrice()
        {
            decimal totalPrice = Price;

            foreach (var ingredient in AdditionalIngredients)
            {
                for(int i = 0; i < ingredient.Quantity; i++)
                    totalPrice += ingredient.Price;
            }

            return totalPrice;
        }
    }

    public class MenuItem : Item
    {
        public List<Ingredient> AdditionalIngredients { get; set; }       
        
    }

    

    public enum MenuCategory
    {
        IceCream,
        Drink,
        Yogurt
    }

    public class BaseIngredient
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public decimal Price { get; set; }
    }

    public class Ingredient : BaseIngredient
    {

    }

    public class SelectedIngredient : BaseIngredient
    {
        public int Quantity { get; set; }
    }
}