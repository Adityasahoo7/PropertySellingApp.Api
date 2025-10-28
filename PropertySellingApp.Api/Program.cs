using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PropertySellingApp.DataAccess;
using PropertySellingApp.DataAccess.Interfaces;
using PropertySellingApp.DataAccess.Repositories;
using PropertySellingApp.Services.Implementations;
using PropertySellingApp.Services.Interfaces;
using PropertySellingApp.Services.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//Get Dbconn from azurekey vault


//var keyVaultUrl = new Uri("https://acreskeyvault.vault.azure.net/");

//// ?? 2. Create a Secret Client (Azure SDK)
//var client = new SecretClient(vaultUri: keyVaultUrl, credential: new DefaultAzureCredential());

//// ?? 3. Get your secret value
//KeyVaultSecret secret = client.GetSecret("localdbconn");

//// ?? 4. Store it in configuration
//builder.Configuration["ConnectionStrings:DefaultConnection"] = secret.Value;

//// ?? 5. Use it for your DbContext
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"])
//);



//Keyvault 

var tenantId = builder.Configuration["Azure:TenantId"];
var clientId = builder.Configuration["Azure:ClientId"];
var clientSecret = builder.Configuration["Azure:ClientSecret"];
var keyVaultUrl = builder.Configuration["Keyvault:KeyVaultUrl"];

var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

var client = new SecretClient(new Uri(keyVaultUrl), credential);
var secret = client.GetSecret("dbconn");

Console.WriteLine($"Database Connection: {secret.Value.Value}");


builder.Services.AddDbContext<AppDbContext>(opt =>
opt.UseSqlServer(secret.Value.Value));




// Add DbContext
//builder.Services.AddDbContext<AppDbContext>(opt =>
//opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConn")));

//Add Dependancy Injection
// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IVisitRequestRepository, VisitRequestRepository>();
// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IVisitRequestService, VisitRequestService>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddHttpClient<IAiService, GroqAiService>();
builder.Services.AddScoped<IAiService, GroqAiService>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IChatService, ChatService>();
//builder.Services.AddScoped<IPropertyService, PropertyService>();
//builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
//builder.Services.AddScoped<IPropertyService, PropertyService>();

builder.Services.AddScoped<IStorage, StorageHelper>();




builder.Services.AddControllers();

// ---------------- CORS ----------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")  // Angular origin
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

//"http://localhost:4200",

//https://contactapp.fwh.is

// JWT Auth
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PropertySellingApp.Api", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer prefix",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme {
            Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        Array.Empty<string>()
    }});
});

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["ApplicationInsights:ConnectionString"]);


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
