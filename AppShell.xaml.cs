using GaleriaMultimedia.Views;

namespace GaleriaMultimedia;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("nuevafoto", typeof(NuevaFotoPage));
		Routing.RegisterRoute("detalle", typeof(DetallePage));
	}
}
