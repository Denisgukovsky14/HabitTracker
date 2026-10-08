using HabitTracker.ViewModels;

namespace HabitTracker.Pages;

public partial class HabitPage : ContentPage
{
    readonly HabitViewModel vm;

    public HabitPage(HabitViewModel vm)
    {
        InitializeComponent();
        BindingContext = this.vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        vm.LoadCommand.Execute(null);
    }
}
