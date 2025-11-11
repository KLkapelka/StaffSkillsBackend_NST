namespace StaffSkillsBackend.DTOs;

public class PersonRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<SkillDto> Skills { get; set; } = new();
}