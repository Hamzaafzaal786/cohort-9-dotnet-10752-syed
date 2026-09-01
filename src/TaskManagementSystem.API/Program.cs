using Microsoft.OpenApi.Models;
using Serilog;
using TaskManagementSystem.API.Middleware;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Mappings;
using TaskManagementSystem.Application.Services;
using TaskManagementSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Validate required environment variables
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? throw new InvalidOperationException("JWT_SECRET environment variable is not set");

var dbConnection = Environment.GetEnvironmentVariable("DB_CONNECTION")
                   ?? builder.Configuration.GetConnectionString("DefaultConnection")
                   ?? throw new InvalidOperationException("DB_CONNECTION is not set");

// Override configuration with environment variables
builder.Configuration["JwtSettings:Secret"] = jwtSecret;
builder.Configuration["ConnectionStrings:DefaultConnection"] = dbConnection;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/taskmanagement-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Management System API",
        Version = "v1",
        Description = "API for Task Management System with JWT Authentication"
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add Infrastructure (DbContext, Identity, JWT, Repositories)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Application services
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IUserService, UserService>();

// Add CORS
// Add CORS
// Add CORS (Development only)

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// Use global exception handling
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Log application start
Log.Information("Task Management System API started");

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();