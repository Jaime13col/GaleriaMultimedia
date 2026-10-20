using GaleriaMultimedia.Services;
using GaleriaMultimedia.ViewModels;
using GaleriaMultimedia.Views;

namespace GaleriaMultimedia;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder.UseMauiApp<App>();

		// Servicios (una sola instancia)
		builder.Services.AddSingleton<DatabaseService>();
		builder.Services.AddSingleton<FotoService>();

		// ViewModels y páginas (una instancia nueva por navegación)
		builder.Services.AddTransient<GaleriaViewModel>();
		builder.Services.AddTransient<NuevaFotoViewModel>();
		builder.Services.AddTransient<DetalleViewModel>();

		builder.Services.AddTransient<GaleriaPage>();
		builder.Services.AddTransient<NuevaFotoPage>();
		builder.Services.AddTransient<DetallePage>();

		return builder.Build();
	}
}
