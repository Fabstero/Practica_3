using Practica_2.Api;
using Practica_2.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddProblemDetails();

var apiBaseUrl = builder.Configuration["Api:BaseUrl"]
    ?? throw new InvalidOperationException("No se encontró la URL base de la API.");

if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var apiUri))
{
    throw new InvalidOperationException("La URL base de la API no es válida.");
}

builder.Services.AddHttpClient<IVotanteServicio, ApiVotanteServicio>(client =>
{
    client.BaseAddress = apiUri;
});

builder.Services.AddHttpClient<IPartidoPoliticoServicio, ApiPartidoPoliticoServicio>(client =>
{
    client.BaseAddress = apiUri;
});

builder.Services.AddHttpClient<IVotoServicio, ApiVotoServicio>(client =>
{
    client.BaseAddress = apiUri;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
