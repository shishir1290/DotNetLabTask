using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Mid.EF;
using Mid.Models; // Import your SignupModel and database context
using System.Web.Security;

public class SignupController : Controller
{
    private FoodEntities5 db = new FoodEntities5(); // Replace YourDbContext with your actual database context class

    [HttpGet]
    public ActionResult Signup()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Signup(SignupModel model, string userType)
    {
        if (ModelState.IsValid)
        {
            // Check if the email is unique for the selected user type
            bool isEmailUnique = IsEmailUniqueForUserType(model.Email, userType);

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            if (!isEmailUnique)
            {
                ModelState.AddModelError("Email", "Email address is already in use for the selected user type.");
                return View(model);
            }

            // Determine the user type selected by the user
            if (userType == "Restaurant")
            {
                var restaurant = new Restaurant
                {
                    name = model.Name,
                    address = model.Address,
                    phone = model.Phone,
                    email = model.Email,
                    password = hashedPassword
                };

                db.Restaurants.Add(restaurant);
                db.SaveChanges();
            }
            else if (userType == "NGO")
            {
                var ngo = new NGO
                {
                    name = model.Name,
                    address = model.Address,
                    phone = model.Phone,
                    email = model.Email,
                    password = hashedPassword
                };

                db.NGOs.Add(ngo);
                db.SaveChanges();
            }
            else if (userType == "Employee")
            {
                var employee = new Employee
                {
                    name = model.Name,
                    address = model.Address,
                    phone = model.Phone,
                    email = model.Email,
                    password = hashedPassword
                };

                db.Employees.Add(employee);
                db.SaveChanges();
            }

            // Generate and send a verification code
            string verificationCode = GenerateRandomCode();
            SendVerificationCode(model.Email, verificationCode);

            // Store verification code in a cookie
            Response.Cookies["VerificationCode"].Value = verificationCode;
            Response.Cookies["VerificationCode"].Expires = DateTime.Now.AddMinutes(3);

            // Redirect to the Verify action
            return RedirectToAction("Verify", new { email = model.Email });
        }

        return View(model);
    }

    [HttpGet]
    public ActionResult Verify(string email)
    {
        ViewBag.Email = email; // Pass email to the view
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Verify(string code, string email)
    {
        string storedCode = Request.Cookies["VerificationCode"]?.Value;

        if (string.IsNullOrEmpty(storedCode) || storedCode != code)
        {
            // If the verification code is wrong, delete the user's information from the database
            var userEmail = email;

            if (userEmail != null)
            {
                var restaurant = db.Restaurants.FirstOrDefault(r => r.email == userEmail);
                var ngo = db.NGOs.FirstOrDefault(n => n.email == userEmail);
                var employee = db.Employees.FirstOrDefault(e => e.email == userEmail);

                if (restaurant != null)
                {
                    db.Restaurants.Remove(restaurant);
                }
                else if (ngo != null)
                {
                    db.NGOs.Remove(ngo);
                }
                else if (employee != null)
                {
                    db.Employees.Remove(employee);
                }

                db.SaveChanges();
                Session.Clear(); // Clear the session

                return RedirectToAction("Signup");
            }
        }
        else
        {
            // Code is valid, remove the cookie
            Response.Cookies["VerificationCode"].Expires = DateTime.Now.AddMinutes(-1);

            // Save user information to the database or take appropriate actions

            return RedirectToAction("Login");
        }

        ModelState.AddModelError("", "Invalid verification code. Please try again.");
        return View();
    }

    // Action to delete and redirect after 3 minutes
    public ActionResult DeleteAndRedirect(string email)
    {
        // Delete the user's information from the database
        var restaurant = db.Restaurants.FirstOrDefault(r => r.email == email);
        var ngo = db.NGOs.FirstOrDefault(n => n.email == email);
        var employee = db.Employees.FirstOrDefault(e => e.email == email);

        if (restaurant != null)
        {
            db.Restaurants.Remove(restaurant);
            db.SaveChanges();
            Session.Clear(); // Clear the session

            return RedirectToAction("Signup");
        }
        else if (ngo != null)
        {
            db.NGOs.Remove(ngo);
            db.SaveChanges();
            Session.Clear(); // Clear the session

            return RedirectToAction("Signup");
        }
        else if (employee != null)
        {
            db.Employees.Remove(employee);
            db.SaveChanges();
            Session.Clear(); // Clear the session

            return RedirectToAction("Signup");
        }

        return RedirectToAction("Signup");
    }





    // Helper method to generate a random 6-digit code
    private string GenerateRandomCode()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    // Helper method to send a verification code via email
    private void SendVerificationCode(string email, string code)
    {
        // Configure your email settings (SMTP server, credentials, etc.) here
        // SMTP server for Gmail example provided
        string smtpServer = "smtp.gmail.com";
        int smtpPort = 587;
        string smtpUsername = "ghoreghore.customer.info@gmail.com";
        string smtpPassword = "zpur hffp fwlc vmxu";

        SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUsername, smtpPassword),
            EnableSsl = true
        };

