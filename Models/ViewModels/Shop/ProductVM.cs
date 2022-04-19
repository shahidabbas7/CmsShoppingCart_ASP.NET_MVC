using CmsShoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Models.ViewModels.Shop
{
    public class ProductVM
    {
        public ProductVM()
        {

        }
        public ProductVM(ProductsDTO row)
        {
            id = row.id;
            Name = row.Name;
            Slug = row.Slug;
            Price = row.Price;
            Description = row.Description;
            CatagoreyName = row.CatagoreyName;
            CatagoreyId = row.CatagoreyId;
            ImageName = row.ImageName;
           
        }
        public int id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Slug { get; set; }
        public decimal Price { get; set; }
        [Required]
        public string Description { get; set; }
        public string CatagoreyName { get; set; }
        [Required]
        public int CatagoreyId { get; set; }
        public string ImageName { get; set; }
        public IEnumerable<SelectListItem> catagories { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }

    }
}