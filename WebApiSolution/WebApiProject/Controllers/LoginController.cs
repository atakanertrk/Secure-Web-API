using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiProject.DataAccess;
using WebApiProject.Helpers;
using WebApiProject.Models;

namespace WebApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IDataAccess _dataAccess;
        private TokenHelper _token;
        public LoginController(IConfiguration config)
        {
            _dataAccess = new SqlServerDataAccess(config);
            _token = new TokenHelper(config);
        }
        /// <summary>
        /// Returns Bearer Token if login success. Otherwise, returns 401 status code
        /// </summary>
        [HttpPost]
        public IActionResult Post([FromBody] LogInModel login)
        {
            UserModel user = new UserModel { HashedUserIdentifier = login.hashedUserNameAndPassword };
            int? userIdIfValid = _dataAccess.IsUserValid(user);
            if (userIdIfValid == -1)
            {
                return Unauthorized();
            }
            string token = _token.GenerateJSONWebToken((int)userIdIfValid);
            return Ok(new { token= token });
        }
    }
}
