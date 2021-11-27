using EComm.API.Auth;
using EComm.Core;
using EComm.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IRepository>(sp => RepositoryFactory.Create(builder.Configuration.GetConnectionString("ECommConnection")));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("MyCustomAuth")
  .AddScheme<AuthenticationSchemeOptions, MyCustomAuthHandler>
    ("MyCustomAuth", options => { });

builder.Services.AddAuthorization(options => {
    options.AddPolicy("AdminsOnly", policy =>
      policy.RequireClaim(ClaimTypes.Role, "Admin"));
});

builder.Services.AddCors(options => {
    options.AddPolicy(name: "AllowedOrigins",
        builder => {
            builder.WithOrigins("http://localhost:5100",
                          "https://localhost:7100");
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler("/servererror");

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowedOrigins");

app.MapControllers();

app.Run();
