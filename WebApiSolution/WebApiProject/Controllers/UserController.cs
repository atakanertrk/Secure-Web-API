﻿using Microsoft.AspNetCore.Http;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebApiProject.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IDataAccess _dataAccess;
        public UserController(IConfiguration config)
        {
            _dataAccess = new SqlServerDataAccess(config);
        }
        /// <summary>
        /// id and userId are not required (can be passed null or 0) (bearer token is required)
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
        /// Deletes specified order of user. Delete works only if user owns that passed id of order. (requires bearer token for identification)
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
            public decimal OrderAmount { get; set; }
            public decimal TotalPrice { get; set; }
            public string Adress { get; set; }
        }
        /// <summary>
        /// Returns list of user's order. (requires bearer token)
        /// </summary>
        [Authorize]
        [HttpGet]
        public IActionResult OrdersOfUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userIdFromToken = Convert.ToInt32(claim[0].Value);

            var orders = _dataAccess.GetUserOrders(userIdFromToken);

            var listOfUserOrders = new List<OrderOfUserModel>();
            foreach (var order in orders)
            {
                var menuItemInfo = _dataAccess.GetMenuItemInfoById(order.MenuItemId);
                var orderOfUserModel = new OrderOfUserModel{
                    Id=order.Id,
                    MenuItemId=order.MenuItemId,
                    MenuItemName=menuItemInfo.ItemName,
                    OrderAmount=order.OrderAmount,
                    Adress=order.Adress,
                    TotalPrice=order.OrderAmount*menuItemInfo.Price
                };
                listOfUserOrders.Add(orderOfUserModel);
            }
            return Ok(listOfUserOrders);
        }
        /// <summary>
        /// Public endpoint, no authorization
        /// </summary>
        [HttpGet]
        public IActionResult GetMenuItemsList()
        {
            var menu = _dataAccess.GetMenuItems();
            return Ok(menu);
        }
        /// <summary>
        /// Public endpoint, no authorization
        /// </summary>
        [HttpGet]
        public IActionResult GetItemDetailsById([FromQuery] int id)
        {
            var details = _dataAccess.GetMenuItemInfoById(id);
            return Ok(details);
        }

        //[HttpGet]
        //[Authorize(AuthenticationSchemes = GoogleDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> HelloAsync()
        //{
        //    var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //    var claims = result.Principal.Identities
        //        .FirstOrDefault().Claims.Select(claim => new
        //        {
        //            claim.Issuer,
        //            claim.OriginalIssuer,
        //            claim.Type,
        //            claim.Value
        //        });
        //    return Ok();
        //}
        //[HttpGet]
        //public async Task<IActionResult> Redirected()
        //{
        //    var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //    var claims = result.Principal.Identities
        //        .FirstOrDefault().Claims.Select(claim => new
        //        {
        //            claim.Issuer,
        //            claim.OriginalIssuer,
        //            claim.Type,
        //            claim.Value
        //        });

        //    return Ok(claims);
        //   // return Ok(User.Identity.Name);
        //}
    }
}
