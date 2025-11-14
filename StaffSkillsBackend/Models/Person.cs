namespace StaffSkillsBackend.Models;

// описание сотрудника
public class Person
{
    public long Id { get; set; }
    public string Name { get; set; } = ""; // полное имя
    public string DisplayName { get; set; } = ""; // отображаемое имя
    public List<Skill> Skills { get; set; } = new(); // список навыков
}