using Microsoft.EntityFrameworkCore;
using myShop.Web.Models;

namespace myShop.Web.Data
{
	public class ApplicationDbContext : DbContext 
	{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            
        }
        public DbSet<Category> Categories { get; set; }

    }
}
