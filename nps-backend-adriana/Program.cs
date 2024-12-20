using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using nps_backend_adriana.Models.Dto.Settings;
using nps_backend_adriana.Models.Interfaces;
using nps_backend_adriana.Models.Repositories;
using nps_backend_adriana.Services;
using nps_backend_adriana.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {     
        var builder = WebApplication.CreateBuilder(args);

        // Adicionar configura��o da URL a partir do appsettings.json
        builder.Services.Configure<NpsApiSettings>(builder.Configuration.GetSection("NpsApi"));

        // Add services to the container.

        var connectionString = builder.Configuration.GetConnectionString("MinhaConexao"); // MinhaConexao est� declarado no appsettings.json
        builder.Services.AddDbContext<NpsDbContext>(options => options.UseSqlServer(connectionString));

        builder.Services.AddScoped<INpsLogRepository, NpsLogRepository>();   // padr�o repository

        builder.Services.AddScoped<INpsLogService, NpsLogService>();       
        builder.Services.AddScoped<IPathProvider, PathProvider>();
        
        // Registrar o NpsLogExporter
        builder.Services.AddScoped<INpsLogExporter, NpsLogExporter>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var logger = provider.GetRequiredService<ILogger<NpsLogExporter>>();            
            return new NpsLogExporter(connectionString, logger);
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins", builder =>
            {
                builder
                    .WithOrigins("http://localhost:5173") // Porta do Vite
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("Content-Disposition");
            });
        });        

        builder.Services.AddHttpClient();

        builder.Services.AddControllers();

        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient);  // ValidationFilter
        builder.Services.AddFluentValidationAutoValidation();     // FluentValidation

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo   // configura��o do swagger para gerar a documenta��o
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
        app.UseCors("AllowAllOrigins"); // Habilita o CORS para permitir requisi��es do React

        app.MapControllers();

        app.Run();

    }

}
