using CmsShoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Models.ViewModels
{
    public class PageVM
    {
        public PageVM()
        {

        }
        public PageVM(PageDTO page)
        {
            id = page.id;
            Title = page.Title;
            Slug = page.Slug;
            Body = page.Body;
            Sorting = page.Sorting;
            HasSidebar = page.HasSidebar;

        }
        public int id { get; set; }
        [Required]
        [StringLength(50,MinimumLength =3)]
        public string Title { get; set; }
        public string Slug { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 6)]
        [AllowHtml]
        public string Body { get; set; }
        public int Sorting { get; set; }
        [Display(Name ="Sidebar")]
        public bool HasSidebar { get; set; }
    }
}