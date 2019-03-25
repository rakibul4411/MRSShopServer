using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MRSShop.Models.BindingTargets {

    public class ProductVM {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Feature { get; set; }
        public int? MinStocklevel { get; set; }
        public int? Stock { get; set; }
        public int PurchaseStock { get; set; }

        //[Required]
        //public string Name {
        //    get => Product.Name; set => Product.Name = value;
        //}

        //[Required]
        //public string Category {
        //    get => Product.Category; set => Product.Category = value;
        //}

        //[Required]
        //public string Description {
        //    get => Product.Description; set => Product.Description = value;
        //}

        //[Range(1, int.MaxValue, ErrorMessage = "Price must be at least 1")]
        //public decimal Price {
        //    get => Product.Price; set => Product.Price = value;
        //}
        //public string Feature { get; set; }
        //public string ImagePath { get; set; }
        //public int? MinStocklevel { get; set; }
        //public int? Stock { get; set; }
        //public int PurchaseStock { get; set; }

        public long? Supplier
        {
            get => Product.Supplier?.SupplierId ?? null;
            set
            {
                if (!value.HasValue)
                {
                    Product.Supplier = null;
                }
                else
                {
                    if (Product.Supplier == null)
                    {
                        Product.Supplier = new Supplier();
                    }
                    Product.Supplier.SupplierId = value.Value;
                }
            }
        }

        public Product Product { get; set; } = new Product();
    }
}
