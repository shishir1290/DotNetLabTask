using Ecommerce.EF;
using Ecommerce.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Ecommerce.Controllers
{
    public class CustomerController : Controller
    {
        private ProductEntities1 db = new ProductEntities1();

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

                var newUser = new User
                {
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Gender = user.Gender,
                    Address = user.Address,
                    Password = user.Password,
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
                var user = db.Users.SingleOrDefault(c => c.Email == loginModel.Email && c.Password == loginModel.Password);
                if (user != null)
                {
                    // Authentication successful
                    Session["CustomerEmail"] = user.Email;

                    // Check the user type and redirect accordingly
                    if (user.UserType == "Customer")
                    {
                        return RedirectToAction("CustomerHome", "Home"); // Redirect to the customer home page
                    }
                    else if (user.UserType == "Admin")
                    {
                        return RedirectToAction("AdminHome", "Home"); // Redirect to the admin home page
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
