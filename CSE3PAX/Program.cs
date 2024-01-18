using CSE3PAX;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//Enable session middleware

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(36000);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register UserDatabaseFunctions class
builder.Services.AddScoped<UserDatabaseFunctions>();

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

app.UseAuthorization();

//Use Session must be between UseRouting and UserRazorPages
app.UseSession();

app.MapRazorPages();

app.Run();
