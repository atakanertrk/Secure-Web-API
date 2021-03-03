using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiProject.DataAccess;
using WebApiProject.Models;
using WebApiProject.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebApiProject.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private MySqlDataAccess _dataAccess;
        public UserController(IConfiguration config)
        {
            _dataAccess = new MySqlDataAccess(config);
        }
        /// <summary>
        /// id and userId are not required (require bearer token)
        /// </summary>
        [Authorize]
        [HttpPut]
        public IActionResult CreateOrder([FromBody] OrderModel order)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userIdFromToken = Convert.ToInt32(claim[0].Value);

            order.UserId = userIdFromToken;
            _dataAccess.InsertOrder(order);
            return Ok();
        }

        /// <summary>
        /// Deletes specified order of user. Delete works if user owns passed id of order. (require bearer token)
        /// </summary>
        [Authorize]
        [HttpDelete]
        public IActionResult DeleteOrder([FromQuery] string orderId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userIdFromToken = Convert.ToInt32(claim[0].Value);

            var usersOrders = _dataAccess.GetUserOrders(userIdFromToken);
            foreach (var orders in usersOrders)
            {
                if (orders.Id.ToString() == orderId)
                {
                    _dataAccess.DeleteOrder(Convert.ToInt32(orderId));
                    return Ok();
                }
            }
            return BadRequest();
        }

        private class OrderOfUserModel
        {
            public int Id { get; set; }
            public int MenuItemId { get; set; }
            public string MenuItemName { get; set; }
            public float OrderAmount { get; set; }
            public int UserId { get; set; }
            public string Adress { get; set; }
        }
        /// <summary>
        /// Returns list of user's order. (require bearer token)
        /// </summary>
        [Authorize]
        [HttpGet]
        public IActionResult OrdersOfUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userIdFromToken = Convert.ToInt32(claim[0].Value);

            int userId = userIdFromToken;
            var orders = _dataAccess.GetUserOrders(userId);
            return Ok(orders);
        }

        [HttpGet]
        public IActionResult GetMenuItemsList()
        {
            var menu = _dataAccess.GetMenuItems();
            return Ok(menu);
        }

        [HttpGet]
        public IActionResult GetItemDetailsById([FromQuery] int id)
        {
            var details = _dataAccess.GetMenuItemInfoById(id);
            return Ok(details);
        }


    }
}
