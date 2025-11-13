using Microsoft.EntityFrameworkCore;
using StaffSkillsBackend.Data;
using StaffSkillsBackend.DTOs;
using StaffSkillsBackend.Models;

namespace StaffSkillsBackend.Services;

/// <summary>
/// сервис для работы с сотрудниками
/// содержит всю бизнес-логику
/// </summary>
public class PersonService : IPersonService
{
    private readonly AppDbContext _context;

    public PersonService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// получение всех сотрудников
    /// </summary>
    public async Task<IEnumerable<PersonResponseDto>> GetAllAsync()
    {
        // получение всех сотрудников и навыков из БД
        var persons = await _context.Persons
            .Include(p => p.Skills) // подгрузка связных навыков
            .ToListAsync();

        // преобразование Entity в DTO
        return persons.Select(p => MapToDto(p));
    }

    /// <summary>
    /// получение сотрудника по ID
    /// </summary>
    public async Task<PersonResponseDto?> GetByIdAsync(long id)
    {
        // поиск сотрудника по ID
        var person = await _context.Persons
            .Include(p => p.Skills)
            .FirstOrDefaultAsync(p => p.Id == id);

        // не найден - возврат null
        if (person == null)
            return null;

        // получение в DTO
        return MapToDto(person);
    }

    /// <summary>
    /// создать нового сотрудника
    /// </summary>
    public async Task<PersonResponseDto> CreateAsync(PersonRequestDto personDto)
    {
        // создать новую Entity из DTO
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

        // добавление в БД
        _context.Persons.Add(person);
        await _context.SaveChangesAsync();

        // возврат созданного сотрудника как DTO
        return MapToDto(person);
    }

    /// <summary>
    /// обновить данные сотрудника
    /// </summary>
    public async Task<PersonResponseDto?> UpdateAsync(long id, PersonRequestDto personDto)
    {
        // поиск существующего сотрудника
        var person = await _context.Persons
            .Include(p => p.Skills)
            .FirstOrDefaultAsync(p => p.Id == id);

        // не найден - возврат null
        if (person == null)
            return null;

        // обновление основных полей
        person.Name = personDto.Name;
        person.DisplayName = personDto.DisplayName;

        // удаление старых навыков
        _context.Skills.RemoveRange(person.Skills);

        // добавление новых навыков
        person.Skills = personDto.Skills.Select(s => new Skill
        {
            Name = s.Name,
            Level = s.Level,
            PersonId = person.Id
        }).ToList();

        // сохранение изменений
        await _context.SaveChangesAsync();

        // возврат обновлённого сотрудника
        return MapToDto(person);
    }

    /// <summary>
    /// удаление сотрудника
    /// </summary>
    public async Task<bool> DeleteAsync(long id)
    {
        // поиск сотрудника
        var person = await _context.Persons.FindAsync(id);

        // не найден - false
        if (person == null)
            return false;

        // удаление. Навыки удалятся автоматически из-за Cascade Delete
        _context.Persons.Remove(person);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// вспомогательный метод для преобразования Entity в DTO
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
