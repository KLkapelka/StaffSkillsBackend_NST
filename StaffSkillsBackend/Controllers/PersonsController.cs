using Microsoft.AspNetCore.Mvc;
using StaffSkillsBackend.DTOs;
using StaffSkillsBackend.Services;

namespace StaffSkillsBackend.Controllers;

/// <summary>
/// контроллер для работы с сотрудниками
/// использование PersonService для бизнес-логики вместо прямой работы с БД
/// </summary>
[ApiController] // автоматическая валидация
[Route("api/v1/[controller]")] // адрес: /api/v1/persons
public class PersonsController : ControllerBase
{
    private readonly IPersonService _personService;
    /// <summary>
    /// инициализация нового экземпляра контроллера PersonsController
    /// </summary>
    public PersonsController(IPersonService personService)
    {
        _personService = personService;
    }

    // получение всех сотрудников
    // GET: /api/v1/persons
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonResponseDto>>> GetAll()
    {
        var persons = await _personService.GetAllAsync();
        return Ok(persons); // возврат - 200
    }

    // получение одного сотрудника по
    // GET: /api/v1/persons/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PersonResponseDto>> GetById(long id)
    {
        var person = await _personService.GetByIdAsync(id);
        
        // не найден - 404
        if (person == null)
            return NotFound(new { message = $"Person with id {id} not found" });
        
        return Ok(person); // возврат - 200
    }

    // создание нового сотрудника
    // POST: /api/v1/persons
    [HttpPost]
    public async Task<ActionResult<PersonResponseDto>> Create([FromBody] PersonRequestDto personDto)
    {
        var person = await _personService.CreateAsync(personDto);
        
        // возврат 201 Created + адрес созданного сотрудника
        return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
    }

    // обновление данных сотрудника
    // PUT: /api/v1/persons/5
    [HttpPut("{id}")]
    public async Task<ActionResult<PersonResponseDto>> Update(long id, [FromBody] PersonRequestDto personDto)
    {
        var person = await _personService.UpdateAsync(id, personDto);
        
        // не найден - 404
        if (person == null)
            return NotFound(new { message = $"Person with id {id} not found" });
        
        return Ok(person); // возврат - 200 
    }

    // удаление сотрудника
    // DELETE: /api/v1/persons/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        var deleted = await _personService.DeleteAsync(id);
        
        // не найден - 404
        if (!deleted)
            return NotFound(new { message = $"Person with id {id} not found" });
        
        return NoContent(); // возврат - 204
    }
}