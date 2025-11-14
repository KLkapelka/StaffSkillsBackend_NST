using Microsoft.EntityFrameworkCore;
using StaffSkillsBackend.Models;

namespace StaffSkillsBackend.Data;

// класс для работы с базой данных
public class AppDbContext : DbContext  
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // таблицы БД
    public DbSet<Person> Persons { get; set; } // таблица сотрудников
    public DbSet<Skill> Skills { get; set; } // таблица навыков
    
    // настройка связей между таблицами
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // один сотрудник -> много навыков
        modelBuilder.Entity<Person>()
            .HasMany(p => p.Skills)         // 1:М - у Person много Skills
            .WithOne(s => s.Person)         // М:1 - у каждого Skill один Person
            .HasForeignKey(s => s.PersonId) // внешний ключ PersonId
            .OnDelete(DeleteBehavior.Cascade); // при удалении Person удаляются его Skills

        // ускорение поиска навыков по сотруднику
        modelBuilder.Entity<Skill>()
            .HasIndex(s => s.PersonId);
    }
}