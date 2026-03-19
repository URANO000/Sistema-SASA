using BusinessLogic.Servicios.Attachments;
using BusinessLogic.Servicios.Autenticacion;
using BusinessLogic.Servicios.Avances;
using BusinessLogic.Servicios.Categorias;
using BusinessLogic.Servicios.Correo;
using BusinessLogic.Servicios.Helpers;
using BusinessLogic.Servicios.Integracion;
using BusinessLogic.Servicios.Inventario;
using BusinessLogic.Servicios.InventarioTelefono;
using BusinessLogic.Servicios.Notificaciones;
using BusinessLogic.Servicios.Prioridad;
using BusinessLogic.Servicios.Rol;
using BusinessLogic.Servicios.SubCategorias;
using BusinessLogic.Servicios.TiqueteHistoriales;
using BusinessLogic.Servicios.Tiquetes;
using BusinessLogic.Servicios.Usuarios;
using DataAccess;
using DataAccess.Identity;
using DataAccess.Repositorios.Attachments;
using DataAccess.Repositorios.Autenticacion;
using DataAccess.Repositorios.Avances;
using DataAccess.Repositorios.Categorias;
using DataAccess.Repositorios.Integracion;
using DataAccess.Repositorios.Inventario;
using DataAccess.Repositorios.InventarioTelefono;
using DataAccess.Repositorios.Notificaciones;
using DataAccess.Repositorios.Prioridad;
using DataAccess.Repositorios.SubCategorias;
using DataAccess.Repositorios.TiqueteHistoriales;
using DataAccess.Repositorios.Tiquetes;
using DataAccess.Repositorios.Usuarios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SASA.Configuration;
using SASA.Services.Correo;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.SignIn.RequireConfirmedEmail = true;

        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

        options.User.RequireUniqueEmail = true;

        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Tokens
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(1);
});

// Cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    options.SlidingExpiration = true;

    options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
    {
        OnValidatePrincipal = async context =>
        {
            var userId = context.Principal?
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return;

            var cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
            var lastStr = await cache.GetStringAsync($"last-activity:{userId}");

            if (!long.TryParse(lastStr, out var lastUnix))
            {
                await cache.SetStringAsync(
                    $"last-activity:{userId}",
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
                );
                return;
            }

            var last = DateTimeOffset.FromUnixTimeSeconds(lastUnix);
            var idleTimeout = TimeSpan.FromMinutes(15);

            if (DateTimeOffset.UtcNow - last > idleTimeout)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync();
            }
        }
    };
});

// Security Stamp
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromMinutes(5);
});

// Repositories y servicios de negocio
builder.Services.AddScoped<ITiqueteRepository, TiqueteRepository>();
builder.Services.AddScoped<ITiqueteService, TiqueteService>();

builder.Services.AddScoped<ISubCategoriaRepository, SubCategoriaRepository>();
builder.Services.AddScoped<ISubCategoriaService, SubCategoriaService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IRolService, RolService>();

builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

builder.Services.AddScoped<IPrioridadRepository, PrioridadRepository>();
builder.Services.AddScoped<IPrioridadService, PrioridadService>();

builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();

//Todo lo necesario para el módulo de tiquetes y colas

builder.Services.AddScoped<IAvanceRepository, AvanceRepository>();
builder.Services.AddScoped<IAvanceService, AvanceService>();

builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();

builder.Services.AddScoped<ITiqueteHistorialRepository, TiqueteHistorialRepository>();
builder.Services.AddScoped<ITiqueteHistorialService, TiqueteHistorialService>();
builder.Services.AddScoped<ISubCategoriaRepository, SubCategoriaRepository>();
builder.Services.AddScoped<ISubCategoriaService, SubCategoriaService>();

// Integración
builder.Services.AddScoped<IIntegracionHistorialRepository, IntegracionHistorialRepository>();
builder.Services.AddScoped<IIntegracionService, IntegracionService>();

