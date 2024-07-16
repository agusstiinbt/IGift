using IGift.Infrastructure.Data;
using IGift.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IGift.Application.Interfaces.Identity;
using IGift.Infrastructure.Services.Identity;
using IGift.Application.Interfaces;
using IGift.Infrastructure;
using IGift.Server.Middleware;
using IGift.Application.Interfaces.IMailService;
using IGift.Infrastructure.Services.Mail;
using Hangfire;
using IGift.Shared;
using HangfireBasicAuthenticationFilter;
using IGift.Application.Features.Notifications.Query;
using IGift.Application.Interfaces.Repositories;
using IGift.Infrastructure.Repositories;
using IGift.Application.Interfaces.Files;
using IGift.Infrastructure.Services.Files;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddIdentity<IGiftUser, IGiftRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services
    .AddAuthentication(authentication =>
    {
        authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSecurityKey"]!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JwtIssuer"],
        ValidAudience = builder.Configuration["JwtAudience"],
        ClockSkew = TimeSpan.Zero//Para más información sobre esto leer el README
    };
});

//Repositories
builder.Services.AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
builder.Services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));

//Scopes
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
builder.Services.AddScoped<IUploadService, UploadService>();

//Mapeo
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


//CORS
builder.Services.AddCors(options =>
options.AddDefaultPolicy(
    builder =>
    {
        builder.AllowCredentials().AllowAnyHeader().AllowAnyMethod().WithOrigins();
    }));

//hangfire
builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();

//MediatR


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllNotificationQuery).Assembly));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seeder = services.GetRequiredService<IDatabaseSeeder>();
    seeder.Initialize();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

//Hangfire
app.UseHangfireDashboard("/HangfireDashboard", new DashboardOptions
{
    //AppPath = "" //The path for the Back To Site link. Set to null in order to hide the Back To  Site link.
    DashboardTitle = "IGift Dashboard",
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter
        {
            User =AppConstants.AdminEmail,
            Pass = AppConstants.DefaultPassword
        }
    }
});

app.MapRazorPages();

app.UseMiddleware<MyMiddleware>();


app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
