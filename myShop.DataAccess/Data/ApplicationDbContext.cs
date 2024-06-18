using Microsoft.EntityFrameworkCore;
using myShop.Entities.Models;

namespace myShop.DataAccess
{
	public class ApplicationDbContext : DbContext 
	{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            
        }
        public DbSet<Category> Categories { get; set; }

    }
}
