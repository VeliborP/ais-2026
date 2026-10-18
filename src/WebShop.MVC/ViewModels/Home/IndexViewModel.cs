using Microsoft.AspNetCore.Mvc.Rendering;
using WebShop.BLL.Models;
using WebShop.DAL.Models;

namespace WebShop.MVC.ViewModels.Home
{
    public class IndexViewModel
    {
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public ProductSort Sort { get; set; } = ProductSort.Newest;
        public int Page { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int AllProductsCount { get; set; }

        public List<ProductViewModel> Products { get; set; } = [];
        public List<CategoryFilterViewModel> Categories { get; set; } = [];

        public string? SelectedCategoryName => Categories.FirstOrDefault(c => c.Id == CategoryId)?.Name;

        public bool HasActiveFilters => !string.IsNullOrWhiteSpace(Search) || CategoryId.HasValue || MinPrice.HasValue || MaxPrice.HasValue;

        public List<SelectListItem> SortOptions => new()
        {
            new SelectListItem("Najnovije", ProductSort.Newest.ToString(), Sort == ProductSort.Newest),
            new SelectListItem("Naziv A-Š", ProductSort.NameAsc.ToString(), Sort == ProductSort.NameAsc),
            new SelectListItem("Naziv Š-A", ProductSort.NameDesc.ToString(), Sort == ProductSort.NameDesc),
            new SelectListItem("Cena rastuće", ProductSort.PriceAsc.ToString(), Sort == ProductSort.PriceAsc),
            new SelectListItem("Cena opadajuće", ProductSort.PriceDesc.ToString(), Sort == ProductSort.PriceDesc)
        };

        public ProductFilter ToFilter()
        {
            return new ProductFilter
            {
                Search = Search,
                CategoryId = CategoryId,
                MinPrice = MinPrice,
                MaxPrice = MaxPrice,
                Sort = Sort,
                Page = Page
            };
        }

        public Dictionary<string, string> RouteValues(
            bool keepSearch = true,
            bool keepCategory = true,
            bool keepPrice = true,
            int? categoryId = null,
            int page = 1)
        {
            var values = new Dictionary<string, string>();

            if (keepSearch && !string.IsNullOrWhiteSpace(Search))
                values["search"] = Search;

            var category = categoryId ?? (keepCategory ? CategoryId : null);
            if (category.HasValue)
                values["categoryId"] = category.Value.ToString();

            if (keepPrice && MinPrice.HasValue)
                values["minPrice"] = MinPrice.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

            if (keepPrice && MaxPrice.HasValue)
                values["maxPrice"] = MaxPrice.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

            if (Sort != ProductSort.Newest)
                values["sort"] = Sort.ToString();

            if (page > 1)
                values["page"] = page.ToString();

            return values;
        }

        public void Load(PagedResult<Product> result, IEnumerable<Category> categories)
        {
            Page = result.Page;
            TotalCount = result.TotalCount;
            TotalPages = result.TotalPages;
            Products = result.Items.Select(ProductViewModel.FromEntity).ToList();
            Categories = categories
                .OrderBy(c => c.Name)
                .Select(c => new CategoryFilterViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductCount = c.Products.Count
                })
                .ToList();
            AllProductsCount = Categories.Sum(c => c.ProductCount);
        }
    }

    public class CategoryFilterViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int ProductCount { get; set; }
    }

    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? CategoryName { get; set; }
        public string? ImagePath { get; set; }
        public decimal Price { get; set; }

        public static ProductViewModel FromEntity(Product product)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryName = product.Category?.Name,
                ImagePath = product.ImagePath,
                Price = product.Price
            };
        }
    }
}
