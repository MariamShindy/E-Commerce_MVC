using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using myShop.Entities.IRepositories;
using myShop.Entities.Models;
using myShop.Entities.ViewModels;
using System.Drawing.Imaging;

namespace myShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetData()
        {
            var products = _unitOfWork._ProductRepository.GetAll(includeWord: "Category");

            return Json(new { data = products });
        }
        [HttpGet]
        public IActionResult Create()
        {
            ProductViewModel productVm = new ProductViewModel()
            {
                Product = new Product(),
                CategoryList = _unitOfWork._CategoryRepository.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };
            return View(productVm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductViewModel productVm, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var upload = Path.Combine(rootPath, @"images\products");
                    var imageExtension = Path.GetExtension(file.FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + imageExtension), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVm.Product.Img = @"\images\products\" + fileName + imageExtension;
                }
                _unitOfWork._ProductRepository.Add(productVm.Product);
                _unitOfWork.Complete();
                TempData["create"] = "Data has created successfully !";
                return RedirectToAction("Index");
            }
            return View(productVm.Product);
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var product = _unitOfWork._ProductRepository.GetFirstOrDefault(x => x.Id == id);
            if (id == null | id == 0 | product == null)
            {
                return NotFound();
            }
            ProductViewModel productVm = new ProductViewModel()
            {
                Product = product,
                CategoryList = _unitOfWork._CategoryRepository.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };
            return View(productVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductViewModel productVm, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var upload = Path.Combine(rootPath, @"images\products");
                    var imageExtension = Path.GetExtension(file.FileName);

                    if (productVm.Product.Img != null)
                    {
                        var oldImg = Path.Combine(rootPath, productVm.Product.Img.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImg))
                        {
                            System.IO.File.Delete(oldImg);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + imageExtension), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVm.Product.Img = @"\images\products\" + fileName + imageExtension;
                }
                _unitOfWork._ProductRepository.Update(productVm.Product);
                _unitOfWork.Complete();
                TempData["update"] = "Data has updated successfully !";
                return RedirectToAction("Index");
            }
            // ModelState is invalid, return the view with the entire ProductViewModel
            productVm.CategoryList = _unitOfWork._CategoryRepository.GetAll()
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

            return View(productVm);
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var ProductInDb = _unitOfWork._ProductRepository.GetFirstOrDefault(x => x.Id == id);
            if (ProductInDb == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Error while deleting the product"
                });
            }
            _unitOfWork._ProductRepository.Remove(ProductInDb);
            var oldImg = Path.Combine(_webHostEnvironment.WebRootPath, ProductInDb.Img.TrimStart('\\'));
            if (System.IO.File.Exists(oldImg))
            {
                System.IO.File.Delete(oldImg);
            }
            _unitOfWork.Complete();
            TempData["delete"] = "Data has deleted successfully !";
            return Json(new
            {
                success = true,
                message = "Data has deleted successfully"
            });
        }
    }
}
