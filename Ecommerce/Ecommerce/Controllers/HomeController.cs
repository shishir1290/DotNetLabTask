using Ecommerce.Auth;
using Ecommerce.EF;
using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private ProductEntities db = new ProductEntities();
        // GET: Home
        [Logged]
        public ActionResult Index()
        {
            var productList = db.Products.ToList();
            return View(productList);
        }

        [Logged]
        public ActionResult Buy(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.id == id);

            if (product == null)
            {
                // Product not found, you can handle this case (e.g., show an error message)
                return RedirectToAction("Index"); // Redirect to some other page
            }

            return View(product);
        }

        [Logged]
        public ActionResult CustomerProfile()
        {
            // Get the customer's email from the session
            string customerEmail = Session["CustomerEmail"] as string;

            if (string.IsNullOrEmpty(customerEmail))
            {
                // Redirect to the login page if the user is not logged in
                return RedirectToAction("Login", "Customer");
            }

            // Query the database to retrieve the customer's profile
            var profile = db.Customers.FirstOrDefault(cp => cp.Email == customerEmail);

            if (profile == null)
            {
                // Handle the case where the profile is not found
                // You can redirect to an error page or take another appropriate action
                return RedirectToAction("Index", "Home"); // Redirect to a default page for this example
            }

            // Pass the customer's profile to the view
            return View(profile);
        }


        [Logged]
        public ActionResult Order(int id)
        {
            // Check if the user is logged in
            string customerEmail = Session["CustomerEmail"] as string;
            if (string.IsNullOrEmpty(customerEmail))
            {
                // User is not logged in, redirect to the login page
                return RedirectToAction("Login", "Customer");
            }

            // Retrieve the customer's profile
            var customerProfile = db.Customers.FirstOrDefault(cp => cp.Email == customerEmail);
            if (customerProfile == null)
            {
                // Customer profile not found, handle appropriately
                return RedirectToAction("Index", "Home"); // Redirect to a default page
            }

            // Retrieve the order
            var order = db.Products.FirstOrDefault(p => p.id == id);
            if (order == null)
            {
                // Order not found, handle appropriately (e.g., show an error message)
                return RedirectToAction("Index"); // Redirect to some other page
            }

            // Create an instance of the view model and populate it
            var viewModel = new ViewModel
            {
                Product = order,
                Customer = customerProfile
            };

            return View(viewModel);
        }


        [Logged]
        public ActionResult Cart()
        {
            List<string> cartItems = new List<string>();

            // Retrieve the cart items from cookies
            foreach (var cookieName in Request.Cookies.AllKeys)
            {
                if (cookieName.StartsWith("CartItems_"))
                {
                    HttpCookie cookie = Request.Cookies[cookieName];
                    string items = cookie.Value;
                    cartItems.AddRange(items.Split(','));
                }
            }

            return View(cartItems);
        }

        [Logged]
        public ActionResult AddToCart(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.id == id);

            if (product != null)
            {
                // Create a cookie for the cart or retrieve an existing one
                string cookieName = "CartItems_" + id;
                HttpCookie cartCookie = Request.Cookies[cookieName] ?? new HttpCookie(cookieName);

                // Add the item to the cart
                cartCookie.Value = string.Join(",", (cartCookie.Value + "," + product.ProductName + "|" + product.ProductPrice).Split(',').Distinct());
                cartCookie.Expires = DateTime.Now.AddDays(7);

                Response.Cookies.Add(cartCookie);
            }

            return RedirectToAction("Index");
        }



        [Logged]
        public ActionResult DeleteCartItem(int index)
        {
            List<string> cartItems = new List<string>();

            // Retrieve the cart items from cookies
            foreach (var cookieName in Request.Cookies.AllKeys)
            {
                if (cookieName.StartsWith("CartItems_"))
                {
                    HttpCookie cookie = Request.Cookies[cookieName];
                    string items = cookie.Value;
                    cartItems.AddRange(items.Split(','));
                }
            }

            // Remove the item at the specified index
            if (index >= 0 && index < cartItems.Count)
            {
                cartItems.RemoveAt(index);
            }

            // Update the cart cookies
            foreach (var cookieName in Request.Cookies.AllKeys)
            {
                if (cookieName.StartsWith("CartItems_"))
                {
                    HttpCookie cartCookie = Request.Cookies[cookieName];
                    cartCookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(cartCookie);
                }
            }

            // Re-add the remaining cart items to the cookies
            foreach (var item in cartItems)
            {
                string[] cartItemParts = item.Split('|');
                if (cartItemParts.Length >= 2)
                {
                    var productID = cartItemParts[0];
                    var product = db.Products.FirstOrDefault(p => p.id == int.Parse(productID));
                    if (product != null)
                    {
                        string cookieName = "CartItems_" + product.id;
                        HttpCookie cartCookie = new HttpCookie(cookieName);
                        cartCookie.Value = product.ProductName + "|" + product.ProductPrice;
                        cartCookie.Expires = DateTime.Now.AddDays(7);
                        Response.Cookies.Add(cartCookie);
                    }
                }
            }

            return RedirectToAction("Cart");
        }







        // ClearCart action to clear the cart
        public ActionResult ClearCart()
        {
            // Clear the cart by removing cart item cookies
            foreach (var cookieName in Request.Cookies.AllKeys)
            {
                if (cookieName.StartsWith("CartItems_"))
                {
                    HttpCookie cartCookie = Request.Cookies[cookieName];
                    cartCookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(cartCookie);
                }
            }

            return RedirectToAction("Cart");
        }





        public ActionResult Logout()
        {
            // Sign the user out
            FormsAuthentication.SignOut();

            // Clear the session (optional)
            Session.Clear();

            // Redirect to the login page or any other desired page
            return RedirectToAction("Login", "Customer"); // Redirect to the login page
        }

    }
}