using Microsoft.Win32;
using Paradoxical.Extensions;
using System;
using System.Diagnostics;
using System.IO;

namespace Paradoxical.Services;

public interface IFileService
{
    string SaveDir { get; }
    string SaveFile { get; }
    string SavePath { get; }

    string ModDir { get; }
    string ModFile { get; }
    string ModPath { get; }

    void SetSave(string file);
    void SetMod(string file);
    bool? ShowNewDialog();
    bool? ShowOpenDialog();
    bool? ShowBackupDialog();
    void NewSave();
    void BackupSave();
    void OpenSave();
    void Export();
    void ExportAs();
}

public class FileService : IFileService
{
    public IDataService Data { get; }
    public IBuildService Build { get; }

    public FileService(
        IDataService data,
        IBuildService build)
    {
        Data = data;
        Build = build;
    }

    public string SaveDir { get; private set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public string SaveFile { get; private set; } = string.Empty;
    public string SavePath { get; private set; } = string.Empty;

    public void SetSave(string file)
    {
        SaveDir = Path.GetDirectoryName(file) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        SaveFile = Path.GetFileNameWithoutExtension(file) ?? string.Empty;
        SavePath = file;
    }

    public bool? ShowNewDialog()
    {
        SaveFileDialog dlg = new()
        {
            Title = "Create Mod",
            CreatePrompt = false,
            OverwritePrompt = true,
            Filter = "Database|*.db3",
            DefaultExt = ".db3",
            AddExtension = true,
            InitialDirectory = SaveDir,
            FileName = SaveFile,
        };

        bool? result = dlg.ShowDialog();
        if (result != true)
        { return result; }

        SetSave(dlg.FileName);

        return result;
    }

    public bool? ShowBackupDialog()
    {
        SaveFileDialog dlg = new()
        {
            Title = "Backup Mod",
            CreatePrompt = false,
            OverwritePrompt = true,
            Filter = "Database|*.db3",
            DefaultExt = ".db3",
            AddExtension = true,
            InitialDirectory = SaveDir,
            FileName = SaveFile,
        };

        bool? result = dlg.ShowDialog();
        if (result != true)
        { return result; }

        SetSave(dlg.FileName);

        return result;
    }

    public bool? ShowOpenDialog()
    {
        OpenFileDialog dlg = new()
        {
            Title = "Open Mod",
            Filter = "Database|*.db3",
            DefaultExt = ".db3",
            AddExtension = true,
            InitialDirectory = SaveDir,
            FileName = SaveFile,
        };

        bool? result = dlg.ShowDialog();
        if (result != true)
        { return result; }

        SetSave(dlg.FileName);

        return result;
    }

    public void NewSave()
    {
        if (Directory.Exists(SaveDir) == false)
        { return; }

        if (SaveFile == string.Empty)
        { return; }

        Data.Connect(SavePath, false);

        Data.DropTables();
        Data.CreateTables();
    }

    public void BackupSave()
    {
        if (Directory.Exists(SaveDir) == false)
        { return; }

        if (SaveFile == string.Empty)
        { return; }

        Data.Connect(SavePath, true);
        Data.CreateTables();
    }

    public void OpenSave()
    {
        if (File.Exists(SavePath) == false)
        { return; }

        Data.Connect(SavePath, false);
        Data.CreateTables();
    }

    public string ModDir { get; private set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public string ModFile { get; private set; } = string.Empty;
    public string ModPath { get; private set; } = string.Empty;

    public void SetMod(string file)
    {
        ModDir = Path.GetDirectoryName(file) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        ModFile = Path.GetFileNameWithoutExtension(file) ?? string.Empty;
        ModPath = file;
    }

    public void Export()
    {
        if (Directory.Exists(ModDir) == false || ModFile == string.Empty)
        {
            ExportAs();
            return;
        }

        Build.Export(ModDir, ModFile);
    }

    public void ExportAs()
    {
        SaveFileDialog dlg = new()
        {
            Title = "Build Mod",
            CreatePrompt = false,
            OverwritePrompt = true,
            Filter = "Paradox Mod File|*.mod",
            DefaultExt = ".mod",
            AddExtension = true,
            InitialDirectory = ModDir,
            FileName = ModFile,
        };

        if (dlg.ShowDialog() != true)
        { return; }

        SetMod(dlg.FileName);

        if (Directory.Exists(ModDir) == false)
        { return; }

        if (ModFile == string.Empty)
        { return; }

        Build.Export(ModDir, ModFile);

        Process.Start("explorer.exe", ModDir);
    }
}
