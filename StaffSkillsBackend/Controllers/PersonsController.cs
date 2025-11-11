using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffSkillsBackend.Data;
using StaffSkillsBackend.DTOs;
using StaffSkillsBackend.Models;

namespace StaffSkillsBackend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PersonsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<PersonsController> _logger;

    public PersonsController(AppDbContext context, ILogger<PersonsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // получение всех persons - GET: api/v1/persons
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonResponseDto>>> GetPersons()
    {
        try
        {
            var persons = await _context.Persons
                .Include(p => p.Skills) // загрузка skills вместе с person
                .ToListAsync();

            var response = persons.Select(p => new PersonResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                DisplayName = p.DisplayName,
                Skills = p.Skills.Select(s => new SkillDto
                {
                    Name = s.Name,
                    Level = s.Level
                }).ToList()
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ошибка при получении persons");
            return StatusCode(500, "внутренняя ошибка сервера");
        }
    }

    // получение person по id - GET: api/v1/persons/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<PersonResponseDto>> GetPerson(long id)
    {
        try
        {
            var person = await _context.Persons
                .Include(p => p.Skills)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null)
            {
                return NotFound($"person с id {id} не найден");
            }

            var response = new PersonResponseDto
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

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ошибка при получении person с id {Id}", id);
            return StatusCode(500, "внутренняя ошибка сервера");
        }
    }

    // создание нового person - POST: api/v1/persons
    [HttpPost]
    public async Task<ActionResult<PersonResponseDto>> CreatePerson(PersonRequestDto personDto)
    {
        try
        {
            // валидация уровня навыков
            if (personDto.Skills.Any(s => s.Level < 1 || s.Level > 10))
            {
                return BadRequest("уровень навыка должен быть от 1 до 10");
            }

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

            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            var response = new PersonResponseDto
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

            return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ошибка при создании person");
            return StatusCode(500, "внутренняя ошибка сервера");
        }
    }

    // обновление person - PUT: api/v1/persons/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePerson(long id, PersonRequestDto personDto)
    {
        try
        {
            // валидация уровня навыков
            if (personDto.Skills.Any(s => s.Level < 1 || s.Level > 10))
            {
                return BadRequest("уровень навыка должен быть от 1 до 10");
            }

            var person = await _context.Persons
                .Include(p => p.Skills)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null)
            {
                return NotFound($"person с id {id} не найден");
            }

            // обновление поля person
            person.Name = personDto.Name;
            person.DisplayName = personDto.DisplayName;

            // удаление старых skills
            _context.Skills.RemoveRange(person.Skills);

            // добавление новых skills
            person.Skills = personDto.Skills.Select(s => new Skill
            {
                Name = s.Name,
                Level = s.Level,
                PersonId = id
            }).ToList();

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ошибка при обновлении person с id {Id}", id);
            return StatusCode(500, "внутренняя ошибка сервера");
        }
    }

    // удаление person - DELETE: api/v1/persons/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePerson(long id)
    {
        try
        {
            var person = await _context.Persons.FindAsync(id);

            if (person == null)
            {
                return NotFound($"person с id {id} не найден");
            }

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ошибка при удалении person с id {Id}", id);
            return StatusCode(500, "внутренняя ошибка сервера");
        }
    }
}
