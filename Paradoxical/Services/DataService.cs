﻿using Paradoxical.Model;
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

    public void DropTables()
    {
        Connection.DropTable<Mod>();

        Connection.DropTable<Event>();
        Connection.DropTable<EventImmediateEffect>();
        Connection.DropTable<EventAfterEffect>();
        Connection.DropTable<EventTrigger>();

        Connection.DropTable<Portrait>();

        Connection.DropTable<Option>();
        Connection.DropTable<OptionTrigger>();
        Connection.DropTable<OptionEffect>();

        Connection.DropTable<OnAction>();
        Connection.DropTable<OnActionTrigger>();
        Connection.DropTable<OnActionEffect>();
        Connection.DropTable<OnActionEvent>();
        Connection.DropTable<OnActionOnAction>();

        Connection.DropTable<Decision>();
        Connection.DropTable<DecisionIsShownTrigger>();
        Connection.DropTable<DecisionIsValidTrigger>();
        Connection.DropTable<DecisionIsValidFailureTrigger>();
        Connection.DropTable<DecisionEffect>();

        Connection.DropTable<Trigger>();

        Connection.DropTable<Effect>();
    }

    public void TruncateTables()
    {
        Connection.DeleteAll<Mod>();

        Connection.DeleteAll<Event>();
        Connection.DeleteAll<EventImmediateEffect>();
        Connection.DeleteAll<EventAfterEffect>();
        Connection.DeleteAll<EventTrigger>();

        Connection.DeleteAll<Portrait>();

        Connection.DeleteAll<Option>();
        Connection.DeleteAll<OptionTrigger>();
        Connection.DeleteAll<OptionEffect>();

        Connection.DeleteAll<OnAction>();
        Connection.DeleteAll<OnActionTrigger>();
        Connection.DeleteAll<OnActionEffect>();
        Connection.DeleteAll<OnActionEvent>();
        Connection.DeleteAll<OnActionOnAction>();

        Connection.DeleteAll<Decision>();
        Connection.DeleteAll<DecisionIsShownTrigger>();
        Connection.DeleteAll<DecisionIsValidTrigger>();
        Connection.DeleteAll<DecisionIsValidFailureTrigger>();
        Connection.DeleteAll<DecisionEffect>();

        Connection.DeleteAll<Trigger>();

        Connection.DeleteAll<Effect>();
    }
}
