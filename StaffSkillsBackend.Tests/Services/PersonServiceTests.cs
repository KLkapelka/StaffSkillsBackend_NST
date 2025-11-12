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
/// Тестируют бизнес-логику изолированно
/// </summary>
public class PersonServiceTests
{
    /// <summary>
    /// Вспомогательный метод: создаёт in-memory БД для каждого теста
    /// Каждый тест получает чистую БД
    /// </summary>
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Уникальное имя для каждой БД
            .Options;

        return new AppDbContext(options);
    }

    /// <summary>
    /// Тест: GetAllAsync должен вернуть всех сотрудников
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPersons()
    {
        // Arrange (Подготовка)
        var context = GetInMemoryDbContext();
        var service = new PersonService(context);

        // Добавляем тестовые данные
        context.Persons.AddRange(
            new Person { Name = "John Doe", DisplayName = "John", Skills = new List<Skill>() },
            new Person { Name = "Jane Doe", DisplayName = "Jane", Skills = new List<Skill>() }
        );
        await context.SaveChangesAsync();

        // Act (Действие)
        var result = await service.GetAllAsync();

        // Assert (Проверка)
        result.Should().HaveCount(2); // Должно быть 2 сотрудника
        result.Should().Contain(p => p.Name == "John Doe"); // Должен быть John
        result.Should().Contain(p => p.Name == "Jane Doe"); // Должна быть Jane
    }

    /// <summary>
    /// Тест: GetByIdAsync с валидным ID должен вернуть сотрудника
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnPerson()
    {
        // Arrange
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

        // Act
        var result = await service.GetByIdAsync(person.Id);

        // Assert
        result.Should().NotBeNull(); // Не должен быть null
        result!.Name.Should().Be("John Doe"); // Имя должно совпадать
        result.DisplayName.Should().Be("John"); // DisplayName должно совпадать
    }

    /// <summary>
    /// Тест: GetByIdAsync с несуществующим ID должен вернуть null
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new PersonService(context);

        // Act
        var result = await service.GetByIdAsync(999); // Несуществующий ID

        // Assert
        result.Should().BeNull(); // Должен вернуть null
    }

    /// <summary>
    /// Тест: CreateAsync должен создать сотрудника с навыками
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldCreatePersonWithSkills()
    {
        // Arrange
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

        // Act
        var result = await service.CreateAsync(personDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("John Doe");
        result.Skills.Should().HaveCount(2); // Должно быть 2 навыка
        result.Skills.Should().Contain(s => s.Name == "C#" && s.Level == 8);
        result.Skills.Should().Contain(s => s.Name == "SQL" && s.Level == 7);

        // Проверяем, что сохранилось в БД
        var personInDb = await context.Persons.Include(p => p.Skills).FirstAsync();
        personInDb.Name.Should().Be("John Doe");
        personInDb.Skills.Should().HaveCount(2);
    }

    /// <summary>
    /// Тест: UpdateAsync с валидным ID должен обновить сотрудника
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WithValidId_ShouldUpdatePerson()
    {
        // Arrange
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
            Name = "John Smith", // Новое имя
            DisplayName = "Johnny", // Новый DisplayName
            Skills = new List<SkillDto>
            {
                new() { Name = "Python", Level = 9 } // Новый навык
            }
        };

        // Act
        var result = await service.UpdateAsync(person.Id, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("John Smith"); // Имя обновилось
        result.DisplayName.Should().Be("Johnny"); // DisplayName обновился
        result.Skills.Should().HaveCount(1);
        result.Skills.Should().Contain(s => s.Name == "Python" && s.Level == 9);
    }

    /// <summary>
    /// Тест: UpdateAsync с несуществующим ID должен вернуть null
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new PersonService(context);

        var updateDto = new PersonRequestDto
        {
            Name = "John Doe",
            DisplayName = "John",
            Skills = new List<SkillDto>()
        };

        // Act
        var result = await service.UpdateAsync(999, updateDto);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Тест: DeleteAsync с валидным ID должен удалить и вернуть true
    /// </summary>
    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteAndReturnTrue()
    {
        // Arrange
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

        // Act
        var result = await service.DeleteAsync(person.Id);

        // Assert
        result.Should().BeTrue(); // Должен вернуть true
        
        // Проверяем, что удалился из БД
        var personInDb = await context.Persons.FindAsync(person.Id);
        personInDb.Should().BeNull();
    }

    /// <summary>
    /// Тест: DeleteAsync с несуществующим ID должен вернуть false
    /// </summary>
    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new PersonService(context);

        // Act
        var result = await service.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }
}
