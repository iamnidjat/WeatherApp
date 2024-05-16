using Microsoft.AspNetCore.Localization;
using System.Globalization;
using WeatherApp.Services.Implementations;
using WeatherApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IForecastRepository, ForecastRepository>();
builder.Services.AddScoped<IGetAdditionalDataRepository, GetAdditionalDataRepository>();
builder.Services.AddScoped<IGetAirQualityDataRepository, GetAirQualityDataRepository>();
builder.Services.AddScoped<IPreciseForecastData, PreciseForecastDataRepository>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddDbContext<WeatherAppDbContext>(options => {
    var connectionString = builder.Configuration.GetConnectionString("WeahterAppDb");

    options.UseSqlServer(connectionString);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => //CookieAuthenticationOptions
    {
        options.LoginPath = new PathString("/Account/Login");
        options.LogoutPath = new PathString("/Account/Logout");
    });

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

app.UseRouting();

app.UseAuthorization();

app.UseRequestLocalization();

var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("ru-RU"),
    new CultureInfo("az-AZ"),
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Forecast}/{action=SearchCity}/{id?}");

app.Run();
