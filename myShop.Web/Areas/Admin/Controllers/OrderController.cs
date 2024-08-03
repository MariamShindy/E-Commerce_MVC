using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myShop.Entities.IRepositories;
using myShop.Entities.Models;
using myShop.Entities.ViewModels;
using Stripe;
using Utilities;

namespace myShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize (Roles=Roles.AdminRole)]

	public class OrderController : Controller
    {
        [BindProperty]
        public OrderViewModel OrderViewModel { get; set; }

        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
          _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetData()
        {
            IEnumerable<Order> orders;
            orders = _unitOfWork._OrderRepository.GetAll(includeWord:"ApplicationUser");
            return Json( new {data = orders});
        }
        public IActionResult Details(int orderId)
        {
            OrderViewModel orderViewModel = new OrderViewModel()
            {
                Order = _unitOfWork._OrderRepository.GetFirstOrDefault(o => o.Id == orderId, includeWord: "ApplicationUser"),
                OrderItems = _unitOfWork._OrderItemRepository.GetAll(o => o.OrderId == orderId, includeWord: "Product")
            };
            return View(orderViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
		public IActionResult Details()
		{
            var orderDb = _unitOfWork._OrderRepository.GetFirstOrDefault(o => o.Id == OrderViewModel.Order.Id);
            orderDb.Name = OrderViewModel.Order.Name;
			orderDb.Phone = OrderViewModel.Order.Phone;
			orderDb.City = OrderViewModel.Order.City;
			orderDb.Address = OrderViewModel.Order.Address;

            if(OrderViewModel.Order.Carrier != null)
                orderDb.Carrier = OrderViewModel.Order.Carrier;
			if (OrderViewModel.Order.TrackingNumber != null)
				orderDb.TrackingNumber = OrderViewModel.Order.TrackingNumber;

            _unitOfWork._OrderRepository.Update(orderDb);
            _unitOfWork.Complete();
			TempData["update"] = "Data has updated successfully !";
			return RedirectToAction("Details", "Order", new {orderId = orderDb.Id});
		}
        [HttpPost]
        [ValidateAntiForgeryToken]
		public IActionResult StartProcessing()
		{
            _unitOfWork._OrderRepository.UpdateOrderStatus(OrderViewModel.Order.Id, Status.Processing, null);
            _unitOfWork.Complete();

			TempData["update"] = "Order status has updated successfully !";
			return RedirectToAction("Details", "Order", new { orderId = OrderViewModel.Order.Id });
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartShipping()
		{
			var orderDb = _unitOfWork._OrderRepository.GetFirstOrDefault(o => o.Id == OrderViewModel.Order.Id);
            orderDb.TrackingNumber = OrderViewModel.Order.TrackingNumber;
            orderDb.Carrier = OrderViewModel.Order.Carrier;
            orderDb.OrderStatus = Status.Shipped;
            orderDb.ShippingDate = DateTime.Now;
			_unitOfWork._OrderRepository.Update(orderDb);
			_unitOfWork.Complete();

			TempData["update"] = "Order has shipped successfully !";
			return RedirectToAction("Details", "Order", new { orderId = OrderViewModel.Order.Id });
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CancelOrder()
		{
			var orderDb = _unitOfWork._OrderRepository.GetFirstOrDefault(o => o.Id == OrderViewModel.Order.Id);
			if(orderDb .PaymentStatus == Status.Approved)
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderDb.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(option);
                _unitOfWork._OrderRepository.UpdateOrderStatus(orderDb.Id, Status.Cancelled, Status.Refund);
            }
            else
            {
				_unitOfWork._OrderRepository.UpdateOrderStatus(orderDb.Id, Status.Cancelled, Status.Cancelled);
			}
            _unitOfWork.Complete();
			TempData["update"] = "Order has cancelled successfully !";
			return RedirectToAction("Details", "Order", new { orderId = OrderViewModel.Order.Id });
		}
	}
}
