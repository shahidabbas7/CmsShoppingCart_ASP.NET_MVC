using CmsShoppingCart.Models.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart//index
        public ActionResult Index()
        {
            //init the cart list
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();
            //check if cart is empty
            if (cart.Count == 0 || Session["cart"] != null)
            {
                ViewBag.  = "Your Cart is empty";
                return View();        
            }

            //calculate total and save to viewbag
            decimal total = 0;
            foreach(var item in cart)
            {
                total += item.Total;
            }
            ViewBag.Grandtotal = total;
            //return view with model
            return View(cart);
        }
        // GET: Cart//CartPartial
        public ActionResult CartPartial()
        {
            //init cart vm
            CartVM model = new CartVM();
            //init quantity
            int qty = 0;
            //init price
            decimal price = 0m;
            //check for cart session
            if (Session["cart"] != null)
            {
                //get total qty and price
                var list = (List < CartVM >) Session["cart"];
                foreach(var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity*item.Price;
                }
            }
            else
            {
                //set qty and price to zero
                qty = 0;
                price = 0;
            }
            //return partial view with model
            return PartialView(model);
        }

    }
}