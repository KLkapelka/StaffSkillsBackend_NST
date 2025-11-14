using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StaffSkillsBackend.Data;
using StaffSkillsBackend.DTOs;
using StaffSkillsBackend.Models;
using StaffSkillsBackend.Services;
using Xunit;

namespace StaffSkillsBackend.Tests.Services;

/// <summary>
/// Unit тесты для PersonService
/// тестируют бизнес-логику изолированно
/// </summary>
public class PersonServiceTests
{
    // вспомогательный метод создание тестовой БД 
    private AppDbContext GetTestDatabase()
    {
            // создание БД в памяти
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // уникальное имя
                .Options;
                return new AppDbContext(options);
    }

    // тест GetAllAsync должен вернуть всех сотрудников 
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPersons()
    {
        // подготовка
        var context = GetTestDatabase();
        var service = new PersonService(context);

        // добавление тестовых сотрудников
        context.Persons.Add(new Person { Name = "Lolo Filchencov", DisplayName = "Lolo", Skills = new List<Skill>() });
        context.Persons.Add(new Person { Name = "FGK Filchencov", DisplayName = "FGK", Skills = new List<Skill>() });
        await context.SaveChangesAsync();

        // действие
        var result = await service.GetAllAsync();

        // проверка
        result.Should().HaveCount(2); // 2 сотрудника
        result.Should().Contain(p => p.Name == "Lolo Filchencov"); // Lolo
        result.Should().Contain(p => p.Name == "FGK Filchencov"); // FGK
    }

    // тест GetByIdAsync с существующим ID должен вернуть сотрудника
    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnPerson()
    {
        // подготовка
        var context = GetTestDatabase();
        var service = new PersonService(context);

        var person = new Person { Name = "Lolo Filchencov", DisplayName = "Lolo", Skills = new List<Skill>() };
        context.Persons.Add(person);
        await context.SaveChangesAsync();

        // действие
        var result = await service.GetByIdAsync(person.Id);

        // проверка
        result.Should().NotBeNull(); // не должен быть null
        result!.Name.Should().Be("Lolo Filchencov"); // имя должно совпадать
    }

    // тест GetByIdAsync с несуществующим ID должен вернуть null
    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // подготовка
        var context = GetTestDatabase();
        var service = new PersonService(context);

        // действие
        var result = await service.GetByIdAsync(999); // несуществующий ID

        // проверка
        result.Should().BeNull(); // возвращение null
    }

    // тест CreateAsync должен создать сотрудника с навыками
    [Fact]
    public async Task CreateAsync_ShouldCreatePersonWithSkills()
    {
        // подготовка
        var context = GetTestDatabase();
        var service = new PersonService(context);

        var personDto = new PersonRequestDto
        {
            Name = "Lolo Filchencov",
            DisplayName = "Lolo",
            Skills = new List<SkillDto>
            {
                new() { Name = "C#", Level = 8 },
                new() { Name = "SQL", Level = 7 }
            }
        };

        // действие
        var result = await service.CreateAsync(personDto);

        // проверка
        result.Should().NotBeNull();
        result.Name.Should().Be("Lolo Filchencov");
        result.Skills.Should().HaveCount(2); // 2 навыка
        result.Skills.Should().Contain(s => s.Name == "C#" && s.Level == 8);
        result.Skills.Should().Contain(s => s.Name == "SQL" && s.Level == 7);
    }

    // тест UpdateAsync с существующим ID должен обновить сотрудника
    [Fact]
    public async Task UpdateAsync_WithValidId_ShouldUpdatePerson()
    {
        // подготовка
        var context = GetTestDatabase();
        var service = new PersonService(context);

        var person = new Person
        {
            Name = "Lolo Filchencov",
            DisplayName = "Lolo",
            Skills = new List<Skill> { new() { Name = "C#", Level = 5 } }
        };
        context.Persons.Add(person);
        await context.SaveChangesAsync();

        var updateDto = new PersonRequestDto
        {
            Name = "Lolo Smith", // новое имя
            DisplayName = "Lolony",
            Skills = new List<SkillDto> { new() { Name = "Python", Level = 9 } }
        };

        // действие
        var result = await service.UpdateAsync(person.Id, updateDto);

        // проверка
        result.Should().NotBeNull();
        result!.Name.Should().Be("Lolo Smith"); // имя обновилось
        result.Skills.Should().HaveCount(1);
        result.Skills.Should().Contain(s => s.Name == "Python" && s.Level == 9);
    }

    // тест DeleteAsync с существующим ID должен удалить
    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteAndReturnTrue()
    {
        // подготовка
        var context = GetTestDatabase();
        var service = new PersonService(context);

        var person = new Person { Name = "Lolo Filchencov", DisplayName = "Lolo", Skills = new List<Skill>() };
        context.Persons.Add(person);
        await context.SaveChangesAsync();

        // действие
        var result = await service.DeleteAsync(person.Id);

        // проверка
        result.Should().BeTrue(); // вернуть true
        
        var personInDb = await context.Persons.FindAsync(person.Id);
        personInDb.Should().BeNull(); // сотрудник удалён из БД
    }

    // тест DeleteAsync с несуществующим ID должен вернуть false
    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // подготовка
        var context = GetTestDatabase();
        var service = new PersonService(context);

        // действие
        var result = await service.DeleteAsync(999);

        // проверка
        result.Should().BeFalse();
    }
}