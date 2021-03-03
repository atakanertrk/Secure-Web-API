using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebApiProject.Models;

namespace WebApiProject.DataAccess
{
    public class MySqlDataAccess : IDataAccess
    {
        private string _conStr;
        public MySqlDataAccess(IConfiguration config)
        {
            _conStr = ConfigurationExtensions.GetConnectionString(config, "MySqlConnection");
        }
        public int InsertUser(UserModel user)
        {
            using (IDbConnection cnn = new MySqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@UserName", user.UserName);
                p.Add("@HashedUserIdentifier", user.HashedUserIdentifier);

                string sql = "INSERT INTO users (UserName,HashedUserIdentifier) VALUES (@UserName,@HashedUserIdentifier); SELECT LAST_INSERT_ID();";

                int output = cnn.Query<int>(sql, p).ToList().First();
                return output;
            }
        }
        /// <summary>
        /// return null if user is not valid, otherwise returns userId
        /// </summary>
        public int? IsUserValid(UserModel user)
        {
            using (IDbConnection cnn = new MySqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@HashedUserIdentifier", user.HashedUserIdentifier);

                string sql = "SELECT Id FROM users WHERE HashedUserIdentifier=@HashedUserIdentifier;";

                var output = cnn.Query<int?>(sql, p).ToList().FirstOrDefault();

                if (output == null)
                {
                    return -1;
                }
                return output;
            }
        }
        public List<MenuItemModel> GetMenuItems()
        {
            using (IDbConnection cnn = new MySqlConnection(_conStr))
            {
                string sql = "SELECT * FROM menuitems";

                var output = cnn.Query<MenuItemModel>(sql).ToList();
                return output;
            }
        }
        public MenuItemModel GetMenuItemInfoById(int id)
        {
            using (IDbConnection cnn = new MySqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@Id", id);

                string sql = "SELECT * FROM menuitems WHERE Id=@Id;";

                var output = cnn.Query<MenuItemModel>(sql, p).ToList().FirstOrDefault();

                return output;
            }
        }
        public void InsertOrder(OrderModel order)
        {
            using (IDbConnection cnn = new MySqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@MenuItemId", order.MenuItemId);
                p.Add("@OrderAmount", order.OrderAmount);
                p.Add("@UserId", order.UserId);
                p.Add("@Adress", order.Adress);

                string sql = "INSERT INTO orders (MenuItemId,OrderAmount,UserId,Adress) VALUES (@MenuItemId,@OrderAmount,@UserId,@Adress);";

                cnn.Execute(sql, p);
            }
        }

        public List<OrderModel> GetUserOrders(int userId)
        {
            using (IDbConnection cnn = new MySqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@userId", userId);

                string sql = "SELECT * FROM orders WHERE userId=@userId;";

                var output = cnn.Query<OrderModel>(sql, p).ToList();

                return output;
            }
        }

        public void DeleteOrder(int orderId)
        {
            using (IDbConnection cnn = new MySqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@Id", orderId);

                string sql = "DELETE FROM orders WHERE Id=@Id;";

                cnn.Execute(sql, p);
            }
        }
    }
}
