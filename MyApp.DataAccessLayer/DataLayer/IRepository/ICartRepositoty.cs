using MyApp.Models;
using MyApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DataAccessLayer.DataLayer.IRepository
{
    public interface ICartRepositoty: IRepository<Cart> 
    {
        void AddQuantity(Cart cart, int quantity);
        void DeleteQuantity(Cart cart, int quantity);
    }
}
