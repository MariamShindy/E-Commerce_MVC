using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using myShop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myShop.Entities.ViewModels
{
	public class ProductViewModel
	{
        public Product Product { get; set; }
		[ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
