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

    // ПОЛУЧЕНИЕ ВСЕХ СОТРУДНИКОВ
    public async Task<IEnumerable<PersonResponseDto>> GetAllAsync()
    {
        // получение всех сотрудников из БД + навыками
        var persons = await _context.Persons
            .Include(p => p.Skills) // загрузка навыков
            .ToListAsync();

        // преобразование сотрудников в DTO
        var result = new List<PersonResponseDto>();
        foreach (var person in persons)
        {
            var dto = new PersonResponseDto
            {
                Id = person.Id,
                Name = person.Name,
                DisplayName = person.DisplayName,
                Skills = new List<SkillDto>()
            };

            // добавление навыков
            foreach (var skill in person.Skills)
            {
                dto.Skills.Add(new SkillDto
                {
                    Name = skill.Name,
                    Level = skill.Level
                });
            }

            result.Add(dto);
        }

        return result;
    }

    // ПОЛУЧЕНИЕ ОДНОГО СОТРУДНИКА ПО ID
    public async Task<PersonResponseDto?> GetByIdAsync(long id)
    {
        // поиск сотрудника в БД
        var person = await _context.Persons
            .Include(p => p.Skills)
            .FirstOrDefaultAsync(p => p.Id == id);

        // не найден - null
        if (person == null)
            return null;

        // преобразование в DTO
        var dto = new PersonResponseDto
        {
            Id = person.Id,
            Name = person.Name,
            DisplayName = person.DisplayName,
            Skills = new List<SkillDto>()
        };

        // добавление навыков
        foreach (var skill in person.Skills)
        {
            dto.Skills.Add(new SkillDto
            {
                Name = skill.Name,
                Level = skill.Level
            });
        }

        return dto;
    }

    // СОЗДАНИЕ НОВОГО СОТРУДНИКА
    public async Task<PersonResponseDto> CreateAsync(PersonRequestDto personDto)
    {
        // создание объекта Person
        var person = new Person
        {
            Name = personDto.Name,
            DisplayName = personDto.DisplayName,
            Skills = new List<Skill>()
        };

        // добавление навыков
        foreach (var skillDto in personDto.Skills)
        {
            person.Skills.Add(new Skill
            {
                Name = skillDto.Name,
                Level = skillDto.Level
            });
        }

        // сохранение в БД
        _context.Persons.Add(person);
        await _context.SaveChangesAsync();

        // возврат созданного сотрудника как DTO
        var result = new PersonResponseDto
        {
            Id = person.Id,
            Name = person.Name,
            DisplayName = person.DisplayName,
            Skills = new List<SkillDto>()
        };

        foreach (var skill in person.Skills)
        {
            result.Skills.Add(new SkillDto
            {
                Name = skill.Name,
                Level = skill.Level
            });
        }

        return result;
    }

    // ОБНОВЛЕНИЕ ДАННЫХ СОТРУДНИКА
    public async Task<PersonResponseDto?> UpdateAsync(long id, PersonRequestDto personDto)
    {
        // поиск существующего сотрудника
        var person = await _context.Persons
            .Include(p => p.Skills)
            .FirstOrDefaultAsync(p => p.Id == id);

        // не найден - null
        if (person == null)
            return null;

        // обновление данных
        person.Name = personDto.Name;
        person.DisplayName = personDto.DisplayName;

        // удаление старые навыков
        person.Skills.Clear();
        _context.Skills.RemoveRange(_context.Skills.Where(s => s.PersonId == id));

        // добавление новых навыков
        foreach (var skillDto in personDto.Skills)
        {
            person.Skills.Add(new Skill
            {
                Name = skillDto.Name,
                Level = skillDto.Level,
                PersonId = id
            });
        }

        // сохранение изменений
        await _context.SaveChangesAsync();

        // возврат обновлённого сотрудника
        var result = new PersonResponseDto
        {
            Id = person.Id,
            Name = person.Name,
            DisplayName = person.DisplayName,
            Skills = new List<SkillDto>()
        };

        foreach (var skill in person.Skills)
        {
            result.Skills.Add(new SkillDto
            {
                Name = skill.Name,
                Level = skill.Level
            });
        }

        return result;
    }

    // УДАЛЕНИЕ СОТРУДНИКА 
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
}