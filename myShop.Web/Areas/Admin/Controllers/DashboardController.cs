using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myShop.Entities.IRepositories;
using Utilities;

namespace myShop.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles =Roles.AdminRole)]
	public class DashboardController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public DashboardController(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
		}
        public IActionResult Index()
		{
			ViewBag.Orders = _unitOfWork._OrderRepository.GetAll().Count();
			ViewBag.ApprovedOrders = _unitOfWork._OrderRepository.GetAll(o => o.OrderStatus == Status.Approved).Count();
			ViewBag.Products = _unitOfWork._ProductRepository.GetAll().Count();
			ViewBag.Users = _unitOfWork._ApplicationUserRepository.GetAll().Count();
			ViewBag.Categories = _unitOfWork._CategoryRepository.GetAll().Count();
            return View();
		}
	}
}
