using Microsoft.AspNetCore.Mvc;
using MyApp.DataAccessLayer.DataLayer.IRepository;
using System.Security.Claims;

namespace WebApp.ViewComponents
{
    public class CartViewComponent: ViewComponent
    {
        private readonly IUnitOfWork database;

        public CartViewComponent(IUnitOfWork _database)
        {
            database = _database;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claims != null)
            {
                if (HttpContext.Session.GetInt32("CartCount") != null)
                {
                    return View(HttpContext.Session.GetInt32("CartCount"));
                }
                else
                {
                    var count = database.Cart.GetAll(x => x.ApplicationUserId == claims.Value).ToList().Count();
                    HttpContext.Session.SetInt32("CartCount", count);
                    return View(count);
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View();
            }
        }
    }
}
