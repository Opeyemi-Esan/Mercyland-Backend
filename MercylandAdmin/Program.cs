using MercylandAdmin.Inplementation;
using MercylandAdmin.Inplimentation;
using MercylandAdmin.Interface;
using MercylandAdmin.Models;
using MercylandAdmin.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var logDirectory = @"C:\Users\HP PC\Documents\coding\React Job\MercylandFolder\MercylandAdmin\Logging";
if (!Directory.Exists(logDirectory))
{
    //Directory.CreateDirectory(logDirectory);
}

// Configure Serilog  
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() 
    .WriteTo.Console() 
    .WriteTo.File(Path.Combine(logDirectory, "Loggs.txt"), rollingInterval: RollingInterval.Day) // Log to a file  
    .CreateLogger();

try
{
    Log.Information("Starting up the application...");

    // Configure JWT authentication  
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var key = jwtSettings["Secret"];
    var issuer = jwtSettings["ValidIssuer"];
    var audience = jwtSettings["ValidAudience"];

    if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
    {
        throw new ArgumentNullException("JwtSettings settings are not configured correctly.");
    }

    // Add services to the container.  
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
    builder.Services.AddScoped<IPropertyService, PropertyService>();
    builder.Services.AddScoped<IHomeSliderService, HomeSliderService>();
    builder.Services.AddScoped<IRealEstateService, RealEstateService>();
    builder.Services.AddScoped<IVideoAdvertService, VideoAdvertService>();
    builder.Services.AddScoped<IDocumentUploadService, DocumentUploadService>();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(option=>
    {
        option.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Mercyland API",
            Version = "v1"
        });

        option.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Please enter your token in the format **Bearer {your token}**",
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });

        option.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
                new string[] {}
            }
        });
    });

    builder.Services.AddDbContext<AppDbContext>(options => 
        options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection")));

    builder.Services.AddIdentity<UserModel, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

    // Configure JWT authentication  
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
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

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ReactApp", policyBuilder =>
            policyBuilder.WithOrigins("http://localhost:3000") // Your frontend's URL  
                         .AllowAnyHeader() // Allow any request header  
                         .AllowAnyMethod() // Allow any HTTP method  
                         .AllowCredentials()); // Allow credentials  
    });
    

    var app = builder.Build();

    // Configure the HTTP request pipeline.  
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();
    app.UseCors("ReactApp");
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    // Log fatal error and ensure flush before exit  
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush(); // Ensure Serilog flushes all log events  
}