using AttendanceSystem.Data;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AttendanceSystem.Services;
using AttendanceSystem.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Set up Serilog for logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddSession();

// Configure Email Settings
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// Register Email Service
builder.Services.AddTransient<EmailService>();

// ADD AUTHENTICATION CONFIGURATION
builder.Services.AddAuthentication("CustomSession")
    .AddCookie("CustomSession", options =>
    {
        options.LoginPath = "/Index";          
        options.LogoutPath = "/Logout/Logout";        
        options.AccessDeniedPath = "/AccessDenied";   
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });



// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/student_view", "StudentOnly");
    options.Conventions.AuthorizeFolder("/Admin_view", "AdminOnly");
    options.Conventions.AllowAnonymousToPage("/Index");
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
});

builder.Services.AddDbContext<AttendanceSystemContext>(options =>
    options.UseSqlServer(

        builder.Configuration.GetConnectionString("DefaultConnection"),
       sqlServerOptionsAction: sqlOptions =>
       {
           // This is the magic line that fixes the error
           sqlOptions.EnableRetryOnFailure(
               maxRetryCount: 5,
               maxRetryDelay: TimeSpan.FromSeconds(30),
               errorNumbersToAdd: null);
       }));

builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.SchemaName = "dbo";
    options.TableName = "AttendanceSystemCache";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseSession();      

    
app.UseAuthentication(); 
app.UseAuthorization(); 

app.MapRazorPages();

app.Run();