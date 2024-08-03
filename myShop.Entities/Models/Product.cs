using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace myShop.Entities.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [DisplayName("Image")]
        [ValidateNever]
        public string Img { get; set; }
		[ValidateNever]
		public Category Category { get; set; }
        [Required]
		[DisplayName("Category")]
		public int CategoryId { get; set; }

    }
}
