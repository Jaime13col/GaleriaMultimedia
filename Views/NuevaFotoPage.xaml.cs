using GaleriaMultimedia.ViewModels;

namespace GaleriaMultimedia.Views;

public partial class NuevaFotoPage : ContentPage
{
	public NuevaFotoPage(NuevaFotoViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
