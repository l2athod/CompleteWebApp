using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        // null! means not equals to null
        public string ProductName { get; set; } = null!;
        [Required]
        public string Description { get; set; }
        [Required]
        public double Price { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        // [NotMapped]      -> It will not intefear while checking for model validation
        //public string FullName => $"{FirstName} {LastName}";
        [ValidateNever]
        public Category Category { get; set; }
    }
}
