using ibanking_server.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Discovery.Client;
using System;
using tution_service.Data;
using tution_service.Exceptions;
using tution_service.Extensions;
using tution_service.Services;
using tution_service.SyncDataServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddDiscoveryClient(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.Configure<ApiBehaviorOptions>(options
    => options.SuppressModelStateInvalidFilter = true);
builder.Services.AddHttpClient<ITransactionClient, TransactionClient>();
builder.Services.AddScoped<ITutionService, TutionService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<TutionDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("Mydb"));
});

builder.Services.AddControllers(option =>
{
    option.Filters.Add(typeof(GlobalExceptionHandler));
});


builder.Services.AddHttpClient("banking-service", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BankingService:BaseUrl"]);
}).AddServiceDiscovery();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


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
