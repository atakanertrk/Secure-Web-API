using System.Collections.Generic;
using WebApiProject.Models;

namespace WebApiProject.DataAccess
{
    public interface IDataAccess
    {
        void DeleteOrder(int orderId);
        MenuItemModel GetMenuItemInfoById(int id);
        List<MenuItemModel> GetMenuItems();
        List<OrderModel> GetUserOrders(int userId);
        void InsertOrder(OrderModel order);
        int InsertUser(UserModel user);
        int? IsUserValid(UserModel user);
    }
}