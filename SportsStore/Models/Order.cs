using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SportsStore.Models
{
    public class Order
    {
        [BindNever]
        public int OrderID { get; set; }

        [BindNever]
        public ICollection<CartLine> Lines { get; set; } = new List<CartLine>();

        [Required(ErrorMessage = "Please enter a name")]
        [DisplayName("Full Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the first address line")]
        [DisplayName("Address Line 1")]
        public string Line1 { get; set; }

        [DisplayName("Address Line 2 (Optional)")]
        public string Line2 { get; set; }

        [DisplayName("Address Line 3 (Optional)")]
        public string Line3 { get; set; }

        [Required(ErrorMessage = "Please enter a city name")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter a state name")]
        [DisplayName("State/Province")]
        public string State { get; set; }

        [DisplayName("Postal/Zip Code")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "Please enter a country name")]
        public string Country { get; set; }

        [DisplayName("Gift Wrap")]
        public bool GiftWrap { get; set; }

        [BindNever]
        public bool Shipped { get; set; }

        // Combine address parts into one field (for display purposes in view)
        public string FullAddress => $"{Line1} {Line2} {Line3}, {City}, {State}, {Zip}, {Country}";
    }
}