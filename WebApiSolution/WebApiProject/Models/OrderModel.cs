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
        public float OrderAmount { get; set; }
        public int UserId { get; set; }
    }
}
