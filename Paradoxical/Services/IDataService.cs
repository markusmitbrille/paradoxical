using SQLite;

namespace Paradoxical.Services;

public interface IDataService
{
    SQLiteAsyncConnection Connection { get; }

    void Connect(string file);
    void Disconnect();
}
