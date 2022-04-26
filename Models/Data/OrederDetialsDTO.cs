using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.Data
{
    [Table("TblOrderDetials")]
    public class OrderDetialsDTO
    {
        [Key]
        public int id { get; set; }
        public int Orderid { get; set; }
        public int Userid { get; set; }
        public int Productid { get; set; }
        public int Quantity { get; set; }
        [ForeignKey("Orderid")]
        public virtual OrderDTO orders { get; set; }
        [ForeignKey("Userid")]
        public virtual UsersDTO user { get; set; }
        [ForeignKey("Productid")]
        public virtual ProductsDTO products { get; set; }
    }
}