using Microsoft.AspNetCore.Mvc;
using MyApp.DataAccessLayer.DataLayer.IRepository;
using MyApp.Models.ViewModels;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private IUnitOfWork database;
        public CategoryController(IUnitOfWork _unitOfWork)
        {
            database = _unitOfWork;
        }

        public IActionResult Index()
        {
            CategoryViewModel categoryViewModel = new CategoryViewModel();
            categoryViewModel.Categories = database.Category.GetAll();
            return View(categoryViewModel);
        }
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        database.Category.Add(category);
        //        database.Save();
        //        TempData["success"] = "Category added successfully...";
        //        return RedirectToAction("Index");
        //    }
        //    return View(category);
        //}

        [HttpGet]
        public IActionResult CreateUpdate(int? id)
        {
            CategoryViewModel categoryViewModel = new CategoryViewModel();
            if (id is null or 0)
            {
                return View(categoryViewModel);
            }
            else
            {
                categoryViewModel.Category = database.Category.Get(x => x.CategoryId == id);
                if (categoryViewModel.Category is null)
                {
                    return NotFound();
                }
                return View(categoryViewModel);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUpdate(CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                if (categoryViewModel.Category.CategoryId == 0)
                {
                    database.Category.Add(categoryViewModel.Category);
                    TempData["success"] = "Category created successfully...";
                }
                else
                {
                    database.Category.Update(categoryViewModel.Category);
                    TempData["success"] = "Category updated successfully...";
                }
                database.Save();

                return RedirectToAction("Index");
            }
            return View(categoryViewModel);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            CategoryViewModel categoryViewModel = new CategoryViewModel();
            if (id is null or 0)
            {
                return NotFound();
            }
            categoryViewModel.Category = database.Category.Get(x => x.CategoryId == id);
            if (categoryViewModel.Category is null)
            {
                return NotFound();
            }
            return View(categoryViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                database.Category.Delete(categoryViewModel.Category);
                database.Save();
                TempData["success"] = "Category deleted successfully...";
                return RedirectToAction("Index");
            }
            return View(categoryViewModel);
        }
    }
}
