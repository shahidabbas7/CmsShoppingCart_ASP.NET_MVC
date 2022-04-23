using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
using System.Web.Mvc;

namespace CmsShoppingCart.Controllers
{
    public class PageController : Controller
    {
        // GET: Page//index
        public ActionResult Index(string page="")
        {
            //get/set page slug
            if (page == "")
                page = "home";
            //declare model and dto
            PageVM model;
            PageDTO dto;
            //check if page exist
            using(Contextdb db=new Contextdb())
            {
                if (!db.pages.Any(x => x.Slug.Equals(page)))
                {
                    return RedirectToAction("index", new { page = "" });
                }
                //get page dto
                dto = db.pages.Where(x => x.Slug == page).FirstOrDefault();
            }
            //set page title
            ViewBag.PageTitle = dto.Title;
            //check for side bar
            if (dto.HasSidebar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }
            //int model
            model = new PageVM(dto);
            //return view with model
            return View(model);
        }
        // GET: Page//PageMenuPartial
        public ActionResult PageMenuPartial()
        {
            //declare page vm list
            List<PageVM> pagevmlist;
            //get all pages except home
            using(Contextdb db=new Contextdb())
            {
                pagevmlist = db.pages.ToArray().OrderBy(x=>x.Sorting).Where(x => x.Slug != "home").Select(x => new PageVM(x)).ToList();
            }
            //return view with list
            return PartialView(pagevmlist);
        }
        // GET: Page//SidebarPartial
        public ActionResult SidebarPartial()
        {
            //declare model
            SidebarVM model;
            //init model
            using(Contextdb db=new Contextdb())
            {
                SidebarDTO dto = db.sidebar.Find(1);

                model = new SidebarVM(dto);
            }
            //return view with model
            return PartialView(model);
        }
    }
}