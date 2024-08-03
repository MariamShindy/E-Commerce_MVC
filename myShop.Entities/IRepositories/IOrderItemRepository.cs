using myShop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myShop.Entities.Repositories
{
    public interface IOrderItemRepository : IGenericRepository<OrderItem>
    {
        void Update(OrderItem orderItem);
    }
}
