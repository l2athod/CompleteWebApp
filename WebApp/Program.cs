using Microsoft.EntityFrameworkCore;
using MyApp.DataAccessLayer.Data;
using MyApp.DataAccessLayer.DataLayer.IRepository;
using MyApp.DataAccessLayer.DataLayer.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using MyApp.CommonHelper;
using Stripe;
using Azure;
using MyApp.DataAccessLayer.DbInitializer;

var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IEmailSender,EmailSender>();
builder.Services.AddScoped<IDbInitializer, DbInitialize>();
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "";
    options.AppSecret = "";
});
builder.Services.AddDbContext<ApplicationDbContext>(optins =>
{
    optins.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection"));
});
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("PaymentSettings"));
builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//builder.Services.Configure<SecurityStampValidatorOptions>(options =>
//{
//    options.ValidationInterval = TimeSpan.Zero;
//});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
databaseSeed();
//app.UseMiddleware<NoCacheMiddleware>();
app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("PaymentSettings:SecretKey").Get<string>();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages(); 
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}"
);
app.Run();

void databaseSeed()
{
    using(var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}