using Mid.Auth;
using Mid.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mid.Controllers
{
    public class EmployeeController : Controller
    {
        private FoodEntities5 db = new FoodEntities5();
        // GET: Employee

        [Logged]
        public ActionResult EmployeeHome()
        {
            var foodItems = db.FoodItems.ToList();
            return View(foodItems);
        }


        [Logged]
        public ActionResult UpdateOrderStatus(int id, string value)
        {
            var email = Session["UserEmail"] as string;
            var employee = db.Employees.FirstOrDefault(n => n.email == email);
            var foodTrack = db.FoodTrackes.FirstOrDefault(f => f.id == id);
            if (foodTrack != null)
            {
                if (value == "Collect")
                {
                    foodTrack.EmployeeId = employee.id;
                    foodTrack.collecting_time = DateTime.Now;
                    foodTrack.status = "Collected";
                    db.SaveChanges();
                }
                

            }
            return RedirectToAction("NGOHome", "NGO");
        }
    }
}