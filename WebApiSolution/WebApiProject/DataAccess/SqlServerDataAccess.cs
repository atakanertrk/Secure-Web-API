using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApiProject.Models;

namespace WebApiProject.DataAccess
{
    public class SqlServerDataAccess : IDataAccess
    {
        private string _conStr;
        public SqlServerDataAccess(IConfiguration config)
        {
            _conStr = ConfigurationExtensions.GetConnectionString(config, "SqlServerConnectionString");
        }

        public void DeleteOrder(int orderId)
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@Id", orderId);

                string sql = "DELETE FROM Orders WHERE Id=@Id;";

                cnn.Execute(sql, p);
            }
        }

        public MenuItemModel GetMenuItemInfoById(int id)
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@Id", id);

                string sql = "SELECT * FROM MenuItems WHERE Id=@Id;";

                var output = cnn.Query<MenuItemModel>(sql, p).ToList().FirstOrDefault();

                return output;
            }
        }

        public List<MenuItemModel> GetMenuItems()
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                string sql = "SELECT * FROM MenuItems";

                var output = cnn.Query<MenuItemModel>(sql).ToList();
                return output;
            }
        }

        public List<OrderModel> GetUserOrders(int userId)
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@userId", userId);

                string sql = "SELECT * FROM Orders WHERE UserId=@userId;";

                var output = cnn.Query<OrderModel>(sql, p).ToList();

                return output;
            }
        }

        public void InsertOrder(OrderModel order)
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@MenuItemId", order.MenuItemId);
                p.Add("@OrderAmount", order.OrderAmount);
                p.Add("@UserId", order.UserId);
                p.Add("@Adress", order.Adress);

                string sql = "INSERT INTO Orders (MenuItemId,OrderAmount,UserId,Adress) VALUES (@MenuItemId,@OrderAmount,@UserId,@Adress);";

                cnn.Execute(sql, p);
            }
        }

        public int InsertUser(UserModel user)
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@UserName", user.UserName);
                p.Add("@HashedUserIdentifier", user.HashedUserIdentifier);

                string sql = "INSERT INTO Users (UserName,HashedUserIdentifier) VALUES (@UserName,@HashedUserIdentifier); SELECT SCOPE_IDENTITY();";

                int output = cnn.Query<int>(sql, p).ToList().First();
                return output;
            }
        }

        public int? IsUserValid(UserModel user)
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@HashedUserIdentifier", user.HashedUserIdentifier);

                string sql = "SELECT Id FROM Users WHERE HashedUserIdentifier=@HashedUserIdentifier;";

                var output = cnn.Query<int?>(sql, p).ToList().FirstOrDefault();

                if (output == null)
                {
                    return -1;
                }

                return output;
            }
        }
    }
}
