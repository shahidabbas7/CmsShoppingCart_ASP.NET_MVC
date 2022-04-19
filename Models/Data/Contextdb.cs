using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.Data
{
    public class Contextdb:DbContext
    {
        public DbSet<PageDTO> pages { get; set; }
        public DbSet<SidebarDTO> sidebar { get; set; }
        public DbSet<CategoryDTO> catagory { get; set; }
        public DbSet<ProductsDTO> products { get; set; }
    }
}