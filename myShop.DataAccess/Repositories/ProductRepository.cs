using myShop.Entities.Models;
using myShop.Entities.IRepositories;

namespace myShop.DataAccess.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProudctRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var ProductInDb = _context.Products.FirstOrDefault(x => x.Id == product.Id);
            if (ProductInDb != null)
            {
                ProductInDb.Price = product.Price;
                ProductInDb.Img = product.Img;
                ProductInDb.Name = product.Name;
                ProductInDb.Description = product.Description;
                ProductInDb.CategoryId = product.CategoryId;
            }
        }
    }
}
