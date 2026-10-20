using GaleriaMultimedia.Models;
using SQLite;

namespace GaleriaMultimedia.Services;

/// <summary>CRUD sobre SQLite (sqlite-net-pcl). La BD vive en AppDataDirectory, así sobrevive al cierre de la app.</summary>
public class DatabaseService
{
	SQLiteAsyncConnection? _db;

	async Task<SQLiteAsyncConnection> InitAsync()
	{
		if (_db is not null) return _db;

		var ruta = Path.Combine(FileSystem.AppDataDirectory, "galeria.db3");
		_db = new SQLiteAsyncConnection(ruta);
		await _db.CreateTableAsync<Foto>();
		return _db;
	}

	public async Task<List<Foto>> GetFotosAsync()
	{
		var db = await InitAsync();
		return await db.Table<Foto>().OrderByDescending(f => f.Fecha).ToListAsync();
	}

	public async Task<Foto?> GetFotoAsync(int id)
	{
		var db = await InitAsync();
		return await db.Table<Foto>().Where(f => f.Id == id).FirstOrDefaultAsync();
	}

	/// <summary>Inserta si Id == 0, actualiza si ya existe.</summary>
	public async Task<int> SaveFotoAsync(Foto foto)
	{
		var db = await InitAsync();
		return foto.Id != 0 ? await db.UpdateAsync(foto) : await db.InsertAsync(foto);
	}

	public async Task<int> DeleteFotoAsync(Foto foto)
	{
		var db = await InitAsync();
		return await db.DeleteAsync(foto);
	}
}
