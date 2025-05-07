// Models/EFOrderRepository.cs
using Microsoft.EntityFrameworkCore;
using System;
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
            // Ensure context tracking is working correctly
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

            if (order.OrderID == 0)
            {
                try
                {
                    // For new orders, we need to handle the products carefully
                    foreach (var line in order.Lines)
                    {
                        // Make sure the product is attached but not tracked for changes
                        if (line.Product != null)
                        {
                            // Set the ProductId from the Product object
                            if (line.Product.ProductId.HasValue)
                            {
                                line.ProductId = line.Product.ProductId.Value;
                            }

                            // Check if the product is already being tracked
                            var trackedProduct = context.Products.Local.FirstOrDefault(p =>
                                p.ProductId.HasValue && p.ProductId == line.Product.ProductId);

                            if (trackedProduct == null)
                            {
                                // If not tracked, attach it
                                context.Products.Attach(line.Product);
                            }
                            else
                            {
                                // If already tracked, use the tracked entity
                                line.Product = trackedProduct;
                            }
                        }
                    }

                    // Add the order to the context
                    context.Orders.Add(order);

                    // Save changes to the database
                    context.SaveChanges();
                    return order.OrderID;
                }
                catch (Exception ex)
                {
                    // You might want to log this exception
                    Console.WriteLine($"Error saving order: {ex.Message}");

                    // If there's an inner exception, log that too
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }

                    throw; // Re-throw to handle in the controller
                }
            }
            else
            {
                // For existing orders, update them
                var dbOrder = context.Orders
                    .Include(o => o.Lines)
                    .ThenInclude(l => l.Product)
                    .FirstOrDefault(o => o.OrderID == order.OrderID);

                if (dbOrder != null)
                {
                    // Update order properties
                    context.Entry(dbOrder).CurrentValues.SetValues(order);

                    // Update or add lines
                    foreach (var line in order.Lines)
                    {
                        var dbLine = dbOrder.Lines.FirstOrDefault(l => l.CartLineID == line.CartLineID);
                        if (dbLine != null)
                        {
                            // Update existing line
                            context.Entry(dbLine).CurrentValues.SetValues(line);

                            // Make sure the product reference is correct
                            if (line.Product != null && line.Product.ProductId.HasValue)
                            {
                                dbLine.ProductId = line.Product.ProductId.Value;

                                // Update the product reference if needed
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
                            // Add new line
                            // Make sure ProductId is set correctly
                            if (line.Product != null && line.Product.ProductId.HasValue)
                            {
                                line.ProductId = line.Product.ProductId.Value;

                                // Attach the product if needed
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

                            // Add the line to the order
                            dbOrder.Lines.Add(line);
                        }
                    }

                    // Remove lines not in the updated order
                    foreach (var dbLine in dbOrder.Lines.ToList())
                    {
                        if (!order.Lines.Any(l => l.CartLineID == dbLine.CartLineID))
                        {
                            context.Remove(dbLine);
                        }
                    }
                }

                // Save all changes
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