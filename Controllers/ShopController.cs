using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop//index
        public ActionResult Index()
        {
            return RedirectToAction("index","page");
        }
        // GET: Shop//CatagoreyMenuPartial
        public ActionResult CatagoreyMenuPartial()
        {
            //declare catagorey vm list
            List<CatagoryVM> catagorylist;
            //init the catagoreyvm list
            using (Contextdb db = new Contextdb())
            {
                catagorylist = db.catagory.ToArray().OrderBy(x => x.Sorting).Select(x => new CatagoryVM(x)).ToList();
            }
            //return the view
            return PartialView(catagorylist);

        }
        // GET: Shop//Catagory
        public ActionResult Catagory(string name)
        {
            //declare a list of productvm
            List<ProductVM> catagorylist;
            using(Contextdb db=new Contextdb())
            {

                //get catagoryid
                CategoryDTO dto = db.catagory.Where(x => x.Name == name).FirstOrDefault();
                int catid = dto.id;
                //init the list
                catagorylist = db.products.ToArray().Where(x => x.CatagoreyId == catid).Select(x=> new ProductVM(x)).ToList();
                //get catagory name
                var productcat = db.products.Where(x => x.CatagoreyId == catid).FirstOrDefault();
                ViewBag.catname = productcat.CatagoreyName;
            }
            //return view with list
            return View(catagorylist);
        }
    }
}