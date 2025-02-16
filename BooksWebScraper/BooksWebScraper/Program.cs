using BooksWebScraperWebAPI.Data;
using BooksWebScraperWebAPI.Interfaces;
using BooksWebScraperWebAPI.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, config) => config.WriteTo.Console());

// Configure Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Hangfire
builder.Services.AddHangfire(config => config
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

// Add Services
builder.Services.AddScoped<IScraperService, ScraperService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseHangfireDashboard();

// Schedule periodic scraping
RecurringJob.AddOrUpdate<ScraperService>(s => s.ScrapeAndStoreBooks(), Cron.Hourly);

app.Run();
