using Microsoft.EntityFrameworkCore;
using StaffSkillsBackend.Data;

var builder = WebApplication.CreateBuilder(args);

// регистрация контекста БД
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// добавление контроллеров
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// настройка HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// подключение контроллеров
app.MapControllers();

app.Run();