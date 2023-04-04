using MyApp.DataAccessLayer.DataLayer.Repository;
using MyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DataAccessLayer.DataLayer.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        // This interface will contain all methods (category repo methods + generic repo methods ) for category model
        void Update(OrderDetail orderDetail);
    }
}
