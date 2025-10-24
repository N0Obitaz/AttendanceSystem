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
    options.Conventions.AuthorizeFolder("/TransactionLogs");
    options.Conventions.AllowAnonymousToPage("/Index");
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
});

builder.Services.AddDbContext<AttendanceSystemContext>(options =>   
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

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

// ADD THIS MIDDLEWARE - ORDER IS CRITICAL!
app.UseSession();      // Session must come before Authentication

    
app.UseAuthentication(); // Add this line - it was missing
app.UseAuthorization();  // This was already here

app.MapRazorPages();

app.Run();