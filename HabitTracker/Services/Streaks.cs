namespace HabitTracker.Services;

public static class Streaks
{
    /// <summary>Текущая серия. Если сегодня ещё не отмечено, серия не прервана — считаем со вчера.</summary>
    public static int Current(ISet<DateOnly> days, DateOnly today)
    {
        var day = days.Contains(today) ? today : today.AddDays(-1);
        var count = 0;
        while (days.Contains(day))
        {
            count++;
            day = day.AddDays(-1);
        }
        return count;
    }

    public static int Best(IEnumerable<DateOnly> days)
    {
        int best = 0, run = 0;
        DateOnly? prev = null;
        foreach (var day in days.Order())
        {
            run = prev?.AddDays(1) == day ? run + 1 : 1;
            best = Math.Max(best, run);
            prev = day;
        }
        return best;
    }

    /// <summary>1 день, 2 дня, 5 дней, 11 дней, 21 день.</summary>
    public static string DaysWord(int n) => (n % 100, n % 10) switch
    {
        ( >= 11 and <= 14, _) => "дней",
        (_, 1) => "день",
        (_, >= 2 and <= 4) => "дня",
        _ => "дней",
    };
}
