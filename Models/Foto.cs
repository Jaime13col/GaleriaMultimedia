using System.Globalization;
using SQLite;

namespace GaleriaMultimedia.Models;

public class Foto
{
	static readonly CultureInfo Es = new("es-CO");

	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }

	public string Titulo { get; set; } = "";
	public string Descripcion { get; set; } = "";
	public string Categoria { get; set; } = "";
	public DateTime Fecha { get; set; } = DateTime.Now;
	public string RutaImagen { get; set; } = "";

	// Propiedades de solo presentación (no se guardan en SQLite)
	[Ignore] public string FechaCorta => Fecha.ToString("d MMM", Es).TrimEnd('.');
	[Ignore] public string FechaLarga => Fecha.ToString("d 'de' MMMM 'de' yyyy", Es);
	[Ignore] public string Subtitulo => $"{Categoria} · {FechaCorta}";
	[Ignore] public ImageSource? Imagen => string.IsNullOrEmpty(RutaImagen) ? null : ImageSource.FromFile(RutaImagen);
}
