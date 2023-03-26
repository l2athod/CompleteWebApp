using Microsoft.AspNetCore.Mvc;
using MyApp.DataAccessLayer.DataLayer.IRepository;
using MyApp.Models;
using System.Diagnostics;

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

        public IActionResult Index()
        {
            IEnumerable<Product> products = database.Product.GetAll(includeItems: "Category");
            return View(products);
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
