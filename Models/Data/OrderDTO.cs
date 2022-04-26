using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.Data
{
    [Table("TblOrder")]
    public class OrderDTO
    {
        [Key]
        public int Orderid { get; set; }
        public int Userid { get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey("Userid")]
        public virtual UsersDTO user { get; set; }
    }
}