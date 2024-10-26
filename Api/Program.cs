using Api.AppHelper;
using DBServices.Interfaces;
using DBServices.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PMT-API", Version = "v1" });

    // Configure Swagger to use Bearer Token Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token **_only_**"
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

builder.Services.AddHttpContextAccessor();

// Database context setup
builder.Services.AddDbContext<PMTContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PMT")));

// Dependency injection for services
builder.Services.AddScoped(typeof(GenericService<>));
builder.Services.AddTransient<CommonModule>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();


// JWT Authentication setup
var key = builder.Configuration["Jwt:Key"]; // Use Configuration to fetch the secret key
var keyBytes = Encoding.UTF8.GetBytes(key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false
    };

});
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:4200") 
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});
var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure authentication is applied before authorization
app.UseHttpsRedirection();
app.UseAuthentication(); // Use Authentication first
app.UseAuthorization();  // Then use Authorization

app.MapControllers();
app.UseCors("MyCorsPolicy");
app.Map("/test", async context =>
{
    await context.Response.WriteAsync("This is a test route!");
});
app.Run();


