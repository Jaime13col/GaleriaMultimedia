using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GaleriaMultimedia.Models;
using GaleriaMultimedia.Services;

namespace GaleriaMultimedia.ViewModels;

public partial class DetalleViewModel : ObservableObject, IQueryAttributable
{
	readonly DatabaseService _db;
	readonly FotoService _fotos;
	int _id;

	public DetalleViewModel(DatabaseService db, FotoService fotos)
	{
		_db = db;
		_fotos = fotos;
	}

	[ObservableProperty] private Foto foto = new();

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.TryGetValue("id", out var valor) && int.TryParse(valor?.ToString(), out var id))
		{
			_id = id;
			_ = CargarAsync();
		}
	}

	[RelayCommand]
	async Task CargarAsync()
	{
		if (_id == 0) return;
		var f = await _db.GetFotoAsync(_id);
		if (f is not null) Foto = f;
	}

	[RelayCommand]
	Task VolverAsync() => Shell.Current.GoToAsync("..");

	[RelayCommand]
	Task EditarAsync() => Shell.Current.GoToAsync($"nuevafoto?id={_id}");

	[RelayCommand]
	async Task CompartirAsync()
	{
		try
		{
			await Share.Default.RequestAsync(new ShareFileRequest
			{
				Title = Foto.Titulo,
				File = new ShareFile(Foto.RutaImagen)
			});
		}
		catch (Exception ex)
		{
			await Shell.Current.DisplayAlertAsync("Error", $"No se pudo compartir: {ex.Message}", "OK");
		}
	}

	[RelayCommand]
	async Task EliminarAsync()
	{
		var confirmar = await Shell.Current.DisplayAlertAsync(
			"Eliminar foto",
			$"¿Seguro que quieres eliminar \"{Foto.Titulo}\"? Esta acción no se puede deshacer.",
			"Eliminar", "Cancelar");
		if (!confirmar) return;

		await _db.DeleteFotoAsync(Foto);
		_fotos.EliminarArchivo(Foto.RutaImagen);
		await Shell.Current.GoToAsync("..");
	}
}
