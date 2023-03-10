using DorisApp.WebAPI.DataAccess;
using DorisApp.WebAPI.DataAccess.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//App Services
builder.Services.AddTransient<ISqlDataAccess, SqlDataAccess>();
builder.Services.AddTransient<IRoleData, RoleData>();

//Add Logger Services
builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
});

// Add Logger Instance
builder.Services.AddScoped<ILogger>(serviceProvider =>
{
    var factory = serviceProvider.GetRequiredService<ILoggerFactory>();
    return factory.CreateLogger("AppLogger");
});

//JWT Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["JwtConfig:Key"];

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            //ValidIssuer = "apiWithAuthBackend",
            //ValidAudience = "apiWithAuthBackend",
            ClockSkew = TimeSpan.FromMinutes(5),
            IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key)
                ),
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();