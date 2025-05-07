using Microsoft.EntityFrameworkCore;
using SportsStore.Models;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure database context
builder.Services.AddDbContext<StoreDBContext>(opts =>
{
    opts.UseSqlServer(
        builder.Configuration["ConnectionStrings:SportsStoreConnection"]);
});

// Add repository service

builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();

// Configure session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Increased session time
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
});

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();

// Configure cart service
builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));

// Add Razor Pages support (for Tag Helpers)
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession(); // Enable session middleware

app.UseRouting();
app.UseAuthorization();

// Configure routes
app.MapControllerRoute(
    name: "categorypage",
    pattern: "{category}/Page{productPage:int}",
    defaults: new { Controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "page",
    pattern: "Page{productPage:int}",
    defaults: new { Controller = "Home", action = "Index", category = (string)null });

app.MapControllerRoute(
    name: "category",
    pattern: "{category}",
    defaults: new { Controller = "Home", action = "Index", productPage = 1 });
// Add this specific route for the cart before the default route
app.MapControllerRoute(
    name: "cart",
    pattern: "Cart/Cart",
    defaults: new { controller = "Cart", action = "Cart" });




// Default route - this handles all other controller/action combinations
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages (if any)
app.MapRazorPages();

// Seed the database
SeedData.EnsurePopulated(app);

app.Run();