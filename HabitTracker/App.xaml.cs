using System.Globalization;

namespace HabitTracker;

public partial class App : Application
{
	public App()
	{
		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ru-RU");
		InitializeComponent();
		UserAppTheme = Preferences.Get("dark", false) ? AppTheme.Dark : AppTheme.Light;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell()) { Title = "Привычки" };
	}
}
