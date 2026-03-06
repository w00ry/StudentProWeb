using Microsoft.EntityFrameworkCore;
using StudentProWeb.Models; // DbContext'in olduðu klasör

var builder = WebApplication.CreateBuilder(args);

// --- 1. ADIM: VERÝTABANI BAÐLANTISI ---
builder.Services.AddDbContext<StudentProDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. ADIM: SESSION (OTURUM) AYARLARI ---
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// --- 3. ADIM: OTOMATÝK VERÝTABANI OLUÞTURMA SÝHRÝ ---
// Uygulama her çalýþtýðýnda veritabaný yoksa otomatik oluþturur.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<StudentProDbContext>();
        // Bu komut, veritabaný yoksa SQL'de oluþturur ve tablolarý basar.
        context.Database.EnsureCreated();
        Console.WriteLine(">>> Veritabaný kontrol edildi: Hazýr!");
    }
    catch (Exception ex)
    {
        Console.WriteLine(">>> Veritabaný oluþturulurken bir hata oluþtu: " + ex.Message);
    }
}

// Pipeline Ayarlarý
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Eski sürümler için MapStaticAssets yerine bunu da ekleyebilirsin
app.UseRouting();

// Session kullanýmý Routing'den sonra, Authorization'dan önce olmalý
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();