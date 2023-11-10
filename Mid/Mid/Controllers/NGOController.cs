using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mid.Auth;
using Mid.EF;

namespace Mid.Controllers
{
    public class NGOController : Controller
    {

        private FoodEntities5 db = new FoodEntities5();
        // GET: Admin
        [Logged]
        public ActionResult NGOHome()
        {
            var foodItems = db.FoodItems.ToList();
            return View(foodItems);
        }

        [Logged]
        public ActionResult UpdateOrderStatus(int id, string value)
        {
            var email = Session["UserEmail"] as string;
            var NGO = db.NGOs.FirstOrDefault(n => n.email == email);
            var foodTrack = db.FoodTrackes.FirstOrDefault(f => f.id == id);
            if (foodTrack != null)
            {
                if(value == "Accept")
                {
                    foodTrack.NGOId = NGO.id;
                    foodTrack.accept_time = DateTime.Now;
                    foodTrack.status = "Accepted";
                    db.SaveChanges();
                }
                else if(value == "Reject")
                {
                    foodTrack.NGOId = NGO.id;
                    foodTrack.accept_time = DateTime.Now;
                    foodTrack.status = "Rejected";
                    db.SaveChanges();
                }
                
            }
            return RedirectToAction("NGOHome","NGO");
        }
    }
}