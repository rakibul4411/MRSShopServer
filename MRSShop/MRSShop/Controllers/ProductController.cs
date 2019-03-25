using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MRSShop.Models;

namespace MRSShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DataContext context;

        public ProductController(DataContext _context)
        {
            context = _context;
        }

        // GET: api/Product
        [HttpGet]
        public IActionResult GetProducts(string category, string search,
                bool related = false, bool metadata = false)
        {
            IQueryable<Product> query = context.Products;

            if (!string.IsNullOrWhiteSpace(category))
            {
                string catLower = category.ToLower();
                query = query.Where(p => p.Category.ToLower().Contains(catLower));
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                string searchLower = search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchLower)
                    || p.Description.ToLower().Contains(searchLower));
            }

            if (related && HttpContext.User.IsInRole("Admin"))
            {
                query = query.Include(p => p.Supplier).Include(p => p.Ratings);
                List<Product> data = query.ToList();
                data.ForEach(p =>
                {
                    if (p.Supplier != null)
                    {
                        p.Supplier.Products = null;
                    }
                    if (p.Ratings != null)
                    {
                        p.Ratings.ForEach(r => r.Product = null);
                    }
                });
                return metadata ? CreateMetadata(data) : Ok(data);
            }
            else
            {
                return metadata ? CreateMetadata(query) : Ok(query);
            }
        }

        private IActionResult CreateMetadata(IEnumerable<Product> products)
        {
            return Ok(new
            {
                data = products,
                categories = context.Products.Select(p => p.Category)
                    .Distinct().OrderBy(c => c)
            });
        }
        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(long id)
        {
            var product = await context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(long id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            context.Entry(product).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Product
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            context.Products.Add(product);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(long id)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            context.Products.Remove(product);
            await context.SaveChangesAsync();

            return product;
        }

        private bool ProductExists(long id)
        {
            return context.Products.Any(e => e.ProductId == id);
        }
    }
}
