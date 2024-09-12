using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using WeatherForecastApp.Data;
using WeatherForecastApp.Services;
using WeatherForecastApp.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var confBuilder = new ConfigurationBuilder()
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddConfiguration(builder.Configuration);

try
{
    var relativePath = @"../WeatherForecastApp";
    var absolutePath = Path.GetFullPath(relativePath);

    var fileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(absolutePath);

    confBuilder.AddJsonFile(fileProvider, "appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile(fileProvider, $"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();
}
catch
{
    confBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables();
}

builder.Services.Configure<AppConfig>(builder.Configuration);

// Adding services to the container.
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<IWeatherService, CachedWeatherService>();


builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

builder.Services.AddControllers();


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();
builder.Services.AddHsts(options =>
{
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddAuthorization();


builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherForecastApp V1.0.0.0");
    c.DocumentTitle = "Title Documentation";
    c.DocExpansion(DocExpansion.None);
});


app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});
app.UseCors(option =>
{
    option.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
    ;
});

app.Use(async (ctx, next) =>
{
    ctx.Response.Headers.Add("Content-Security-Policy", "default-src 'self';");
    await next();
});

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-FRAME-OPTIONS", "SAME-ORIGIN");
    await next();
});

if (app.Environment.IsDevelopment() == false)
{
    app.UseHsts();
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
