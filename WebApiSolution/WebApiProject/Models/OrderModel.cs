using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiProject.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public decimal OrderAmount { get; set; }
        public int UserId { get; set; }
        public string Adress { get; set; }
    }
}
