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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void PaymentStatus(int orderHeaderId, string SessionId, string PaymentIntentId)
        {
            var orderHeader = _context.OrderHeaders.FirstOrDefault(x => x.OrderHeaderId == orderHeaderId);
            orderHeader.DateOfPayment = DateTime.Now;
            orderHeader.PaymentIntentId = PaymentIntentId;
            orderHeader.SessionId = SessionId;
        }

        public void Update(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);
            //var CategoryItem = _context.Categories.FirstOrDefault(x => x.CategoryId == category.CategoryId);
            //if (CategoryItem != null)
            //{
            //    CategoryItem.CategoryName = category.CategoryName;
            //    CategoryItem.OrderedDate = category.OrderedDate;
            //    CategoryItem.DisplayOrder = category.DisplayOrder;
            //}
        }

        public void UpdateStatus(int orderHeaderId, string orderStatus, string? paymentStatus = null)
        {
            var order = _context.OrderHeaders.FirstOrDefault(x => x.OrderHeaderId == orderHeaderId);
            if (order is not null)
            {
                order.OrderStatus = orderStatus;
            }
            if(paymentStatus is not null)
            {
                order.PaymentStatus = paymentStatus;
            }
        }
    }
}
