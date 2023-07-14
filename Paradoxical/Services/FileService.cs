using Microsoft.Win32;
using Paradoxical.Extensions;
using System;
using System.Diagnostics;
using System.IO;

namespace Paradoxical.Services;

public interface IFileService
{
    void New();
    void Open();
    void Backup();
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

    private string SaveDir { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    private string SaveFile { get; set; } = string.Empty;
    private string SavePath { get; set; } = string.Empty;

    private string ModDir { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    private string ModFile { get; set; } = string.Empty;

    public void New()
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

        if (dlg.ShowDialog() != true)
        { return; }

        SaveDir = Path.GetDirectoryName(dlg.FileName) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        SaveFile = Path.GetFileNameWithoutExtension(dlg.FileName) ?? string.Empty;
        SavePath = dlg.FileName;

        if (Directory.Exists(SaveDir) == false)
        { return; }

        if (SaveFile == string.Empty)
        { return; }

        Data.Connect(SavePath, false);

        Data.DropTables();
        Data.CreateTables();
    }

    public void Backup()
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

        if (dlg.ShowDialog() != true)
        { return; }

        SaveDir = Path.GetDirectoryName(dlg.FileName) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        SaveFile = Path.GetFileNameWithoutExtension(dlg.FileName) ?? string.Empty;
        SavePath = dlg.FileName;

        if (Directory.Exists(SaveDir) == false)
        { return; }

        if (SaveFile == string.Empty)
        { return; }

        Data.Connect(SavePath, true);

        Data.CreateTables();
    }

    public void Open()
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

        if (dlg.ShowDialog() != true)
        { return; }

        SaveDir = Path.GetDirectoryName(dlg.FileName) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        SaveFile = Path.GetFileNameWithoutExtension(dlg.FileName) ?? string.Empty;
        SavePath = dlg.FileName;

        if (File.Exists(SavePath) == false)
        { return; }

        Data.Connect(SavePath, false);

        Data.CreateTables();
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

        ModDir = Path.GetDirectoryName(dlg.FileName) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        ModFile = Path.GetFileNameWithoutExtension(dlg.FileName) ?? string.Empty;

        if (Directory.Exists(ModDir) == false)
        { return; }

        if (ModFile == string.Empty)
        { return; }

        Build.Export(ModDir, ModFile);

        Process.Start("explorer.exe", ModDir);
    }
}
