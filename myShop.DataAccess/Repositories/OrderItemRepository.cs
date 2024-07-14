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
	public class OrderItemRepository : GenericRepository<OrderItem> ,IOrderItemRepository
	{
		private readonly ApplicationDbContext _applicationDbContext;

		public OrderItemRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
			_applicationDbContext = applicationDbContext;
		}

		public void Update(OrderItem orderItem)
		{ 
			_applicationDbContext.OrderItems.Update(orderItem);
		}
	}
}
