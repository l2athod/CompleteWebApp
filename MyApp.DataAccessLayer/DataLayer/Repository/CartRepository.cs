using MyApp.DataAccessLayer.Data;
using MyApp.DataAccessLayer.DataLayer.IRepository;
using MyApp.Models;
using MyApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DataAccessLayer.DataLayer.Repository
{
    public class CartRepository : Repository<Cart>, ICartRepositoty
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void AddQuantity(Cart cart, int quantity)
        {
            cart.Count += quantity;
        }

        public void DeleteQuantity(Cart cart, int quantity)
        {
            cart.Count -= quantity;
        }
    }
}
