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

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            _CategoryRepository = new CategoryRepository(context);
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
