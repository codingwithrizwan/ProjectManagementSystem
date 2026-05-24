using PMS.Web.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddPageRoute("/Index", "home");
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ApiAuthHeaderHandler>();
builder.Services.AddHttpClient("PmsApi", client =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7064";
    client.BaseAddress = new Uri(baseUrl);
})
.AddHttpMessageHandler<ApiAuthHeaderHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
