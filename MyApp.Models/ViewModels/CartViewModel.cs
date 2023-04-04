using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Models.ViewModels
{
    public class CartViewModel
    {
        public List<Cart> ListOfCart { get; set;}
        public OrderHeader OrderHeader{ get; set;}    
    }
}
