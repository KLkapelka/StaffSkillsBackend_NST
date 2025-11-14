using StaffSkillsBackend.DTOs;

namespace StaffSkillsBackend.Services;

/// <summary>
/// интерфейс для работы с сотрудниками и их навыками
/// </summary>
public interface IPersonService
{
    // получение всех сотрудников
    Task<IEnumerable<PersonResponseDto>> GetAllAsync();
    
    // получение одного сотрудника по номеру
    Task<PersonResponseDto?> GetByIdAsync(long id);
    
    // создание нового сотрудника
    Task<PersonResponseDto> CreateAsync(PersonRequestDto personDto);

    // обновление данных сотрудника
    Task<PersonResponseDto?> UpdateAsync(long id, PersonRequestDto personDto);

    // удаление сотрудника
    Task<bool> DeleteAsync(long id);
}