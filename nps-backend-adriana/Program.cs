using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo   // configuração do swagger para gerar a documentação
    {
        Title = "nps-backend-adriana",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "Adriana",
            Email = "adriana.p@ambevtech.com.br",
            Url = new Uri("https://adriana.com.br")
        }
    });
    var xmlFile = "nps-backend-adriana.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//app.UseRouting();
app.UseCors("AllowAllOrigins"); // Habilita o CORS para permitir requisições do React

app.MapControllers();

app.Run();
