using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SonicStore.Areas.SonicStore.Models;
using SonicStore.Areas.SonicStore.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(option =>
{
    option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/SonicStore/Login/Index";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.LoginPath = "/SonicStore/Login/login";
    options.AccessDeniedPath = "/error";
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
    options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

});
builder.Services.AddSingleton<IVnPayService, VnPayService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

builder.Services.AddDbContext<SonicStoreContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("SonicStore");
    options.UseSqlServer(connectionString);
});
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
//{
//    option.LoginPath = "/login";
//    option.AccessDeniedPath = "/error";
//});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.Cookie.Name = "session";
    option.IdleTimeout = new TimeSpan(0, 50, 0);
});
builder.Services.AddSingleton<IVnPayService, VnPayService>();
//builder.Services.AddSingleton<VnPayService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseSession();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/home");
        return;
    }
    await next();
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name: "SonicStore",
        areaName: "SonicStore",
        pattern: "SonicStore/{controller=Home}/{action=HomeScreen}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "register-google",
        pattern: "SonicStore/Login/RegisterGoogle",
        defaults: new { controller = "Login", action = "RegisterGoogle" });

    endpoints.MapControllerRoute(
        name: "google-response",
        pattern: "SonicStore/Login/GoogleResponse",
        defaults: new { controller = "Login", action = "GoogleResponse" });

    endpoints.MapControllerRoute(
        name: "notice-success",
        pattern: "SonicStore/Login/NoticeSuccess",
        defaults: new { controller = "Login", action = "NoticeSuccess" });
});

app.Run();
