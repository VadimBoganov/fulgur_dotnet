using Api;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        var secretKey = builder.Configuration["Auth:SecretKey"]
            ?? throw new InvalidOperationException("Auth:SecretKey is not configured.");

        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Auth:Issuer"],
            ValidAudience = builder.Configuration["Auth:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        // Accept the token from the HttpOnly auth cookie (falls back to the
        // Authorization header if present).
        opts.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (string.IsNullOrEmpty(context.Token)
                    && context.Request.Cookies.TryGetValue("access_token", out var cookieToken))
                {
                    context.Token = cookieToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddDbContext<AdminContext>();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(opts => 
{
    opts.EnableAnnotations();
    opts.SwaggerDoc("v1", new OpenApiInfo { Title = "fulgur api", Version = "v1" });
    opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    opts.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var allowSpecificOrigins = "_allowAdminOrigins";

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:3000", "http://127.0.0.1:5500"];

builder.Services.AddCors(opts =>
{
    opts.AddPolicy(allowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .WithMethods("GET", "POST", "PUT", "DELETE")
                  .WithHeaders("Content-Type")
                  .AllowCredentials();
        });
});

builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<IProductTypesService, ProductTypesService>();
builder.Services.AddScoped<IProductSubTypeService, ProductSubTypeService>();
builder.Services.AddScoped<IProductItemService, ProductItemService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IContactRequestsService, ContactRequestsService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddRouting(opts => opts.LowercaseUrls = true);

// The API runs behind the Caddy reverse proxy; honor the forwarded client IP so
// rate limiting partitions by the real caller rather than the proxy address.
builder.Services.Configure<ForwardedHeadersOptions>(opts =>
{
    opts.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    opts.KnownNetworks.Clear();
    opts.KnownProxies.Clear();
});

builder.Services.AddRateLimiter(opts =>
{
    opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    opts.AddPolicy("contact", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(5),
                QueueLimit = 0
            }));
});

var app = builder.Build();

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(handler => handler.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
    }));
}

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AdminContext>();
await db.Database.MigrateAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(allowSpecificOrigins);

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US"),
});

app.UseStatusCodePages();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
