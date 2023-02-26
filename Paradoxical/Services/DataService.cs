using SQLite;
using System;

namespace Paradoxical.Services;

public interface IDataService
{
    SQLiteAsyncConnection Connection { get; }

    void Connect(string file);
    void Disconnect();
}

public class DataService : IDataService
{
    private SQLiteAsyncConnection? connection;
    public SQLiteAsyncConnection Connection => connection ?? throw new InvalidOperationException("Not connected!");

    public void Connect(string path)
    {
        Disconnect();

        try
        {
            connection = new(path);
        }
        catch (Exception)
        {
            System.Windows.MessageBox.Show("Could not load mod database!",
                "Load Mod",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error,
                System.Windows.MessageBoxResult.Yes);

            return;
        }

        CreateTables();
    }

    public void Disconnect()
    {
        connection?.CloseAsync();
        connection = null;
    }

    private void CreateTables()
    {
        throw new NotImplementedException();
    }
}
