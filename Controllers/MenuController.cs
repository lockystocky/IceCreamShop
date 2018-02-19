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
using System.IO;

namespace IceCreamShop.Controllers
{
    public class FileLogger
    {
        public static void Log(string logInfo)
        {
            File.AppendAllText(@"D:\\test.txt", logInfo);
        }
    }

    public class MenuController : Controller
    {
        private ShopDbContext db = new ShopDbContext();

        // GET: Menu
        public ActionResult Index()
        {            
            return View(db.MenuItems.ToList());
        }

        [Authorize]
        public ActionResult CreateItem(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MenuItem menuItem = db.MenuItems
               .Where(x => x.Id == id)
               .Include(x => x.AdditionalIngredients)
               .FirstOrDefault();

            if (menuItem == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<SelectedIngredient> selectedIngredients = new List<SelectedIngredient>();
            foreach(var ingr in menuItem.AdditionalIngredients)
            {
                
                SelectedIngredient selectedIngr = new SelectedIngredient
                {
                    Name = ingr.Name,
                    Price = ingr.Price,
                    Weight = ingr.Weight,
                    Quantity = 0
                };
                selectedIngredients.Add(selectedIngr);
            }

            return View(selectedIngredients);
        }

        public OrderedItem CreateOrderedItem(int menuItemId)
        {
            MenuItem menuItem = db.MenuItems
                 .Where(x => x.Id == menuItemId)
                 .Include(x => x.AdditionalIngredients)
                 .FirstOrDefault();

            if (menuItem == null)
            {
                return null;
            }

            OrderedItem orderedItem = new OrderedItem
            {
                Name = menuItem.Name,
                Category = menuItem.Category,
                Weight = menuItem.Weight,
                Price = menuItem.Price,
                Decsription = menuItem.Decsription,
                AdditionalIngredients = new List<SelectedIngredient>()
            };

            return orderedItem;
        } 

        public Order CreateOrderForCurCustomer()
        {
            var context = new ApplicationDbContext();
            var currentUserId = User.Identity.GetUserId();
            var currentUser = context.Users.FirstOrDefault(u => u.Id == currentUserId);

            Order order = new Order
            {
                Customer = new Customer
                {
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    Email = currentUser.Email,
                    Phone = currentUser.Phone,
                    ApplicationUserId = currentUserId
                },
                Delivered = false,
                Paid = false,
                OrderedItems = new List<OrderedItem>(),
                Date = DateTime.Now
            };
            db.Orders.Add(order);
            db.SaveChanges();
            return order;
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateItem(int? id, List<SelectedIngredient> selectedIngredients)
        {
            
            db.Database.Log = logInfo => FileLogger.Log(logInfo);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            OrderedItem orderedItem = CreateOrderedItem((int)id);
                        
            foreach(SelectedIngredient selectedIngr in selectedIngredients)
            {   
                if(selectedIngr.Quantity > 0)
                    orderedItem.AdditionalIngredients.Add(selectedIngr);
            }

            orderedItem.Price = orderedItem.CountPrice();
            orderedItem.Quantity = 1;
            orderedItem.Amount = orderedItem.CountAmount();
            FileLogger.Log(DateTime.Now + "amount = " + orderedItem.Amount.ToString());

            var currentUserId = User.Identity.GetUserId();
            
            Order order = db.Orders
                .Where(o => o.Customer.ApplicationUserId == currentUserId && !o.Paid)
                .Include(o => o.OrderedItems.Select(item => item.AdditionalIngredients))
                .FirstOrDefault();

            if (order == null)
            {
                order = CreateOrderForCurCustomer();                
            }

            order.OrderedItems.Add(orderedItem);
            order.TotalBill = order.CountBill();
            
            db.SaveChanges();
                        
             return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult GetOrder()
        {
            var currentUserId = User.Identity.GetUserId();

            Order order = db.Orders
                .Where(o => o.Customer.ApplicationUserId == currentUserId
                && !o.Paid)
                .Include(o => o.OrderedItems)
                .FirstOrDefault();
                                   
            return Json(order, JsonRequestBehavior.AllowGet);
        }


        /* [Authorize]
         [HttpPost]
         public ActionResult CreateItem(int itemId)
         {
             MenuItem item = db.MenuItems
                 .Where(x => x.Id == itemId)
                 .Include(x => x.AdditionalIngredients)
                 .FirstOrDefault();

             return View(item);
         }*/

        /*[Authorize]
        public ActionResult AddItem(int itemId)
        {
            MenuItem menuItem = db.MenuItems
                .Where(item => item.Id == itemId)
                .FirstOrDefault();

            if (menuItem == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var context = new ApplicationDbContext();
            var currentUserId = User.Identity.GetUserId();
            var currentUser = context.Users.FirstOrDefault(u => u.Id == currentUserId);

            Order order = db.Orders
                .Include(o => o.Customer)
                .Where(o => o.Customer.Email == currentUser.Email
                && o.Customer.Phone == currentUser.Phone
                && !o.Paid)
                .FirstOrDefault();

            if (order == null)
            {
                order = new Order
                {
                    Customer = new Customer
                    {
                        FirstName = currentUser.FirstName,
                        LastName = currentUser.LastName,
                        Email = currentUser.Email,
                        Phone = currentUser.Phone
                    },
                    Delivered = false,
                    Paid = false,
                    Items = new List<OrderedItem>()
                };
            }

            order.Items.Add(menuItem);
            db.SaveChanges();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        */

        public ActionResult FilteredItems(string categParam)
        {
            MenuCategory category = (MenuCategory)Enum.Parse(typeof(MenuCategory), categParam);

            var items = db.MenuItems
                .Where(item => item.Category == category)
                .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // GET: Menu/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuItem menuItem = db.MenuItems
               .Where(x => x.Id == id)
               .Include(x => x.AdditionalIngredients)
               .FirstOrDefault();

            if (menuItem == null)
            {
                return HttpNotFound();
            }
            return View(menuItem);
        }

        // GET: Menu/CreateAdditionalIngredient/id
        public ActionResult CreateAdditionalIngredient(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
                 
            return View();
        }

        [HttpPost]
        public ActionResult CreateAdditionalIngredient(Ingredient ingredient, int id)
        {            
            if (ModelState.IsValid)
            {
                MenuItem item = db.MenuItems
                   .Where(x => x.Id == id)
                   .Include(x => x.AdditionalIngredients)
                   .FirstOrDefault();

                item.AdditionalIngredients.Add(ingredient);
                db.SaveChanges();
                return RedirectToAction("Details/" + id);
            }

            return View(ingredient);
        }

        // GET: Menu/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Menu/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Decsription,Category,Weight,Price")] MenuItem menuItem)
        {
            if (ModelState.IsValid)
            {
                menuItem.Quantity = 50;
                db.MenuItems.Add(menuItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(menuItem);
        }

        // GET: Menu/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuItem menuItem = db.MenuItems.Find(id);
            if (menuItem == null)
            {
                return HttpNotFound();
            }
            return View(menuItem);
        }

        // POST: Menu/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Decsription,Category,Weight,Price")] MenuItem menuItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(menuItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(menuItem);
        }

        // GET: Menu/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuItem menuItem = db.MenuItems.Find(id);
            if (menuItem == null)
            {
                return HttpNotFound();
            }
            return View(menuItem);
        }

        // POST: Menu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MenuItem menuItem = db.MenuItems.Find(id);
            db.MenuItems.Remove(menuItem);
            db.SaveChanges();
            return RedirectToAction("Index");
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
