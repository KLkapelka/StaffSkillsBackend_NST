using StaffSkillsBackend.DTOs;

namespace StaffSkillsBackend.Services;

/// <summary>
/// интерфейс для работы с сотрудниками и их навыками
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// получение всех сотрудников с их навыками
    /// </summary>
    /// <returns>список сотрудников</returns>
    Task<IEnumerable<PersonResponseDto>> GetAllAsync();

    /// <summary>
    /// получение сотрудника по ID
    /// </summary>
    /// <param name="id">ID сотрудника</param>
    /// <returns>сотрудник или null</returns>
    Task<PersonResponseDto?> GetByIdAsync(long id);

    /// <summary>
    /// создание нового сотрудника
    /// </summary>
    /// <param name="personDto">данные сотрудника</param>
    /// <returns>созданный сотрудник</returns>
    Task<PersonResponseDto> CreateAsync(PersonRequestDto personDto);

    /// <summary>
    /// обновить данные сотрудника
    /// </summary>
    /// <param name="id">ID сотрудника</param>
    /// <param name="personDto">новые данные</param>
    /// <returns>обновленный сотрудник или null</returns>
    Task<PersonResponseDto?> UpdateAsync(long id, PersonRequestDto personDto);

    /// <summary>
    /// удалить сотрудника
    /// </summary>
    /// <param name="id">ID сотрудника</param>
    /// <returns>true - удален, false - не найден</returns>
    Task<bool> DeleteAsync(long id);
}