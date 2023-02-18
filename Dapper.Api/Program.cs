using System.Reflection;
using Dapper.Api.ActionFilters;
using Dapper.Api.Extensions;
using Dapper.CQRS.Models.Users;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers(config =>
{
    config.Filters.Add(new ValidateModelStateAttribute());
});
builder.Services.AddMediatR(Assembly.GetAssembly(typeof(AddUserCommand)));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#region MyCustom Service Collections

builder.Services.SetOptionModels(configuration);
builder.Services.SetServiceLifeCycles(configuration);

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();