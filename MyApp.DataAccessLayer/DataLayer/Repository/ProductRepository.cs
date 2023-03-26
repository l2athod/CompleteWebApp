using MyApp.DataAccessLayer.Data;
using MyApp.DataAccessLayer.DataLayer.IRepository;
using MyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DataAccessLayer.DataLayer.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var ProductItem = _context.Products.FirstOrDefault(x => x.ProductId == product.ProductId);
            if (ProductItem is not null)
            {
                ProductItem.ProductName = product.ProductName;
                ProductItem.Price = product.Price;
                ProductItem.Description = product.Description;
                if (product.ImageUrl is not null)
                {
                    ProductItem.ImageUrl = product.ImageUrl;
                }
            }
        }
    }
}
