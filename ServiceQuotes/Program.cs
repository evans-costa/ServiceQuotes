using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using ServiceQuotes.Context;
using ServiceQuotes.Extensions;
using ServiceQuotes.Logging;
using ServiceQuotes.Mappings;
using ServiceQuotes.Repositories;
using ServiceQuotes.Repositories.Interfaces;
using ServiceQuotes.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information,
}));

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var sqlServerConnection = builder.Configuration["ConnectionStrings:DefaultConnection"];

builder.Services.AddDbContext<ServiceQuoteApiContext>(options =>
{
    options.UseSqlServer(sqlServerConnection);
});

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IQuotesRepository, QuotesRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

builder.Services.AddAutoMapper(typeof(CustomerDTOMappingProfile));
builder.Services.AddAutoMapper(typeof(ProductDTOMappingProfile));
builder.Services.AddAutoMapper(typeof(QuoteDTOMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
