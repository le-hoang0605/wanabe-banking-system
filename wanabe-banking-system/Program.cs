using Authentications;
using Microsoft.OpenApi;
using Parties;
using Transactions;
using wanabe_banking_system.UseCases;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DBConnection");


builder.Services.AddTransactionsModule(builder.Configuration);
builder.Services.AddAuthenticationsModule(builder.Configuration);
builder.Services.AddPartiesModule(builder.Configuration);

builder.Services.AddScoped<LoginOrchestrator>();

builder.Services.AddControllers()
    .AddApplicationPart(typeof(Accounts.DependencyInjection).Assembly)
    .AddApplicationPart(typeof(Transactions.DependencyInjection).Assembly)
    .AddApplicationPart(typeof(Authentications.DependencyInjection).Assembly)
    .AddApplicationPart(typeof(Parties.DependencyInjection).Assembly);



// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllers();
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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.UseStaticFiles();

app.MapGet("/", () => "banking system API");
app.Run();