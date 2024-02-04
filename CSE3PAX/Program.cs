using CSE3PAX;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Enable session middleware
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

});

// Register UserDatabaseFunctions class
builder.Services.AddScoped<UserDatabaseFunctions>();

// Configure authentication with cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.Cookie.HttpOnly = true;
        options.AccessDeniedPath = "/Shared/AccessDenied";
        options.LoginPath = "/Index";

    });

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("isAdministrator", policy =>
    {
        policy.RequireClaim("isAdministrator", "True");
    });
    options.AddPolicy("isManager", policy =>
    {
        policy.RequireClaim("isManager", "True");
    });
    options.AddPolicy("isLecturer", policy =>
    {
        policy.RequireClaim("isLecturer", "True");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Use Session must be between UseRouting and UseRazorPages
app.UseSession();

app.MapRazorPages();

app.Run();
