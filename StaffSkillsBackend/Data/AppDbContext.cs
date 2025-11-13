using Microsoft.EntityFrameworkCore;
using StaffSkillsBackend.Models;

namespace StaffSkillsBackend.Data;

public class AppDbContext : DbContext  
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // таблицы БД
    public DbSet<Person> Persons { get; set; }
    public DbSet<Skill> Skills { get; set; }
    
    // настройка отношений таблиц
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // настройка связи один-ко-многим Person и Skill
        modelBuilder.Entity<Person>()
            .HasMany(p => p.Skills)         // 1:М - у Person много Skills
            .WithOne(s => s.Person)         // М:1 - у каждого Skill один Person 
            .HasForeignKey(s => s.PersonId) // внешний ключ PersonId 
            .OnDelete(DeleteBehavior.Cascade);   // при удалении Person удаляются его Skills

        // индекс для быстрого поиска навыков по сотруднику
        modelBuilder.Entity<Skill>()
            .HasIndex(s => s.PersonId);
    }
}