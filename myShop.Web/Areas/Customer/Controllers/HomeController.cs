using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myShop.Entities.IRepositories;
using myShop.Entities.Models;
using System.Security.Claims;
using System.Security.Cryptography;

namespace myShop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var products = _unitOfWork._ProductRepository.GetAll();
            return View(products);
        }
        public IActionResult Details(int Id)
        {
			var product = _unitOfWork._ProductRepository.GetFirstOrDefault(p => p.Id == Id, includeWord: "Category");

			if (product == null)
			{
				return NotFound();
			}
			ShoppingCart shoppingCart = new()
            {
                Product = product,
                Count = 1
            };
             return View(shoppingCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
		public IActionResult Details(ShoppingCart shoppingCart)
		{
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return Unauthorized();
            }
            shoppingCart.ApplicationUserId = claim.Value;
			shoppingCart.Product = _unitOfWork._ProductRepository.GetFirstOrDefault(p => p.Id == shoppingCart.Product.Id);

			if (shoppingCart.Product == null)
			{
				return NotFound();
			}
			shoppingCart.Id = 0;

			ShoppingCart shoppingCartObj = _unitOfWork._ShoppingCartRepository.GetFirstOrDefault(s => s.ApplicationUserId == claim.Value && s.Product.Id == shoppingCart.Product.Id);
			
			if (shoppingCartObj == null)
            {
				Console.WriteLine("**************No existing shopping cart found, adding new one**************.");
				_unitOfWork._ShoppingCartRepository.Add(shoppingCart);
			}
            else
            {
                Console.WriteLine("**************Existing shopping cart found, updating count.**************");
				_unitOfWork._ShoppingCartRepository.IncreaseCount(shoppingCartObj, shoppingCart.Count);
            }
			_unitOfWork.Complete();
			return RedirectToAction("Index");
		}
	}
}
 