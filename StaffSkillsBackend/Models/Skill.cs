namespace StaffSkillsBackend.Models;

// описнаие навыков сотрудника
public class Skill
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public byte Level { get; set; } // от 1 до 10
    public long PersonId { get; set; } // к какому сотруднику относится
    public Person Person { get; set; } = null!; // связь с сотрудником
}