var builder = WebApplication.CreateBuilder(args);

// Регистрация Blockchain как Singleton
builder.Services.AddSingleton<Blockchain>();

// Добавление контроллеров
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();
