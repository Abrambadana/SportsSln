using System.Collections.Generic;
using System.Linq;
namespace SportsStore.Models
{
    public class Cart
    {
        public List<CartLine> Lines { get; set; } = new List<CartLine>();
        public virtual void AddItem(Product product, int quantity)
        {
            CartLine? line = Lines
                .FirstOrDefault(p => p.Product.ProductId == product.ProductId);
            if (line == null)
            {
                Lines.Add(new CartLine
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }
        public virtual void RemoveLine(int productId) =>
            Lines.RemoveAll(l => l.Product.ProductId == productId);
        public decimal ComputeTotalValue() =>
            Lines.Sum(e => e.Product.Price * e.Quantity);
        public virtual void Clear() => Lines.Clear();
    }

    public class CartLine
    {
        public int CartLineID { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }

        // Foreign key relationship
        public int? OrderID { get; set; }  // Make nullable to avoid validation issues
        public Order Order { get; set; }
        public long ProductId { get; set; } // Changed from nullable to non-nullable
    }
}