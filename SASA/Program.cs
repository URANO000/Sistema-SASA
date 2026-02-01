using BusinessLogic.Servicios.Correo;
using BusinessLogic.Servicios.Rol;
using BusinessLogic.Servicios.Tiquetes;
using BusinessLogic.Servicios.Usuarios;
using DataAccess;
using DataAccess.Identity;
using DataAccess.Repositorios.Tiquetes;
using DataAccess.Repositorios.Usuarios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SASA.Services.Correo;

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

// Cookies (rutas + expiración por inactividad)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/Account/AccessDenied";

    // #19: cierre automático por inactividad
    // Se ajusta el tiempo según el sprint (ej: 10, 15, 20 min). Dejo 10 por defecto.
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);

    // Renueva el tiempo de expiración con actividad (requests)
    options.SlidingExpiration = true;

    // Recomendado
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});


//Repositories y Servicios de negocio
builder.Services.AddScoped<ITiqueteRepository, TiqueteRepository>();
builder.Services.AddScoped<ITiqueteService, TiqueteService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IRolService, RolService>();

builder.Services.Configure<SmtpEmailSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

builder.Services.AddTransient<ActivationEmailService>();

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

//Seeder (temporal)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

    const string adminRole = "Admin";
    var email = "test@sasa.com";

    // Crear rol Admin si no existe
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new ApplicationRole { Name = adminRole, Estado = true });
    }

    // Crear usuario admin si no existe
    var user = await userManager.FindByEmailAsync(email);
    if (user is null)
    {
        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            Estado = true,
            LockoutEnabled = true
        };

        await userManager.CreateAsync(user, "Test123!");
    }

    // Asignarlo al rol Admin si no está
    if (!await userManager.IsInRoleAsync(user, adminRole))
    {
        await userManager.AddToRoleAsync(user, adminRole);
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
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
