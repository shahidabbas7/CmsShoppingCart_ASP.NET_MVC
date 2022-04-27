using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PageController : Controller
    {
        // GET: Admin/Page//index
        public ActionResult Index()
        {
            //declare pagelist
            List<PageVM> pagelist;
            //init pagelist
            using(Contextdb db=new Contextdb())
            {
                pagelist = db.pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }
            return View(pagelist);
        }
        // GET: Admin/Page//addpage
        public ActionResult addpage()
        {
            PageVM page = new PageVM();
            return View(page);
        }
        // post: Admin/Page//addpage
        [HttpPost]
        public ActionResult addpage(PageVM pagemodel)
        {
            //check model state
            if(!ModelState.IsValid)
            {
             return View(pagemodel);
            }
            using(Contextdb DB=new Contextdb())
            {
                //delcare slug
                string Slug;
                //init Page DTO
                PageDTO DTO = new PageDTO();
                //DTO title
                DTO.Title = pagemodel.Title;
                //check for and set slug if needed
                if (string.IsNullOrWhiteSpace(pagemodel.Slug))
                {
                    Slug = pagemodel.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    Slug = pagemodel.Slug.Replace(" ", "-").ToLower();
                }
                //make sure title and slug are unique
                if (DB.pages.Any(x => x.Title == pagemodel.Title) || (DB.pages.Any(x => x.Slug == Slug))){
                    ModelState.AddModelError("", "Title or Slug already exist");
                    return View(pagemodel);
                }
                //DTO the rest
                DTO.Slug = Slug;
                DTO.Body = pagemodel.Body;
                DTO.Sorting = 100;
                DTO.HasSidebar = pagemodel.HasSidebar;
                //save dto
                DB.pages.Add(DTO);
                DB.SaveChanges();
            }
            //set temp data message
            TempData["sm"] = "Page successfully added";
            //redirect to action
            return RedirectToAction("addpage");

        }
        // GET: Admin/Page//edit page
        public ActionResult editpage(int id)
        {
            //declare page vm
            PageVM model = new PageVM();
            using(Contextdb db=new Contextdb())
            {
                //get the page
                PageDTO dto = db.pages.Find(id);
                //check if the page exist
                if (dto == null)
                {
                    return Content("page not found");
                }
                //init page
                model = new PageVM(dto);
            }

            //return the view with model
            return View(model);
        }
        // post: Admin/Page//editpage
        [HttpPost]
        public ActionResult editpage(PageVM model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return RedirectToAction("edit");

            }
            using(Contextdb db=new Contextdb())
            {
               //get page id
                 int id = model.id;
                //declare slug
              string slug="home";
                //get the page
                PageDTO dto =db.pages.Find(id);
                //dto the title
                dto.Title = model.Title;
                //check for slug and set it if needed
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }
                //make sure that title and slug are unique
                if (db.pages.Where(x => x.id != id).Any(x => x.Slug == slug) || db.pages.Where(x => x.id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "Title or slug already exist");
                    return View(model);
                }
                //dto the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                //save the dto
                db.SaveChanges();

            }
            //set temp message
            TempData["sm"] = "page successfully edited";
            //redirect
            return RedirectToAction("editpage");
        }
        // GET: Admin/Page//pageDetails
        public ActionResult pageDetails(int id)
        {
            //declare vm
            PageVM model = new PageVM();
            using(Contextdb db=new Contextdb())
            {
                //get the page 
                PageDTO dto = db.pages.Find(id);
                //confirm that the page exist
                if (dto == null)
                {
                    return Content("Page Cannot be found");
                }
                //init page vm
                model = new PageVM(dto);
            }
            //return view with model
            return View(model);
        }
        // GET: Admin/Page//Deletepage
        public ActionResult Deletepage(int id)
        {
            using(Contextdb db=new Contextdb())
            {
                //get page
                PageDTO dto = db.pages.Find(id);
                //remove page
                db.pages.Remove(dto);
                //save dto
                db.SaveChanges();
            }
            //redirect to action
            return RedirectToAction("index");
        }
        // post: Admin/Page//ReorderPage
        [HttpPost]
        public void ReorderPage(int[] id)
        {
            using(Contextdb db=new Contextdb())
            {
                //init conter
                int counter = 1;
                //declare dto 
                PageDTO dto;
                //set sorting for each page
                foreach (var pageid in id)
                {
                    dto = db.pages.Find(pageid);
                    dto.Sorting = counter;
                    db.SaveChanges();
                    counter++;
                }
            }
           

        }
        // get: Admin/Page//sidebaredit
        public ActionResult EditSidebar()
        {
            //declare vm
            SidebarVM model;
            using(Contextdb db=new Contextdb())
            {
                //get dto
                SidebarDTO dto = db.sidebar.Find(1);
                //init model
                model = new SidebarVM(dto);
            }
            //return page
            return View(model);
        }
        // post: Admin/Page//sidebaredit
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using(Contextdb db=new Contextdb())
            {
                //get the dto
                SidebarDTO dto = db.sidebar.Find(1);
                //assign the body to dto
                dto.body = model.body;
                //save dto
                db.SaveChanges();
            }
            //set success message
            TempData["sm"] = "successfully Edited";
            //return the view
            return RedirectToAction("EditSidebar");
        }
    }
}