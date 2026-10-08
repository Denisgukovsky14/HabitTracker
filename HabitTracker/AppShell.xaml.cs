using HabitTracker.Pages;

namespace HabitTracker;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("habit", typeof(HabitPage));
	}
}
