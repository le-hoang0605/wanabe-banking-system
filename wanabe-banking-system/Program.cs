using Accounts;
using Authentications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Parties;
using System.Text;
using Transactions;
using wanabe_banking_system.UseCases;
using wanabe_banking_system.UseCases.RegisterOrchestrator;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DBConnection");

//resigter modules
builder.Services.AddTransactionsModule(builder.Configuration);
builder.Services.AddAuthenticationsModule(builder.Configuration);
builder.Services.AddPartiesModule(builder.Configuration);
builder.Services.AddAccountsModule(builder.Configuration);



//register orchestrator
builder.Services.AddScoped<RegisterOrchestrator>();
builder.Services.AddScoped<LoginOrchestrator>();

builder.Services.AddScoped<RegisterOrchestrator>();
builder.Services.AddScoped<LoginOrchestrator>();
builder.Services.AddScoped<TransferOrchestrator>();

// Load JWT settings from configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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



builder.Services.AddControllers()
    .AddApplicationPart(typeof(Accounts.DependencyInjection).Assembly)
    .AddApplicationPart(typeof(Transactions.DependencyInjection).Assembly)
    .AddApplicationPart(typeof(Authentications.DependencyInjection).Assembly)
    .AddApplicationPart(typeof(Parties.DependencyInjection).Assembly);



// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // 1. Define the "Bearer" security scheme
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()   
                .AllowAnyMethod()   
                .AllowAnyHeader(); 
        });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.UseStaticFiles();

app.MapGet("/", () => "banking system API");
app.Run();


public partial class Program { }