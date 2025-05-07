// Models/IOrderRepository.cs
using System.Collections.Generic;

namespace SportsStore.Models
{
    public interface IOrderRepository
    {
        IQueryable<Order> Orders { get; }
        int SaveOrder(Order order);
        void MarkShipped(int orderId);
    }
}