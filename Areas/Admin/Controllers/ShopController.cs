using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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
        // post: Admin/shop//addproduct
        [HttpPost]
        public ActionResult addproduct(ProductVM model, HttpPostedFileBase file)
        {
            //check model state
           
                if (!ModelState.IsValid)
                {
                using (Contextdb db = new Contextdb())
                {
                    model.catagories = new SelectList(db.catagory.ToList(), "id", "Name");
                    return View(model);
                }
                }
                 //Declare Product id
                  int id;
            using (Contextdb db = new Contextdb())
                {
                //make sure that product name is unique
                if (db.products.Any(x => x.Name == model.Name))
                {
                    model.catagories = new SelectList(db.catagory.ToList(), "id", "Name");
                    ModelState.AddModelError("", "Name already exist");
                    return View(model);
                }

                //init and save product dto
                ProductsDTO dto = new ProductsDTO();
                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CatagoreyId = model.CatagoreyId;
                CategoryDTO catdto = db.catagory.FirstOrDefault(x => x.id == model.CatagoreyId);
                dto.CatagoreyName = catdto.Name;
                db.products.Add(dto);
                db.SaveChanges();
                //get inserted id
                id = dto.id;
            }
            //set temp data
            TempData["SM"] = "product successfully added";
            //upload image
            #region Upload Image
            //create neccasrry directorories
            var orignalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            var pathString1 = Path.Combine(orignalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(orignalDirectory.ToString(), "Products\\"+id.ToString());
            var pathString3 = Path.Combine(orignalDirectory.ToString(), "Products\\" + id.ToString()+"\\Thumbs");
            var pathString4 = Path.Combine(orignalDirectory.ToString(), "Products\\" + id.ToString()+"\\Gallery");
            var pathString5 = Path.Combine(orignalDirectory.ToString(), "Products\\" + id.ToString()+ "\\Gallery\\Thumbs");
            if (Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);
            if (Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);
            if (Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);
            if (Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);
            if (Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);
            //check if file was uploaded
            if (file != null && file.ContentLength > 0)
            {
                //get file extension
                string ext = file.ContentType.ToLower();
                //verify file extension
                if(ext!="image/jpeg"&&
                    ext != "image/pjpeg"&&
                    ext != "image/gif"&&
                    ext != "image/x-png"&&
                    ext != "image/png"
                    )
                {
                    using(Contextdb db=new Contextdb())
                    {
                        model.catagories = new SelectList(db.catagory.ToList(), "id", "Name");
                        ModelState.AddModelError("", "the image was not uploaded-wrong extension.");
                        return View(model);
                    }
                }
                //init image name
                string imageName = file.FileName;
                //save image name to dto
                using(Contextdb db=new Contextdb())
                {
                    ProductsDTO dto = db.products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }
                //set original and thumb image paths
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);
                //save orignal
                file.SaveAs(path);
                //create and save thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);

            }
            #endregion
            //redirect to page
            return RedirectToAction("addproduct");
        }
    } 
}