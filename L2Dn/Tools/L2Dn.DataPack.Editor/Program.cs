using System.Reflection;
using L2Dn.DataPack.Db;
using L2Dn.DataPack.Editor.Components;
using Radzen;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContextFactory<DataPackDbContext>(optionsBuilder =>
{
    const string databaseFileName = "datapack.sqlite";
    string assemblyLocation = Assembly.GetExecutingAssembly().Location;
    string? assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
    string databasePath = string.IsNullOrEmpty(assemblyDirectory)
        ? databaseFileName
        : Path.Combine(assemblyDirectory, databaseFileName);

    DesignTimeDataPackDbContextFactory.ConfigureOptions(optionsBuilder, databasePath);
});

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRadzenComponents();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();