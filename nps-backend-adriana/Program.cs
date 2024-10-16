using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using nps_backend_adriana.Models.Dto.Settings;
using nps_backend_adriana.Models.Interfaces;
using nps_backend_adriana.Models.Repositories;
using nps_backend_adriana.Services;
using nps_backend_adriana.Services.Interfaces;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Adicionar configuração da URL a partir do appsettings.json
builder.Services.Configure<NpsApiSettings>(builder.Configuration.GetSection("NpsApi"));

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("MinhaConexao"); // MinhaConexao está declarado no appsettings.json
builder.Services.AddDbContext<NpsDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<INpsLogRepository, NpsLogRepository>();   // padrão repository

builder.Services.AddScoped<INpsLogService, NpsLogService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.Services.AddHttpClient();

builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient);  // ValidationFilter
builder.Services.AddFluentValidationAutoValidation();     // FluentValidation


builder.Services.AddScoped<NpsLogService>();   // Registrar o NpsLogService

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRouting();
app.UseCors("AllowAllOrigins"); // Habilita o CORS para permitir requisições do React

app.MapControllers();

app.Run();
