using FuzzySharp;
using MaterialDesignThemes.Wpf;
using Paradoxical.Core;
using Paradoxical.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Paradoxical.View;

/// <summary>
/// Interaction logic for CompleteBox.xaml
/// </summary>
public partial class CompleteBox : Window
{
    [Flags]
    public enum Kind
    {
        None = 0,
        Scope = 0b00000001,
        CodeSnippet = 0b00000010,
        LocalizationFunction = 0b00000100,
        Localization = LocalizationFunction | Scope,
        Code = CodeSnippet | Scope,
    }

    public class Item
    {
        public string Name { get; init; } = string.Empty;
        public string Code { get; init; } = string.Empty;
        public PackIconKind Icon { get; init; } = PackIconKind.None;
        public Kind Kind { get; init; } = Kind.None;
        public int Score { get; set; } = 0;
    }

    private IEnumerable<Item> Items { get; } = new Item[]
    {
        new()
        {
            Name = "THIS",
            Code = "THIS",
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
        },
        new()
        {
            Name = "ROOT",
            Code = "ROOT",
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
        },
        new()
        {
            Name = "FROM",
            Code = "FROM",
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
        },
        new()
        {
            Name = "PREV",
            Code = "PREV",
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
        },
        new()
        {
            Name = "scope",
            Code = "scope:",
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
        },
        new()
        {
            Name = "always",
            Code = "always = ",
            Icon = PackIconKind.CodeBraces,
            Kind = Kind.CodeSnippet,
        },
        new()
        {
            Name = "exists",
            Code = "exists = ",
            Icon = PackIconKind.CodeBraces,
            Kind = Kind.CodeSnippet,
        },
        new()
        {
            Name = "save_scope_as",
            Code = "save_scope_as = ",
            Icon = PackIconKind.CodeBraces,
            Kind = Kind.CodeSnippet,
        },
        new()
        {
            Name = "set_variable",
            Code = "set_variable = ",
            Icon = PackIconKind.CodeBraces,
            Kind = Kind.CodeSnippet,
        },
        new()
        {
            Name = "change_variable",
            Code = "change_variable = ",
            Icon = PackIconKind.CodeBraces,
            Kind = Kind.CodeSnippet,
        },
        new()
        {
            Name = "has_variable",
            Code = "has_variable = ",
            Icon = PackIconKind.CodeBraces,
            Kind = Kind.CodeSnippet,
        },
        new()
        {
            Name = "days",
            Code = "days = ",
            Icon = PackIconKind.CodeBraces,
            Kind = Kind.CodeSnippet,
        },
        new()
        {
            Name = "months",
            Code = "months = ",
            Icon = PackIconKind.CodeBraces,
            Kind = Kind.CodeSnippet,
        },
        new()
        {
            Name = "years",
            Code = "years = ",
            Icon = PackIconKind.CodeBraces,
            Kind = Kind.CodeSnippet,
        },
        new()
        {
            Name = "GetName",
            Code = "GetName",
            Icon = PackIconKind.Function,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetCulture",
            Code = "GetCulture",
            Icon = PackIconKind.Function,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFaith",
            Code = "GetFaith",
            Icon = PackIconKind.Function,
            Kind = Kind.LocalizationFunction,
        },
    };

    private ICollectionView View => CollectionViewSource.GetDefaultView(Items);
    private IEnumerable<Item> FilteredItems => View.Cast<Item>();

    public Item? Selected { get; set; }
    public string? Filter { get; set; }
    public Kind AllowedItems { get; set; }

    public bool? Result { get; set; }

    public CompleteBox()
    {
        InitializeComponent();

        ItemsBox.ItemsSource = Items;
        View.Filter = Predicate;
        View.SortDescriptions.Add(new(nameof(Item.Score), ListSortDirection.Descending));
    }

    private void KeyDownHandler(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Up)
        {
            SelectPrevious();
            e.Handled = true;
        }
        if (e.Key == Key.Down)
        {
            SelectNext();
            e.Handled = true;
        }
    }

    private bool Predicate(object obj)
    {
        if (obj is not Item item)
        { return false; }

        if (AllowedItems.HasFlag(item.Kind) == false)
        { return false; }

        if (string.IsNullOrEmpty(Filter) == true)
        { return true; }

        if (Fuzz.PartialRatio(item.Name.ToLowerInvariant(), Filter.ToLowerInvariant()) >= 60)
        { return true; }

        return false;
    }

    public void SelectPrevious()
    {
        var i = ItemsBox.SelectedIndex - 1;
        if (i >= 0)
        {
            ItemsBox.SelectedIndex = i;
        }
    }

    public void SelectNext()
    {
        var i = ItemsBox.SelectedIndex + 1;
        if (i < ItemsBox.Items.Count)
        {
            ItemsBox.SelectedIndex = i;
        }
    }

    public void UpdateScores()
    {
        foreach (var item in Items)
        {
            if (string.IsNullOrEmpty(Filter) == true)
            {
                item.Score = 0;
                continue;
            }

            item.Score = Fuzz.PartialRatio(item.Name.ToLowerInvariant(), Filter.ToLowerInvariant());
        }
    }

    public void UpdateView()
    {
        View.Refresh();
    }

    public void UpdateSelection()
    {
        if (Selected != null && Predicate(Selected) == true)
        { return; }

        Item? selected = FilteredItems.FirstOrDefault();
        ItemsBox.SelectedItem = selected;
    }

    private void SelectedHandler(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListView box)
        { return; }

        if (box.SelectedItem is not Item selected)
        { return; }

        Selected = selected;
    }

    private void MouseDownHandler(object sender, MouseButtonEventArgs e)
    {
        Result = true;
        Close();
    }
}
