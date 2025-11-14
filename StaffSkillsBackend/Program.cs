using Microsoft.EntityFrameworkCore;
using StaffSkillsBackend.Data;
using StaffSkillsBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// подключение БД
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// регистрация сервиса
builder.Services.AddScoped<IPersonService, PersonService>();

// добавление контроллеров
builder.Services.AddControllers();

// добавление Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// если режим разработки - включить Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// переадресация на HTTPS
app.UseHttpsRedirection();

// подключение контроллеров
app.MapControllers();

// запуск приложения
app.Run();