        MailMessage mailMessage = new MailMessage
        {
            From = new MailAddress("ghoreghore.customer.info@gmail.com"),
            Subject = "Verification Code [" + code + "]",
            IsBodyHtml = true, // Set this to render the email as HTML
            Body = @"
   <!DOCTYPE html>
<html>
<head>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f5f5f5;
        }
        .container {
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #ffffff;
            border: 1px solid #ddd;
        }
        .blue-box {
            background-color: #00FF78;
            padding: 20px;
            border: 1px solid #005aa7;
            text-align: center;
        }
        h1 {
            color: #000;
            font-size: 28px;
        }
        .email-content {
            padding: 20px;
        }
        .verification-code {
            font-weight: bold;
            background-color: #00DCFF ;
            padding: 10px;
            border: 1px solid #005aa7;
            display: inline-block;
            color: #000;
            font-size: 24px;
            text-align: center;
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='blue-box'>
            <h1>Your Verification Code</h1>
        </div>
        <div class='email-content'>
            <p>Welcome to our Ecommerce site! To complete your registration, please enter the verification code below:</p>
            <div align='center'>
                <p class='verification-code' align='center'>" + code + @"</p>
            </div>
            <p>If you did not sign up for our service, you can safely ignore this email.</p>
            <p>Thank you for choosing our Ecommerce platform!</p>
        </div>
    </div>
</body>
</html>

"
        };
        mailMessage.To.Add(email);

        smtpClient.Send(mailMessage);
    }

    private bool IsEmailUniqueForUserType(string email, string userType)
    {
        if (userType == "Restaurant")
        {
            return !db.Restaurants.Any(r => r.email == email);
        }
        else if (userType == "NGO")
        {
            return !db.NGOs.Any(n => n.email == email);
        }
        else if (userType == "Employee")
        {
            return !db.Employees.Any(e => e.email == email);
        }

        return false;
    }



    //---------------------------------Login---------------------------------//
    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Login(LoginModel loginModel)
    {
        if (ModelState.IsValid)
        {
            // Try to find the user in each table
            var restaurant = db.Restaurants.SingleOrDefault(r => r.email == loginModel.Email);
            var ngo = db.NGOs.SingleOrDefault(n => n.email == loginModel.Email);
            var employee = db.Employees.SingleOrDefault(e => e.email == loginModel.Email);

            // Check if any of the user types have a matching email
            if (restaurant != null && BCrypt.Net.BCrypt.Verify(loginModel.Password, restaurant.password))
            {
                // Authentication successful for a restaurant user
                Session["UserEmail"] = restaurant.email;
                Session["Password"] = BCrypt.Net.BCrypt.HashPassword(restaurant.password);
                return RedirectToAction("CreateFoodItems", "Restaurant"); // Redirect to the restaurant home page
            }
            else if (ngo != null && BCrypt.Net.BCrypt.Verify(loginModel.Password, ngo.password))
            {
                // Authentication successful for an NGO user
                Session["UserEmail"] = ngo.email;
                Session["Password"] = BCrypt.Net.BCrypt.HashPassword(ngo.password);
                return RedirectToAction("NGOHome", "NGO"); // Redirect to the NGO home page
            }
            else if (employee != null && BCrypt.Net.BCrypt.Verify(loginModel.Password, employee.password))
            {
                // Authentication successful for an employee user
                Session["UserEmail"] = employee.email;
                Session["Password"] = BCrypt.Net.BCrypt.HashPassword(employee.password);
                return RedirectToAction("EmployeeHome", "Employee"); // Redirect to the employee home page
            }
            else
            {
                ModelState.AddModelError("", "Invalid login credentials. Please try again.");
            }
        }
        return View(loginModel);
    }


    public ActionResult Logout()
    {
        // Sign the user out
        FormsAuthentication.SignOut();

        // Clear the session (optional)
        Session.Clear();

        // Redirect to the login page or any other desired page
        return RedirectToAction("Login", "Signup"); // Redirect to the login page
    }

}