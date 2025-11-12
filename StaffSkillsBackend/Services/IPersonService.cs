using StaffSkillsBackend.DTOs;

namespace StaffSkillsBackend.Services;

/// <summary>
/// Интерфейс для работы с сотрудниками и их навыками
/// Определяет контракт для бизнес-логики
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Получить всех сотрудников с их навыками
    /// </summary>
    /// <returns>Список сотрудников</returns>
    Task<IEnumerable<PersonResponseDto>> GetAllAsync();

    /// <summary>
    /// Получить сотрудника по ID
    /// </summary>
    /// <param name="id">ID сотрудника</param>
    /// <returns>Сотрудник или null, если не найден</returns>
    Task<PersonResponseDto?> GetByIdAsync(long id);

    /// <summary>
    /// Создать нового сотрудника
    /// </summary>
    /// <param name="personDto">Данные сотрудника</param>
    /// <returns>Созданный сотрудник</returns>
    Task<PersonResponseDto> CreateAsync(PersonRequestDto personDto);

    /// <summary>
    /// Обновить данные сотрудника
    /// </summary>
    /// <param name="id">ID сотрудника</param>
    /// <param name="personDto">Новые данные</param>
    /// <returns>Обновленный сотрудник или null, если не найден</returns>
    Task<PersonResponseDto?> UpdateAsync(long id, PersonRequestDto personDto);

    /// <summary>
    /// Удалить сотрудника
    /// </summary>
    /// <param name="id">ID сотрудника</param>
    /// <returns>true если удален, false если не найден</returns>
    Task<bool> DeleteAsync(long id);
}