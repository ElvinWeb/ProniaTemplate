using Microsoft.EntityFrameworkCore;
using ProniaProject.DataAccessLayer;

var builder = WebApplication.CreateBuilder(args);
string _stringConnection = "Server=DESKTOP-KA8SSD4;Database=ProniaAdminDB;Trusted_Connection=True";

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();  

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(_stringConnection));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.MapControllerRoute(
    name: "areas",
    pattern: "{area=exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
