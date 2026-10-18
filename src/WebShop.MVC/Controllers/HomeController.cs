using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebShop.BLL.Interfaces;
using WebShop.BLL.Models;
using WebShop.MVC.Models;
using WebShop.MVC.ViewModels.Home;

namespace WebShop.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public async Task<IActionResult> Index(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice, ProductSort sort = ProductSort.Newest, int page = 1)
        {
            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                (minPrice, maxPrice) = (maxPrice, minPrice);

            var viewModel = new IndexViewModel
            {
                Search = search?.Trim(),
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Sort = sort,
                Page = page
            };

            var result = await _productService.SearchAsync(viewModel.ToFilter());
            var categories = await _productService.GetCategoriesWithProductsAsync();
            viewModel.Load(result, categories);

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            List<StudentViewModel> studentList = new List<StudentViewModel>
            {
                new StudentViewModel
                {
                    Id = 1,
                    Name = "Marko Markovic",
                    IdNumber = "13/2022"
                },
                new StudentViewModel
                {
                    Id = 2,
                    Name = "Nikola Nikolic",
                    IdNumber = "14/2022"
                },
                new StudentViewModel
                {
                    Id = 3,
                    Name = "Sima Simic",
                    IdNumber = "15/2022"
                }
            };


            return View(studentList);
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
