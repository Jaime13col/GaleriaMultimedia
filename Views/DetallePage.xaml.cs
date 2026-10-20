using GaleriaMultimedia.ViewModels;

namespace GaleriaMultimedia.Views;

public partial class DetallePage : ContentPage
{
	readonly DetalleViewModel _vm;

	public DetallePage(DetalleViewModel vm)
	{
		InitializeComponent();
		BindingContext = _vm = vm;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _vm.CargarCommand.ExecuteAsync(null); // refresca al volver de editar
	}
}
