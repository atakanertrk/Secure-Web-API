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
    public class SigninController : ControllerBase
    {
        private IDataAccess _dataAccess;
        private TokenHelper _token;
        public SigninController(IConfiguration config)
        {
            _dataAccess = new SqlServerDataAccess(config);
            _token = new TokenHelper(config);
        }
        /// <summary>
        /// Returns Bearer token if SignIn suceess, Otherwise returns 400 bad request
        /// </summary>
        [HttpPost]
        public IActionResult Post([FromBody] SigninModel signin)
        {
            UserModel user = new UserModel { UserName = signin.UserName, HashedUserIdentifier = signin.HashedUserNameAndPassword };
            try
            {
                int insertedId = _dataAccess.InsertUser(user);
                string token = _token.GenerateJSONWebToken(insertedId);
                return Ok(new { token = token});
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
