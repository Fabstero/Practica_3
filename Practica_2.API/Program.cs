using Microsoft.EntityFrameworkCore;
using Practica_2.Application.Mapping;
using Practica_2.Application.Services;
using Practica_2.Infrastructure.Data;
using Practica_2.Infrastructure.Repositories;
using Practica_2.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

var conexion = builder.Configuration
    .GetConnectionString("ConexionSQLite")
    ?? throw new InvalidOperationException("No se encontró la conexión SQLite.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(conexion));

builder.Services.AddScoped(typeof(IRepositorioGenerico<>), typeof(RepositorioGenerico<>));
builder.Services.AddScoped<IVotanteRepository, VotanteRepository>();
builder.Services.AddScoped<IPartidoPoliticoRepository, PartidoPoliticoRepository>();
builder.Services.AddScoped<IVotoRepository, VotoRepository>();
builder.Services.AddScoped<IVotanteServicio, VotanteServicio>();
builder.Services.AddScoped<IPartidoPoliticoServicio, PartidoPoliticoServicio>();
builder.Services.AddScoped<IVotoServicio, VotoServicio>();
builder.Services.AddAutoMapper(cfg => { }, typeof(MapeoClases));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.InicializarAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
