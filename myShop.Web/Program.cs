using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using myShop.DataAccess;
using myShop.DataAccess.Repositories;
using myShop.Entities.IRepositories;
using myShop.Entities.Models;
using Stripe;
using Utilities;

namespace myShop.Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();
			builder.Services.AddRazorPages();
			builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
			builder.Configuration.GetConnectionString("DefaultConnection"),
			 sqlServerOptionsAction: sqlOptions =>
			 {
				 sqlOptions.EnableRetryOnFailure(
					 maxRetryCount: 5,
					 maxRetryDelay: TimeSpan.FromSeconds(10),
					 errorNumbersToAdd: null);
			 })
			);

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(4)).AddDefaultTokenProviders().AddDefaultUI()
            .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
			builder.Services.AddSingleton<IEmailSender, EmailSender>();
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();
			app.MapRazorPages();
			app.MapControllerRoute(
				name: "default",
				pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "Customer",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
		}
	}
}
