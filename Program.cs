<<<<<<< HEAD
﻿using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UserService.Data;
=======
<<<<<<< HEAD
﻿using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.Helpers;
using Microsoft.OpenApi.Models;
>>>>>>> 0cf120da0bdc5533440939b2d1292185d4a9f130

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
<<<<<<< HEAD
builder.Services.AddHttpClient();

// Configure Database
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? ""))
        };
    });

// ✅ CORS Configuration - Must allow React app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")  // React frontend
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "User Service API",
        Version = "v1",
        Description = "User Management Microservice"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer {token}')",
=======

// Add HttpClient service
builder.Services.AddHttpClient();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Auth Service API",
        Version = "v1",
        Description = "Microservices Authentication Service"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \n\n" +
                      "Enter 'Bearer' [space] and then your token in the text input below.\n\n" +
                      "Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
>>>>>>> 0cf120da0bdc5533440939b2d1292185d4a9f130
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

<<<<<<< HEAD
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
=======
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
>>>>>>> 0cf120da0bdc5533440939b2d1292185d4a9f130
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
<<<<<<< HEAD
                }
            },
            new string[] {}
=======
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
>>>>>>> 0cf120da0bdc5533440939b2d1292185d4a9f130
        }
    });
});

<<<<<<< HEAD
var app = builder.Build();

=======
// Add DbContext with SQL Server
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add JWT Helper
builder.Services.AddScoped<JwtHelper>();

// ✅ FIXED CORS - Allow localhost:3000 specifically
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")  // React app
=======
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot configuration file
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add JWT Authentication
var jwtSecret = "YourSuperSecretKeyForJWTTokenGeneration123456789";
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "StudentManagementAuthService",
            ValidAudience = "StudentManagementClients",
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

// Add Ocelot services
builder.Services.AddOcelot();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
>>>>>>> ba9def238678208eb864981aa9b49f329be4c17e
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

<<<<<<< HEAD
>>>>>>> 0cf120da0bdc5533440939b2d1292185d4a9f130
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
<<<<<<< HEAD
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Service API V1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

// ✅ MIDDLEWARE ORDER IS CRITICAL
app.UseCors("AllowReactApp");  // Must be BEFORE Authentication

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ✅ Explicitly set port
app.Run("http://localhost:7080");
=======
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Service API V1");
    });
}

// ✅ IMPORTANT: CORS must come BEFORE UseHttpsRedirection
app.UseCors();

app.UseAuthorization();
app.MapControllers();
=======
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Use Ocelot middleware
await app.UseOcelot();
>>>>>>> ba9def238678208eb864981aa9b49f329be4c17e

app.Run();
>>>>>>> 0cf120da0bdc5533440939b2d1292185d4a9f130
