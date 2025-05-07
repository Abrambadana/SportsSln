using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SportsStore.Models
{
    public class EFOrderRepository : IOrderRepository
    {
        private StoreDBContext context;

        public EFOrderRepository(StoreDBContext ctx)
        {
            context = ctx;
        }

        public IQueryable<Order> Orders => context.Orders
            .Include(o => o.Lines)
            .ThenInclude(l => l.Product);

        public int SaveOrder(Order order)
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

            if (order.OrderID == 0)
            {
                foreach (var line in order.Lines)
                {
                    if (line.Product != null)
                    {
                        if (line.Product.ProductId.HasValue)
                        {
                            line.ProductId = line.Product.ProductId.Value;
                        }

                        var trackedProduct = context.Products.Local.FirstOrDefault(p =>
                            p.ProductId.HasValue && p.ProductId == line.Product.ProductId);

                        if (trackedProduct == null)
                        {
                            context.Products.Attach(line.Product);
                        }
                        else
                        {
                            line.Product = trackedProduct;
                        }
                    }
                }

                context.Orders.Add(order);
                context.SaveChanges();
                return order.OrderID;
            }
            else
            {
                var dbOrder = context.Orders
                    .Include(o => o.Lines)
                    .ThenInclude(l => l.Product)
                    .FirstOrDefault(o => o.OrderID == order.OrderID);

                if (dbOrder != null)
                {
                    context.Entry(dbOrder).CurrentValues.SetValues(order);

                    foreach (var line in order.Lines)
                    {
                        var dbLine = dbOrder.Lines.FirstOrDefault(l => l.CartLineID == line.CartLineID);
                        if (dbLine != null)
                        {
                            context.Entry(dbLine).CurrentValues.SetValues(line);

                            if (line.Product != null && line.Product.ProductId.HasValue)
                            {
                                dbLine.ProductId = line.Product.ProductId.Value;

                                if (dbLine.Product.ProductId != line.Product.ProductId)
                                {
                                    var product = context.Products.Find(line.Product.ProductId);
                                    if (product != null)
                                    {
                                        dbLine.Product = product;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (line.Product != null && line.Product.ProductId.HasValue)
                            {
                                line.ProductId = line.Product.ProductId.Value;

                                var product = context.Products.Find(line.ProductId);
                                if (product != null)
                                {
                                    line.Product = product;
                                }
                                else
                                {
                                    context.Products.Attach(line.Product);
                                }
                            }

                            dbOrder.Lines.Add(line);
                        }
                    }

                    foreach (var dbLine in dbOrder.Lines.ToList())
                    {
                        if (!order.Lines.Any(l => l.CartLineID == dbLine.CartLineID))
                        {
                            context.Remove(dbLine);
                        }
                    }
                }

                context.SaveChanges();
                return order.OrderID;
            }
        }

        public void MarkShipped(int orderId)
        {
            Order order = context.Orders.FirstOrDefault(o => o.OrderID == orderId);
            if (order != null)
            {
                order.Shipped = true;
                context.SaveChanges();
            }
        }
    }
}
