using myShop.Entities.Models;
using myShop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace myShop.DataAccess.Repositories
{
	public class OrderRepository : GenericRepository<Order> ,IOrderRepository
	{
		private readonly ApplicationDbContext _applicationDbContext;

		public OrderRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
			_applicationDbContext = applicationDbContext;
		}
 
		public void Update(Order order)
		{
			_applicationDbContext.Orders.Update(order);
		}

		public void UpdateOrderStatus(int id, string orderStatus, string? paymentStatus)
		{
			var orderFromDb = _applicationDbContext.Orders.FirstOrDefault(o => o.Id == id);
			if (orderFromDb != null)
			{
				orderFromDb.OrderStatus = orderStatus;
				orderFromDb.PaymentDate = DateTime.Now;
				if(paymentStatus != null)
				{ 
				orderFromDb.PaymentStatus = paymentStatus;
				}
			}
		}
	}
}
