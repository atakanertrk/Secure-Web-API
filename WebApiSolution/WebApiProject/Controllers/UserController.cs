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
        [Authorize]
        [HttpPut]
        public IActionResult CreateOrder([FromBody] OrderModel order)
        {
            order.UserId = GetAuthenticatedUsersId();
            _dataAccess.InsertOrder(order);
            return Ok();
        }

        [Authorize]
        [HttpDelete]
        public IActionResult DeleteOrder([FromQuery] string orderId)
        {
            var usersOrders = _dataAccess.GetUserOrders(GetAuthenticatedUsersId());
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

        [Authorize]
        [HttpGet]
        public IActionResult OrdersOfUser()
        {
            int userId = GetAuthenticatedUsersId();
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

        public int GetAuthenticatedUsersId()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            return Convert.ToInt32(claim[0].Value);
        }
    }
}
