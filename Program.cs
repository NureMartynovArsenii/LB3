var builder = WebApplication.CreateBuilder(args);

// ����������� Blockchain ��� Singleton
builder.Services.AddSingleton<Blockchain>();

// ���������� ������������
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();
