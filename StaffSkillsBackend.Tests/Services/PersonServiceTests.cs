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
    /// <summary>
    /// вспомогательный метод: создаёт in-memory БД для каждого теста
    /// каждый тест получает чистую БД
    /// </summary>
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // уникальное имя для каждой БД
            .Options;

        return new AppDbContext(options);
    }

    /// <summary>
    /// тест GetAllAsync должен вернуть всех сотрудников
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPersons()
    {
        // подготовка
        var context = GetInMemoryDbContext();
        var service = new PersonService(context);

        // тестовые данные
        context.Persons.AddRange(
            new Person { Name = "John Doe", DisplayName = "John", Skills = new List<Skill>() },
            new Person { Name = "Jane Doe", DisplayName = "Jane", Skills = new List<Skill>() }
        );
        await context.SaveChangesAsync();

        // действие
        var result = await service.GetAllAsync();

        // проверка
        result.Should().HaveCount(2); // должно быть 2 сотрудника
        result.Should().Contain(p => p.Name == "John Doe"); // должен быть John
        result.Should().Contain(p => p.Name == "Jane Doe"); // должна быть Jane
    }

    /// <summary>
    /// тест GetByIdAsync с валидным ID должен вернуть сотрудника
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnPerson()
    {
        // подготовка
        var context = GetInMemoryDbContext();
        var service = new PersonService(context);

        var person = new Person 
        { 
            Name = "John Doe", 
            DisplayName = "John",
            Skills = new List<Skill>()
        };
        context.Persons.Add(person);
        await context.SaveChangesAsync();

        // действие
        var result = await service.GetByIdAsync(person.Id);

        // проверка
        result.Should().NotBeNull(); // Не должен быть null
        result!.Name.Should().Be("John Doe"); // Имя должно совпадать
        result.DisplayName.Should().Be("John"); // DisplayName должно совпадать
    }

    /// <summary>
    /// тест GetByIdAsync с несуществующим ID должен вернуть null
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // подготовка
        var context = GetInMemoryDbContext();
        var service = new PersonService(context);

        // действие
        var result = await service.GetByIdAsync(999); // несуществующий ID

        // проверка
        result.Should().BeNull(); // Должен вернуть null
    }

    /// <summary>
    /// тест CreateAsync должен создать сотрудника с навыками
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldCreatePersonWithSkills()
    {
        // подготовка
        var context = GetInMemoryDbContext();
        var service = new PersonService(context);

        var personDto = new PersonRequestDto
        {
            Name = "John Doe",
            DisplayName = "John",
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
        result.Name.Should().Be("John Doe");
        result.Skills.Should().HaveCount(2); // Должно быть 2 навыка
        result.Skills.Should().Contain(s => s.Name == "C#" && s.Level == 8);
        result.Skills.Should().Contain(s => s.Name == "SQL" && s.Level == 7);

        // дополнительная проверка
        var personInDb = await context.Persons.Include(p => p.Skills).FirstAsync();
        personInDb.Name.Should().Be("John Doe");
        personInDb.Skills.Should().HaveCount(2);
    }

    /// <summary>
    /// тест UpdateAsync с валидным ID должен обновить сотрудника
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WithValidId_ShouldUpdatePerson()
    {
        // подготовка
        var context = GetInMemoryDbContext();
        var service = new PersonService(context);

        var person = new Person
        {
            Name = "John Doe",
            DisplayName = "John",
            Skills = new List<Skill>
            {
                new() { Name = "C#", Level = 5 }
            }
        };
        context.Persons.Add(person);
        await context.SaveChangesAsync();

        var updateDto = new PersonRequestDto
        {
            Name = "John Smith", // новое имя
            DisplayName = "Johnny", // новый DisplayName
            Skills = new List<SkillDto>
            {
                new() { Name = "Python", Level = 9 } // новый навык
            }
        };

        // действие
        var result = await service.UpdateAsync(person.Id, updateDto);

        // проверка
        result.Should().NotBeNull();
        result!.Name.Should().Be("John Smith"); // имя обновилось
        result.DisplayName.Should().Be("Johnny"); // DisplayName обновился
        result.Skills.Should().HaveCount(1);
        result.Skills.Should().Contain(s => s.Name == "Python" && s.Level == 9);
    }

    /// <summary>
    /// тест UpdateAsync с несуществующим ID должен вернуть null
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldReturnNull()
    {
        // подготовка
        var context = GetInMemoryDbContext();
        var service = new PersonService(context);

        var updateDto = new PersonRequestDto
        {
            Name = "John Doe",
            DisplayName = "John",
            Skills = new List<SkillDto>()
        };

        // действие
        var result = await service.UpdateAsync(999, updateDto);

        // проверка
        result.Should().BeNull();
    }

    /// <summary>
    /// тест DeleteAsync с валидным ID должен удалить и вернуть true
    /// </summary>
    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteAndReturnTrue()
    {
        // подготовка
        var context = GetInMemoryDbContext();
        var service = new PersonService(context);

        var person = new Person 
        { 
            Name = "John Doe", 
            DisplayName = "John",
            Skills = new List<Skill>()
        };
        context.Persons.Add(person);
        await context.SaveChangesAsync();

        // действие
        var result = await service.DeleteAsync(person.Id);

        // проверка
        result.Should().BeTrue(); // должен вернуть true
        
        // дополнительная проверка
        var personInDb = await context.Persons.FindAsync(person.Id);
        personInDb.Should().BeNull();
    }

    /// <summary>
    /// тест DeleteAsync с несуществующим ID должен вернуть false
    /// </summary>
    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // подготовка
        var context = GetInMemoryDbContext();
        var service = new PersonService(context);

        // действие
        var result = await service.DeleteAsync(999);

        // проверка
        result.Should().BeFalse();
    }
}
