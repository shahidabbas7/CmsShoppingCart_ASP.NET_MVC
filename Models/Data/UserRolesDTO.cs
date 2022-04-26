using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.Data
{
    [Table("tblUserRoles")]
    public class UserRolesDTO
    {
        [Key,Column(Order =0)]
        public int Userid { get; set; }
        [Key,Column(Order =1)]
        public int Roleid { get; set; }
        [ForeignKey("Userid")]
        public virtual UsersDTO User { get; set; }
        [ForeignKey("Roleid")]
        public virtual RolesDTO Roles { get; set; }
    }
}