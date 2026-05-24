
// program.cs file start up project
using FluentValidation;
using MediatR;
using PMS.Application.Common.Behaviors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerServices();
// Infrastructure DI (includes authentication)
builder.Services.AddInfrastructure(builder.Configuration);
// Register MediatR handlers from Application assembly
builder.Services.AddMediatR(
    typeof(AssemblyReference).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<AssemblyReference>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Here Authorization policies registration
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(ApiPolicies.AdminOnly, policy => policy.RequireRole(RoleNames.Admin));
    options.AddPolicy(ApiPolicies.EmployeeOnly, policy => policy.RequireRole(RoleNames.Employee));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
// Expose OpenAPI and Swagger UI at app root for convenience (available in all environments).
app.MapOpenApi();
app.UseSwaggerUI();

// Global exception handler
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.Services.SeedIdentityAsync(app.Configuration);

app.Run();
