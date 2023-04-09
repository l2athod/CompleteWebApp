using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using MyApp.CommonHelper;
using MyApp.DataAccessLayer.DataLayer.IRepository;
using MyApp.Models;
using MyApp.Models.ViewModels;
using Stripe.Checkout;
using System.Security.Claims;

namespace WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

    public class CartController : Controller
    {
        private readonly IUnitOfWork database;
        private CartViewModel cart { get; set; }

        public CartController(IUnitOfWork _database)
        {
            database = _database;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            cart = new CartViewModel()
            {
                ListOfCart = database.Cart.GetAll (x=> x.ApplicationUserId==claims.Value, includeItems: "Product"),
                OrderHeader = new OrderHeader()
            };

            foreach (var cart in cart.ListOfCart)
            {
                this.cart.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
            }
            return View(cart);
        }
        [HttpPost]
        public IActionResult plus(int id)
        {
            var cart = database.Cart.Get(x => x.CartId == id);
            database.Cart.AddQuantity(cart, 1);
            database.Save();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult minus(int id)
        {
            var cart = database.Cart.Get(x => x.CartId == id);
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            if(cart.Count <= 1)
            {
                database.Cart.Delete(cart);
                var count = database.Cart.GetAll(x => x.ApplicationUserId == claims.Value).ToList().Count();

                HttpContext.Session.SetInt32("CartCount",count-1);
            }
            else
            {
                database.Cart.DeleteQuantity(cart, 1);
            }
            database.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult OrderSuccess(int id)
        {
            var orderHeader = database.OrderHeader.Get(x => x.OrderHeaderId == id);
            var service = new SessionService();
            var session = service.Get(orderHeader.SessionId);


            database.OrderHeader.PaymentStatus(id, session.Id, session.PaymentIntentId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                database.OrderHeader.UpdateStatus(id, OrderStatus.StatusApproved, PaymentStatus.StatusApproved);
            }
            List<Cart> carts = database.Cart.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            database.Cart.DeleteRange(carts);
            database.Save();

            var routeValue = new RouteValueDictionary (new { action = "Index", controller = "Order", area = "Admin" });
            return RedirectToRoute(routeValue);
        }

        [HttpGet]
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            cart = new CartViewModel()
            {
                ListOfCart = database.Cart.GetAll(x => x.ApplicationUserId == claims.Value, includeItems: "Product"),
                OrderHeader = new OrderHeader()
            };
            cart.OrderHeader.ApplicationUser = database.ApplicationUser.Get(x => x.Id == claims.Value);
            cart.OrderHeader.Name = cart.OrderHeader.ApplicationUser.Name;
            cart.OrderHeader.Phone = cart.OrderHeader.ApplicationUser.PhoneNumber;
            cart.OrderHeader.Address = cart.OrderHeader.ApplicationUser.Address;
            cart.OrderHeader.City = cart.OrderHeader.ApplicationUser.City;
            cart.OrderHeader.State = cart.OrderHeader.ApplicationUser.State;
            cart.OrderHeader.PostalCode = cart.OrderHeader.ApplicationUser.PinCode;


            foreach (var cartitem in cart.ListOfCart)
            {
                cart.OrderHeader.OrderTotal += (cartitem.Product.Price * cartitem.Count);
            }
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(CartViewModel cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            cart.ListOfCart = database.Cart.GetAll(x => x.ApplicationUserId == claims.Value,includeItems: "Product");
            cart.OrderHeader.OrderStatus = OrderStatus.StatusPending;
            cart.OrderHeader.PaymentStatus = PaymentStatus.StatusPending;
            cart.OrderHeader.DateOfOrder = DateTime.Now;
            cart.OrderHeader.ApplicationUserId= claims.Value;
            
            foreach (var cartitem in cart.ListOfCart)
            {
                cart.OrderHeader.OrderTotal += (cartitem.Product.Price * cartitem.Count);
            }

            database.OrderHeader.Add(cart.OrderHeader);
            database.Save();

            foreach(var item in cart.ListOfCart)
            {
                var orderDetail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = cart.OrderHeader.OrderHeaderId,
                    Count = item.Count,
                    Price = item.Product.Price
                };
                database.OrderDetail.Add(orderDetail);
                database.Save();
            }

            var domain = "https://localhost:7289/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain  + $"customer/Cart/OrderSuccess?id={cart.OrderHeader.OrderHeaderId}",
                CancelUrl = domain + $"customer/Cart/Index",
            };

            foreach (var item in cart.ListOfCart)
            {
                var lineItemOption = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = ((long)item.Product.Price)*100,
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
            database.OrderHeader.PaymentStatus(cart.OrderHeader.OrderHeaderId, session.Id, session.PaymentIntentId);
            database.Save();

            database.Cart.DeleteRange(cart.ListOfCart);
            database.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}
