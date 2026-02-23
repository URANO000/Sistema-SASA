using BusinessLogic.Servicios.Categorias;
using BusinessLogic.Servicios.Correo;
using BusinessLogic.Servicios.Prioridad;
using BusinessLogic.Servicios.Notificaciones;
using BusinessLogic.Servicios.Rol;
using BusinessLogic.Servicios.Tiquetes;
using BusinessLogic.Servicios.Usuarios;
using DataAccess;
using DataAccess.Identity;
using DataAccess.Repositorios.Categorias;
using DataAccess.Repositorios.Prioridad;
using DataAccess.Repositorios.Notificaciones;
using DataAccess.Repositorios.Tiquetes;
using DataAccess.Repositorios.Usuarios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SASA.Configuration;
using SASA.Services.Correo;
using DataAccess.Repositorios.Avances;
using BusinessLogic.Servicios.Avances;
using DataAccess.Repositorios.Attachments;


var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        // Activación por correo
        options.SignIn.RequireConfirmedEmail = true;

        // Bloqueo por intentos fallidos
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

        // Email único
        options.User.RequireUniqueEmail = true;

        // Password
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Cookies (rutas)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

//Repositories y Servicios de negocio
builder.Services.AddScoped<ITiqueteRepository, TiqueteRepository>();
builder.Services.AddScoped<ITiqueteService, TiqueteService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IRolService, RolService>();

builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IPrioridadRepository, PrioridadRepository>();
builder.Services.AddScoped<IPrioridadService, PrioridadService>();

builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();

builder.Services.AddScoped<IAvanceRepository, AvanceRepository>();
builder.Services.AddScoped<IAvanceService, AvanceService>();
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();

// Configuración de correo (Microsoft Graph)
builder.Services.AddScoped<ICorreoNotificacionesService, CorreoNotificacionesService>();
builder.Services.AddScoped<IEmailService, EmailService>(); //Graph EmailService
builder.Services.Configure<ConfiguracionEmail>(builder.Configuration.GetSection("GraphEmail"));

// Configuración general de la aplicación para la dirección base, etc.
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));


// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

//Seeder (temporal)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

    const string adminRole = "Administrador";

    var email = "test@sasa.com";
    var user = await userManager.FindByEmailAsync(email);

    if (user is null)
    {
        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true, // para que no falle por RequireConfirmedEmail
            Estado = true,
            LockoutEnabled = true
        };

        await userManager.CreateAsync(user, "Test123!");
    }
}

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

app.MapGet("/", (HttpContext ctx) =>
{
    return (ctx.User.Identity?.IsAuthenticated ?? false)
        ? Results.Redirect("/Home/Index")
        : Results.Redirect("/login");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
