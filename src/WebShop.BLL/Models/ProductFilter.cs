namespace WebShop.BLL.Models
{
    public class ProductFilter
    {
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public ProductSort Sort { get; set; } = ProductSort.Newest;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }

    public enum ProductSort
    {
        Newest,
        NameAsc,
        NameDesc,
        PriceAsc,
        PriceDesc
    }
}
