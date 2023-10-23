using Ecommerce.EF;
using Ecommerce.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Ecommerce.Controllers
{
    public class CustomerController : Controller
    {
        private ProductEntities db = new ProductEntities();

        [HttpGet]
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(CustomerSignupModel customer)
        {
            if (ModelState.IsValid)
            {
                var newCustomer = new Customer
                {
                    Name = customer.Name,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    Gender = customer.Gender,
                    Address = customer.Address,
                    Password = customer.Password
                };

                db.Customers.Add(newCustomer);
                db.SaveChanges();

                return RedirectToAction("Login"); // Redirect to the desired action after a successful signup
            }

            return View(customer);
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
                var customer = db.Customers.SingleOrDefault(c => c.Email == loginModel.Email && c.Password == loginModel.Password);
                if (customer != null)
                {
                    // Authentication successful
                    Session["CustomerEmail"] = customer.Email;
                    return RedirectToAction("Index", "Home"); // Redirect to the desired page after login
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
