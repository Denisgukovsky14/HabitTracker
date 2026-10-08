using HabitTracker.ViewModels;

namespace HabitTracker.Pages;

public partial class MainPage : ContentPage
{
    readonly MainViewModel vm;

    public MainPage(MainViewModel vm)
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
