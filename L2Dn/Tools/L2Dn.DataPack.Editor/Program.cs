using System.Reflection;
using L2Dn.DataPack.Db;
using L2Dn.DataPack.Editor.Components;
using Radzen;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContextFactory<DataPackDbContext>(optionsBuilder
    => DesignTimeDataPackDbContextFactory.ConfigureOptions(optionsBuilder, 
        Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "datapack.sqlite")));

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