using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.Data
{
    [Table("Tblproducts")]
    public class ProductsDTO
    {
        [Key]
        public int id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string CatagoreyName { get; set; }
        public int CatagoreyId { get; set; }
        public string ImageName { get; set; }
        [ForeignKey("CatagoreyId")]
        public virtual CategoryDTO catagorey { get; set; }
    }
}