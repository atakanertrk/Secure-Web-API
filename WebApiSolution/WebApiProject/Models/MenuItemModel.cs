using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiProject.Models
{
    public class MenuItemModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
    }
}
