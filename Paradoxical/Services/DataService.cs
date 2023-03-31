﻿using Paradoxical.Model;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;
using SQLite;
using System;

namespace Paradoxical.Services;

public interface IDataService
{
    SQLiteConnection Connection { get; }

    void Connect(string file);

    void CreateTables();
    void DropTables();
    void TruncateTables();
}

public class DataService : IDataService
{
    private SQLiteConnection connection = new(":memory:");
    public SQLiteConnection Connection
    {
        get => connection;
        private set => connection = value;
    }

    public DataService()
    {
        CreateTables();
    }

    public void Connect(string connectionString)
    {
        SQLiteConnection connection;

        try
        {
            // open new connection
            connection = new(connectionString);
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

        Connection.CreateTable<Event>();
        Connection.CreateTable<EventImmediate>();
        Connection.CreateTable<EventAfter>();
        Connection.CreateTable<EventTrigger>();

        Connection.CreateTable<Portrait>();

        Connection.CreateTable<Option>();
        Connection.CreateTable<OptionTrigger>();
        Connection.CreateTable<OptionEffect>();

        Connection.CreateTable<Trigger>();

        Connection.CreateTable<Effect>();
    }

    public void DropTables()
    {
        Connection.DropTable<Mod>();

        Connection.DropTable<Event>();
        Connection.DropTable<EventImmediate>();
        Connection.DropTable<EventAfter>();
        Connection.DropTable<EventTrigger>();

        Connection.DropTable<Portrait>();

        Connection.DropTable<Option>();
        Connection.DropTable<OptionTrigger>();
        Connection.DropTable<OptionEffect>();

        Connection.DropTable<Trigger>();

        Connection.DropTable<Effect>();
    }

    public void TruncateTables()
    {
        Connection.DeleteAll<Mod>();

        Connection.DeleteAll<Event>();
        Connection.DeleteAll<EventImmediate>();
        Connection.DeleteAll<EventAfter>();
        Connection.DeleteAll<EventTrigger>();

        Connection.DeleteAll<Portrait>();

        Connection.DeleteAll<Option>();
        Connection.DeleteAll<OptionTrigger>();
        Connection.DeleteAll<OptionEffect>();

        Connection.DeleteAll<Trigger>();

        Connection.DeleteAll<Effect>();
    }
}
