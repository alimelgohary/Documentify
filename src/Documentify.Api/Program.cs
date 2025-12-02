using Documentify.Infrastructure;
using Documentify.ApplicationCore;
using FluentValidation;
using MediatR;
using Documentify.ApplicationCore.Common.Behaviors;
using Documentify.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration, builder.Environment); // TODO: Pass logger
builder.Services.AddMediatR(
    cfg => cfg.RegisterServicesFromAssemblyContaining<IApplicationCoreMarker>());

builder.Services.AddValidatorsFromAssembly(typeof(IApplicationCoreMarker).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
