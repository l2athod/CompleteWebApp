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
        public DateTime DateOfOrder { get; set; }
        public DateTime DateOfShipping { get; set; }
        public double OrderTotal { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set;}
        public string? TrackingNumber { get; set;}
        public string? Carrier{ get; set;}
        public string? SessionId{ get; set; }
        public string? PaymentIntentId { get; set;}
        public DateTime DateOfPayment { get; set; }
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
