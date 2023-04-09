using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Models
{
    public class OrderHeader
    {
        public int OrderHeaderId { get; set; }
        public string ApplicationUserId { get; set; }
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Required]
        [Display(Name="Order Date")]
        public DateTime DateOfOrder { get; set; }
        [Display(Name = "Shipping Date")]
        public DateTime DateOfShipping { get; set; }
        public double OrderTotal { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set;}
        public string? TrackingNumber { get; set;}
        public string? Carrier{ get; set;}
        public string? SessionId{ get; set; }
        public string? PaymentIntentId { get; set;}
        [Display(Name = "Payment Date")]
        public DateTime DateOfPayment { get; set; }
        [Display(Name = "Due Date")]
        public DateTime DueDate { set; get; }
        [Required]
        public string Phone { set; get; }
        [Required]
        public string Address { set; get; }
        [Required]
        public string City { set; get; }
        [Required]
        public string State { set; get; }
        [Required]
        public string PostalCode { set; get; }
        [Required]
        public string Name { set; get; }

    }
}
