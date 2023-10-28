using Ecommerce.Auth;
using Ecommerce.EF;
using Ecommerce.Models;
using Newtonsoft.Json;
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
        private ProductEntities3 db = new ProductEntities3();
        // GET: Home
        [Logged]
        public ActionResult CustomerHome()
        {
            var productList = db.Products.ToList();
            return View(productList);
        }

        [Logged]
        public ActionResult Details(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.id == id);

            if (product == null)
            {
                // Product not found, you can handle this case (e.g., show an error message)
                return RedirectToAction("CustomerHome"); // Redirect to some other page
            }

            return View(product);
        }

        [Logged]
        public ActionResult UserProfile()
        {
            // Get the customer's email from the session
            string customerEmail = Session["CustomerEmail"] as string;

            if (string.IsNullOrEmpty(customerEmail))
            {
                // Redirect to the login page if the user is not logged in
                return RedirectToAction("Login", "Customer");
            }

            // Query the database to retrieve the customer's profile
            var profile = db.Users.FirstOrDefault(cp => cp.Email == customerEmail);

            if (profile == null)
            {
                // Handle the case where the profile is not found
                // You can redirect to an error page or take another appropriate action
                return RedirectToAction("CustomerHome", "Home"); // Redirect to a default page for this example
            }

            // Pass the customer's profile to the view
            return View(profile);
        }



        [Logged]
        public ActionResult AddToCart(int id)
        {
            using (var db = new ProductEntities3())
            {
                var product = db.Products.FirstOrDefault(p => p.id == id);

                if (product != null)
                {
                    string cookieName = "CartItems_" + product.id; // Use a unique name for each product
                    HttpCookie cartCookie = Request.Cookies[cookieName] ?? new HttpCookie(cookieName);

                    // Get the current cart items from the cookie
                    string cartItems = cartCookie.Value;

                    // Check if cartItems is not null and not empty before splitting
                    if (!string.IsNullOrEmpty(cartItems))
                    {
                        // Split the cart items into an array, avoiding null values
                        string[] cartItemsArray = cartItems.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        // Check if the product is already in the cart
                        if (!cartItemsArray.Any(item => item.Contains($"{product.id}|")))
                        {
                            // Add the product to the cart items in the format "productId|productName|productPrice"
                            cartItems += $"{product.id}|{product.ProductName}|{product.ProductPrice},";

                            // Update the cart cookie
                            cartCookie.Value = cartItems;
                            cartCookie.Expires = DateTime.Now.AddDays(7);
                            Response.Cookies.Add(cartCookie);
                        }
                    }
                    else
                    {
                        // If cartItems is empty, just add the current product
                        cartItems = $"{product.id}|{product.ProductName}|{product.ProductPrice},";
                        cartCookie.Value = cartItems;
                        cartCookie.Expires = DateTime.Now.AddDays(7);
                        Response.Cookies.Add(cartCookie);
                    }
                }
            }

            return RedirectToAction("CustomerHome");
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
        public ActionResult DeleteCartItem(int id)
        {
            // Find and remove the item with the specified ID
            var cartItems = new List<string>();

            foreach (var cookieName in Request.Cookies.AllKeys)
            {
                if (cookieName.StartsWith("CartItems_"))
                {
                    HttpCookie cookie = Request.Cookies[cookieName];
                    string items = cookie.Value;
                    cartItems.AddRange(items.Split(','));
                }
            }

            // Filter out the item with the specified ID
            cartItems = cartItems.Where(item =>
            {
                string[] cartItemParts = item.Split('|');
                if (cartItemParts.Length >= 1)
                {
                    if (int.TryParse(cartItemParts[0], out int productId))
                    {
                        return productId != id;
                    }
                }
                return false;
            }).ToList();

            // Clear and update the cart cookies
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
                if (cartItemParts.Length >= 3)
                {
                    var productID = cartItemParts[0];
                    var productName = cartItemParts[1];
                    var productPrice = cartItemParts[2];

                    string cookieName = "CartItems_" + productID;
                    HttpCookie cartCookie = new HttpCookie(cookieName);
                    cartCookie.Value = productID + "|" + productName + "|" + productPrice;
                    cartCookie.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Add(cartCookie);
                }
            }

            return RedirectToAction("Cart");
        }





        [Logged]
        public ActionResult OrderAll()
        {
            // Check if the user is logged in
            string customerEmail = Session["CustomerEmail"] as string;
            if (string.IsNullOrEmpty(customerEmail))
            {
                // User is not logged in, redirect to the login page
                return RedirectToAction("Login", "Customer");
            }

            using (var db = new ProductEntities3())
            {
                // Retrieve the customer's profile
                var customerProfile = db.Users.FirstOrDefault(cp => cp.Email == customerEmail);
                if (customerProfile == null)
                {
                    // Customer profile not found, handle appropriately
                    return RedirectToAction("CustomerHome", "Home"); // Redirect to a default page
                }

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

                // Insert orders into the database
                foreach (var item in cartItems)
                {
                    string[] cartItemParts = item.Split('|');
                    if (cartItemParts.Length >= 1)
                    {
                        if (int.TryParse(cartItemParts[0], out int productId))
                        {
                            // Create a new order and set the customer and product IDs
                            var newOrder = new Order
                            {
                                CustomerId = customerProfile.id,
                                ProductId = productId,
                                Status = "Processing"
                            };

                            // Save the order to the database
                            db.Orders.Add(newOrder);
                        }
                    }
                }

                // Clear the cart by removing cart item cookies
                foreach (var cookieName in Request.Cookies.AllKeys)
                {
                    if (cookieName.StartsWith("CartItems_"))
                    {
                        HttpCookie cartCookie = Request.Cookies[cookieName];
                        cartCookie.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Add(cartCookie);
                    }

                    // Save changes to the database
                    db.SaveChanges();
                }

                // You can add order processing logic here if needed

                return View(); // Redirect to an order confirmation view
            }
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