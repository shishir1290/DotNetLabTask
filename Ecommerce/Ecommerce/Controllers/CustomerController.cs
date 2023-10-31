using Ecommerce.EF;
using Ecommerce.Models;
using System.Linq;
using BCrypt.Net;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using System;
using System.Web;
using System.Net.Http;

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
                // Hash the password using BCrypt with automatic salt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                var newUser = new User
                {
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Gender = user.Gender,
                    Address = user.Address,
                    Password = hashedPassword,
                    UserType = user.UserType
                };

                Session["UserEmail"] = user.Email;

                db.Users.Add(newUser);
                db.SaveChanges();

                // Generate and send verification code
                string verificationCode = GenerateRandomCode();
                SendVerificationCode(user.Email, verificationCode);

                // Store verification code in a cookie
                Response.Cookies["VerificationCode"].Value = verificationCode;
                Response.Cookies["VerificationCode"].Expires = DateTime.Now.AddMinutes(5);

                // Redirect to the Verify action with the email parameter
                return RedirectToAction("Verify");
            }

            return View(user);
        }


        [HttpGet]
        public ActionResult Verify()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Verify(string code)
        {
            string storedCode = Request.Cookies["VerificationCode"]?.Value;

            if (string.IsNullOrEmpty(storedCode) || storedCode != code)
            {
                ModelState.AddModelError("", "Invalid verification code. Please try again.");
                // If the verification code is wrong, delete the user's information from the database
                var userEmail = Session["UserEmail"] as string;
                if (userEmail != null)
                {
                    var userToDelete = db.Users.FirstOrDefault(u => u.Email == userEmail);
                    if (userToDelete != null)
                    {
                        db.Users.Remove(userToDelete);
                        db.SaveChanges();
                        Session.Clear(); // Clear the session
                        return RedirectToAction("Signup");
                    }
                    
                }
                return View();
            }

            // Code is valid, remove the cookie and proceed
            Response.Cookies["VerificationCode"].Expires = DateTime.Now.AddMinutes(-1);

            // Save user information to the database

            return RedirectToAction("Login");
        }


        // Helper method to generate a random 6-digit code
        private string GenerateRandomCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        // Helper method to send verification code via email
        private void SendVerificationCode(string email, string code)
        {
            // Configure your email settings (SMTP server, credentials, etc.) here
            // SMTP server for Gmail example provided
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "sadmanurshishir1290@gmail.com";
            string smtpPassword = "Your_Gmail_App_Password";

            SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("sadmanurshishir1290@gmail.com"),
                Subject = "Email Verification Code",
                Body = "Your verification code: " + code
            };
            mailMessage.To.Add(email);

            smtpClient.Send(mailMessage);
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
                    Session["Password"] = BCrypt.Net.BCrypt.HashPassword(user.Password);

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
