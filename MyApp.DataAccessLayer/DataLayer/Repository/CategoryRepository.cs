using MyApp.DataAccessLayer.Data;
using MyApp.DataAccessLayer.DataLayer.IRepository;
using MyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DataAccessLayer.DataLayer.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Category category)
        {
            var CategoryItem = _context.Categories.FirstOrDefault(x => x.CategoryId == category.CategoryId);
            if (CategoryItem != null)
            {
                CategoryItem.CategoryName = category.CategoryName;
                CategoryItem.OrderedDate = category.OrderedDate;
                CategoryItem.DisplayOrder = category.DisplayOrder;
            }
        }
    }
}
