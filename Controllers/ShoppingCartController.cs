using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
        public async Task<ActionResult> Index()
        { 
            var currentUserId = User.Identity.GetUserId();

            var orders = await db.Orders
                .Where(o => o.Customer.ApplicationUserId == currentUserId
                && !o.Delivered)
                .Include(o => o.OrderedItems.Select(oi => oi.AdditionalIngredients))
                .ToListAsync();

            return View(orders);
        }


        public async Task<ActionResult> RemoveItem(int id)
        {
            var order = await GetCurrentOrder();

            var item = order.OrderedItems
                .Where(i => i.Id == id)
                .FirstOrDefault();

            if(item == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            order.OrderedItems.Remove(item);
            order.TotalBill = order.CountBill();
            await db.SaveChangesAsync();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public async Task<Order> GetCurrentOrder()
        {
            var currentUserId = User.Identity.GetUserId();

            var order = await db.Orders
                .Where(o => o.Customer.ApplicationUserId == currentUserId
                && !o.Paid)
                .Include(o => o.OrderedItems)
                .FirstOrDefaultAsync();

            return order;
        }

        [HttpPost]
        public async Task<ActionResult> ChangeItemQuantity(int id, int newQuantity)
        {
            if(newQuantity < 1)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var order = await GetCurrentOrder();

            var item = order.OrderedItems
                .Where(i => i.Id == id)
                .FirstOrDefault();

            if (item == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            item.Quantity = newQuantity;
            item.Amount = item.CountAmount();
            order.TotalBill = order.CountBill();
            await db.SaveChangesAsync();

            decimal[] newAmountAndPrice = { item.Amount, order.TotalBill};

            //return new HttpStatusCodeResult(HttpStatusCode.OK);
            return Json(newAmountAndPrice, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetItemsQuantity()
        {
            Order order = await GetCurrentOrder();
            int quantity = 0;
            foreach(var item in order.OrderedItems)
            {
                quantity += item.Quantity;
            }

            return Json(quantity, JsonRequestBehavior.AllowGet);
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
