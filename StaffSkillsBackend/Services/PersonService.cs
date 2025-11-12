using Microsoft.EntityFrameworkCore;
using StaffSkillsBackend.Data;
using StaffSkillsBackend.DTOs;
using StaffSkillsBackend.Models;

namespace StaffSkillsBackend.Services;

/// <summary>
/// Сервис для работы с сотрудниками
/// Содержит всю бизнес-логику
/// </summary>
public class PersonService : IPersonService
{
    private readonly AppDbContext _context;

    public PersonService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить всех сотрудников
    /// </summary>
    public async Task<IEnumerable<PersonResponseDto>> GetAllAsync()
    {
        // Получаем всех сотрудников вместе с навыками из БД
        var persons = await _context.Persons
            .Include(p => p.Skills) // Подгружаем связанные навыки
            .ToListAsync();

        // Преобразуем Entity в DTO
        return persons.Select(p => MapToDto(p));
    }

    /// <summary>
    /// Получить сотрудника по ID
    /// </summary>
    public async Task<PersonResponseDto?> GetByIdAsync(long id)
    {
        // Ищем сотрудника по ID
        var person = await _context.Persons
            .Include(p => p.Skills)
            .FirstOrDefaultAsync(p => p.Id == id);

        // Если не найден - возвращаем null
        if (person == null)
            return null;

        // Преобразуем в DTO
        return MapToDto(person);
    }

    /// <summary>
    /// Создать нового сотрудника
    /// </summary>
    public async Task<PersonResponseDto> CreateAsync(PersonRequestDto personDto)
    {
        // Создаём новую Entity из DTO
        var person = new Person
        {
            Name = personDto.Name,
            DisplayName = personDto.DisplayName,
            Skills = personDto.Skills.Select(s => new Skill
            {
                Name = s.Name,
                Level = s.Level
            }).ToList()
        };

        // Добавляем в БД
        _context.Persons.Add(person);
        await _context.SaveChangesAsync();

        // Возвращаем созданного сотрудника как DTO
        return MapToDto(person);
    }

    /// <summary>
    /// Обновить данные сотрудника
    /// </summary>
    public async Task<PersonResponseDto?> UpdateAsync(long id, PersonRequestDto personDto)
    {
        // Находим существующего сотрудника
        var person = await _context.Persons
            .Include(p => p.Skills)
            .FirstOrDefaultAsync(p => p.Id == id);

        // Если не найден - возвращаем null
        if (person == null)
            return null;

        // Обновляем основные поля
        person.Name = personDto.Name;
        person.DisplayName = personDto.DisplayName;

        // Удаляем старые навыки
        _context.Skills.RemoveRange(person.Skills);

        // Добавляем новые навыки
        person.Skills = personDto.Skills.Select(s => new Skill
        {
            Name = s.Name,
            Level = s.Level,
            PersonId = person.Id
        }).ToList();

        // Сохраняем изменения
        await _context.SaveChangesAsync();

        // Возвращаем обновлённого сотрудника
        return MapToDto(person);
    }

    /// <summary>
    /// Удалить сотрудника
    /// </summary>
    public async Task<bool> DeleteAsync(long id)
    {
        // Ищем сотрудника
        var person = await _context.Persons.FindAsync(id);

        // Если не найден - возвращаем false
        if (person == null)
            return false;

        // Удаляем (навыки удалятся автоматически из-за Cascade Delete)
        _context.Persons.Remove(person);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Вспомогательный метод для преобразования Entity в DTO
    /// </summary>
    private static PersonResponseDto MapToDto(Person person)
    {
        return new PersonResponseDto
        {
            Id = person.Id,
            Name = person.Name,
            DisplayName = person.DisplayName,
            Skills = person.Skills.Select(s => new SkillDto
            {
                Name = s.Name,
                Level = s.Level
            }).ToList()
        };
    }
}
