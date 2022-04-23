using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
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
        // GET: Shop//products-detials
        [ActionName("products-detials")]
        public ActionResult ProductDetial(string name)
        {
            //declare the vm and dto
            ProductVM model;
            ProductsDTO dto;
            //init the product id
            int id = 0;
            using(Contextdb db=new Contextdb())
            {
                //check if product exist
                if (!db.products.Any(x => x.Slug == name))
                {
                    return RedirectToAction("index", "shop");
                }
                //init product dto
                dto = db.products.Where(x => x.Slug == name).FirstOrDefault();
                //get id
                id = dto.id;
                //init model
                model = new ProductVM(dto);
            }
            //get gallery images
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs")).
                  Select(fn => Path.GetFileName(fn));
            //return the view
            return View("ProductDetial", model);
        }
    }
}