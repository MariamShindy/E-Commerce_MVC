using myShop.Entities.Models;
using myShop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myShop.Entities.IRepositories
{
    public interface IProudctRepository :IGenericRepository<Product>
    {
        void Update(Product product);

    }
}
