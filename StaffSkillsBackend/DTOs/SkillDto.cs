namespace StaffSkillsBackend.DTOs;

// упрощённое описание навыка без связей с БД
public class SkillDto
{
    public string Name { get; set; } = "";
    public byte Level { get; set; }
}