using FuzzySharp;
using MaterialDesignThemes.Wpf;
using Paradoxical.Core;
using Paradoxical.Dumps;
using Paradoxical.Extensions;
using Paradoxical.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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

        CodeScope =
            0b0000000000000010,

        CodeSnippet =
            0b0000000000000100,

        LocalizationFunction =
            0b0000000000001000,

        LocalizationArgument =
            0b0000000000010000,

        LocalizationStyle =
            0b0000000000100000,

        LocalizationIcon =
            0b0000000001000000,

        LocalizationScope =
            0b0000000010000000,

        Theme =
            0b0000000100000000,

        Animation =
            0b0000001000000000,

        Code =
            Scope
            | CodeScope
            | CodeSnippet,

        Localization =
            Scope
            | LocalizationScope
            | LocalizationFunction
            | LocalizationArgument
            | LocalizationStyle
            | LocalizationIcon,

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

    private static List<Item> Suggestions { get; } = new List<Item>()
    {
        #region LOCALIZATION SCOPES

        new()
        {
            Name = "Char",
            Code = "Char",
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.LocalizationScope,
        },

        #endregion
        #region CODE SCOPES

        new()
        {
            Name = "scope",
            Code = "scope:",
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.CodeScope,
        },

        #endregion
        #region LOCALIZATION FUNCTIONS

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

        #endregion
        #region LOCALIZATION ARGUMENTS

        new()
        {
            Name = "Uppercase Argument",
            Code = "|U",
            Offset = 0,
            Tags = new[] { "argumentupper", "argumentupper" },
            Icon = PackIconKind.FormatPaint,
            Kind = Kind.LocalizationArgument,
        },
        new()
        {
            Name = "Lowercase Argument",
            Code = "|L",
            Offset = 0,
            Tags = new[] { "argumentlower", "lowerargument" },
            Icon = PackIconKind.FormatPaint,
            Kind = Kind.LocalizationArgument,
        },
        new()
        {
            Name = "Positive Argument",
            Code = "|P",
            Offset = 0,
            Tags = new[] { "argumentpos", "posargument" },
            Icon = PackIconKind.FormatPaint,
            Kind = Kind.LocalizationArgument,
        },
        new()
        {
            Name = "Negative Argument",
            Code = "|N",
            Offset = 0,
            Tags = new[] { "argumentneg", "negargument" },
            Icon = PackIconKind.FormatPaint,
            Kind = Kind.LocalizationArgument,
        },
        new()
        {
            Name = "Game Concept Argument",
            Code = "|E",
            Offset = 0,
            Tags = new[] { "argumentgameconcept", "gameconceptargument" },
            Icon = PackIconKind.FormatPaint,
            Kind = Kind.LocalizationArgument,
        },
        new()
        {
            Name = "White Argument",
            Code = "|V",
            Offset = 0,
            Tags = new[] { "argument", "white" },
            Icon = PackIconKind.FormatPaint,
            Kind = Kind.LocalizationArgument,
        },

        #endregion
        #region LOCALIZATION STYLES

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

        #endregion
        #region LOCALIZATION ICONS

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

        #endregion
        #region THEMES

        new()
        {
            Name = "abduct_scheme",
            Code = "abduct_scheme",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "alliance",
            Code = "alliance",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "bastardy",
            Code = "bastardy",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "battle",
            Code = "battle",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "befriend_scheme",
            Code = "befriend_scheme",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "claim_throne_scheme",
            Code = "claim_throne_scheme",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "corruption",
            Code = "corruption",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "crown",
            Code = "crown",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "culture_change",
            Code = "culture_change",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "death",
            Code = "death",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "default",
            Code = "default",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "diplomacy",
            Code = "diplomacy",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "diplomacy_family_focus",
            Code = "diplomacy_family_focus",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "diplomacy_foreign_affairs_focus",
            Code = "diplomacy_foreign_affairs_focus",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "diplomacy_majesty_focus",
            Code = "diplomacy_majesty_focus",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "dread",
            Code = "dread",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "dungeon",
            Code = "dungeon",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "dynasty",
            Code = "dynasty",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "education",
            Code = "education",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "fabricate_hook_scheme",
            Code = "fabricate_hook_scheme",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "faith",
            Code = "faith",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "family",
            Code = "family",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "feast_activity",
            Code = "feast_activity",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "friend_relation",
            Code = "friend_relation",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "friendly",
            Code = "friendly",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "generic_intrigue_scheme",
            Code = "generic_intrigue_scheme",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "healthcare",
            Code = "healthcare",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "hunt_activity",
            Code = "hunt_activity",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "hunting",
            Code = "hunting",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "intrigue",
            Code = "intrigue",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "intrigue_intimidation_focus",
            Code = "intrigue_intimidation_focus",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "intrigue_skulduggery_focus",
            Code = "intrigue_skulduggery_focus",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "intrigue_temptation_focus",
            Code = "intrigue_temptation_focus",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "learning",
            Code = "learning",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "learning_medicine_focus",
            Code = "learning_medicine_focus",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "learning_scholarship_focus",
            Code = "learning_scholarship_focus",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "learning_theology_focus",
            Code = "learning_theology_focus",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "love",
            Code = "love",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "lover_relation",
            Code = "lover_relation",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "marriage",
            Code = "marriage",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "martial",
            Code = "martial",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "martial_authority_focus",
            Code = "martial_authority_focus",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "martial_chivalry_focus",
            Code = "martial_chivalry_focus",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "martial_strategy_focus",
            Code = "martial_strategy_focus",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "medicine",
            Code = "medicine",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "mental_break",
            Code = "mental_break",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "mental_health",
            Code = "mental_health",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "murder_scheme",
            Code = "murder_scheme",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "party",
            Code = "party",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "pet",
            Code = "pet",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "physical_health",
            Code = "physical_health",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "pilgrimage_activity",
            Code = "pilgrimage_activity",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "pregnancy",
            Code = "pregnancy",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "prison",
            Code = "prison",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "realm",
            Code = "realm",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "recovery",
            Code = "recovery",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "rival_relation",
            Code = "rival_relation",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "romance_scheme",
            Code = "romance_scheme",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "secret",
            Code = "secret",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "seduce_scheme",
            Code = "seduce_scheme",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "seduction",
            Code = "seduction",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "skull",
            Code = "skull",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "stewardship",
            Code = "stewardship",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "stewardship_domain_focus",
            Code = "stewardship_domain_focus",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "stewardship_duty_focus",
            Code = "stewardship_duty_focus",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "stewardship_wealth_focus",
            Code = "stewardship_wealth_focus",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "sway_scheme",
            Code = "sway_scheme",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "unfriendly",
            Code = "unfriendly",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "vassal",
            Code = "vassal",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "war",
            Code = "war",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },
        new()
        {
            Name = "witchcraft",
            Code = "witchcraft",
            Icon = PackIconKind.Landscape,
            Kind = Kind.Theme,
        },

        #endregion
        #region ANIMATIONS
        
        new()
        {
            Name = "idle",
            Code = "idle",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "chancellor",
            Code = "chancellor",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "steward",
            Code = "steward",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "marshal",
            Code = "marshal",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "spymaster",
            Code = "spymaster",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "chaplain",
            Code = "chaplain",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "anger",
            Code = "anger",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "rage",
            Code = "rage",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "disapproval",
            Code = "disapproval",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "disbelief",
            Code = "disbelief",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "disgust",
            Code = "disgust",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "fear",
            Code = "fear",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "sadness",
            Code = "sadness",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "shame",
            Code = "shame",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "shock",
            Code = "shock",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "worry",
            Code = "worry",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "boredom",
            Code = "boredom",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "grief",
            Code = "grief",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "paranoia",
            Code = "paranoia",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "dismissal",
            Code = "dismissal",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "flirtation",
            Code = "flirtation",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "flirtation_left",
            Code = "flirtation_left",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "love",
            Code = "love",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "schadenfreude",
            Code = "schadenfreude",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "stress",
            Code = "stress",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "happiness",
            Code = "happiness",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "ecstasy",
            Code = "ecstasy",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "admiration",
            Code = "admiration",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "lunatic",
            Code = "lunatic",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "scheme",
            Code = "scheme",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "beg",
            Code = "beg",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "pain",
            Code = "pain",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "poison",
            Code = "poison",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "aggressive_axe",
            Code = "aggressive_axe",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "aggressive_mace",
            Code = "aggressive_mace",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "aggressive_sword",
            Code = "aggressive_sword",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "aggressive_dagger",
            Code = "aggressive_dagger",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "aggressive_spear",
            Code = "aggressive_spear",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "aggressive_hammer",
            Code = "aggressive_hammer",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "celebrate_axe",
            Code = "celebrate_axe",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "celebrate_mace",
            Code = "celebrate_mace",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "celebrate_sword",
            Code = "celebrate_sword",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "celebrate_dagger",
            Code = "celebrate_dagger",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "celebrate_spear",
            Code = "celebrate_spear",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "celebrate_hammer",
            Code = "celebrate_hammer",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "loss_1",
            Code = "loss_1",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "chess_certain_win",
            Code = "chess_certain_win",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "chess_cocky",
            Code = "chess_cocky",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "laugh",
            Code = "laugh",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "lantern",
            Code = "lantern",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "eyeroll",
            Code = "eyeroll",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "eavesdrop",
            Code = "eavesdrop",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "assassin",
            Code = "assassin",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "toast",
            Code = "toast",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "toast_goblet",
            Code = "toast_goblet",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "drink",
            Code = "drink",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "drink_goblet",
            Code = "drink_goblet",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "newborn",
            Code = "newborn",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "sick",
            Code = "sick",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "severelywounded",
            Code = "severelywounded",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "prisonhouse",
            Code = "prisonhouse",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "prisondungeon",
            Code = "prisondungeon",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "war_attacker",
            Code = "war_attacker",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "war_defender",
            Code = "war_defender",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "war_over_tie",
            Code = "war_over_tie",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "war_over_win",
            Code = "war_over_win",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "war_over_loss",
            Code = "war_over_loss",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "pregnant",
            Code = "pregnant",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "personality_honorable",
            Code = "personality_honorable",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "personality_dishonorable",
            Code = "personality_dishonorable",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "personality_bold",
            Code = "personality_bold",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "personality_coward",
            Code = "personality_coward",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "personality_greedy",
            Code = "personality_greedy",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "personality_content",
            Code = "personality_content",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "personality_vengeful",
            Code = "personality_vengeful",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "personality_forgiving",
            Code = "personality_forgiving",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "personality_rational",
            Code = "personality_rational",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "personality_irrational",
            Code = "personality_irrational",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "personality_compassionate",
            Code = "personality_compassionate",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "personality_callous",
            Code = "personality_callous",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "personality_zealous",
            Code = "personality_zealous",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "personality_cynical",
            Code = "personality_cynical",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "frontend_center_idle",
            Code = "frontend_center_idle",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "frontend_left_idle",
            Code = "frontend_left_idle",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "frontend_right_idle",
            Code = "frontend_right_idle",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_chancellor",
            Code = "throne_room_chancellor",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_kneel_1",
            Code = "throne_room_kneel_1",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_kneel_2",
            Code = "throne_room_kneel_2",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_curtsey_1",
            Code = "throne_room_curtsey_1",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_messenger_1",
            Code = "throne_room_messenger_1",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_messenger_2",
            Code = "throne_room_messenger_2",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_messenger_3",
            Code = "throne_room_messenger_3",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_conversation_1",
            Code = "throne_room_conversation_1",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_conversation_2",
            Code = "throne_room_conversation_2",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_conversation_3",
            Code = "throne_room_conversation_3",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_conversation_4",
            Code = "throne_room_conversation_4",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_cheer_1",
            Code = "throne_room_cheer_1",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_cheer_2",
            Code = "throne_room_cheer_2",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_applaud_1",
            Code = "throne_room_applaud_1",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_bow_1",
            Code = "throne_room_bow_1",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_bow_2",
            Code = "throne_room_bow_2",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_bow_3",
            Code = "throne_room_bow_3",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_one_handed_passive_1",
            Code = "throne_room_one_handed_passive_1",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_one_handed_passive_2",
            Code = "throne_room_one_handed_passive_2",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_two_handed_passive_1",
            Code = "throne_room_two_handed_passive_1",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_writer",
            Code = "throne_room_writer",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "test_case_1",
            Code = "test_case_1",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },
        new()
        {
            Name = "throne_room_ruler",
            Code = "throne_room_ruler",
            Icon = PackIconKind.Animation,
            Kind = Kind.Animation,
        },

	    #endregion
    };

    private IEnumerable<Item> Items { get; } = Suggestions.ToArray();

    private ICollectionView View => CollectionViewSource.GetDefaultView(Items);
    private IEnumerable<Item> FilteredItems => View.Cast<Item>();

    public Item? Selected { get; set; }

    public string Filter { get; set; } = string.Empty;
    public Kind AllowedItems { get; set; } = Kind.All;
    public int MaxItems { get; set; } = 10;

    public bool? Result { get; set; }

    public double AnchorX { get; set; }
    public double AnchorY { get; set; }
    public double OffsetX { get; set; }
    public double OffsetY { get; set; }
    public double InverseOffsetX { get; set; }
    public double InverseOffsetY { get; set; }

    public CompleteBox()
    {
        InitializeComponent();

        ItemsBox.ItemsSource = Items;
        View.Filter = Predicate;
        View.SortDescriptions.Add(new(nameof(Item.Score), ListSortDirection.Descending));
    }

    static CompleteBox()
    {
        var triggers = TriggerInfo.ParseLog();
        var effects = EffectInfo.ParseLog();
        var scopes = ScopeInfo.ParseLog();

        foreach (var info in triggers)
        {
            Suggestions.Add(new()
            {
                Name = info.Name,
                Code = info.Name,
                Tooltip = info.Tooltip,
                Icon = PackIconKind.CodeBraces,
                Kind = Kind.CodeSnippet,
            });
        }

        foreach (var info in effects)
        {
            Suggestions.Add(new()
            {
                Name = info.Name,
                Code = info.Name,
                Tooltip = info.Tooltip,
                Icon = PackIconKind.CodeBraces,
                Kind = Kind.CodeSnippet,
            });
        }

        foreach (var info in scopes)
        {
            Suggestions.Add(new()
            {
                Name = info.Name,
                Code = info.Name,
                Tooltip = info.Tooltip,
                Icon = PackIconKind.ArrowRightBottom,
                Kind = Kind.Scope,
            });
        }
    }

    static string ReadEmbeddedResource(string resourceName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        using Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new Exception($"Resource '{resourceName}' not found in the assembly.");
        }

        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
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

            var defaults = Items.Where(item => AllowedItems.HasFlag(item.Kind) == true).Take(MaxItems);
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

            item.Score = Fuzz.Ratio(filter, name);

            foreach (string tag in item.Tags)
            {
                if (string.IsNullOrEmpty(tag) == true)
                { continue; }

                int score = Fuzz.Ratio(filter, tag);
                if (score > item.Score)
                {
                    item.Score = score;
                }
            }
        }

        var garbage = valids.OrderBy(item => item.Score).Take(valids.Count() - MaxItems);
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

    private void MouseUpHandler(object sender, MouseButtonEventArgs e)
    {
        Result = true;
        Close();
    }

    private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
    {
        double screenWidth = SystemParameters.PrimaryScreenWidth;
        double screenHeight = SystemParameters.PrimaryScreenHeight;

        if (AnchorX + ActualWidth > screenWidth)
        {
            Left = screenWidth - ActualWidth - InverseOffsetX;
        }
        else
        {
            Left = AnchorX + OffsetX;
        }

        if (AnchorY + ActualHeight > screenHeight)
        {
            Top = AnchorY - ActualHeight - InverseOffsetY;
        }
        else
        {
            Top = AnchorY + OffsetY;
        }
    }
}
