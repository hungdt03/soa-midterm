using ibanking_server.AsyncDataService;
using ibanking_server.Data;
using ibanking_server.Exceptions;
using ibanking_server.Extensions;
using ibanking_server.Services;
using ibanking_server.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plain.RabbitMQ;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<BankingDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("BankDB"));
});

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddControllers(option =>
{
    option.Filters.Add(typeof(GlobalExceptionHandler));
});

builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.Configure<ApiBehaviorOptions>(options
    => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqp://guest:guest@localhost:5673"));
builder.Services.AddSingleton<IPublisher>(x => new Publisher(x.GetService<IConnectionProvider>(),
    "update_tution_exchange",
    ExchangeType.Headers
));

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<OTPUtils>();
builder.Services.AddScoped<EmailUtils>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
