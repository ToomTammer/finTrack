using finTrack.Controllers.Data;
using finTrack.Interfaces;
using finTrack.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using finTrack.Models;
using finTrack.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using finTrack.Middlewares;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager config = builder.Configuration;

builder.Services
    .AddHttpContextAccessor()
    .AddControllersWithViews()
    .AddDataAnnotationsLocalization();

builder.Services.AddSession();
builder.Services.AddSingleton<IConfiguration>(config);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDBContext>(option =>
{
    option.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});


builder.Services.AddIdentity<AppUser, IdentityRole>(option =>
{
    option.Password.RequireDigit = true;
    option.Password.RequireLowercase = true;
    option.Password.RequireUppercase = true;
    option.Password.RequireNonAlphanumeric = true;
    option.Password.RequiredLength = 12;
})
.AddEntityFrameworkStores<ApplicationDBContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme =
    option.DefaultChallengeScheme =
    option.DefaultForbidScheme =
    option.DefaultScheme =
    option.DefaultSignInScheme =
    option.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>{
    
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, 
        ValidIssuer = builder.Configuration["JWT:Issuer"], 
        ValidateAudience = true, 
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true, 
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]!)
        )

    };
});

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAcountRepository, AcountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
// Add the global scope for JwtService.
builder.Services.AddScoped<TokenService>(); 
builder.Services.AddTransient<JwtMiddleware>();

var app = builder.Build();

app.UseMiddleware<JwtMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        dbContext.Database.Migrate();
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
