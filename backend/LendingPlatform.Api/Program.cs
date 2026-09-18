using FluentValidation;
using LendingPlatform.Application.DTOs;
using LendingPlatform.Application.Interfaces;
using LendingPlatform.Application.Services;
using LendingPlatform.Application.Validators;
using LendingPlatform.Domain.Services;
using LendingPlatform.Infrastructure.Persistence;
using LendingPlatform.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using LendingPlatform.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database
builder.Services.AddDbContext<LendingDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("LendingDatabase")));

// Application services
builder.Services.AddScoped<ILoanApplicationService, LoanApplicationService>();

// Repositories
builder.Services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();

// Domain services
builder.Services.AddScoped<LoanDecisionEngine>();

// Validators
builder.Services.AddScoped<
    IValidator<CreateLoanApplicationRequest>,
    CreateLoanApplicationValidator>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();


app.UseCors("Frontend");

app.MapControllers();

app.Run();
public partial class Program
{
}