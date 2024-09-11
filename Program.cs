using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ST10157545_GIFTGIVERS.Controllers;
using ST10157545_GIFTGIVERS.Models;
using st10157545_giftgiversPOEs.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext with SQL Server
builder.Services.AddDbContext<DatabaseController>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureConnection")));

// Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.Name = "ST10157545_GIFTGIVERS.Auth";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

// Register the session service
builder.Services.AddTransient<SessionService>(); // Register SessionService

// Register the background service
builder.Services.AddHostedService<TokenRefreshService>(); // Register TokenRefreshService

//Service for Subscription:
builder.Services.AddScoped<SubscriptionService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Make sure authentication is used
app.UseAuthorization();


app.UseMiddleware<TokenMiddleware>(); //for automatice token refreshment 


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
