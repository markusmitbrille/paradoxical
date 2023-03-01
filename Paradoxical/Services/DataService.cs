using Paradoxical.Model;
using SQLite;
using System;
using System.Threading.Tasks;

namespace Paradoxical.Services;

public interface IDataService
{
    SQLiteAsyncConnection Connection { get; }

    Task Connect(string file);
    Task Disconnect();
}

public class DataService : IDataService
{
    public DataService()
    {
    }

    private SQLiteAsyncConnection? connection;
    public SQLiteAsyncConnection Connection => connection ?? throw new InvalidOperationException("Not connected!");

    public async Task Connect(string path)
    {
        await Disconnect();

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

        await CreateTables();
    }

    public async Task Disconnect()
    {
        await Connection.CloseAsync();

        connection = null;
    }

    private async Task CreateTables()
    {
        await Connection.CreateTableAsync<Mod>();

        await Connection.CreateTableAsync<Event>();
        await Connection.CreateTableAsync<EventImmediateEffect>();
        await Connection.CreateTableAsync<EventAfterEffect>();
        await Connection.CreateTableAsync<EventTrigger>();

        await Connection.CreateTableAsync<Portrait>();

        await Connection.CreateTableAsync<Option>();
        await Connection.CreateTableAsync<OptionTrigger>();
        await Connection.CreateTableAsync<OptionEffect>();

        await Connection.CreateTableAsync<OnAction>();
        await Connection.CreateTableAsync<OnActionTrigger>();
        await Connection.CreateTableAsync<OnActionEffect>();
        await Connection.CreateTableAsync<OnActionEvent>();
        await Connection.CreateTableAsync<OnActionOnAction>();

        await Connection.CreateTableAsync<Decision>();
        await Connection.CreateTableAsync<DecisionIsShownTrigger>();
        await Connection.CreateTableAsync<DecisionIsValidTrigger>();
        await Connection.CreateTableAsync<DecisionIsValidFailureTrigger>();
        await Connection.CreateTableAsync<DecisionEffect>();

        await Connection.CreateTableAsync<Trigger>();

        await Connection.CreateTableAsync<Effect>();
    }
}
