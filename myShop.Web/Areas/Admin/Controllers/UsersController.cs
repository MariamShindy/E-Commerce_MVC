using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myShop.DataAccess;
using System.Security.Claims;
using Utilities;

namespace myShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles=Roles.AdminRole)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UsersController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public IActionResult Index()
        {
			//var claimsIdentity = (ClaimsIdentity)User.Identity;
			//var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

			if (claim == null)
			{ 
				return Unauthorized();
			}
			string userId = claim.Value;

			return View(_applicationDbContext.ApplicationUsers.Where(u => u.Id != userId).ToList());
        }
        public IActionResult LockUnlock(string? id)
		{
			var user = _applicationDbContext.ApplicationUsers.FirstOrDefault(u => u.Id == id);
			if (user == null)
			{
				return NotFound();
			}
			if (user.LockoutEnd == null || user.LockoutEnd < DateTime.Now)
			{
				user.LockoutEnd = DateTime.Now.AddYears(1);
			}
			else
			{
				user.LockoutEnd = DateTime.Now;
			}
			_applicationDbContext.SaveChanges();
			return RedirectToAction("Index","Users",new {area = "Admin"});
		}


	}
}
