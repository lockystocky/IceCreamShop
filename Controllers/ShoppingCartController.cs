using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IceCreamShop.Models;
using Microsoft.AspNet.Identity;

namespace IceCreamShop.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private ShopDbContext db = new ShopDbContext();

        // GET: ShoppingCart
        public ActionResult Index()
        { 
            var currentUserId = User.Identity.GetUserId();

            var orders = db.Orders
                .Where(o => o.Customer.ApplicationUserId == currentUserId
                && !o.Delivered)
                .Include(o => o.OrderedItems)
                .ToList();

            return View(orders);
        }

        public ActionResult RemoveItem(int id)
        {
            var currentUserId = User.Identity.GetUserId();

            var order = db.Orders
                .Where(o => o.Customer.ApplicationUserId == currentUserId
                && !o.Paid)
                .Include(o => o.OrderedItems)
                .FirstOrDefault();

            var item = order.OrderedItems
                .Where(i => i.Id == id)
                .FirstOrDefault();

            if(item == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            order.OrderedItems.Remove(item);
            order.TotalBill = order.CountBill();
            db.SaveChanges();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
