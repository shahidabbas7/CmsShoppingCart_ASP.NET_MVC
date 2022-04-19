using CmsShoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Models.ViewModels
{
    public class SidebarVM
    {
        public SidebarVM()
        {
               
        }
        public SidebarVM(SidebarDTO row )
        {
            id = row.id;
            body = row.body;
        }
        public int id { get; set; }
        [AllowHtml]
        public string body { get; set; }
    }
}