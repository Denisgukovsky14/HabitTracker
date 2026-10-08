using HabitTracker.Services;

namespace HabitTracker.Tests;

public class StreaksTests
{
    static readonly DateOnly Today = new(2026, 10, 1);

    static HashSet<DateOnly> Days(params int[] offsets) => offsets.Select(o => Today.AddDays(o)).ToHashSet();

    [Fact]
    public void Current_CountsBackFromToday() => Assert.Equal(3, Streaks.Current(Days(0, -1, -2, -4), Today));

    [Fact]
    public void Current_TodayNotYetChecked_StreakStillAlive() => Assert.Equal(2, Streaks.Current(Days(-1, -2), Today));

    [Fact]
    public void Current_MissedYesterday_IsZero() => Assert.Equal(0, Streaks.Current(Days(-2, -3), Today));

    [Fact]
    public void Best_FindsLongestRun() => Assert.Equal(3, Streaks.Best(Days(-10, -9, -8, -5, -4, 0)));

    [Fact]
    public void Best_Empty_IsZero() => Assert.Equal(0, Streaks.Best([]));

    [Theory]
    [InlineData(1, "день"), InlineData(2, "дня"), InlineData(5, "дней"), InlineData(11, "дней"),
     InlineData(14, "дней"), InlineData(21, "день"), InlineData(22, "дня"), InlineData(111, "дней")]
    public void DaysWord_RussianPlural(int n, string word) => Assert.Equal(word, Streaks.DaysWord(n));
}
