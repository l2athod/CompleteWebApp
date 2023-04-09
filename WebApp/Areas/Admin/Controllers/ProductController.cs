using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyApp.CommonHelper;
using MyApp.DataAccessLayer.DataLayer.IRepository;
using MyApp.Models.ViewModels;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = WebSiteRole.Role_Admin)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

    public class ProductController : Controller
    {
        private readonly IUnitOfWork database;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductController(IUnitOfWork _unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            database = _unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
        }

        #region APICALL
        public IActionResult GetAllProducts()
        {
            var products = database.Product.GetAll(includeItems: "Category");
            return Json(new { data = products });
        }
        #endregion
        
        public IActionResult Index()
        {
            ProductViewModel productViewModel = new ProductViewModel();
            productViewModel.Products = database.Product.GetAll();
            return View(productViewModel);
        }
        [HttpGet]
        public IActionResult CreateUpdate(int? id)
        {
            ProductViewModel productViewModel = new ProductViewModel()
            {
                Product = new(),
                Categories = database.Category.GetAll().Select(x => new SelectListItem()
                {
                    Text = x.CategoryName,
                    Value = x.CategoryId.ToString()
                })
            };
            if (id is null or 0)
            {
                return View(productViewModel);
            }
            else
            {
                productViewModel.Product = database.Product.Get(x => x.ProductId == id);
                if (productViewModel.Product is null)
                {
                    return NotFound();
                }
                return View(productViewModel);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUpdate(ProductViewModel productViewModel, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string fileName = string.Empty;
                if (file is not null)
                {
                    string uploadDirectory = Path.Combine(webHostEnvironment.WebRootPath, "ProductImages");
                    fileName = Guid.NewGuid().ToString() + "-" + file.FileName;
                    string filePath = Path.Combine(uploadDirectory, fileName);
                    if (productViewModel.Product.ImageUrl is not null)
                    {
                        var oldFilePath = Path.Combine(webHostEnvironment.WebRootPath, productViewModel.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productViewModel.Product.ImageUrl = @"\ProductImages\" + fileName;
                }
                if (productViewModel.Product.ProductId == 0)
                {
                    database.Product.Add(productViewModel.Product);
                    TempData["success"] = "Product created successfully...";
                }
                else
                {
                    database.Product.Update(productViewModel.Product);
                    TempData["success"] = "Product updated successfully...";
                }
                database.Save();
                return RedirectToAction("Index");
            }
            return View(productViewModel);
        }

        #region DeleteAPICall
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var product = database.Product.Get(x => x.ProductId == id);
            if (product is null)
            {
                return Json(new { success = false, message = "Error while fetching data" }); ;
            }
            else
            {
                var oldFilePath = Path.Combine(webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                database.Product.Delete(product);
                database.Save();
                return Json(new { success = true, message = "successfully fetch the data " });
            }
        }
        #endregion
    }
}
