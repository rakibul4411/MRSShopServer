using Microsoft.AspNetCore.Mvc;
using MRSShop.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using MRSShop.Models.BindingTargets;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace MRSShop.Controllers {

    [Route("api/products")]
    //[Authorize(Roles = "Admin")]
    [ApiController]
    public class ProductValuesController : Controller {
        private DataContext context;
        private IHostingEnvironment _environment;

        public ProductValuesController(DataContext ctx, IHostingEnvironment environment) {
            context = ctx;
            _environment = environment;
        }

         [HttpGet("{id}")]
        [AllowAnonymous]
        public Product GetProduct(long id) {
                       IQueryable<Product> query = context.Products
                .Include(p => p.Ratings);
                        
            //if (HttpContext.User.IsInRole("Admin")) {
            //    query = query.Include(p => p.Supplier)
            //        .ThenInclude(s => s.Products);
            //}                          

            Product result = query.First(p => p.ProductId == id);

            if (result != null) {
                if (result.Supplier != null) {
                    result.Supplier.Products = result.Supplier.Products.Select(p =>
                        new Product {
                            ProductId = p.ProductId,
                            Name = p.Name,
                            Category = p.Category,
                            Description = p.Description,
                            Price = p.Price,
                        });
                }

                if (result.Ratings != null) {
                    foreach (Rating r in result.Ratings) {
                        r.Product = null;
                    }
                }
            }
            return result;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetProducts(string category, string search,
                bool related = false, bool metadata = false) {
            IQueryable<Product> query = context.Products;

            if (!string.IsNullOrWhiteSpace(category)) {
                string catLower = category.ToLower();
                query = query.Where(p => p.Category.ToLower().Contains(catLower));
            }
            if (!string.IsNullOrWhiteSpace(search)) {
                string searchLower = search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchLower)
                    || p.Description.ToLower().Contains(searchLower));
            }

            if (related && HttpContext.User.IsInRole("Admin")) {
                query = query.Include(p => p.Supplier).Include(p => p.Ratings);
                List<Product> data = query.ToList();
                data.ForEach(p => {
                    if (p.Supplier != null) {
                        p.Supplier.Products = null;
                    }
                    if (p.Ratings != null) {
                        p.Ratings.ForEach(r => r.Product = null);
                    }
                });
                return metadata ? CreateMetadata(data) : Ok(data);
            } else {
                return metadata ? CreateMetadata(query) : Ok(query);
            }
        }

        private IActionResult CreateMetadata(IEnumerable<Product> products) {
            return Ok(new {
                data = products,
                categories = context.Products.Select(p => p.Category)
                    .Distinct().OrderBy(c => c)
            });
        }


        [HttpPost]
        public IActionResult CreateProduct([FromBody] ProductVM pdata)
        {           

            Product pr = new Product();
            pr.Name = pdata.Name;
            pr.Category = pdata.Category;
            pr.Description = pdata.Description;
            pr.Feature = pdata.Feature;
            pr.MinStocklevel = pdata.MinStocklevel;
            pr.Stock = pdata.Stock;
            pr.PurchaseStock = pdata.PurchaseStock;
            pr.Price = pdata.Price;
            

            if (pr.Supplier != null && pr.Supplier.SupplierId != 0)
            {
                context.Attach(pr.Supplier);
            }

            context.Add(pr);
            context.SaveChanges();
            return Ok("Add succesful");          
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceProduct(long id, [FromBody] ProductVM pdata)
        {
            Product pr = new Product();
            pr.Name = pdata.Name;
            pr.Category = pdata.Category;
            pr.Description = pdata.Description;
            pr.Feature = pdata.Feature;
            pr.MinStocklevel = pdata.MinStocklevel;
            pr.Stock = pdata.Stock;
            pr.PurchaseStock = pdata.PurchaseStock;
            pr.Price = pdata.Price;


            if (pr.Supplier != null && pr.Supplier.SupplierId != 0)
            {
                context.Attach(pr.Supplier);
            }

            context.Entry(pr).State = EntityState.Modified;
            context.SaveChanges();
            return Ok("Add succesful");
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateProduct(long id,
                [FromBody]JsonPatchDocument<ProductVM> patch)
        {

            Product product = context.Products
                                .Include(p => p.Supplier)
                                .First(p => p.ProductId == id);
            ProductVM pdata = new ProductVM { Product = product };
            patch.ApplyTo(pdata, ModelState);

            if (ModelState.IsValid && TryValidateModel(pdata))
            {

                if (product.Supplier != null && product.Supplier.SupplierId != 0)
                {
                    context.Attach(product.Supplier);
                }
                context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id}")]
        public void DeleteProduct(long id) {
            context.Products.Remove(new Product { ProductId = id });
            context.SaveChanges();
        }

    }
}
