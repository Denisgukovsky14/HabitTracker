using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HabitTracker.Models;
using HabitTracker.Services;

namespace HabitTracker.ViewModels;

public record DayCell(DateOnly Day, bool Done, bool IsToday, bool IsFuture)
{
    public string Number => Day.Day.ToString();
}

public partial class HabitViewModel(HabitDatabase db) : ObservableObject, IQueryAttributable
{
    const int Weeks = 12;

    Habit habit = new();
    HashSet<DateOnly> days = [];

    static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);

    public ObservableCollection<DayCell> Cells { get; } = [];

    [ObservableProperty] string name = "";
    [ObservableProperty] string current = "";
    [ObservableProperty] string best = "";
    [ObservableProperty] string total = "";
    [ObservableProperty] string rate = "";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        habit = (Habit)query["Habit"];
        Name = habit.Name;
    }

    [RelayCommand]
    async Task LoadAsync()
    {
        days = await db.GetDaysAsync(habit.Id);

        // Сетка начинается с понедельника, Weeks недель назад, и заканчивается воскресеньем текущей недели.
        var monday = Today.AddDays(-(((int)Today.DayOfWeek + 6) % 7));
        var start = monday.AddDays(-7 * (Weeks - 1));
        Cells.Clear();
        for (var d = start; d < monday.AddDays(7); d = d.AddDays(1))
            Cells.Add(new DayCell(d, days.Contains(d), d == Today, d > Today));

        UpdateStats();
    }

    [RelayCommand]
    async Task ToggleAsync(DayCell cell)
    {
        if (cell.IsFuture)
            return;
        await db.ToggleAsync(habit.Id, cell.Day);
        if (!days.Remove(cell.Day))
            days.Add(cell.Day);

        var i = Cells.IndexOf(cell);
        if (i >= 0)
            Cells[i] = cell with { Done = !cell.Done };
        UpdateStats();
    }

    [RelayCommand]
    async Task DeleteAsync()
    {
        var ok = await Shell.Current.DisplayAlert(
            "Удалить привычку?", $"«{habit.Name}» и вся её история будут удалены.", "Удалить", "Отмена");
        if (!ok)
            return;
        await db.DeleteHabitAsync(habit.Id);
        await Shell.Current.GoToAsync("..");
    }

    void UpdateStats()
    {
        var cur = Streaks.Current(days, Today);
        var best = Streaks.Best(days);
        var last30 = days.Count(d => d > Today.AddDays(-30) && d <= Today);

        Current = $"{cur} {Streaks.DaysWord(cur)}";
        Best = $"{best} {Streaks.DaysWord(best)}";
        Total = days.Count.ToString();
        Rate = $"{last30 * 100 / 30}%";
    }
}
