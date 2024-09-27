using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using st10157545_giftgiversPOEs.Controllers;
using st10157545_giftgiversPOEs.Models;
using st10157545_giftgiversPOEs.Services;

var builder = WebApplication.CreateBuilder(args);

//OAuth Login and it will allow me to use api's 

//to call an API from your ASP.NET MVC application, you need to obtain an access token issued for the API you want to call.
//As the SDK is configured to use OAuth's Implicit Grant with Form Post, no access token will be returned by default. In order to do so,
//we should be using the Authorization Code Grant, which requires the use of a ClientSecret. Next, to obtain the token to access an external API,
//call WithAccessToken and set the audience to the API Identifier.
//You can get the API Identifier from the API Settings for the API you want to use.to call an API from your ASP.NET MVC application,
//you need to obtain an access token issued for the API you want to call. As the SDK is configured to use OAuth's Implicit Grant with Form Post,
//no access token will be returned by default. In order to do so, we should be using the Authorization Code Grant, which requires the use of a ClientSecret.
//Next, to obtain the token to access an external API, call WithAccessToken and set the audience to the API Identifier. You can get the API Identifier from the API Settings for the API you want to use.
//builder.Services.AddAuth0WebAppAuthentication(options =>
//{
//    options.Domain = builder.Configuration["Auth0:Domain"];
//    options.ClientId = builder.Configuration["Auth0:ClientId"];
//})
//.WithAccessToken(options =>
//{
//    options.Audience = builder.Configuration["Auth0:Audience"];//will replace with the access token of the api i want to
//});

// Register HttpClient service
builder.Services.AddHttpClient();

// Register the session service
builder.Services.AddTransient<SessionService>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.AddTransient<EmailService>();


// Register the background service
builder.Services.AddHostedService<TokenRefreshService>();

// Service for Subscription:
builder.Services.AddScoped<SubscriptionService>();
builder.Services.AddScoped<GuardianNewsService>();
builder.Services.AddScoped<FacebookService>();
builder.Services.AddScoped<InstagramService>();
builder.Services.AddScoped<TwitterService>();
builder.Services.AddScoped<NotificationService>();


// Add services to the container.
builder.Services.AddControllersWithViews();

// Add in-memory caching
builder.Services.AddMemoryCache();

// Configure DbContext with SQL Server
builder.Services.AddDbContext<DatabaseController>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureConnection"))
      .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, LogLevel.Information));

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

// Adding UserRoles policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
    options.AddPolicy("VolunteerPolicy", policy => policy.RequireRole("Volunteer"));
});
builder.Configuration.AddEnvironmentVariables();

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

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TokenMiddleware>(); // For automatic token refreshment 

app.Use(async (context, next) =>
{
    if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
    {
        if (context.User.IsInRole("Admin"))
        {
            if (!context.Request.Path.StartsWithSegments("/Admin"))
            {
                context.Response.Redirect("/Admin/Index");
                return;
            }
        }
        else if (context.User.IsInRole("User"))
        {
            if (!context.Request.Path.StartsWithSegments("/Home"))
            {
                context.Response.Redirect("/Home/Index");
                return;
            }
        }
        else if (context.User.IsInRole("Volunteer"))
        {
            if (!context.Request.Path.StartsWithSegments("/Volunteer"))
            {
                context.Response.Redirect("/Volunteer/Index");
                return;
            }
        }
    }
    await next.Invoke();
});

// Route for normal users
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Route for admin users
app.MapControllerRoute(
    name: "Admin",
    pattern: "Admin/{controller=Admin}/{action=Index}/{id?}");

// Route for volunteer users
app.MapControllerRoute(
    name: "Volunteer",
    pattern: "Volunteer/{controller=Volunteer}/{action=Index}/{id?}");

app.Run();