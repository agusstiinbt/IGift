using IGift.Application.CQRS.LocalesAdheridos.Query;
using IGift.Application.Interfaces.Files;
using IGift.Application.Interfaces.Repositories.Generic.Auditable;
using IGift.Infrastructure.Repositories.Generic.Auditable;
using IGift.Infrastructure.Services.Files;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetAllLocalAdheridoQuery>());

//Mapeo
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddTransient<IUploadService, UploadService>();


//Repositories
builder.Services.AddTransient(typeof(IAuditableUnitOfWork<>), typeof(AuditableUnitOfWork<>));
builder.Services.AddTransient(typeof(IAuditableRepository<,>), typeof(AuditableRepository<,>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
