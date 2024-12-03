using System.Net;
using System.Text;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using IGift.Application.Interfaces.DDBB.Sql;
using IGift.Application.Interfaces.Files;
using IGift.Application.Interfaces.Identity;
using IGift.Application.Interfaces.IMailService;
using IGift.Application.Interfaces.Repositories;
using IGift.Application.Requests.Notifications.Query;
using IGift.Infrastructure.Data;
using IGift.Infrastructure.Models;
using IGift.Infrastructure.Repositories;
using IGift.Infrastructure.Services.DDBB.Sql;
using IGift.Infrastructure.Services.Files;
using IGift.Infrastructure.Services.Identity;
using IGift.Infrastructure.Services.Mail;
using IGift.Server.Hubs;
using IGift.Server.Middleware;
using IGift.Shared.Constants;
using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//TODO fijarse como vamos a usar los logs
//serilog
var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
var logFilePath = Path.Combine(desktopPath, "app-logs.txt");

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); // Usa Serilog en lugar del logger predeterminado

// Add services to the container.


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

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query[AppConstants.StorageConstants.Local.Access_Token];

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(AppConstants.SignalR.HubUrl))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = c =>
        {
            if (c.Exception is SecurityTokenExpiredException)
            {
                c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                c.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(Result.Fail("The token is expired"));
                return c.Response.WriteAsync(result);
            }
            else
            {
#if DEBUG
                c.NoResult();
                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                c.Response.ContentType = "text/plain";
                return c.Response.WriteAsync(c.Exception.ToString());
#else
                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                c.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(Result.Fail(localizer["An unhandled error has occurred."]));
                                return c.Response.WriteAsync(result);
#endif
            }
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(Result.Fail("You are not Authorized."));
                return context.Response.WriteAsync(result);
            }

            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/json";
            var result = JsonConvert.SerializeObject(Result.Fail("You are not authorized to access this resource."));
            return context.Response.WriteAsync(result);
        }
    };
});//TODO estudiar esto
//TODO debemos agregar por acá el AddAuthorization de blazorHero para los permisos/roles

//Repositories
builder.Services.AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
builder.Services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));

//Scopes
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IProfilePicture, ProfilePictureService>();

//builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Files")));



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

//SignalR
builder.Services.AddSignalR();


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
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IGift.SqlServerService API v1");
});

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
            User =AppConstants.StorageConstants.Server.AdminEmail,
            Pass = AppConstants.StorageConstants.Server.DefaultPassword
        }
    }
});

app.MapRazorPages();

app.UseMiddleware<MyMiddleware>();


app.MapControllers();
app.MapFallbackToFile("index.html");

//SignalR
app.MapHub<SignalRHub>(AppConstants.SignalR.HubUrl);

app.Run();
