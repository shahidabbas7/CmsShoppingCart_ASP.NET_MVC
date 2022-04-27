using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels;
using CmsShoppingCart.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CmsShoppingCart.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account//index
        public ActionResult Index()
        {
            return RedirectToAction("login");
        }

        // GET: Account//Login
        public ActionResult login()
        {
            string username = User.Identity.Name;
            if (!string.IsNullOrEmpty(username))
                return RedirectToAction("user-profile");
            LoginUserVM login = new LoginUserVM();
            return View(login);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("login");
        }

        // GET: Account//Login
        [HttpPost]
        public ActionResult login(LoginUserVM model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //check if the user is valid
            bool isvalid = false;
            using(Contextdb db=new Contextdb())
            {
                if (db.users.Any(x => x.Username.Equals(model.UserName)) && db.users.Any(x => x.Password.Equals(model.Password)))
                    {
                    isvalid = true;
                }
            }
            if (!isvalid)
            {
                ModelState.AddModelError("", "Username or password does not match");
                return View(model);
            }
            else
            {
                FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                return Redirect(FormsAuthentication.GetRedirectUrl(model.UserName, model.RememberMe));
            }
        }
        // GET: Account//CreateAccount
        [HttpGet]
        [ActionName("create-account")]
        public ActionResult CreateAccount()
        {
            UsersVm user = new UsersVm();
            return View("CreateAccount", user);
        }
        // GET: Account//CreateAccount
        [HttpPost]
        [ActionName("create-account")]
        public ActionResult CreateAccount(UsersVm model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }
            //check if password match
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError(" ", "Password does not match");
                return View("CreateAccount", model);
            }
            using (Contextdb db = new Contextdb())
            {
                //make sure if username unique
                if (db.users.Any(x => x.Username == model.Username))
                {
                    ModelState.AddModelError(" ", "username already exist");
                    model.Username = "";
                    return View("CreateAccount", model);
                }
                //create user dto
                UsersDTO dto = new UsersDTO();
                //save userdto
                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAddress = model.EmailAddress;
                dto.Username = model.Username;
                dto.Password = model.Password;
                db.users.Add(dto);
                db.SaveChanges();
                //get inserted id
                int id = dto.id;
                //add to userrolesdto
                UserRolesDTO rolesdto = new UserRolesDTO()
                {
                    Userid=id,
                    Roleid=2
                };
                db.UserRoles.Add(rolesdto);
                db.SaveChanges();
            }
            //set temp message
            TempData["SM"] = "you are now registered and can login";
           //redirect to view
            return RedirectToAction("login");
        }
        // GET: Account//UserNavPartial
        public ActionResult UserNavPartial()
        {
            //get username
            string username = User.Identity.Name;
            //declare model
            UserNavPartialVM model;
            using(Contextdb db=new Contextdb())
            {
                //get the user 
                UsersDTO dto = db.users.FirstOrDefault(x => x.Username == username);
                //build the model
                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName=dto.LastName
                };
            }
            //return the view
            return PartialView(model);
        }
        // GET: Account//user-profile
        [HttpGet]
        [ActionName("user-profile")]
        public ActionResult UserProfile()
        {
            //get user name
            string username = User.Identity.Name;
            //declare model
            UserProfileVM model;
            using(Contextdb db=new Contextdb())
            {
                //get user
                UsersDTO dto = db.users.FirstOrDefault(x => x.Username == username);
                //build model
                model = new UserProfileVM(dto);
            }

            //return view with model
            return View("UserProfile", model);
        }
        // GET: Account//user-profile
        [HttpPost]
        [ActionName("user-profile")]
        public ActionResult UserProfile(UserProfileVM model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }
            //confirm password
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "password does not match");
                    return View("UserProfile", model);
                }
            }
           
            //get username
            string username = User.Identity.Name;
            using(Contextdb db=new Contextdb())
            {
                //make sure that user name is unique
                if (db.users.Where(x=>x.id!=model.id).Any(x => x.Username == username))
                {
                    ModelState.AddModelError("", "username"+model.Username+"already exist");
                    return View("UserProfile", model);
                }
                //edit dto
                int id = model.id;
                UsersDTO dto = db.users.Find(id);
                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAddress = model.EmailAddress;
                dto.Username = model.Username;
                dto.FirstName = model.FirstName;
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }
                //save
                db.SaveChanges();
            }
            //set tempdata message
            TempData["SM"] = "You have edited your profile";
            //return view
            return Redirect("user-profile");
        }
        public ActionResult Orders()
        {
            //init list of orders for admin
            List<OrderUserVM> orderuservmlist = new List<OrderUserVM>();
            using (Contextdb db = new Contextdb())
            {
                //get userid
                UsersDTO user = db.users.FirstOrDefault(x => x.Username == User.Identity.Name);
                int userid = user.id;
                //init list of ordervm
                List<OrderVM> order = db.orders.Where(x=>x.Userid==userid).ToArray().Select(x => new OrderVM(x)).ToList();
                //loop through list of ordervm
                foreach (var item in order)
                {
                    //init product dictionary
                    Dictionary<string, int> ProductAndQty = new Dictionary<string, int>();
                    //declare total
                    decimal total = 0m;
                    //init list of orderdetials dto
                    List<OrderDetialsDTO> orderdetials = db.orderdetials.Where(x => x.Orderid == item.Orderid).ToList();
                    //loop throgh list of orderdetialsdto
                    foreach (var orderdetial in orderdetials)
                    {
                        //get product 
                        ProductsDTO product = db.products.Where(x => x.id == orderdetial.Productid).FirstOrDefault();
                        //get price 
                        decimal price = product.Price;
                        //get product name
                        string productname = product.Name;
                        //add to product dictionary 
                        ProductAndQty.Add(productname, orderdetial.Quantity);
                        //get total
                        total += orderdetial.Quantity * price;
                        //add to orderforadmin list
                        orderuservmlist.Add(new OrderUserVM()
                        {
                            OrederNumber = item.Orderid,
                            ProductsAndQTY = ProductAndQty,
                            Total = total,
                            CreatedAt = item.CreatedAT
                        });
                    }
                }

            }
            //return view with orderforadminvm list
            return View(orderuservmlist);
        }
    }
}