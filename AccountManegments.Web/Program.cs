using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddScoped<WebAPI, WebAPI>();
builder.Services.AddScoped<UserSession>();
builder.Services.AddScoped<Common>();
builder.Services.AddScoped<APIServices, APIServices>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {


            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.IsEssential = true;
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.LoginPath = "/Authentication/UserLogin";
            options.LogoutPath = "/Authentication/UserLogin";
            options.AccessDeniedPath = "/Home/UnAuthorised";

        });
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "UserName";
    options.Cookie.Expiration = TimeSpan.FromDays(30);
});
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromDays(30);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;

});

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
UserSession.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=UserLogin}/{id?}");

app.Run();
