using Paradoxical.Model;
using SQLite;
using System;

namespace Paradoxical.Services;

public interface IDataService
{
    SQLiteConnection Connection { get; }

    void Connect(string file);
    void Disconnect();
}

public class DataService : IDataService
{
    public DataService()
    {
    }

    private SQLiteConnection? connection;
    public SQLiteConnection Connection => connection ?? throw new InvalidOperationException("Not connected!");

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
        Connection.Close();

        connection = null;
    }

    private void CreateTables()
    {
        Connection.CreateTable<Mod>();

        Connection.CreateTable<Event>();
        Connection.CreateTable<EventImmediateEffect>();
        Connection.CreateTable<EventAfterEffect>();
        Connection.CreateTable<EventTrigger>();

        Connection.CreateTable<Portrait>();

        Connection.CreateTable<Option>();
        Connection.CreateTable<OptionTrigger>();
        Connection.CreateTable<OptionEffect>();

        Connection.CreateTable<OnAction>();
        Connection.CreateTable<OnActionTrigger>();
        Connection.CreateTable<OnActionEffect>();
        Connection.CreateTable<OnActionEvent>();
        Connection.CreateTable<OnActionOnAction>();

        Connection.CreateTable<Decision>();
        Connection.CreateTable<DecisionIsShownTrigger>();
        Connection.CreateTable<DecisionIsValidTrigger>();
        Connection.CreateTable<DecisionIsValidFailureTrigger>();
        Connection.CreateTable<DecisionEffect>();

        Connection.CreateTable<Trigger>();

        Connection.CreateTable<Effect>();
    }
}
