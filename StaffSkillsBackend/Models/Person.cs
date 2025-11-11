namespace StaffSkillsBackend.Models;

public class Person
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty; // отображаемое имя
    public List<Skill> Skills { get; set; } = new(); // список навыков
}