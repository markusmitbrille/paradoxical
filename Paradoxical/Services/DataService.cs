using Paradoxical.Model;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;
using SQLite;
using System;

namespace Paradoxical.Services;

public interface IDataService
{
    SQLiteConnection Connection { get; }
    bool IsInMemory { get; }

    void Connect(string file, bool backup);

    void CreateTables();
    void DropTables();
    void TruncateTables();

    bool IsInTransaction { get; }
    void BeginTransaction();
    void CommitTransaction();
    void RollbackTransaction();
}

public class DataService : IDataService
{
    private SQLiteConnection connection = new(":memory:");
    public SQLiteConnection Connection
    {
        get => connection;
        private set => connection = value;
    }

    public bool IsInMemory => Connection.DatabasePath == ":memory:";

    public DataService()
    {
        CreateTables();
    }

    public void Connect(string file, bool backup)
    {
        if (backup == true)
        {
            Connection.Backup(file);
        }

        SQLiteConnection connection;

        try
        {
            // open new connection
            connection = new(file);
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

        // close old connection
        Connection.Close();

        // set new connection
        Connection = connection;
    }

    public void CreateTables()
    {
        Connection.CreateTable<Mod>();

        Connection.CreateTable<Script>();

        Connection.CreateTable<Event>();

        Connection.CreateTable<Portrait>();

        Connection.CreateTable<Option>();
        Connection.CreateTable<OptionLink>();

        Connection.CreateTable<Onion>();

        Connection.CreateTable<Decision>();
        Connection.CreateTable<DecisionLink>();

        Connection.CreateTable<Link>();
    }

    public void DropTables()
    {
        Connection.DropTable<Mod>();

        Connection.DropTable<Script>();

        Connection.DropTable<Event>();

        Connection.DropTable<Portrait>();

        Connection.DropTable<Option>();
        Connection.DropTable<OptionLink>();

        Connection.DropTable<Onion>();

        Connection.DropTable<Decision>();
        Connection.DropTable<DecisionLink>();

        Connection.DropTable<Link>();
    }

    public void TruncateTables()
    {
        Connection.DeleteAll<Mod>();

        Connection.DeleteAll<Script>();

        Connection.DeleteAll<Event>();

        Connection.DeleteAll<Portrait>();

        Connection.DeleteAll<Option>();
        Connection.DeleteAll<OptionLink>();

        Connection.DeleteAll<Onion>();

        Connection.DeleteAll<Decision>();
        Connection.DeleteAll<DecisionLink>();

        Connection.DeleteAll<Link>();
    }

    public bool IsInTransaction => Connection.IsInTransaction;

    public void BeginTransaction()
    {
        Connection.BeginTransaction();
    }

    public void CommitTransaction()
    {
        Connection.Commit();
    }

    public void RollbackTransaction()
    {
        Connection.Rollback();
    }
}
