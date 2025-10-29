using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using MvcIdentiyFirstPrinciples.Models;
using MvcIdentiyFirstPrinciples.Services;
using Roles = MvcIdentiyFirstPrinciples.Roles;

namespace MvcIdentiyFirstPrinciples;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.ReturnUrlParameter = "ReturnUrl";
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.LogoutPath = "/Account/Logout";
            });
        builder.Services.AddSession();
        builder.Services.AddSingleton<UserDb>()
        .AddOptions<UserDbOptions>().Configure(options => {
            int userId = 1;
            options.Users = [
                new () {
                    UserId = userId++,
                    Email = "henry@example.com",
                    Password = "qwerty100",
                    Username = "henry",
                    Role = Roles.ADMIN_ROLE
                },
                new () {
                    UserId = userId++,
                    Email = "george@example.com",
                    Password = "qwerty100",
                    Username = "george",
                    Role = Roles.USER_ROLE
                }
            ];
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}
