using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace GaleriaMultimedia.Services;

public class PermisoDenegadoException(string recurso) : Exception($"Permiso denegado: {recurso}")
{
	public string Recurso { get; } = recurso;
}

/// <summary>Cámara, galería y permisos. Devuelve la ruta de la imagen ya copiada a la carpeta de la app.</summary>
public class FotoService
{
	public async Task<string?> TomarFotoAsync()
	{
		if (!MediaPicker.Default.IsCaptureSupported)
			throw new FeatureNotSupportedException("Este dispositivo no tiene cámara disponible.");

		await AsegurarPermisoAsync<Permissions.Camera>("la cámara");

		var archivo = await MediaPicker.Default.CapturePhotoAsync();
		return archivo is null ? null : await CopiarAAppAsync(archivo);
	}

	public async Task<string?> ElegirDeGaleriaAsync()
	{
		await AsegurarPermisoAsync<Permissions.Photos>("tus fotos");

		var archivo = await MediaPicker.Default.PickPhotoAsync();
		return archivo is null ? null : await CopiarAAppAsync(archivo);
	}

	public void EliminarArchivo(string? ruta)
	{
		try
		{
			if (!string.IsNullOrEmpty(ruta) && File.Exists(ruta))
				File.Delete(ruta);
		}
		catch { /* si no se puede borrar, no es crítico */ }
	}

	static async Task AsegurarPermisoAsync<T>(string recurso) where T : Permissions.BasePermission, new()
	{
		var estado = await Permissions.CheckStatusAsync<T>();
		if (EsValido(estado)) return;

		estado = await Permissions.RequestAsync<T>();
		if (EsValido(estado)) return;

		throw new PermisoDenegadoException(recurso);
	}

	static bool EsValido(PermissionStatus estado)
		=> estado is PermissionStatus.Granted or PermissionStatus.Limited;

	static async Task<string> CopiarAAppAsync(FileResult archivo)
	{
		var extension = Path.GetExtension(archivo.FileName);
		if (string.IsNullOrEmpty(extension)) extension = ".jpg";

		var destino = Path.Combine(FileSystem.AppDataDirectory, $"{Guid.NewGuid():N}{extension}");

		await using var origen = await archivo.OpenReadAsync();
		await using var salida = File.Create(destino);
		await origen.CopyToAsync(salida);

		return destino;
	}
}
