using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.Data
{
    [Table("TblSidebar")]
    public class SidebarDTO
    {
        [Key]
        public int id { get; set; }
        public string body { get; set; }
    }
}