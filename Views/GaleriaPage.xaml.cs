using GaleriaMultimedia.ViewModels;

namespace GaleriaMultimedia.Views;

public partial class GaleriaPage : ContentPage
{
	readonly GaleriaViewModel _vm;

	public GaleriaPage(GaleriaViewModel vm)
	{
		InitializeComponent();
		BindingContext = _vm = vm;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _vm.CargarCommand.ExecuteAsync(null); // refresca al volver de crear/editar/eliminar
	}
}
