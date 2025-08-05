using DjayAccounts.Api;
using DjayAccounts.Core;
using DjayAccounts.DbPersistence;
using DjayAccounts.EntityFramework.Contexts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connection = new SqliteConnection(builder.Configuration.GetConnectionString("AccountDbContext"));
connection.Open();

// Add services to the container.
builder.Services.AddScoped<AccountManager>();
builder.Services.AddScoped<AccountDbPersistence>();
builder.Services
    .AddEntityFrameworkSqlite()
    .AddDbContext<AccountDbContext>(options =>
        options.UseSqlite(connection));

builder.Services.AddAutoMapper(_ =>
{
    _.AddProfile<AccountProfile>();
    _.AddProfile<AccountApiProfile>();
});

builder.Services.AddHostedService<DbInitializerHostedService>();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        // Converts all enums to strings globally
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Banking API",
        Version = "v1",
        Description = "This API allows managing accounts and customers.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Support Team",
            Email = "wrzucher@gmail.com",
        },
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    c.UseInlineDefinitionsForEnums();
});

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

app.Run();
