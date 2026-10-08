using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HabitTracker.Models;
using HabitTracker.Services;

namespace HabitTracker.ViewModels;

public record HabitRow(Habit Habit, bool DoneToday, int Streak)
{
    public string StreakText => Streak == 0
        ? "Начните сегодня"
        : $"🔥 {Streak} {Streaks.DaysWord(Streak)} подряд";
}

public partial class MainViewModel(HabitDatabase db) : ObservableObject
{
    static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);

    public ObservableCollection<HabitRow> Habits { get; } = [];

    public string DateText
    {
        get
        {
            var s = DateTime.Today.ToString("dddd, d MMMM");
            return char.ToUpper(s[0]) + s[1..];
        }
    }

    [ObservableProperty]
    string summary = "";

    static bool IsDark => Application.Current!.UserAppTheme == AppTheme.Dark;

    public string ThemeIcon => IsDark ? "☀️" : "🌙";

    [RelayCommand]
    void ToggleTheme()
    {
        var dark = !IsDark;
        Application.Current!.UserAppTheme = dark ? AppTheme.Dark : AppTheme.Light;
        Preferences.Set("dark", dark);
        OnPropertyChanged(nameof(ThemeIcon));
    }

    [RelayCommand]
    async Task LoadAsync()
    {
        var habits = await db.GetHabitsAsync();
        var days = await db.GetDaysByHabitAsync();
        Habits.Clear();
        foreach (var h in habits)
            Habits.Add(MakeRow(h, days.GetValueOrDefault(h.Id) ?? []));
        UpdateSummary();
        OnPropertyChanged(nameof(DateText));
    }

    [RelayCommand]
    async Task AddAsync()
    {
        var name = await Shell.Current.DisplayPromptAsync(
            "Новая привычка", "Например: зарядка, чтение, программирование",
            "Добавить", "Отмена", maxLength: 40);
        if (string.IsNullOrWhiteSpace(name))
            return;
        await db.AddHabitAsync(name.Trim());
        await LoadAsync();
    }

    [RelayCommand]
    async Task ToggleAsync(HabitRow row)
    {
        await db.ToggleAsync(row.Habit.Id, Today);
        var i = Habits.IndexOf(row);
        if (i >= 0)
            Habits[i] = MakeRow(row.Habit, await db.GetDaysAsync(row.Habit.Id));
        UpdateSummary();
    }

    [RelayCommand]
    Task OpenAsync(HabitRow row) =>
        Shell.Current.GoToAsync("habit", new Dictionary<string, object> { ["Habit"] = row.Habit });

    static HabitRow MakeRow(Habit h, HashSet<DateOnly> days) =>
        new(h, days.Contains(Today), Streaks.Current(days, Today));

    void UpdateSummary() => Summary = Habits.Count == 0
        ? ""
        : $"Сегодня выполнено {Habits.Count(x => x.DoneToday)} из {Habits.Count}";
}
