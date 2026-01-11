using System.Text.Json;
using CartonCaps.Referrals.Api.Data;
using CartonCaps.Referrals.Api.Data.Repositories;
using CartonCaps.Referrals.Api.Services;
using CartonCaps.Referrals.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("ApiGateway", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "X-User-Id",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Description = "Fake API Gateway header for user identification"
        });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "ApiGateway"
                    }
                },
                Array.Empty<string>()
            }
        });
});

builder.Services.AddAuthentication("FakeScheme")
    .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>("FakeScheme", options => { });
builder.Services.AddAuthorization();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

builder.Services.AddDbContext<ReferralDbContext>(options => options.UseSqlite("Data Source=referrals.db"));

builder.Services.AddScoped<IReferralsService, ReferralsService>();
builder.Services.AddScoped<IReferralsRepository, ReferralsRepository>();
builder.Services.AddScoped<ITrackingGenerator, TrackingGenerator>();
builder.Services.AddScoped<IDeepLinkGenerator, MockDeepLinkGenerator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ReferralDbContext>();
    context.Database.EnsureCreated();

    if (!context.Users.Any() && !context.Referrals.Any())
    {
        DatabaseSeeder.Seed(context);
    }
}

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
