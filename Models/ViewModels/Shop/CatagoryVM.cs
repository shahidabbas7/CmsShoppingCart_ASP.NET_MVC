using CmsShoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.ViewModels.Shop
{
    public class CatagoryVM
    {
        public CatagoryVM()
        {

        }
        public CatagoryVM(CategoryDTO row)
        {
            id = row.id;
            Name = row.Name;
            Slug = row.Slug;
            Sorting = row.Sorting;
        }
        public int id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
    }
}