using BusinessLogic.Servicios.Notificaciones;
using DataAccess;
using DataAccess.Repositorios.Notificaciones;
using Microsoft.EntityFrameworkCore;
using SASA.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

//dbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC
builder.Services.AddControllersWithViews();

// Fake auth service
builder.Services.AddScoped<IAuthService, FakeAuthService>();
builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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
app.UseAuthorization();

app.MapGet("/", (HttpContext ctx) =>
{
    var email = ctx.Session.GetString("auth_email");
    return !string.IsNullOrEmpty(email)
        ? Results.Redirect("/Home/Index")
        : Results.Redirect("/login");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
