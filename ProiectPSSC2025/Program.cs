using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using ProiectPSSC2025.Interfaces;
using ProiectPSSC2025.Models.Contexts;
using ProiectPSSC2025.Services;
using ProiectPSSC2025.Services.Interfaces;
using ProiectPSSC2025.Services.Repositories;
using ProiectPSSC2025.Services.Workfows;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure EF Core
builder.Services.AddDbContext<ReservationContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,           
            maxRetryDelay: TimeSpan.FromSeconds(10), 
            errorNumbersToAdd: null))   
);


// AutoMapper registration
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Register Interfaces and Implementations
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationService, ReservationService>();

builder.Services.AddScoped<IBillingService, BillingService>();
builder.Services.AddScoped<IRoomManagement, RoomManagement>();
builder.Services.AddHostedService<RoomManagementBackgroundService>();

// Configure Azure Service Bus
string serviceBusConnectionString = builder.Configuration["ServiceBus:ConnectionString"];
if (!string.IsNullOrWhiteSpace(serviceBusConnectionString))
{
    builder.Services.AddSingleton<ServiceBusClient>(serviceProvider =>
    {
        return new ServiceBusClient(serviceBusConnectionString);
    });
}
else
{
    throw new InvalidOperationException("ServiceBus connection string is not configured. Please check your configuration.");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
