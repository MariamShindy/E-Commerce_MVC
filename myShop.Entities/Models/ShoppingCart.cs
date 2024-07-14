using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myShop.Entities.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
		public int ProductId { get; set; }

		[ForeignKey("ApplicationUserId")]
        [ValidateNever]
		public ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("ProductId")]
		[ValidateNever]
		public Product Product { get; set; }
        [Range(1, 10, ErrorMessage = "You must enter a value between 1 and 10")]
        public int Count { get; set; }

    }
}