// Inventario Equipos
builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddScoped<IActivoInventarioRepository, ActivoInventarioRepository>();
builder.Services.AddScoped<IActivoInventarioTiqueteRepository, ActivoInventarioTiqueteRepository>();
builder.Services.AddScoped<ICatalogosInventarioRepository, CatalogosInventarioRepository>();
builder.Services.AddScoped<IMantenimientoActivoRepository, MantenimientoActivoRepository>();

// Inventario Teléfonos
builder.Services.AddScoped<IActivoTelefonoRepository, ActivoTelefonoRepository>();
builder.Services.AddScoped<IActivoTelefonoService, ActivoTelefonoService>();

// Autenticación
builder.Services.AddScoped<ILoginAttemptRepository, LoginAttemptRepository>();
builder.Services.AddScoped<ILoginAttemptService, LoginAttemptService>();

// Correo
builder.Services.AddScoped<ICorreoNotificacionesService, CorreoNotificacionesService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<ConfiguracionEmail>(builder.Configuration.GetSection("GraphEmail"));

// Config general

//Helpers
builder.Services.AddScoped<IHelper, Helper>();

// Configuración general de la aplicación para la dirección base, etc.
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

// Antiforgery
builder.Services.AddAntiforgery(o =>
{
    o.Cookie.Name = "SASA.AntiForgery";
    o.Cookie.SameSite = SameSiteMode.Lax;
    o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(
        new DirectoryInfo(
            Path.Combine(builder.Environment.ContentRootPath, "dp_keys")))
    .SetApplicationName("SASA");

// Cache
builder.Services.AddDistributedMemoryCache();

// MVC + autorización global
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
});

var app = builder.Build();

// Seeder
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

    const string adminRoleName = "Administrador";
    const string adminRoleId = "ROLE_ADMIN";
    const string adminEmail = "test@sasa.com";
    const string adminPassword = "Test123!";

    var role = await roleManager.FindByNameAsync(adminRoleName);

    if (role is null)
    {
        var newRole = new ApplicationRole
        {
            Id = adminRoleId,
            Name = adminRoleName,
            NormalizedName = adminRoleName.ToUpperInvariant(),
            Estado = true
        };

        var roleResult = await roleManager.CreateAsync(newRole);

        if (!roleResult.Succeeded)
        {
            throw new Exception("Error creando rol admin: " +
                string.Join(" | ", roleResult.Errors.Select(e => e.Description)));
        }
    }

    var user = await userManager.FindByEmailAsync(adminEmail);

    if (user is null)
    {
        user = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            Estado = true,
            LockoutEnabled = true
        };

        var userResult = await userManager.CreateAsync(user, adminPassword);

        if (!userResult.Succeeded)
        {
            throw new Exception("Error creando usuario admin: " +
                string.Join(" | ", userResult.Errors.Select(e => e.Description)));
        }
    }

    if (!await userManager.IsInRoleAsync(user, adminRoleName))
    {
        var addRoleResult = await userManager.AddToRoleAsync(user, adminRoleName);

        if (!addRoleResult.Succeeded)
        {
            throw new Exception("Error asignando rol admin: " +
                string.Join(" | ", addRoleResult.Errors.Select(e => e.Description)));
        }
    }
}

app.UseExceptionHandler("/Home/Error");
app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (ctx, next) =>
{
    await next();

    if (ctx.User?.Identity?.IsAuthenticated != true)
        return;

    var path = ctx.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

    var shouldIgnore =
        path.StartsWith("/login") ||
        path.StartsWith("/forgot-password") ||
        path.StartsWith("/reset-password") ||
        path.StartsWith("/set-password") ||
        path.StartsWith("/logout");

    if (shouldIgnore)
        return;

    var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId))
        return;

    var cache = ctx.RequestServices.GetRequiredService<IDistributedCache>();
    await cache.SetStringAsync(
        $"last-activity:{userId}",
        DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
    );
});

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
