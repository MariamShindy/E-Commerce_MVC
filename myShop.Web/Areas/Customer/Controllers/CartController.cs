using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myShop.Entities.IRepositories;
using myShop.Entities.Models;
using myShop.Entities.ViewModels;
using Stripe.BillingPortal;
using Stripe.Checkout;
using Stripe.FinancialConnections;
using System.Security.Claims;
using Utilities;

namespace myShop.Web.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
        public  ShoppingCartViewModel ShoppingCartViewModel { get; set; }
        public int TotalCarts { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
		}
        public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

			if (claim == null)
			{
				return Unauthorized();
			}
			ShoppingCartViewModel = new ShoppingCartViewModel() 
			{ 
			CartsList = _unitOfWork._ShoppingCartRepository.GetAll(u => u.ApplicationUserId == claim.Value,includeWord:"Product"),

		    TotalCarts = 0 // new
			};
			foreach(var item in ShoppingCartViewModel.CartsList)
			{
				ShoppingCartViewModel.TotalCarts += (item.Count * item.Product.Price);
				Console.WriteLine($"Item: {item.Product.Name}, Price: {item.Product.Price}, Count: {item.Count}, Total: {ShoppingCartViewModel.TotalCarts}");
			}
 			return View(ShoppingCartViewModel);
		}
		[HttpGet]
		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

			if (claim == null)
			{
				return Unauthorized();
			}

			ShoppingCartViewModel = new ShoppingCartViewModel()
			{
				CartsList = _unitOfWork._ShoppingCartRepository.GetAll(u => u.ApplicationUserId == claim.Value, includeWord: "Product"),
				Order = new()
			};
			ShoppingCartViewModel.Order.ApplicationUser = _unitOfWork._ApplicationUserRepository.GetFirstOrDefault(x => x.Id == claim.Value);
			ShoppingCartViewModel.Order.Name = ShoppingCartViewModel.Order.ApplicationUser.Name;
			ShoppingCartViewModel.Order.Address = ShoppingCartViewModel.Order.ApplicationUser.Address;
			ShoppingCartViewModel.Order.City = ShoppingCartViewModel.Order.ApplicationUser.City;
			ShoppingCartViewModel.Order.Phone = ShoppingCartViewModel.Order.ApplicationUser.PhoneNumber;
			
			ShoppingCartViewModel.TotalCarts = 0; // new
			foreach (var item in ShoppingCartViewModel.CartsList)
			{
				ShoppingCartViewModel.TotalCarts += (item.Count * item.Product.Price);
				Console.WriteLine($"Item: {item.Product.Name}, Price: {item.Product.Price}, Count: {item.Count}, Total: {ShoppingCartViewModel.TotalCarts}");
			}
			//new
			ShoppingCartViewModel.Order.TotalPrice = ShoppingCartViewModel.TotalCarts;
			return View(ShoppingCartViewModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Summary(ShoppingCartViewModel shoppingCartViewModel)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

			if (claim == null)
			{
				return Unauthorized();
			}
			if (shoppingCartViewModel.Order == null)
			{
				shoppingCartViewModel.Order = new Order();
			}
			var user = _unitOfWork._ApplicationUserRepository.GetFirstOrDefault(x => x.Id == claim.Value);
			if (user == null)
			{
				return NotFound();
			}
			shoppingCartViewModel.CartsList = _unitOfWork._ShoppingCartRepository.GetAll(u => u.ApplicationUserId == claim.Value, includeWord: "Product");
            shoppingCartViewModel.Order.OrderStatus = Status.Pending;
			shoppingCartViewModel.Order.PaymentStatus = Status.Pending;
			shoppingCartViewModel.Order.OrderDate = DateTime.Now;
			shoppingCartViewModel.Order.ApplicationUserId = claim.Value;

			foreach (var item in shoppingCartViewModel.CartsList)
			{
				shoppingCartViewModel.TotalCarts += (item.Count * item.Product.Price);
			}
			shoppingCartViewModel.Order.TotalPrice = shoppingCartViewModel.TotalCarts;
			_unitOfWork._OrderRepository.Add(shoppingCartViewModel.Order);
			_unitOfWork.Complete();
			foreach (var item in shoppingCartViewModel.CartsList)
			{
				OrderItem orderItem = new OrderItem()
				{
					ProductId = item.ProductId,
					OrderId = shoppingCartViewModel.Order.Id,
					Price = item.Product.Price,
					Count = item.Count
				};
				_unitOfWork._OrderItemRepository.Add(orderItem);
				_unitOfWork.Complete();
			}
			string sessionUrl = StripeOperations(shoppingCartViewModel);
			_unitOfWork.Complete();
			Response.Headers.Add("Location", sessionUrl);
			return new StatusCodeResult(StatusCodes.Status303SeeOther);
		}
		public string StripeOperations(ShoppingCartViewModel shoppingCartViewModel)
		{
			var domain = "https://localhost:44309/";
			var options = new Stripe.Checkout.SessionCreateOptions
			{
				LineItems = new List<SessionLineItemOptions>(),
				SuccessUrl = domain + $"customer/cart/orderconfirmation?id={shoppingCartViewModel.Order.Id}",
				Mode = "payment",
				CancelUrl = domain + $"customer/cart/index"
			};
			foreach (var item in shoppingCartViewModel.CartsList)
			{
				var sessionLineOption = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Product.Price * 100),
						Currency = "usd",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Name
						},
					},
					Quantity = item.Count,
				};
				options.LineItems.Add(sessionLineOption);
			}
			var service = new Stripe.Checkout.SessionService();
			Stripe.Checkout.Session session = service.Create(options);
			shoppingCartViewModel.Order.SessionId = session.Id;
			return session.Url;
		}
		public IActionResult OrderConfirmation(int id)
		{
			Order order = _unitOfWork._OrderRepository.GetFirstOrDefault(x => x.Id == id);
			var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Get(order.SessionId);
			if(session.PaymentStatus.ToLower() == "paid")
			{
				_unitOfWork._OrderRepository.UpdateOrderStatus(id, Status.Approved, Status.Approved);
				order.PaymentIntentId = session.PaymentIntentId;
				_unitOfWork.Complete();
			}
			List<ShoppingCart> shoppingCarts = _unitOfWork._ShoppingCartRepository.GetAll(u => u.ApplicationUserId == order.ApplicationUserId).ToList();
			_unitOfWork._ShoppingCartRepository.RemoveRange(shoppingCarts);
			_unitOfWork.Complete();
			return View(id);
		}
		public IActionResult Plus(int cartId)
		{
			var shoppingCart = _unitOfWork. _ShoppingCartRepository.GetFirstOrDefault(x => x.Id == cartId);
			_unitOfWork._ShoppingCartRepository.IncreaseCount(shoppingCart,1);
			_unitOfWork.Complete();
			return RedirectToAction("Index");
		}
		public IActionResult Minus(int cartId)
		{
			var shoppingCart = _unitOfWork._ShoppingCartRepository.GetFirstOrDefault(x => x.Id == cartId);
			if(shoppingCart.Count <= 1)
			{
				_unitOfWork._ShoppingCartRepository.Remove(shoppingCart);
				_unitOfWork.Complete();
				return RedirectToAction("Index","Home");
			}
			else 
			{ 	
			_unitOfWork._ShoppingCartRepository.DecreaseCount(shoppingCart, 1);
			}
			_unitOfWork.Complete();
			return RedirectToAction("Index");

		}
		public IActionResult Remove(int cartId)
		{
			var shoppingCart = _unitOfWork._ShoppingCartRepository.GetFirstOrDefault(x => x.Id == cartId);
			_unitOfWork._ShoppingCartRepository.Remove(shoppingCart);
			_unitOfWork.Complete();
			return RedirectToAction("Index");
		}
	}
}
