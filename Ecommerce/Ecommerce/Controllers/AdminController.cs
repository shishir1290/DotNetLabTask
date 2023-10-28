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
        public ActionResult UpdateOrderStatus(int id)
        {
            using (var db = new ProductEntities3())
            {
                // Retrieve the order by its ID
                var orderToUpdate = db.Orders.FirstOrDefault(o => o.id == id);

                if (orderToUpdate == null)
                {
                    // Order not found, handle appropriately (e.g., return an error view)
                    TempData["ErrorMessage"] = "Order not found.";
                    return RedirectToAction("AdminHome");
                }

                // Check if the order's status is "Processing"
                if (orderToUpdate.Status == "Processing")
                {
                    // Update the order status to "Approved"
                    orderToUpdate.Status = "Approved";

                    // Save changes to the database
                    db.SaveChanges();
                }
                else if (orderToUpdate.Status == "Approved")
                {
                    // Order is already approved, set an error message
                    TempData["ErrorMessage"] = "This order is already approved.";
                }

                // You can add additional logic here if needed

                return RedirectToAction("AdminHome");
            }
        }




    }
}