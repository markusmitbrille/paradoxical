﻿using Paradoxical.Model;
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
        Connection.CreateTable<EventImmediate>();
        Connection.CreateTable<EventAfter>();
        Connection.CreateTable<EventTrigger>();

        Connection.CreateTable<Portrait>();

        Connection.CreateTable<Option>();
        Connection.CreateTable<OptionTrigger>();
        Connection.CreateTable<OptionEffect>();

        Connection.CreateTable<Decision>();
        Connection.CreateTable<DecisionShownTrigger>();
        Connection.CreateTable<DecisionFailureTrigger>();
        Connection.CreateTable<DecisionValidTrigger>();
        Connection.CreateTable<DecisionAiPotentialTrigger>();

        Connection.CreateTable<Trigger>();

        Connection.CreateTable<Effect>();
    }

    public void DropTables()
    {
        Connection.DropTable<Mod>();

        Connection.DropTable<Script>();

        Connection.DropTable<Event>();
        Connection.DropTable<EventImmediate>();
        Connection.DropTable<EventAfter>();
        Connection.DropTable<EventTrigger>();

        Connection.DropTable<Portrait>();

        Connection.DropTable<Option>();
        Connection.DropTable<OptionTrigger>();
        Connection.DropTable<OptionEffect>();

        Connection.DropTable<Decision>();
        Connection.DropTable<DecisionShownTrigger>();
        Connection.DropTable<DecisionFailureTrigger>();
        Connection.DropTable<DecisionValidTrigger>();
        Connection.DropTable<DecisionAiPotentialTrigger>();

        Connection.DropTable<Trigger>();

        Connection.DropTable<Effect>();
    }

    public void TruncateTables()
    {
        Connection.DeleteAll<Mod>();

        Connection.DeleteAll<Script>();

        Connection.DeleteAll<Event>();
        Connection.DeleteAll<EventImmediate>();
        Connection.DeleteAll<EventAfter>();
        Connection.DeleteAll<EventTrigger>();

        Connection.DeleteAll<Portrait>();

        Connection.DeleteAll<Option>();
        Connection.DeleteAll<OptionTrigger>();
        Connection.DeleteAll<OptionEffect>();

        Connection.DeleteAll<Decision>();
        Connection.DeleteAll<DecisionShownTrigger>();
        Connection.DeleteAll<DecisionFailureTrigger>();
        Connection.DeleteAll<DecisionValidTrigger>();
        Connection.DeleteAll<DecisionAiPotentialTrigger>();

        Connection.DeleteAll<Trigger>();

        Connection.DeleteAll<Effect>();
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
