using UnganaConnect.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UnganaConnect.Service;
using UnganaConnect.Middleware;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/unganaconnect-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog();

// Add services to the container.

//BD Context Registeration
Env.Load();

var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
builder.Services.AddDbContext<UnganaConnectDbcontext>(options =>
    options.UseNpgsql(connectionString));


// JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Environment.GetEnvironmentVariable("Jwt__Key");
        var issuer = Environment.GetEnvironmentVariable("Jwt__Issuer");
        var audience = Environment.GetEnvironmentVariable("Jwt__Audience");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddControllers();
// Enable MVC with views to serve integrated UI
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// App services
builder.Services.AddSingleton<BlobServices>();
builder.Services.AddSingleton<CertificateService>();
builder.Services.AddSingleton<FileServices>();
builder.Services.AddScoped<AdminSeederService>();
// UI integration services
builder.Services.AddHttpClient("UiApiClient");
builder.Services.AddScoped<UnganaConnect.Service.UI.ApiService>();
builder.Services.AddScoped<UnganaConnect.Service.UI.AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add global exception handling
app.UseGlobalExceptionMiddleware();

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Serve static files from wwwroot (for integrated UI assets)
app.UseStaticFiles();

// Enable session for MVC flows
app.UseSession();

// Seed admin users on startup
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<AdminSeederService>();
    await seeder.SeedAdminUsersAsync();
    await seeder.SeedDefaultMemberAsync();
}

// Map default route for MVC controllers and views
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Keep API controllers mapped
app.MapControllers();

app.Run();
