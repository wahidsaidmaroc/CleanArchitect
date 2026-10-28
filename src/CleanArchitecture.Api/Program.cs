using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Services;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = exception switch
        {
            CleanArchitecture.Domain.Exceptions.DomainException => StatusCodes.Status400BadRequest,
            CleanArchitecture.Application.Exceptions.NotFoundException => StatusCodes.Status404NotFound,
            DbUpdateException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        if (context.Response.StatusCode == StatusCodes.Status500InternalServerError)
        {
            app.Logger.LogError(exception, "An unexpected error occurred.");
        }

        await context.Response.WriteAsJsonAsync(new
        {
            title = context.Response.StatusCode switch
            {
                StatusCodes.Status400BadRequest => "Invalid request",
                StatusCodes.Status404NotFound => "Resource not found",
                StatusCodes.Status409Conflict => "Conflict",
                _ => "An unexpected error occurred"
            },
            detail = exception is CleanArchitecture.Domain.Exceptions.DomainException
                or CleanArchitecture.Application.Exceptions.NotFoundException
                    ? exception.Message
                    : "The request could not be completed.",
            status = context.Response.StatusCode
        });
    });
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.Run();

public partial class Program;
