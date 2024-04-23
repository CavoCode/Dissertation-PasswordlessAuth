using Enhancing_ECommerce_Security_A_Passwordless_Approach.Data;
using Enhancing_ECommerce_Security_A_Passwordless_Approach.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ApplicationDbContext = Enhancing_ECommerce_Security_A_Passwordless_Approach.Data.ApplicationDbContext;
using IEmailSender = Enhancing_ECommerce_Security_A_Passwordless_Approach.Services.IEmailSender;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddTokenProvider("TOTPLoginTokenProvider", typeof(TOTPLoginTokenProvider));

builder.Services.AddControllersWithViews();

builder.Services.AddFido(options =>
{
    options.Licensee = "DEMO";
    options.LicenseKey = "eyJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjQtMDUtMTRUMDE6MDA6MDEuNjYxMDIyNCswMDowMCIsImlhdCI6IjIwMjQtMDQtMTRUMDE6MDA6MDEiLCJvcmciOiJERU1PIiwiYXVkIjo2fQ==.J+jGG2HugbWN5HkJAF6Cvlq6A+Lch950HTQA49G6QTeh4lVNptz3JHIGtidwW9U5q8o6wGgNJMGler+9iIj4T3roAUbW3ZIzonyADle8V60w0ah/qpip+F81ul6q07pspRqfrw/Fh5oGPri3VpK+RFN+MPnJYlKy3O7DU+AJIvhR1eO62mubrL25ryE73dxd1NJDyQi25mKOV4Nt+3AwYBUDIsjennRQ6P7rj45ZmKzZnfl5sf1aeBEHsERqSPONLIJrptpND9iEHR+BJIDU+3FlUbCGA7MgqpiSJ67r+KKOFO1tsFZ38GtpF15tU0Of7bGCZT19Hvjx4ObtJWz7TQsR4EFhXFhW3TZ+ApG265JysgIkP2JKzcSxzVDPiGYmoJI88i1//a7aYGwYM1b56EjbrdOXEfyX9u5GLPTUMyTcq//twocWLBMchhyNfGe8iHKOa9EtTSMVIeshpOwBipI5nerW106y/luR4kIZyz/g/ishCyImtRzpC59C33eHriQVaiDZkzP64x9kC53izihhKYpVvnI/P6EtzsNJ0CN03dUdCY8UG4WgspHgsGHbTUr+9orACQR9QH0SVX21qokKrooT0IXKQbnenQ7MJsKBIjz9vYKqDQL208F2cDuSkXy9tMHBDHvCN4riGraFUF8mhM7UWeFIEcxLsdtlNr4=";
}).AddEntityFrameworkStore(options =>
    options.UseSqlServer(connectionString));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailService>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
