using MyApp.DataAccessLayer.DataLayer.Repository;
using MyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DataAccessLayer.DataLayer.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        // This interface will contain all methods (category repo methods + generic repo methods ) for category model
        void Update(OrderHeader orderHeader);
        void UpdateStatus(int orderHeaderId, string orderStatus, string? paymentStatus = null);
        void PaymentStatus(int orderHeaderId, string SessionId, string PaymentIntentId);
    }
}
