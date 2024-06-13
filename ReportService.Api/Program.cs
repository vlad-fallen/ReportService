using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReportService.Api.Constants;
using ReportService.Api.Data;
using ReportService.Api.Extensions;
using ReportService.Api.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders()
    .AddSimpleConsole()
    .AddDebug();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ReportService")));

builder.Services.AddScoped<IMessageProducer, MessageProducer>();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

if (app.Environment.IsDevelopment())
    app.ApplyMigrations();

if (app.Configuration.GetValue<bool>("UseSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Configuration.GetValue<bool>("UseDeveloperExcpeptionPage"))
    app.UseDeveloperExceptionPage();    // обработчик ошибок для development environment
else
    app.UseExceptionHandler("/error");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/error",
    [EnableCors("AnyOrigin")]
    [ResponseCache(NoStore = true)]
    (HttpContext context) =>
    {
        var exceptionHandler = context.Features.Get<IExceptionHandlerPathFeature>();

        var details = new ProblemDetails();

        details.Detail = exceptionHandler?.Error.Message;

        details.Extensions["traceId"] = System.Diagnostics.Activity.Current?.Id
                                        ?? context.TraceIdentifier;
        details.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";

        details.Status = StatusCodes.Status500InternalServerError;

        app.Logger.LogError(
            CustomLogEvents.Error_Get,
            exceptionHandler?.Error,
            "An unhandled exception occured.");

        return Results.Problem(details);
    });

app.MapControllers();

app.Run();
