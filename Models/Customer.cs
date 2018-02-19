using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IceCreamShop.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; }
        public string ApplicationUserId { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public List<OrderedItem> OrderedItems { get; set; }
        public string Address { get; set; }
        public DateTime Date { get; set; }
        public bool Paid { get; set; }
        public bool Delivered { get; set; }

        [Display(Name = "Delivery charge")]
        public decimal DeliveryCharge { get; set; }

        [Display(Name = "Service charge")]
        public decimal ServiceCharge { get; set; }
        public decimal TotalBill { get; set; }
        public Customer Customer { get; set; }

        public decimal CountBill()
        {
            decimal bill = 0;
            foreach(var item in OrderedItems)
            {
                bill += item.Price;
            }

            return bill;
        }
    }

    
}