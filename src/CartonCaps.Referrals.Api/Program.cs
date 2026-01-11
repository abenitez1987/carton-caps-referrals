using CartonCaps.Referrals.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddDbContext<ReferralDbContext>(options => options.UseSqlite("Data Source=referrals.db"));
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

app.Run();
