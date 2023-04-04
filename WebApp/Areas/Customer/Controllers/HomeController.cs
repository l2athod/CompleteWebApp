using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.DataAccessLayer.DataLayer.IRepository;
using MyApp.Models;
using MyApp.Models.ViewModels;
using System.Diagnostics;
using System.Security.Claims;

namespace WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork database;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork database)
        {
            _logger = logger;
            this.database = database;
        }
        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Product> products = database.Product.GetAll(includeItems: "Category");
            return View(products);
        }

        [HttpGet]
        public IActionResult Details(int? productId)
        {
            Cart cart = new Cart()
            {
                Product = database.Product.Get(x => x.ProductId == productId, includeItems: "Category"),
                Count = 1,
                ProductId = (int)productId
            };
            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(Cart cart)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cart.ApplicationUserId = claims.Value;

                var oldCart = database.Cart.Get(x => x.ProductId == cart.ProductId && x.ApplicationUserId == cart.ApplicationUserId);

                if (oldCart is null)
                {
                    database.Cart.Add(cart);
                }
                else
                {
                    database.Cart.AddQuantity(oldCart, cart.Count);
                }
                database.Save();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
