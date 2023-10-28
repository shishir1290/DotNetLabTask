using Ecommerce.EF;
using Ecommerce.Models;
using System.Linq;
using BCrypt.Net;
using System.Web.Mvc;

namespace Ecommerce.Controllers
{
    public class CustomerController : Controller
    {
        private ProductEntities3 db = new ProductEntities3();

        [HttpGet]
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(CustomerSignupModel user)
        {
            if (ModelState.IsValid)
            {
                // Check if the email already exists in the database
                /*if (db.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use. Please use a different email.");
                    return View(user);
                }*/

                // Hash the password using BCrypt with automatic salt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                var newUser = new User
                {
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Gender = user.Gender,
                    Address = user.Address,
                    Password = hashedPassword, // Store the hashed password in the database
                    UserType = user.UserType
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                return RedirectToAction("Login"); // Redirect to the desired action after a successful signup
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(CustomerLoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(c => c.Email == loginModel.Email);
                if (user != null && BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password))
                {
                    // Authentication successful
                    Session["UserEmail"] = user.Email;

                    // Check the user type and redirect accordingly
                    if (user.UserType == "Customer")
                    {
                        return RedirectToAction("CustomerHome", "Home"); // Redirect to the customer home page
                    }
                    else if (user.UserType == "Admin")
                    {
                        return RedirectToAction("AdminHome", "Admin"); // Redirect to the admin home page
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid user type. Please try again.");
                        return View(loginModel);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login credentials. Please try again.");
                }
            }
            return View(loginModel);
        }
    }
}
