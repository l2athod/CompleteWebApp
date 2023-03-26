using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Models.ViewModels
{
    public class CategoryViewModel
    {
        // if we don't initialize category then it will pass null reference while creating new category.
        // Hence it is neccessary to initialize category before creating it and we are assigning memory to that category at runtime.
        public Category Category { get; set; } = new Category();
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}
