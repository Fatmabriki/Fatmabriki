using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MadaynPlatform.Web.Data;
using MadaynPlatform.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var sqlServerConn = builder.Configuration.GetConnectionString("SqlServerConnection");
var sqliteConn = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MadaynDbContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(sqlServerConn))
    {
        options.UseSqlServer(sqlServerConn);
    }
    else if (!string.IsNullOrWhiteSpace(sqliteConn))
    {
        options.UseSqlite(sqliteConn);
    }
    else
    {
        options.UseSqlite("Data Source=app.db");
    }
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireDigit = false;
    })
    .AddEntityFrameworkStores<MadaynDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

// Session for guest tracking
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Localization basic RTL culture
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// DI Services
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IRatingService, RatingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await DbInitializer.SeedAsync(app.Services);
app.Run();

