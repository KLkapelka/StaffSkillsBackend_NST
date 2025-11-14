namespace StaffSkillsBackend.DTOs;

// данные от клиента при создании или обновлении сотрудника
public class PersonRequestDto
{
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public List<SkillDto> Skills { get; set; } = new();
}