using Microsoft.AspNetCore.Mvc;
using myShop.Entities.IRepositories;
using System.Security.Claims;
using Utilities;
namespace myShop.Web.ViewComponents
{
	public class ShoppingCartViewComponent : ViewComponent
	{
		private readonly IUnitOfWork _unitOfWork;

		public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var claimIdentity = (ClaimsIdentity)User.Identity;
			var claim =   claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
			if (claim != null)
			{
				if (HttpContext.Session.GetInt32(CartSession.CartSessionKey) != null)
				{
					return  View(HttpContext.Session.GetInt32(CartSession.CartSessionKey));
				}
				else
				{
					 HttpContext.Session.SetInt32(CartSession.CartSessionKey, _unitOfWork._ShoppingCartRepository.GetAll(s => s.ApplicationUserId == claim.Value).ToList().Count());
					return View(HttpContext.Session.GetInt32(CartSession.CartSessionKey));
				}
			}	
				HttpContext.Session.Clear();
				return View(0);		
		}
    }
}
