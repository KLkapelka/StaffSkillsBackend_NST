namespace StaffSkillsBackend.Models;

public class Skill
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public byte Level { get; set; } // от 1 до 10
    public long PersonId { get; set; } // внешний ключ, на Id сотрудника
    public Person Person { get; set; } = null!; // навигационное свойство - связь с сотрудником
}