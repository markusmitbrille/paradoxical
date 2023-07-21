using FuzzySharp;
using MaterialDesignThemes.Wpf;
using Paradoxical.Core;
using Paradoxical.Extensions;
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
using System.Xml.Linq;
using static Paradoxical.View.CompleteBox;

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

        Scope =
            0b0000000000000001,

        CodeSnippet =
            0b0000000000000010,

        LocalizationFunction =
            0b0000000000000100,

        LocalizationArgument =
            0b0000000000001000,

        LocalizationStyle =
            0b0000000000010000,

        LocalizationIcon =
            0b0000000000100000,

        LocalizationScope =
            0b0000000001000000,

        Code =
            Scope
            | CodeSnippet,

        Localization =
            Scope
            | LocalizationFunction
            | LocalizationArgument
            | LocalizationStyle
            | LocalizationIcon
            | LocalizationScope,

        All =
            Code | Localization,
    }

    public class Item
    {
        public string Name { get; init; } = string.Empty;
        public string Code { get; init; } = string.Empty;
        public int? Offset { get; init; } = null;
        public string[] Tags { get; init; } = Array.Empty<string>();

        public PackIconKind Icon { get; init; } = PackIconKind.None;
        public Kind Kind { get; init; } = Kind.None;

        public string? Tooltip { get; init; } = null;

        public int Score { get; set; } = 0;
    }

    private IEnumerable<Item> Items { get; } = new Item[]
    {
        new()
        {
            Name = "scope",
            Code = "scope:",
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
        },
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
            Name = "Char",
            Code = "Char",
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.LocalizationScope,
        },
        new()
        {
            Name = "Culture → Culture Head",
            Code = "culture_head",
            Tags = new[] { "culture", "culture_head" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
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
            Name = "GetPlayer",
            Code = "GetPlayer",
            Icon = PackIconKind.Function,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetCharacter",
            Code = "GetCharacter",
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
        new()
        {
            Name = "GetHerHim",
            Code = "GetHerHim",
            Icon = PackIconKind.GenderMaleFemale,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetHerHis",
            Code = "GetHerHis",
            Icon = PackIconKind.GenderMaleFemale,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetHerHisMy",
            Code = "GetHerHisMy",
            Icon = PackIconKind.GenderMaleFemale,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetHersHis",
            Code = "GetHersHis",
            Icon = PackIconKind.GenderMaleFemale,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetHerselfHimself",
            Code = "GetHerselfHimself",
            Icon = PackIconKind.GenderMaleFemale,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetLadyLord",
            Code = "GetLadyLord",
            Icon = PackIconKind.GenderMaleFemale,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetSheHe",
            Code = "GetSheHe",
            Icon = PackIconKind.GenderMaleFemale,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetDaughterSon",
            Code = "GetDaughterSon",
            Icon = PackIconKind.GenderMaleFemale,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetDaughterSonPossessive",
            Code = "GetDaughterSonPossessive",
            Icon = PackIconKind.GenderMaleFemale,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstName",
            Code = "GetFirstName",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameBase",
            Code = "GetFirstNameBase",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNicknamed",
            Code = "GetFirstNameNicknamed",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNicknamedNoTooltip",
            Code = "GetFirstNameNicknamedNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNicknamedNoTooltipRegnal",
            Code = "GetFirstNameNicknamedNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNicknamedOrMe",
            Code = "GetFirstNameNicknamedOrMe",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNicknamedOrMeNoTooltip",
            Code = "GetFirstNameNicknamedOrMeNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNicknamedOrMeNoTooltipRegnal",
            Code = "GetFirstNameNicknamedOrMeNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNicknamedOrMeRegnal",
            Code = "GetFirstNameNicknamedOrMeRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNicknamedPossessive",
            Code = "GetFirstNameNicknamedPossessive",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNicknamedPossessiveNoTooltip",
            Code = "GetFirstNameNicknamedPossessiveNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNicknamedPossessiveNoTooltipRegnal",
            Code = "GetFirstNameNicknamedPossessiveNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNicknamedPossessiveOrMy",
            Code = "GetFirstNameNicknamedPossessiveOrMy",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNicknamedPossessiveOrMyNoTooltip",
            Code = "GetFirstNameNicknamedPossessiveOrMyNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNicknamedPossessiveOrMyNoTooltipRegnal",
            Code = "GetFirstNameNicknamedPossessiveOrMyNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNicknamedPossessiveOrMyRegnal",
            Code = "GetFirstNameNicknamedPossessiveOrMyRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNicknamedPossessiveRegnal",
            Code = "GetFirstNameNicknamedPossessiveRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNicknamedRegnal",
            Code = "GetFirstNameNicknamedRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNoTooltip",
            Code = "GetFirstNameNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameNoTooltipRegnal",
            Code = "GetFirstNameNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameOrMe",
            Code = "GetFirstNameOrMe",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameOrMeNoTooltip",
            Code = "GetFirstNameOrMeNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameOrMeNoTooltipRegnal",
            Code = "GetFirstNameOrMeNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameOrMeRegnal",
            Code = "GetFirstNameOrMeRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNamePossessive",
            Code = "GetFirstNamePossessive",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNamePossessiveNoTooltip",
            Code = "GetFirstNamePossessiveNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNamePossessiveNoTooltipRegnal",
            Code = "GetFirstNamePossessiveNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNamePossessiveOrMy",
            Code = "GetFirstNamePossessiveOrMy",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNamePossessiveOrMyNoTooltip",
            Code = "GetFirstNamePossessiveOrMyNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNamePossessiveOrMyNoTooltipRegnal",
            Code = "GetFirstNamePossessiveOrMyNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNamePossessiveOrMyRegnal",
            Code = "GetFirstNamePossessiveOrMyRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNamePossessiveRegnal",
            Code = "GetFirstNamePossessiveRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFirstNameRegnal",
            Code = "GetFirstNameRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetTitledFirstName",
            Code = "GetTitledFirstName",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullName",
            Code = "GetFullName",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNicknamed",
            Code = "GetFullNameNicknamed",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNicknamedNoTooltip",
            Code = "GetFullNameNicknamedNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNicknamedNoTooltipRegnal",
            Code = "GetFullNameNicknamedNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNicknamedOrMe",
            Code = "GetFullNameNicknamedOrMe",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNicknamedOrMeNoTooltip",
            Code = "GetFullNameNicknamedOrMeNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNicknamedOrMeNoTooltipRegnal",
            Code = "GetFullNameNicknamedOrMeNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNicknamedOrMeRegnal",
            Code = "GetFullNameNicknamedOrMeRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNicknamedPossessive",
            Code = "GetFullNameNicknamedPossessive",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNicknamedPossessiveNoTooltip",
            Code = "GetFullNameNicknamedPossessiveNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNicknamedPossessiveNoTooltipRegnal",
            Code = "GetFullNameNicknamedPossessiveNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNicknamedPossessiveOrMy",
            Code = "GetFullNameNicknamedPossessiveOrMy",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNicknamedPossessiveOrMyNoTooltip",
            Code = "GetFullNameNicknamedPossessiveOrMyNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNicknamedPossessiveOrMyNoTooltipRegnal",
            Code = "GetFullNameNicknamedPossessiveOrMyNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNicknamedPossessiveOrMyRegnal",
            Code = "GetFullNameNicknamedPossessiveOrMyRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNicknamedPossessiveRegnal",
            Code = "GetFullNameNicknamedPossessiveRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNicknamedRegnal",
            Code = "GetFullNameNicknamedRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNoTooltip",
            Code = "GetFullNameNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameNoTooltipRegnal",
            Code = "GetFullNameNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameOrMe",
            Code = "GetFullNameOrMe",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameOrMeNoTooltip",
            Code = "GetFullNameOrMeNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameOrMeNoTooltipRegnal",
            Code = "GetFullNameOrMeNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameOrMeRegnal",
            Code = "GetFullNameOrMeRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNamePossessive",
            Code = "GetFullNamePossessive",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNamePossessiveNoTooltip",
            Code = "GetFullNamePossessiveNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNamePossessiveNoTooltipRegnal",
            Code = "GetFullNamePossessiveNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNamePossessiveOrMy",
            Code = "GetFullNamePossessiveOrMy",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNamePossessiveOrMyNoTooltip",
            Code = "GetFullNamePossessiveOrMyNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNamePossessiveOrMyNoTooltipRegnal",
            Code = "GetFullNamePossessiveOrMyNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNamePossessiveOrMyRegnal",
            Code = "GetFullNamePossessiveOrMyRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNamePossessiveRegnal",
            Code = "GetFullNamePossessiveRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetFullNameRegnal",
            Code = "GetFullNameRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNicknamed",
            Code = "GetNameNicknamed",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNicknamedNoTooltip",
            Code = "GetNameNicknamedNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNicknamedNoTooltipRegnal",
            Code = "GetNameNicknamedNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNicknamedOrMe",
            Code = "GetNameNicknamedOrMe",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNicknamedOrMeNoTooltip",
            Code = "GetNameNicknamedOrMeNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNicknamedOrMeNoTooltipRegnal",
            Code = "GetNameNicknamedOrMeNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNicknamedOrMeRegnal",
            Code = "GetNameNicknamedOrMeRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNicknamedPossessive",
            Code = "GetNameNicknamedPossessive",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNicknamedPossessiveNoTooltip",
            Code = "GetNameNicknamedPossessiveNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNicknamedPossessiveNoTooltipRegnal",
            Code = "GetNameNicknamedPossessiveNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNicknamedPossessiveOrMy",
            Code = "GetNameNicknamedPossessiveOrMy",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNicknamedPossessiveOrMyNoTooltip",
            Code = "GetNameNicknamedPossessiveOrMyNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNicknamedPossessiveOrMyNoTooltipRegnal",
            Code = "GetNameNicknamedPossessiveOrMyNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNicknamedPossessiveOrMyRegnal",
            Code = "GetNameNicknamedPossessiveOrMyRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNicknamedPossessiveRegnal",
            Code = "GetNameNicknamedPossessiveRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNicknamedRegnal",
            Code = "GetNameNicknamedRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNoTooltip",
            Code = "GetNameNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameNoTooltipRegnal",
            Code = "GetNameNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameOrMe",
            Code = "GetNameOrMe",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameOrMeNoTooltip",
            Code = "GetNameOrMeNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameOrMeNoTooltipRegnal",
            Code = "GetNameOrMeNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameOrMeRegnal",
            Code = "GetNameOrMeRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNamePossessive",
            Code = "GetNamePossessive",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNamePossessiveNoTooltip",
            Code = "GetNamePossessiveNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNamePossessiveNoTooltipRegnal",
            Code = "GetNamePossessiveNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNamePossessiveOrMy",
            Code = "GetNamePossessiveOrMy",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNamePossessiveOrMyNoTooltip",
            Code = "GetNamePossessiveOrMyNoTooltip",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNamePossessiveOrMyNoTooltipRegnal",
            Code = "GetNamePossessiveOrMyNoTooltipRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNamePossessiveOrMyRegnal",
            Code = "GetNamePossessiveOrMyRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNamePossessiveRegnal",
            Code = "GetNamePossessiveRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "GetNameRegnal",
            Code = "GetNameRegnal",
            Icon = PackIconKind.FaceMan,
            Kind = Kind.LocalizationFunction,
        },
        new()
        {
            Name = "Uppercase Argument",
            Code = "|U",
            Offset = 0,
            Tags = new[] { "argumentupper", "argumentupper" },
            Icon = PackIconKind.QuestionMark,
            Kind = Kind.LocalizationArgument,
        },
        new()
        {
            Name = "Lowercase Argument",
            Code = "|L",
            Offset = 0,
            Tags = new[] { "argumentlower", "lowerargument" },
            Icon = PackIconKind.QuestionMark,
            Kind = Kind.LocalizationArgument,
        },
        new()
        {
            Name = "Positive Argument",
            Code = "|P",
            Offset = 0,
            Tags = new[] { "argumentpos", "posargument" },
            Icon = PackIconKind.QuestionMark,
            Kind = Kind.LocalizationArgument,
        },
        new()
        {
            Name = "Negative Argument",
            Code = "|N",
            Offset = 0,
            Tags = new[] { "argumentneg", "negargument" },
            Icon = PackIconKind.QuestionMark,
            Kind = Kind.LocalizationArgument,
        },
        new()
        {
            Name = "Game Concept Argument",
            Code = "|E",
            Offset = 0,
            Tags = new[] { "argumentgameconcept", "gameconceptargument" },
            Icon = PackIconKind.QuestionMark,
            Kind = Kind.LocalizationArgument,
        },
        new()
        {
            Name = "White Argument",
            Code = "|V",
            Offset = 0,
            Tags = new[] { "argument", "white" },
            Icon = PackIconKind.QuestionMark,
            Kind = Kind.LocalizationArgument,
        },
        new()
        {
            Name = "Positive Style",
            Code = "#P  #!",
            Offset = 3,
            Tags = new[] { "stylepos", "posstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Negative Style",
            Code = "#N  #!",
            Offset = 3,
            Tags = new[] { "styleneg", "negstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Help Style",
            Code = "#help  #!",
            Offset = 6,
            Tags = new[] { "stylehelp", "helpstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Informational Style",
            Code = "#I  #!",
            Offset = 3,
            Tags = new[] { "styleinfo", "infostyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Warning Style",
            Code = "#warning  #!",
            Offset = 3,
            Tags = new[] { "stylewarning", "warningstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Title Style",
            Code = "#T  #!",
            Offset = 3,
            Tags = new[] { "styletitle", "titlestyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Game Concept Style",
            Code = "#E  #!",
            Offset = 3,
            Tags = new[] { "stylegameconcept", "gameconceptstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Italic Warning Style",
            Code = "#X  #!",
            Offset = 3,
            Tags = new[] { "styleitalicwarning", "italicwarningstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Bold and Italic Style",
            Code = "#S  #!",
            Offset = 3,
            Tags = new[] { "stylebolditalic", "bolditalicstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "White Style",
            Code = "#V  #!",
            Offset = 3,
            Tags = new[] { "stylewhite", "whitestyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Emphasized Style",
            Code = "#EMP  #!",
            Offset = 5,
            Tags = new[] { "styleemph", "emphstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Weak Style",
            Code = "#weak  #!",
            Offset = 6,
            Tags = new[] { "styleweak", "weakstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Bold Style",
            Code = "#bold  #!",
            Offset = 6,
            Tags = new[] { "stylebold", "boldstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Italic Style",
            Code = "#italic  #!",
            Offset = 8,
            Tags = new[] { "styleitalic", "italicstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Icon",
            Code = "@!",
            Offset = 1,
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Warning Icon",
            Code = "@warning_icon!",
            Tags = new[] { "iconwarning", "warningicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Gold Icon",
            Code = "@gold_icon!",
            Tags = new[] { "icon", "gold" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Prestige Icon",
            Code = "@prestige_icon!",
            Tags = new[] { "iconprestige", "prestigeicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Piety Icon",
            Code = "@piety_icon!",
            Tags = new[] { "iconpiety", "pietyicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Christian Piety Icon",
            Code = "@piety_icon_christian!",
            Tags = new[] { "iconpietychristianity", "pietychristianityicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Islam Piety Icon",
            Code = "@piety_icon_islam!",
            Tags = new[] { "iconpietyislam", "pietyislamicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Pagan Piety Icon",
            Code = "@piety_icon_pagan!",
            Tags = new[] { "iconpietypagan", "pietypaganicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Eastern Piety Icon",
            Code = "@piety_icon_eastern!",
            Tags = new[] { "iconpietyeastern", "pietyeasternicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Judaism Piety Icon",
            Code = "@piety_icon_judaism!",
            Tags = new[] { "iconpietyjudaism", "pietyjudaismicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Zoroastrian Piety Icon",
            Code = "@piety_icon_zoroastrian!",
            Tags = new[] { "iconpietyzoroastrian", "pietyzoroastrianicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Time Icon",
            Code = "@time_icon!",
            Tags = new[] { "icontime", "timeicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Cross Icon",
            Code = "@cross_icon!",
            Tags = new[] { "iconcross", "crossicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Stress Icon",
            Code = "@stress_icon!",
            Tags = new[] { "iconstress", "stressicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Dread Icon",
            Code = "@dread_icon!",
            Tags = new[] { "icondread", "dreadicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Exposed Icon",
            Code = "@exposed_icon!",
            Tags = new[] { "iconexposed", "exposedicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Diplomacy Skill Icon",
            Code = "@skill_diplomacy_icon!",
            Tags = new[] { "iconskilldiplomacy", "skilldiplomacyicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Martial Skill Icon",
            Code = "@skill_martial_icon!",
            Tags = new[] { "iconskillmartial", "skillmartialicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Stewardship Skill Icon",
            Code = "@skill_stewardship_icon!",
            Tags = new[] { "iconskillstewardship", "skillstewardshipicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Intrigue Skill Icon",
            Code = "@skill_intrigue_icon!",
            Tags = new[] { "iconskillintrigue", "skillintrigueicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Learning Skill Icon",
            Code = "@skill_learning_icon!",
            Tags = new[] { "iconskilllearning", "skilllearningicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Prowess Skill Icon",
            Code = "@skill_prowess_icon!",
            Tags = new[] { "iconskillprowess", "skillprowessicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Stress Gain Icon",
            Code = "@stress_gain_icon!",
            Tags = new[] { "iconstressgain", "stressgainicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Stress Critical Icon",
            Code = "@stress_critical_icon!",
            Tags = new[] { "iconstresscritical", "stresscriticalicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Stress Loss Icon",
            Code = "@stress_loss_icon!",
            Tags = new[] { "iconstressloss", "stresslossicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Death Icon",
            Code = "@death_icon!",
            Tags = new[] { "icondeath", "deathicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Scheme Icon",
            Code = "@scheme_icon!",
            Tags = new[] { "iconscheme", "schemeicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Crime Icon",
            Code = "@crime_icon!",
            Tags = new[] { "iconcrime", "crimeicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Intimidated Icon",
            Code = "@intimidated_icon!",
            Tags = new[] { "iconintimidated", "intimidatedicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Terrified Icon",
            Code = "@terrified_icon!",
            Tags = new[] { "iconterrified", "terrifiedicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Realm Capital Icon",
            Code = "@realm_capital_icon!",
            Tags = new[] { "iconrealmcapital", "realmcapitalicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Alliance Icon",
            Code = "@alliance_icon!",
            Tags = new[] { "iconalliance", "allianceicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Prison Icon",
            Code = "@prison_icon!",
            Tags = new[] { "iconprison", "prisonicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Titles Icon",
            Code = "@titles_icon!",
            Tags = new[] { "icontitles", "titlesicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Domain Icon",
            Code = "@domain_icon!",
            Tags = new[] { "icondomain", "domainicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Building Icon",
            Code = "@building_icon!",
            Tags = new[] { "iconbuilding", "buildingicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Faction Icon",
            Code = "@faction_icon!",
            Tags = new[] { "iconfaction", "factionicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Friend Icon",
            Code = "@friend_icon!",
            Tags = new[] { "iconfriend", "friendicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Best Friend Icon",
            Code = "@best_friend_icon!",
            Tags = new[] { "iconbestfriend", "bestfriendicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Rival Icon",
            Code = "@rival_icon!",
            Tags = new[] { "iconrival", "rivalicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Nemesis Icon",
            Code = "@nemesis_icon!",
            Tags = new[] { "iconnemesis", "nemesisicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Lover Icon",
            Code = "@lover_icon!",
            Tags = new[] { "iconlover", "lovericon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Soulmate Icon",
            Code = "@soulmate_icon!",
            Tags = new[] { "iconsoulmate", "soulmateicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Catholic Icon",
            Code = "@catholic_icon!",
            Tags = new[] { "iconcatholic", "catholicicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Orthodox Icon",
            Code = "@orthodox_icon!",
            Tags = new[] { "iconorthodox", "orthodoxicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Virtue Icon",
            Code = "@virtue_icon!",
            Tags = new[] { "iconvirtue", "virtueicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Sin Icon",
            Code = "@sin_icon!",
            Tags = new[] { "iconsin", "sinicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Fervor Icon",
            Code = "@fervor_icon!",
            Tags = new[] { "iconfervor", "fervoricon" },
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
    };

    private ICollectionView View => CollectionViewSource.GetDefaultView(Items);
    private IEnumerable<Item> FilteredItems => View.Cast<Item>();

    public Item? Selected { get; set; }

    public string Filter { get; set; } = string.Empty;
    public Kind AllowedItems { get; set; } = Kind.All;
    public int MaxItems { get; set; } = 10;

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

        if (item.Score > 50)
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
        if (Filter.IsEmpty() == true)
        {
            foreach (var item in Items)
            {
                item.Score = 0;
            }

            var defaults = Items.Take(MaxItems);
            foreach (var item in defaults)
            {
                item.Score = 100;
            }

            return;
        }

        var invalids = Items.Where(item => AllowedItems.HasFlag(item.Kind) == false);
        foreach (var item in invalids)
        {
            item.Score = 0;
        }

        var valids = Items.Where(item => AllowedItems.HasFlag(item.Kind) == true);
        foreach (var item in valids)
        {
            string filter = Filter.ToLowerInvariant();
            string name = item.Name.ToLowerInvariant();

            item.Score = Fuzz.PartialRatio(filter, name);

            foreach (string tag in item.Tags)
            {
                if (string.IsNullOrEmpty(tag) == true)
                { continue; }

                int score = Fuzz.PartialRatio(filter, tag);
                if (score > item.Score)
                {
                    item.Score = score;
                }
            }
        }

        var garbage = valids.OrderBy(item => item.Score).Take(Items.Count() - MaxItems);
        foreach (var item in garbage)
        {
            item.Score = 0;
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
