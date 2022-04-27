using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.ViewModels
{
    public class OrderUserVM
    {
        public int OrederNumber { get; set; }
        public decimal Total { get; set; }
        public Dictionary<string, int> ProductsAndQTY { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}