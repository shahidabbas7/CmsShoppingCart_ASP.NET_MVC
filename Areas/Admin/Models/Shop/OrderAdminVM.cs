using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Areas.Admin.Models.Shop
{
    public class OrderAdminVM
    {
        public int OrederNumber { get; set; }
        public string Username { get; set; }
        public decimal Total { get; set; }
        public Dictionary<string,int> ProductsAndQTY { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}