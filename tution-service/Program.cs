using ibanking_server.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plain.RabbitMQ;
using RabbitMQ.Client;
using tution_service.Background;
using tution_service.Data;
using tution_service.Exceptions;
using tution_service.Extensions;
using tution_service.Services;
using tution_service.SyncDataServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.Configure<ApiBehaviorOptions>(options
    => options.SuppressModelStateInvalidFilter = true);
builder.Services.AddHttpClient<ITransactionClient, TransactionClient>();
builder.Services.AddScoped<ITutionService, TutionService>();
builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqp://guest:guest@localhost:5673"));
builder.Services.AddSingleton<ISubscriber>(x => new Subscriber(x.GetService<IConnectionProvider>(), 
    "update_tution_exchange",
    "update-tution-queue",
    "tution.*",
    ExchangeType.Headers
));

builder.Services.AddDbContext<TutionDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("Mydb"));
});


builder.Services.AddHostedService<TutionSubcribeBackground>();

builder.Services.AddControllers(option =>
{
    option.Filters.Add(typeof(GlobalExceptionHandler));
});








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
