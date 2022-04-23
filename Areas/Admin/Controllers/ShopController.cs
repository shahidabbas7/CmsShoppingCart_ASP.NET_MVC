using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Shop;
using PagedList;
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

            // Create necessary directories
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            // Check if a file was uploaded
            if (file != null && file.ContentLength > 0)
            {
                // Get file extension
                string ext = file.ContentType.ToLower();

                // Verify extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Contextdb db = new Contextdb())
                    {
                        model.catagories = new SelectList(db.catagory.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension.");
                        return View(model);
                    }
                }

                // Init image name
                string imageName = file.FileName;

                // Save image name to DTO
                using (Contextdb db = new Contextdb())
                {
                    ProductsDTO dto = db.products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Set original and thumb image paths
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                // Save original
                file.SaveAs(path);

                // Create and save thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }

            #endregion
            //redirect to page
            return RedirectToAction("addproduct");
        }
        // get: Admin/shop//products
        public ActionResult Products(int? page,int? catid)
        {
            //declare list of products
            List<ProductVM> ListOfProductVM;
            //set page number
            var pageNumber = page ?? 1;
            using(Contextdb db=new Contextdb())
            {
                //Initialize the list
                ListOfProductVM = db.products.ToArray()
                        .Where(x => catid == null || catid == 0 || x.CatagoreyId== catid)
                        .Select(x => new ProductVM(x)).ToList();
                //populate catagorey select list
                ViewBag.Categories = new SelectList(db.catagory.ToList(), "id", "Name");
                //set selected catagorey
                ViewBag.SelectedCat = catid.ToString();
            }
            //set pageination
            var onePageOfProducts = ListOfProductVM.ToPagedList(pageNumber, 3);
            ViewBag.OnePageOfProducts = onePageOfProducts;
            //return view with list
            return View(ListOfProductVM);
        }
        // get: Admin/shop//editproducts//id
        public ActionResult EditProducts(int id)
        {
            //declare model vm
            ProductVM model;
            using(Contextdb db=new Contextdb())
            {
                //init product dto
                ProductsDTO dto = db.products.Find(id);
                //chech if product exist
                if (dto == null)
                {
                    return Content("That Product Does Not exist");
                }
                //init model
                model = new ProductVM(dto);
                //get selectlist item
                model.catagories = new SelectList(db.catagory.ToList(), "id", "Name");
                //get all gallery images
                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs")).
                    Select(fn => Path.GetFileName(fn));
            }
            //return view with model
            return View(model);
        }
        [HttpPost]
        // post: Admin/shop//editproducts//id
        public ActionResult EditProducts(ProductVM model,HttpPostedFileBase file)
        {
            //get product id
            int  id = model.id;
            //populate Catagorey select list and gallery image
            using (Contextdb db = new Contextdb()) {
                model.catagories = new SelectList(db.catagory.ToList(), "id", "Name");
            }
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs")).
                   Select(fn => Path.GetFileName(fn));
            //check model state 
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //make sure product name is unique
            using(Contextdb db=new Contextdb())
            {
                if (db.products.Where(x => x.id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError(" ", "name already taken");
                    return View(model);
                }
            }
            //update product
            using(Contextdb db=new Contextdb())
            {
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
            }
            //set temp data
            TempData["SM"] = "Product edited";
            //upload image
            #region Image Upload
            //check for file upload
            if(file!=null&& file.ContentLength > 0)
            {
                //get extension
                string ext = file.ContentType.ToLower();
                //verify extension
                if (ext != "image/jpg" &&
                   ext != "image/jpeg" &&
                   ext != "image/pjpeg" &&
                   ext != "image/gif" &&
                   ext != "image/x-png" &&
                   ext != "image/png")
                {
                    using (Contextdb db = new Contextdb())
                    {
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension.");
                        return View(model);
                    }
                }
                //set upload directory path
                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
                //delete files from the directories
                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);
                foreach (FileInfo file2 in di1.GetFiles())
                    file2.Delete();
                foreach (FileInfo file3 in di2.GetFiles())
                    file3.Delete();
                //save image name
                string ImageName = file.FileName;
                using(Contextdb db=new Contextdb())
                {
                    ProductsDTO dto = db.products.Find(id);
                    dto.ImageName = ImageName;
                    db.SaveChanges();
                }
                //save orignal and thumb image
                var path = string.Format("{0}\\{1}", pathString1, ImageName);
                var path2 = string.Format("{0}\\{1}", pathString2, ImageName);

                // Save original
                file.SaveAs(path);

                // Create and save thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }

            #endregion
            //redirect to action
            return RedirectToAction("EditProducts");
        }
        public ActionResult Deleteproducts(int id)
        {
            //delete product
            using(Contextdb db=new Contextdb())
            {
                ProductsDTO dto = db.products.Find(id);
                db.products.Remove(dto);
                db.SaveChanges();
            }
            //delete folder
            var orignalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            string pathString = Path.Combine(orignalDirectory.ToString(), "Products\\" + id.ToString());
            if (Directory.Exists(pathString))
                Directory.Delete(pathString,true);
            //redirect to action
            return RedirectToAction("products");

        }
        // POST: Admin/Shop/SaveGalleryImages
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            // Loop through files
            foreach (string fileName in Request.Files)
            {
                // Init the file
                HttpPostedFileBase file = Request.Files[fileName];

                // Check it's not null
                if (file != null && file.ContentLength > 0)
                {
                    // Set directory paths
                    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    // Set image paths
                    var path = string.Format("{0}\\{1}", pathString1, file.FileName);
                    var path2 = string.Format("{0}\\{1}", pathString2, file.FileName);

                    // Save original and thumb

                    file.SaveAs(path);
                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200);
                    img.Save(path2);
                }

            }

        }
        public void DeleteImage(int id, string ImageName)
        {
            string FullPath1 = Request.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/"+ImageName);
            string FullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs/"+ImageName);
            if (System.IO.File.Exists(FullPath1))
            System.IO.File.Delete(FullPath1);
            if (System.IO.File.Exists(FullPath2)) 
            System.IO.File.Delete(FullPath2);
        }
    }
    }
