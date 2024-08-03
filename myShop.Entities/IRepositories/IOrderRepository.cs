using myShop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myShop.Entities.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        void Update(Order order);
        void UpdateOrderStatus(int id , string orderStatus ,string? paymentStatus);
    }
}
