using CmsShoppingCart.Models.Cart;
using CmsShoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart//index
        public ActionResult Index()
        {
            // Init the cart list
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Check if cart is empty
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty.";
                return View();
            }

            // Calculate total and save to ViewBag

            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            // Return view with list
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
                 model.Quantity=qty;
                model.Price=price;
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
        // GET: Cart//AddToCartPartial
        public ActionResult AddToCartPartial(int id)
        {
            // Init CartVM list
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Init CartVM
            CartVM model = new CartVM();

            using (Contextdb db = new Contextdb())
            {
                // Get the product
                ProductsDTO product = db.products.Find(id);

                // Check if the product is already in cart
                var productInCart = cart.FirstOrDefault(x => x.Productid == id);

                // If not, add new
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        Productid = product.id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    // If it is, increment
                    productInCart.Quantity++;
                }
            }

            // Get total qty and price and add to model

            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            // Save cart back to session
            Session["cart"] = cart;

            // Return partial view with model
            return PartialView(model);
        }
        public JsonResult IncrementProduct(int productid)
        {
            //init cart list
            List<CartVM> cart=Session["cart"] as List<CartVM>;
            using(Contextdb db=new Contextdb())
            {
                //get cart vm from list
                CartVM model = cart.FirstOrDefault(x => x.Productid == productid);
                //increment qty
                model.Quantity++;
                //store needed data
                var result =new  { qty = model.Quantity, price = model.Price };
                //return json with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            
        }
        public JsonResult DecrementProduct(int productid)
        {
            // init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            using (Contextdb db = new Contextdb())
            {
                //get cart vm from list
                CartVM model = cart.FirstOrDefault(x => x.Productid == productid);
                //decrement qty
                if (model.Quantity > 0)
                {
                 model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }
                //store needed data
                var result = new { qty = model.Quantity, price = model.Price };
                //return json with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public void RemoveProduct(int productid)
        {
            //init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            using (Contextdb db = new Contextdb())
            {
                //get cart vm from list
                CartVM model = cart.FirstOrDefault(x => x.Productid == productid);
                //remove the product
                cart.Remove(model);
                
            }
        }
        public ActionResult PaypalPartial()
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            return PartialView(cart);
        }
        [HttpPost]
        public void PlaceOrder()
        {
            //get cart list 
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            //get username
            string username = User.Identity.Name;
            //declare orderid
            int orderid = 0;
            using(Contextdb db=new Contextdb())
            {
                //init orderdto
                OrderDTO dto = new OrderDTO();
                //get userid
                var user = db.users.FirstOrDefault(x => x.Username == username);
                int userid = user.id;
                //add to orderdto and save
                dto.Userid = userid;
                dto.CreatedAt = DateTime.Now;
                db.orders.Add(dto);
                db.SaveChanges();
                //get inserted id
                orderid = dto.Userid;
                //init orderdetialdto
                OrderDetialsDTO detialdto = new OrderDetialsDTO();
                foreach (var item in cart)
                {
                    detialdto.Userid = userid;
                    detialdto.Orderid = orderid;
                    detialdto.Productid = item.Productid;
                    detialdto.Quantity = item.Quantity;

                    //add to orderdetialdto
                    db.orderdetials.Add(detialdto);
                    db.SaveChanges();
                }

            }

            //email admin
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("206b5bafcf5d30", "a47da0ae1b4233"),
                EnableSsl = true
            };
            client.Send("shahidabbas.ue@gmail.com", "shahidabbas.ue@gmail.com", "New order", "you have a new order with orderid"+orderid);
            //set the session
            Session["cart"] = null;
        }
    }
}