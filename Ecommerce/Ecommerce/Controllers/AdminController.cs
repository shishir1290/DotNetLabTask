﻿using Ecommerce.Auth;
using Ecommerce.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Controllers
{
    public class AdminController : Controller
    {

        private ProductEntities2 db = new ProductEntities2();
        // GET: Admin
        [Logged]
        public ActionResult AdminHome()
        {
            var OrderList = db.Orders.ToList();
            return View(OrderList);
        }
    }
}