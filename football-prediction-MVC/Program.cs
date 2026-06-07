using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using football_prediction_MVC.Data;
using football_prediction_MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity with roles
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// MVC
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<IPredictionService, PredictionService>((serviceProvider, client) =>
{
    client.ConfigureApiClient(serviceProvider.GetRequiredService<IConfiguration>());
});
builder.Services.AddHttpClient<IDataService, DataService>((serviceProvider, client) =>
{
    client.ConfigureApiClient(serviceProvider.GetRequiredService<IConfiguration>());
});
builder.Services.AddHttpClient<ITrainingService, TrainingService>((serviceProvider, client) =>
{
    client.ConfigureApiClient(serviceProvider.GetRequiredService<IConfiguration>());
});

builder.Services.AddSingleton<IFunFactRepository, FunFactRepository>();
builder.Services.AddScoped<IFunFactService, FunFactService>();

// Football assistant chatbot (Claude API + tool use)
builder.Services.AddScoped<ChatToolExecutor>();
builder.Services.AddHttpClient<IChatService, ClaudeChatService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(configuration["Anthropic:BaseUrl"] ?? "https://api.anthropic.com/");
    client.DefaultRequestHeaders.Add("x-api-key", configuration["Anthropic:ApiKey"] ?? string.Empty);
    client.DefaultRequestHeaders.Add("anthropic-version", configuration["Anthropic:Version"] ?? "2023-06-01");
    client.Timeout = TimeSpan.FromSeconds(120);
});

// Allow the chat JSON endpoint to validate the antiforgery token from a request header.
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Admin"));
});

var app = builder.Build();

// Seed roles and default admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAndAdmin(services);
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();

static async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = ["Admin", "User"];

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    const string adminEmail = "admin@example.com";
    const string adminPassword = "Admin@123456";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, adminPassword);
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}