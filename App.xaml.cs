namespace GaleriaMultimedia;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		UserAppTheme = AppTheme.Light; // el diseño del mockup es claro
	}

	protected override Window CreateWindow(IActivationState? activationState)
		=> new Window(new AppShell());
}
