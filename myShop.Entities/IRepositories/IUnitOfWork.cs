using myShop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myShop.Entities.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository _CategoryRepository { get; }
        int Complete();
    }
}
