using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecommerce.Models
{
    public class ViewModel
    {
        public Ecommerce.EF.Product Product { get; set; }
        public Ecommerce.EF.User Customer { get; set; }
    }
}