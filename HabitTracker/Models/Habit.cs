using SQLite;

namespace HabitTracker.Models;

public class Habit
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; } = "";
}

/// <summary>Отметка о выполнении привычки в конкретный день.</summary>
public class Check
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed(Name = "HabitDay", Order = 1, Unique = true)]
    public int HabitId { get; set; }

    /// <summary><see cref="DateOnly.DayNumber"/> — SQLite не умеет хранить DateOnly.</summary>
    [Indexed(Name = "HabitDay", Order = 2, Unique = true)]
    public int Day { get; set; }
}
