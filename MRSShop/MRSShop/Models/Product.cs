using System.Collections.Generic;
namespace MRSShop.Models {

    public class Product {

        public long ProductId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Feature { get; set; }
        public string ImagePath { get; set; }
        public int? MinStocklevel { get; set; }
        public int? Stock { get; set; }
        public int PurchaseStock { get; set; }

        public Supplier Supplier { get; set; }
        public List<Rating> Ratings { get; set; }
    }
}