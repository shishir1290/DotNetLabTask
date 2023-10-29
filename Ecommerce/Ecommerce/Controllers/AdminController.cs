using Ecommerce.Auth;
using Ecommerce.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Controllers
{
    public class AdminController : Controller
    {

        private ProductEntities3 db = new ProductEntities3();
        // GET: Admin
        [Logged]
        public ActionResult AdminHome()
        {
            var OrderList = db.Orders.ToList();
            return View(OrderList);
        }


        [Logged]
        public ActionResult AdminProfile()
        {
            // Get the customer's email from the session
            string UserEmail = Session["UserEmail"] as string;

            if (string.IsNullOrEmpty(UserEmail))
            {
                // Redirect to the login page if the user is not logged in
                return RedirectToAction("Login", "Customer");
            }

            // Query the database to retrieve the customer's profile
            var profile = db.Users.FirstOrDefault(cp => cp.Email == UserEmail);

            if (profile == null)
            {
                // Handle the case where the profile is not found
                // You can redirect to an error page or take another appropriate action
                return RedirectToAction("AdminHome", "Admin"); // Redirect to a default page for this example
            }

            // Pass the customer's profile to the view
            return View(profile);
        }


        [Logged]
        public ActionResult UpdateOrderStatus(int id, string value)
        {
            using (var db = new ProductEntities3())
            {
                var orderToUpdate = db.Orders.FirstOrDefault(o => o.id == id);

                if (orderToUpdate == null)
                {
                    TempData["ErrorMessage"] = "Order not found.";
                    return RedirectToAction("AdminHome");
                }

                if (value == "Approved")
                {
                    HandleApprovedOrder(db, orderToUpdate);
                }
                else if (value == "Refused")
                {
                    HandleRefusedOrder(db, orderToUpdate);
                }

                // You can add additional logic here if needed

                return RedirectToAction("AdminHome");
            }
        }

        private void HandleApprovedOrder(ProductEntities3 db, Order orderToUpdate)
        {
            if (orderToUpdate.Status == "Refused" || orderToUpdate.Status == "Approved")
            {
                TempData["ApprovedErrorMessage"] = "This order is already " + orderToUpdate.Status + ".";
            }
            else if (orderToUpdate.Status == "Processing")
            {
                var product = db.Products.FirstOrDefault(p => p.id == orderToUpdate.ProductId);

                if (product != null)
                {
                    if (product.Quantity >= orderToUpdate.Quantity)
                    {
                        product.Quantity -= (int)orderToUpdate.Quantity;
                        orderToUpdate.Status = "Approved";
                        db.SaveChanges();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Insufficient product quantity.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Product not found.";
                }
            }
        }

        private void HandleRefusedOrder(ProductEntities3 db, Order orderToUpdate)
        {
            if (orderToUpdate.Status == "Approved" || orderToUpdate.Status == "Refused")
            {
                TempData["RefusedErrorMessage"] = "This order is already " + orderToUpdate.Status + ".";
            }
            else if (orderToUpdate.Status == "Processing")
            {
                orderToUpdate.Status = "Refused";
                db.SaveChanges();
            }
        }






    }
}