namespace StaffSkillsBackend.DTOs;

// ответ сервера - данные отправляемые клиенту 
public class PersonResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public List<SkillDto> Skills { get; set; } = new();
}