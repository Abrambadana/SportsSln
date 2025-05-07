namespace SportsStore.Models.ViewModels
{
    public class CartIndexViewModel
    {
        public Cart Cart { get; set; } = new Cart();  // Initialize with empty cart
        public string ReturnUrl { get; set; } = "/";  // Default value
    }
}