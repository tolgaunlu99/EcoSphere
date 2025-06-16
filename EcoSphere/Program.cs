using EcoSphere.Caching;
using EcoSphere.Models;
using Microsoft.AspNetCore.StaticFiles; // 📌 MIME tipi için gerekli
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MyDbContext");
builder.Services.AddDbContext<MyDbContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 30 dakika boyunca session saklanır
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// 📌 MIME tipi tanımı burada
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".geojson"] = "application/geo+json";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// 📌 Cache yüklemesini arka planda başlat
#pragma warning disable CS4014
Task.Run(() =>
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

        if (ObservationCache.IsCacheEmpty())
        {
            ObservationCache.LoadCache(dbContext);
        }

        if (CreaturesCache.IsCacheEmpty())
        {
            CreaturesCache.LoadCache(dbContext);
        }
    }
});
app.Run();
