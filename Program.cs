using Microsoft.AspNetCore.Localization;
using System.Globalization;
using WeatherApp.Services.Implementations;
using WeatherApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor;
using WeatherApp.Services.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc()
             .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
             .AddDataAnnotationsLocalization();

builder.Services.AddTransient<ApplicationDatas>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IForecastRepository, ForecastRepository>();
builder.Services.AddTransient<IGetAdditionalDataRepository, GetAdditionalDataRepository>();
builder.Services.AddTransient<IGetAirQualityDataRepository, GetAirQualityDataRepository>();
builder.Services.AddTransient<IPreciseForecastData, PreciseForecastDataRepository>();
builder.Services.AddTransient<IFavouriteLocationsService, FavouriteLocationsService>();
builder.Services.AddTransient<INotificationsService, NotificationsService>();
builder.Services.AddTransient<IWeatherService, WeatherService>();
builder.Services.AddTransient<IWeatherAlertService, WeatherAlertService>();
builder.Services.AddScoped<WeatherAlertWorker>();
builder.Services.AddHostedService<WeatherAlertBackgroundService>();

builder.Services.AddDbContext<WeatherAppDbContext>(options => {
    var connectionString = builder.Configuration.GetConnectionString("WeatherAppDb");

    options.UseSqlServer(connectionString);
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("ru"),
        new CultureInfo("az")
    };

    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
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

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseRequestLocalization();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Forecast}/{action=SearchCity}");
//app.MapControllers();

app.Run();
