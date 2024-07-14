using myShop.Entities.IRepositories;
using myShop.Entities.Models;
using myShop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myShop.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICategoryRepository _CategoryRepository { get; private set; }
        public IProudctRepository _ProductRepository { get; private set; }
		public IShoppingCartRepository _ShoppingCartRepository { get; private set; }
        public IOrderRepository  _OrderRepository { get; set; }
        public IOrderItemRepository _OrderItemRepository { get; set; }
        public IApplicationUserRepository _ApplicationUserRepository { get; set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            _CategoryRepository = new CategoryRepository(context);
            _ProductRepository = new ProductRepository(context);
            _ShoppingCartRepository = new ShoppingCartRepository(context);
			_OrderRepository = new OrderRepository(context);
			_OrderItemRepository = new OrderItemRepository(context);
			_ApplicationUserRepository = new ApplicationUserRepository(context);
		}

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
