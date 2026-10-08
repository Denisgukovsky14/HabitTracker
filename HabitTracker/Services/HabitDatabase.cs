using HabitTracker.Models;
using SQLite;

namespace HabitTracker.Services;

public class HabitDatabase
{
    readonly SQLiteAsyncConnection db = new(Path.Combine(FileSystem.AppDataDirectory, "habits.db3"));
    bool ready;

    async Task<SQLiteAsyncConnection> Db()
    {
        if (!ready)
        {
            await db.CreateTableAsync<Habit>();
            await db.CreateTableAsync<Check>();
            ready = true;
        }
        return db;
    }

    public async Task<List<Habit>> GetHabitsAsync() =>
        await (await Db()).Table<Habit>().ToListAsync();

    public async Task AddHabitAsync(string name) =>
        await (await Db()).InsertAsync(new Habit { Name = name });

    public async Task DeleteHabitAsync(int habitId)
    {
        var c = await Db();
        await c.Table<Check>().DeleteAsync(x => x.HabitId == habitId);
        await c.DeleteAsync<Habit>(habitId);
    }

    public async Task<Dictionary<int, HashSet<DateOnly>>> GetDaysByHabitAsync()
    {
        var checks = await (await Db()).Table<Check>().ToListAsync();
        return checks
            .GroupBy(x => x.HabitId)
            .ToDictionary(g => g.Key, g => g.Select(x => DateOnly.FromDayNumber(x.Day)).ToHashSet());
    }

    public async Task<HashSet<DateOnly>> GetDaysAsync(int habitId) =>
        (await GetDaysByHabitAsync()).GetValueOrDefault(habitId) ?? [];

    /// <summary>Ставит отметку на день или снимает её, если она уже есть.</summary>
    public async Task ToggleAsync(int habitId, DateOnly day)
    {
        var c = await Db();
        var n = day.DayNumber;
        var removed = await c.Table<Check>().DeleteAsync(x => x.HabitId == habitId && x.Day == n);
        if (removed == 0)
            await c.InsertAsync(new Check { HabitId = habitId, Day = n });
    }
}
