using Microsoft.EntityFrameworkCore;
using nps_backend_adriana.Data;
using nps_backend_adriana.Models.Interfaces;
using nps_backend_adriana.Models.Repositories;
using nps_backend_adriana.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("MinhaConexao"); // MinhaConexao está declarado no appsettings.json
builder.Services.AddDbContext<NpsDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<INpsLogRepository, NpsLogRepository>();   // padrão repository

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
