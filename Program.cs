var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Добавляем Swagger
builder.Services.AddSwaggerGen(option => {
    // Версия v1
    option.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "API для работы с меню и заказами",
        Description = "API для регистрации пользователей, просмотра меню, блюд и создания заказов"
    });

    // Добавляем XML комментарии (с проверкой существования файла)
    var xmlFile = "pr_49.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        option.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Настраиваем Swagger UI
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Food API v1");
    c.RoutePrefix = "swagger"; // Swagger будет доступен по /swagger
});

app.Run();