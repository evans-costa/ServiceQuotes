using Microsoft.EntityFrameworkCore;
using ServiceQuotes.Context;
using ServiceQuotes.DTOs.Mappings;
using ServiceQuotes.Repositories;
using ServiceQuotes.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var sqlServerConnection = builder.Configuration["ConnectionStrings:DefaultConnection"];

builder.Services.AddDbContext<ServiceQuoteApiContext>(options =>
{
    options.UseSqlServer(sqlServerConnection);
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddAutoMapper(typeof(CustomerDTOMappingProfile));
builder.Services.AddAutoMapper(typeof(ProductDTOMappingProfile));

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
