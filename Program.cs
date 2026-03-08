using TaskTrackerApi.Services;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Add services to the container
builder.Services.AddScoped<ITaskRepository, InMemoryTaskRepository>();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Add API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Task Tracker API",
        Version = "v1",
        Description = "A distributed task management microservice built with ASP.NET Core",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Task Tracker",
            Url = new Uri("https://github.com")
        }
    });

    var xmlFile = Path.Combine(AppContext.BaseDirectory, "TaskTrackerApi.xml");
    if (File.Exists(xmlFile))
    {
        c.IncludeXmlComments(xmlFile);
    }
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add logging
builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
    if (builder.Environment.IsDevelopment())
    {
        config.AddDebug();
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Tracker API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", async context =>
{
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsJsonAsync(new
    {
        status = "healthy",
        timestamp = DateTime.UtcNow,
        version = "1.0.0"
    });
});

app.Run();
