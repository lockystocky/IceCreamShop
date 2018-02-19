using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace IceCreamShop.Models
{
    public class ShopDbContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public ShopDbContext() : base("name=ShopDbContext")
        {
        }

        public System.Data.Entity.DbSet<IceCreamShop.Models.MenuItem> MenuItems { get; set; }
        public System.Data.Entity.DbSet<IceCreamShop.Models.Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MenuItem>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("MenuItems");
            });

            modelBuilder.Entity<OrderedItem>().Map(m => {
                m.MapInheritedProperties();
                m.ToTable("OrderedItems");
            });

            modelBuilder.Entity<Ingredient>().Map(m => {
                m.MapInheritedProperties();
                m.ToTable("Ingredients");
            });

            modelBuilder.Entity<SelectedIngredient>().Map(m => {
                m.MapInheritedProperties();
                m.ToTable("SelectedIngredients");
            });
        }
    }
}
