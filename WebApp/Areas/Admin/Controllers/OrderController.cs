using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.CommonHelper;
using MyApp.DataAccessLayer.DataLayer.IRepository;
using MyApp.Models;
using MyApp.Models.ViewModels;
using Stripe;
using Stripe.Checkout;
using Stripe.Issuing;
using System.Security.Claims;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork database;

        public OrderController(IUnitOfWork _database)
        {
            database = _database;
        }

        #region GetAllOrders API
        public JsonResult GetAllOrders(string status)
        {
            IEnumerable<OrderHeader> orderHeader;
            if (User.IsInRole(WebSiteRole.Role_Admin) || User.IsInRole(WebSiteRole.Role_Employee))
            {
                orderHeader = database.OrderHeader.GetAll(includeItems: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                orderHeader = database.OrderHeader.GetAll(x => x.ApplicationUserId == claims.Value, includeItems: "ApplicationUser");
            }

            switch (status)
            {
                case "pending": orderHeader = orderHeader.Where(x => x.OrderStatus == PaymentStatus.StatusPending); break;
                case "approved": orderHeader = orderHeader.Where(x => x.OrderStatus == PaymentStatus.StatusApproved); break;
                case "underprocess": orderHeader = orderHeader.Where(x => x.OrderStatus == OrderStatus.StatusInProcess); break;
                case "shipped": orderHeader = orderHeader.Where(x => x.OrderStatus == OrderStatus.StatusShipped); break;
                default: break;
            }
            return Json(new { data = orderHeader });
        }
        #endregion
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult OrderDetail(int id)
        {
            var orderViewModel = new OrderViewModel()
            {
                OrderHeader = database.OrderHeader.Get(x => x.OrderHeaderId == id, includeItems: "ApplicationUser"),
                OrderDetails = database.OrderDetail.GetAll(x => x.OrderHeaderId == id, includeItems: "Product")
            };
            return View(orderViewModel);
        }
        [HttpPost]
        [Authorize(Roles = WebSiteRole.Role_Admin + "," + WebSiteRole.Role_Employee)]
        public IActionResult OrderDetail(OrderViewModel order)
        {
            var orderHeader = database.OrderHeader.Get(x => x.OrderHeaderId == order.OrderHeader.OrderHeaderId);
            orderHeader.Name = order.OrderHeader.Name;
            orderHeader.Phone = order.OrderHeader.Phone;
            orderHeader.Address = order.OrderHeader.Address;
            orderHeader.City = order.OrderHeader.City;
            orderHeader.State = order.OrderHeader.State;
            orderHeader.PostalCode = order.OrderHeader.PostalCode;
            if (order.OrderHeader.Carrier != null)
            {
                orderHeader.Carrier = order.OrderHeader.Carrier;
            }
            if (order.OrderHeader.TrackingNumber != null)
            {
                orderHeader.TrackingNumber = order.OrderHeader.TrackingNumber;
            }
            database.OrderHeader.Update(orderHeader);
            database.Save();
            TempData["success"] = "OrderHeader Updated Successfully";
            return RedirectToAction("OrderDetail", "Order", new { id = order.OrderHeader.OrderHeaderId });
        }

        [Authorize(Roles = WebSiteRole.Role_Admin + "," + WebSiteRole.Role_Employee)]
        public IActionResult InProcess(OrderViewModel order)
        {
            database.OrderHeader.UpdateStatus(order.OrderHeader.OrderHeaderId, OrderStatus.StatusInProcess);
            database.Save();
            TempData["success"] = "Order Status Updated to InProcess";
            return RedirectToAction("OrderDetail", "Order", new { id = order.OrderHeader.OrderHeaderId });
        }


        [Authorize(Roles = WebSiteRole.Role_Admin + "," + WebSiteRole.Role_Employee)]
        public IActionResult Shipped(OrderViewModel order)
        {
            var orderHeader = database.OrderHeader.Get(x => x.OrderHeaderId == order.OrderHeader.OrderHeaderId);
            orderHeader.Carrier = order.OrderHeader.Carrier;
            orderHeader.TrackingNumber = order.OrderHeader.TrackingNumber;
            orderHeader.OrderStatus = OrderStatus.StatusShipped;
            orderHeader.DateOfShipping = DateTime.Now;

            database.OrderHeader.Update(orderHeader);
            database.Save();
            TempData["success"] = "Order Status Updated to Shipped";
            return RedirectToAction("OrderDetail", "Order", new { id = order.OrderHeader.OrderHeaderId });
        }

        [Authorize(Roles = WebSiteRole.Role_Admin + "," + WebSiteRole.Role_Employee)]
        public IActionResult Cancel(OrderViewModel order)
        {
            var orderHeader = database.OrderHeader.Get(x => x.OrderHeaderId == order.OrderHeader.OrderHeaderId);
            if (orderHeader.PaymentStatus == PaymentStatus.StatusApproved)
            {
                var options = new RefundCreateOptions()
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = order.OrderHeader.PaymentIntentId
                };
                var service = new RefundService();
                var refund = service.Create(options);
                database.OrderHeader.UpdateStatus(order.OrderHeader.OrderHeaderId, OrderStatus.StatusCancelled, OrderStatus.StatusRefund);
            }
            else
            {
                database.OrderHeader.UpdateStatus(order.OrderHeader.OrderHeaderId, OrderStatus.StatusCancelled, OrderStatus.StatusCancelled);
            }
            database.Save();
            TempData["success"] = "Order Status Updated to Cancelled";
            return RedirectToAction("OrderDetail", "Order", new { id = order.OrderHeader.OrderHeaderId });
        }

        public IActionResult PayNow(OrderViewModel order)
        {
            var orderHeader = database.OrderHeader.Get(x => x.OrderHeaderId == order.OrderHeader.OrderHeaderId, includeItems: "ApplicationUser");
            var orderDetails = database.OrderDetail.GetAll(x => x.OrderHeaderId == order.OrderHeader.OrderHeaderId, includeItems: "Product");

            var domain = "https://localhost:7289/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"customer/Cart/OrderSuccess?id={order.OrderHeader.OrderHeaderId}",
                CancelUrl = domain + $"customer/Cart/Index",
            };

            foreach (var item in orderDetails)
            {
                var lineItemOption = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = ((long)item.Product.Price) * 100,
                        Currency = "INR",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.ProductName,
                        },
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(lineItemOption);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            database.OrderHeader.PaymentStatus(order.OrderHeader.OrderHeaderId, session.Id, session.PaymentIntentId);
            database.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}
