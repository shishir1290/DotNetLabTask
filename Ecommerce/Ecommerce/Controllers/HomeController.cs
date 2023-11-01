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
            string customerEmail = Session["UserEmail"] as string;

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
                    string cookieName = "CartItems";
                    HttpCookie cartCookie = Request.Cookies[cookieName];

                    if (cartCookie == null)
                    {
                        cartCookie = new HttpCookie(cookieName);
                        cartCookie.Value = "";  // Initialize the Value property
                    }

                    string cartItems = cartCookie.Value;
                    string[] cartItemsArray = cartItems.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    bool productExistsInCart = false;

                    /*for (int i = 0; i < cartItemsArray.Length; i++)
                    {
                        string[] cartItemParts = cartItemsArray[i].Split('|');
                        if (cartItemParts.Length >= 1)
                        {
                            if (int.TryParse(cartItemParts[0], out int productId))
                            {
                                if (productId == product.id)
                                {
                                    int quantity = cartItemParts.Length >= 4 ? int.Parse(cartItemParts[3]) : 0;
                                    quantity++;
                                    cartItemParts[3] = quantity.ToString();
                                    cartItemsArray[i] = string.Join("|", cartItemParts);
                                    productExistsInCart = true;
                                }
                            }
                        }
                    }*/

                    if (!productExistsInCart)
                    {
                        int quantity = 1;
                        
                        cartItems += $"{product.id}|{product.ProductName}|{product.ProductPrice}|{quantity},";
                    }
                    

                    cartCookie.Value = cartItems;
                    cartCookie.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Add(cartCookie);
                }
            }

            return RedirectToAction("CustomerHome");
        }






        [Logged]
        public ActionResult Cart()
        {
            var cartItemsList = new List<string>();
            var totalQuantity = 0;
            var cookieName = "CartItems";
            var cartCookie = Request.Cookies[cookieName];

            if (cartCookie != null)
            {
                string cartItems = cartCookie.Value;
                string[] cartItemsArray = cartItems.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in cartItemsArray)
                {
                    cartItemsList.Add(item);
                    string[] cartItemParts = item.Split('|');
                    if (cartItemParts.Length >= 4 && int.TryParse(cartItemParts[3], out int quantity))
                    {
                        totalQuantity += quantity;
                    }
                }
            }

            ViewBag.TotalQuantity = totalQuantity;
            return View(cartItemsList);
        }



        [HttpPost]
        public JsonResult UpdateCartItem(int productId, int quantity)
        {
            // Update the cart items in cookies based on the productId and new quantity
            string cookieName = "CartItems";
            HttpCookie cartCookie = Request.Cookies[cookieName];

            if (cartCookie != null)
            {
                string cartItems = cartCookie.Value;
                string[] cartItemsArray = cartItems.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < cartItemsArray.Length; i++)
                {
                    string[] cartItemParts = cartItemsArray[i].Split('|');
                    if (cartItemParts.Length >= 4)
                    {
                        if (int.TryParse(cartItemParts[0], out int id) && id == productId)
                        {
                            // Update the quantity
                            cartItemParts[3] = quantity.ToString();
                            cartItemsArray[i] = string.Join("|", cartItemParts);
                            break;  // We found the item, so we can exit the loop
                        }
                    }
                }

                // Update the cart cookie with the modified cart items
                cartCookie.Value = string.Join(",", cartItemsArray);
                Response.Cookies.Add(cartCookie);
            }

            // You can add additional logic or validation here if needed

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult RemoveCartItem(int productId)
        {
            // Remove the item from the cart cookies
            string cookieName = "CartItems";
            HttpCookie cartCookie = Request.Cookies[cookieName];

            if (cartCookie != null)
            {
                string cartItems = cartCookie.Value;
                string[] cartItemsArray = cartItems.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                cartItemsArray = cartItemsArray
                    .Where(item =>
                    {
                        string[] cartItemParts = item.Split('|');
                        if (cartItemParts.Length >= 1)
                        {
                            if (int.TryParse(cartItemParts[0], out int id))
                            {
                                return id != productId;
                            }
                        }
                        return true;
                    })
                    .ToArray();

                // Update the cart cookie with the modified cart items
                cartCookie.Value = string.Join(",", cartItemsArray);
                Response.Cookies.Add(cartCookie);
            }

            // You can add additional logic or validation here if needed

            return Json(new { success = true });
        }




        [Logged]
        public ActionResult DeleteCartItem(int id)
        {
            string cookieName = "CartItems";
            HttpCookie cartCookie = Request.Cookies[cookieName];

            if (cartCookie != null)
            {
                string cartItems = cartCookie.Value;
                string[] cartItemsArray = cartItems.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                cartItemsArray = cartItemsArray
                    .Where(item =>
                    {
                        string[] cartItemParts = item.Split('|');
                        if (cartItemParts.Length >= 1)
                        {
                            if (int.TryParse(cartItemParts[0], out int productId))
                            {
                                return productId != id;
                            }
                        }
                        return true;
                    })
                    .ToArray();

                // Update the cart cookie with the modified cart items
                cartCookie.Value = string.Join(",", cartItemsArray);
                cartCookie.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Add(cartCookie);
            }

            return RedirectToAction("Cart");
        }






        [Logged]
        public ActionResult OrderAll()
        {
            // Check if the user is logged in
            string customerEmail = Session["UserEmail"] as string;
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
                var cartCookieName = "CartItems";
                var cartCookie = Request.Cookies[cartCookieName];

                if (cartCookie != null)
                {
                    string cartItemsCookieValue = cartCookie.Value;
                    cartItems.AddRange(cartItemsCookieValue.Split(','));
                }

                // Insert orders into the database
                foreach (var item in cartItems)
                {
                    string[] cartItemParts = item.Split('|');
                    if (cartItemParts.Length >= 1)
                    {
                        if (int.TryParse(cartItemParts[0], out int productId) && int.TryParse(cartItemParts[3], out int quantity))
                        {
                            // Create a new order and set the customer and product IDs
                            var newOrder = new Order
                            {
                                CustomerId = customerProfile.id,
                                ProductId = productId,
                                Quantity = quantity, // Set the order quantity
                                Status = "Processing",
                                Date = DateTime.Now,
                            };

                            // Save the order to the database
                            db.Orders.Add(newOrder);
                        }
                    }
                }

                // Save changes to the database
                db.SaveChanges();

                // Clear the cart by removing the cart cookie
                cartCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cartCookie);

                // You can add order processing logic here if needed

                return View(); // Redirect to an order confirmation view
            }
        }



        [Logged]
        public ActionResult ShowAllOrder()
        {
            var OrderList = db.Orders.ToList();
            return View(OrderList);
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