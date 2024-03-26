using ibanking_server.Data;
using ibanking_server.Exceptions;
using ibanking_server.Extensions;
using ibanking_server.Services;
using ibanking_server.SyncDataService;
using ibanking_server.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//builder.Services.AddDiscoveryClient(builder.Configuration);

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

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITutionClient, TutionClient>();
builder.Services.AddScoped<OTPUtils>();
builder.Services.AddScoped<EmailUtils>();

builder.Services.AddHttpClient("tution-service", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["TutionService:BaseUrl"]);
});

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
