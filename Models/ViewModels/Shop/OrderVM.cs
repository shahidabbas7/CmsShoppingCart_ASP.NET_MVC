using CmsShoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.ViewModels.Shop
{
    public class OrderVM
    {
        public OrderVM()
        {

        }
        public OrderVM(OrderDTO order)
        {
            Orderid = order.Orderid;
            Userid = order.Userid;
            CreatedAT = order.CreatedAt;
        }
        public int Orderid { get; set; }
        public int Userid { get; set; }
        public DateTime CreatedAT { get; set; }
    }
}