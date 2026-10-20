using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GaleriaMultimedia.Models;
using GaleriaMultimedia.Services;

namespace GaleriaMultimedia.ViewModels;

public partial class GaleriaViewModel : ObservableObject
{
	readonly DatabaseService _db;
	List<Foto> _todas = [];

	public GaleriaViewModel(DatabaseService db) => _db = db;

	public IReadOnlyList<string> Categorias => Constantes.Filtros;

	[ObservableProperty] private ObservableCollection<Foto> fotos = [];
	[ObservableProperty] private string textoBusqueda = "";
	[ObservableProperty] private string categoriaSeleccionada = "Todas";

	partial void OnTextoBusquedaChanged(string value) => Filtrar();
	partial void OnCategoriaSeleccionadaChanged(string value) => Filtrar();

	[RelayCommand]
	async Task CargarAsync()
	{
		_todas = await _db.GetFotosAsync();
		Filtrar();
	}

	void Filtrar()
	{
		IEnumerable<Foto> q = _todas;

		if (!string.IsNullOrEmpty(CategoriaSeleccionada) && CategoriaSeleccionada != "Todas")
			q = q.Where(f => f.Categoria == CategoriaSeleccionada);

		if (!string.IsNullOrWhiteSpace(TextoBusqueda))
			q = q.Where(f => f.Titulo.Contains(TextoBusqueda.Trim(), StringComparison.CurrentCultureIgnoreCase));

		Fotos = new ObservableCollection<Foto>(q);
	}

	[RelayCommand]
	Task NuevaFotoAsync() => Shell.Current.GoToAsync("nuevafoto");

	[RelayCommand]
	Task AbrirDetalleAsync(Foto? foto)
		=> foto is null ? Task.CompletedTask : Shell.Current.GoToAsync($"detalle?id={foto.Id}");
}
