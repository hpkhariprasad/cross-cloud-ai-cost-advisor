using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using CostAdvisor.UI.Components;
using CostAdvisor.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register HttpClient for API calls
builder.Services.AddHttpClient<ICostAdvisorService, CostAdvisorService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5058");
});

builder.Services.AddBlazorise(options =>
{
    options.Immediate = true;
}).AddBootstrap5Providers().AddFontAwesomeIcons();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => options.DetailedErrors = true);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
