using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/catagory
        public ActionResult catagory()
        {
            //declare vm list
            List<CatagoryVM> catagory;
            using(Contextdb db=new Contextdb())
            {
                //init list
                catagory = db.catagory.ToArray().OrderBy(x => x.Sorting).
                    Select(x => new CatagoryVM(x)).ToList();

            }

            //return the list
            return View(catagory);
        }
        // Post: Admin/Shop/addcatagory
        public string AddNewCategory(string catName)
        {
            //declare id
            string id;
            using(Contextdb db=new Contextdb())
            {
                //check the catagory name is unique
                if (db.catagory.Any(x => x.Name == catName))
                {
                    return "titletaken";
                }
                //init dto
                CategoryDTO dto=new CategoryDTO();
                //add to dto
                dto.Name = catName;
                dto.Slug = catName;
                dto.Sorting = 100;
                //save dto
                db.catagory.Add(dto);
                db.SaveChanges();
                //get the id
                id = dto.id.ToString();
            }
            //return the id
            return id;
        }
        // post: Admin/Page//ReorderPage
        [HttpPost]
        public void ReorderCatagorey(int[] id)
        {
            using (Contextdb db = new Contextdb())
            {
                //init conter
                int counter = 1;
                //declare dto 
                CategoryDTO dto;
                //set sorting for each page
                foreach (var catid in id)
                {
                    dto = db.catagory.Find(catid);
                    dto.Sorting = counter;
                    db.SaveChanges();
                    counter++;
                }
            }


        }
        // post: Admin/Page//Deletepage
        public ActionResult Deletecatagorey(int id)

        {
            using (Contextdb db = new Contextdb())
            {
                //get page
                CategoryDTO dto = db.catagory.Find(id);
                //remove page
                db.catagory.Remove(dto);
                //save dto
                db.SaveChanges();
            }
            //redirect to action
            return RedirectToAction("catagory");
        }
        // post: Admin/shop//ReorderPage
        [HttpPost]
        public string RenameCategory(string newCatName,int id)
        {
            using(Contextdb db=new Contextdb())
            {
                //check catagorey name is unique
                if (db.catagory.Any(x => x.Name == newCatName))
                {
                    return "titletaken";
                }
                //get dto
                CategoryDTO dto = db.catagory.Find(id);
                //edit dto
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();
                db.SaveChanges();
            }
            //return
            return "ok";
        }
        // get: Admin/shop//addproduct
        public ActionResult addproduct()
        {
            //declare vm
            ProductVM model = new ProductVM();
            using(Contextdb db=new Contextdb())
            {
                //init model with select list 
                model.catagories = new SelectList(db.catagory.ToList(), "id", "Name");
            }
            //return view with model
            return View(model);
        }
    }
}