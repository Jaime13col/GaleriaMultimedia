using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GaleriaMultimedia.Models;
using GaleriaMultimedia.Services;

namespace GaleriaMultimedia.ViewModels;

/// <summary>Sirve para crear una foto nueva o, si llega ?id=, para editar una existente.</summary>
public partial class NuevaFotoViewModel : ObservableObject, IQueryAttributable
{
	readonly DatabaseService _db;
	readonly FotoService _fotos;
	Foto? _original;

	public NuevaFotoViewModel(DatabaseService db, FotoService fotos)
	{
		_db = db;
		_fotos = fotos;
	}

	public IReadOnlyList<string> Categorias => Constantes.Categorias;

	[ObservableProperty] private string tituloPagina = "Nueva foto";
	[ObservableProperty] private string titulo = "";
	[ObservableProperty] private string descripcion = "";
	[ObservableProperty] private string categoria = Constantes.Categorias[0];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(Vista), nameof(TieneImagen), nameof(SinImagen))]
	private string rutaImagen = "";

	public bool TieneImagen => !string.IsNullOrEmpty(RutaImagen);
	public bool SinImagen => !TieneImagen;
	public ImageSource? Vista => TieneImagen ? ImageSource.FromFile(RutaImagen) : null;

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.TryGetValue("id", out var valor) && int.TryParse(valor?.ToString(), out var id))
			_ = CargarParaEditarAsync(id);
	}

	async Task CargarParaEditarAsync(int id)
	{
		var foto = await _db.GetFotoAsync(id);
		if (foto is null) return;

		_original = foto;
		TituloPagina = "Editar foto";
		Titulo = foto.Titulo;
		Descripcion = foto.Descripcion;
		Categoria = foto.Categoria;
		RutaImagen = foto.RutaImagen;
	}

	[RelayCommand]
	Task TomarFotoAsync() => ObtenerImagenAsync(_fotos.TomarFotoAsync);

	[RelayCommand]
	Task ElegirGaleriaAsync() => ObtenerImagenAsync(_fotos.ElegirDeGaleriaAsync);

	async Task ObtenerImagenAsync(Func<Task<string?>> obtener)
	{
		try
		{
			var ruta = await obtener();
			if (ruta is null) return; // el usuario canceló

			DescartarImagenTemporal();
			RutaImagen = ruta;
		}
		catch (PermisoDenegadoException ex)
		{
			var abrir = await Shell.Current.DisplayAlertAsync(
				"Permiso necesario",
				$"Para continuar necesitamos acceso a {ex.Recurso}. Puedes activarlo en los ajustes de la app.",
				"Abrir ajustes", "Ahora no");
			if (abrir) AppInfo.Current.ShowSettingsUI();
		}
		catch (FeatureNotSupportedException ex)
		{
			await Shell.Current.DisplayAlertAsync("No disponible", ex.Message, "OK");
		}
		catch (Exception ex)
		{
			await Shell.Current.DisplayAlertAsync("Error", $"No se pudo obtener la imagen: {ex.Message}", "OK");
		}
	}

	[RelayCommand]
	async Task GuardarAsync()
	{
		if (string.IsNullOrWhiteSpace(Titulo))
		{
			await Shell.Current.DisplayAlertAsync("Falta el título", "Escribe un título para la foto.", "OK");
			return;
		}
		if (!TieneImagen)
		{
			await Shell.Current.DisplayAlertAsync("Falta la imagen", "Toma una foto o elígela de la galería.", "OK");
			return;
		}

		var rutaAnterior = _original?.RutaImagen;
		var foto = _original ?? new Foto { Fecha = DateTime.Now };
		foto.Titulo = Titulo.Trim();
		foto.Descripcion = Descripcion?.Trim() ?? "";
		foto.Categoria = Categoria;
		foto.RutaImagen = RutaImagen;

		await _db.SaveFotoAsync(foto);

		// Si en la edición cambiaron la imagen, se borra el archivo viejo
		if (rutaAnterior is not null && rutaAnterior != RutaImagen)
			_fotos.EliminarArchivo(rutaAnterior);

		_original = null; // ya quedó guardado: no hay nada temporal que limpiar
		await Shell.Current.GoToAsync("..");
	}

	[RelayCommand]
	async Task VolverAsync()
	{
		DescartarImagenTemporal();
		await Shell.Current.GoToAsync("..");
	}

	/// <summary>Borra la imagen elegida que todavía no se ha guardado en la BD.</summary>
	void DescartarImagenTemporal()
	{
		if (TieneImagen && RutaImagen != _original?.RutaImagen)
			_fotos.EliminarArchivo(RutaImagen);
	}
}
