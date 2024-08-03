using Microsoft.AspNetCore.Mvc;
using myShop.DataAccess;
using myShop.Entities.IRepositories;
using myShop.Entities.Models;

namespace myShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public IActionResult Index()
        {
            var categories = _unitOfWork._CategoryRepository.GetAll();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork._CategoryRepository.Add(category);
                _unitOfWork.Complete();
                TempData["create"] = "Data has created successfully !";
                return RedirectToAction("Index");
            }
            return View(category);

        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null | id == 0)
            {
                NotFound();
            }
            var categoryInDb = _unitOfWork._CategoryRepository.GetFirstOrDefault(x => x.Id == id);
            return View(categoryInDb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork._CategoryRepository.Update(category);
                _unitOfWork.Complete();
                TempData["update"] = "Data has updated successfully !";
                return RedirectToAction("Index");
            }
            return View(category);
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null | id == 0)
            {
                NotFound();
            }
            var categoryInDb = _unitOfWork._CategoryRepository.GetFirstOrDefault(x => x.Id == id);
            return View(categoryInDb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int? id)
        {
            var categoryInDb = _unitOfWork._CategoryRepository.GetFirstOrDefault(x => x.Id == id);
            if (categoryInDb == null)
            {
                NotFound();
            }
            _unitOfWork._CategoryRepository.Remove(categoryInDb);
            _unitOfWork.Complete();
            TempData["delete"] = "Data has deleted successfully !";
            return RedirectToAction("Index");
        }
    }
}
