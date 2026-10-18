using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebShop.BLL.Interfaces;
using WebShop.MVC.ViewModels;

namespace WebShop.MVC.Controllers
{
    
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService service, ICategoryService categoryService)
        {
            _productService = service;
            _categoryService = categoryService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            var vm = products.Select(ProductViewModel.FromEntity).ToList();
            return View(vm);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();

            return View(ProductViewModel.FromEntity(product));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllAsync();

            var productViewModel = ProductViewModel.FromEntity(new DAL.Models.Product() { Code = "", Name = "" }, categories);

            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProductViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            string? imagePath = null;

            if (vm.ImageFile != null && vm.ImageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(vm.ImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    var categories = await _categoryService.GetAllAsync();
                    vm.Categories = categories.Select(model => new SelectListItem
                    {
                        Value = model.Id.ToString(),
                        Text = model.Code + "-" + model.Name,
                        Selected = model.Id == vm.CategoryId
                    }).ToList();

                    ModelState.AddModelError("ImageFile", "Dozvoljene su samo JPG, PNG i GIF slike.");
                    return View(vm);
                }

                if (vm.ImageFile.Length > 5_000_000)
                {
                    var categories = await _categoryService.GetAllAsync();
                    vm.Categories = categories.Select(model => new SelectListItem
                    {
                        Value = model.Id.ToString(),
                        Text = model.Code + "-" + model.Name,
                        Selected = model.Id == vm.CategoryId
                    }).ToList();

                    ModelState.AddModelError("ImageFile", "Veličina fajla mora biti manja od 5 MB.");
                    return View(vm);
                }

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder);

                string fileName = Guid.NewGuid() + Path.GetExtension(vm.ImageFile.FileName);
                string fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await vm.ImageFile.CopyToAsync(stream);
                }

                imagePath = "/uploads/" + fileName;
            }

            var entity = vm.ToEntity(imagePath);

            await _productService.CreateAsync(entity);

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();

            var categories = await _categoryService.GetAllAsync();
            var productViewModel = ProductViewModel.FromEntity(product, categories);

            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, ProductViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            string? imagePath = vm.ImagePath;

            if (vm.DeleteImage && !string.IsNullOrEmpty(vm.ImagePath))
            {
                string oldPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    vm.ImagePath.TrimStart('/')
                );

                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);

                imagePath = null;
            }

            if (vm.ImageFile != null && vm.ImageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(vm.ImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    var categories = await _categoryService.GetAllAsync();
                    vm.Categories = categories.Select(model => new SelectListItem
                    {
                        Value = model.Id.ToString(),
                        Text = model.Code + "-" + model.Name,
                        Selected = model.Id == vm.CategoryId
                    }).ToList();

                    ModelState.AddModelError("ImageFile", "Dozvoljene su samo JPG, PNG i GIF slike.");
                    return View(vm);
                }

                if (vm.ImageFile.Length > 5_000_000)
                {
                    var categories = await _categoryService.GetAllAsync();
                    vm.Categories = categories.Select(model => new SelectListItem
                    {
                        Value = model.Id.ToString(),
                        Text = model.Code + "-" + model.Name,
                        Selected = model.Id == vm.CategoryId
                    }).ToList();

                    ModelState.AddModelError("ImageFile", "Veličina fajla mora biti manja od 5 MB.");
                    return View(vm);
                }

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder);

                string fileName = Guid.NewGuid() + Path.GetExtension(vm.ImageFile.FileName);
                string fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await vm.ImageFile.CopyToAsync(stream);
                }

                imagePath = "/uploads/" + fileName;
            }

            var entity = vm.ToEntity(imagePath, vm.DeleteImage);

            await _productService.UpdateAsync(entity);

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();

            return View(ProductViewModel.FromEntity(product));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
