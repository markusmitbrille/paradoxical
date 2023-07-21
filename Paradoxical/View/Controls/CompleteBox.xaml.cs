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
        LocalizationArgument = 0b00001000,
        LocalizationStyle = 0b00010000,
        LocalizationIcon = 0b00100000,
        LocalizationScope = 0b01000000,
        Localization = LocalizationFunction | LocalizationArgument | LocalizationStyle | LocalizationIcon | LocalizationScope | Scope,
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
            Icon = PackIconKind.QuestionMark,
            Kind = Kind.LocalizationArgument,
        },
        new()
        {
            Name = "Lowercase Argument",
            Code = "|L",
            Icon = PackIconKind.QuestionMark,
            Kind = Kind.LocalizationArgument,
        },
        new()
        {
            Name = "Positive Argument",
            Code = "|P",
            Icon = PackIconKind.QuestionMark,
            Kind = Kind.LocalizationArgument,
        },
        new()
        {
            Name = "Negative Argument",
            Code = "|N",
            Icon = PackIconKind.QuestionMark,
            Kind = Kind.LocalizationArgument,
        },
        new()
        {
            Name = "Game Concept Argument",
            Code = "|E",
            Icon = PackIconKind.QuestionMark,
            Kind = Kind.LocalizationArgument,
        },
        new()
        {
            Name = "White Argument",
            Code = "|V",
            Icon = PackIconKind.QuestionMark,
            Kind = Kind.LocalizationArgument,
        },
        new()
        {
            Name = "Positive Style",
            Code = "#P  #!",
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Negative Style",
            Code = "#N  #!",
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Help Style",
            Code = "#help  #!",
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Informational Style",
            Code = "#I  #!",
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Warning Style",
            Code = "#warning  #!",
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Title Style",
            Code = "#T  #!",
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Game Concept Style",
            Code = "#E  #!",
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Italic Warning Style",
            Code = "#X  #!",
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Bold and Italic Style",
            Code = "#S  #!",
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "White Style",
            Code = "#V  #!",
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Emphasized Style",
            Code = "#EMP  #!",
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Weak Style",
            Code = "#weak  #!",
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Bold Style",
            Code = "#bold  #!",
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Italic Style",
            Code = "#italic  #!",
            Icon = PackIconKind.Style,
            Kind = Kind.LocalizationStyle,
        },
        new()
        {
            Name = "Icon",
            Code = "@!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Warning Icon",
            Code = "@warning_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Gold Icon",
            Code = "@gold_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Prestige Icon",
            Code = "@prestige_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Piety Icon",
            Code = "@piety_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Christian Piety Icon",
            Code = "@piety_icon_christian!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Islam Piety Icon",
            Code = "@piety_icon_islam!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Pagan Piety Icon",
            Code = "@piety_icon_pagan!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Eastern Piety Icon",
            Code = "@piety_icon_eastern!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Judaism Piety Icon",
            Code = "@piety_icon_judaism!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Zoroastrian Piety Icon",
            Code = "@piety_icon_zoroastrian!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Time Icon",
            Code = "@time_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Cross Icon",
            Code = "@cross_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Stress Icon",
            Code = "@stress_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Dread Icon",
            Code = "@dread_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Exposed Icon",
            Code = "@exposed_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Portrait Punishment Icon",
            Code = "@portrait_punishment_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Diplomacy Skill Icon",
            Code = "@skill_diplomacy_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Martial Skill Icon",
            Code = "@skill_martial_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Stewardship Skill Icon",
            Code = "@skill_stewardship_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Intrigue Skill Icon",
            Code = "@skill_intrigue_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Learning Skill Icon",
            Code = "@skill_learning_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Prowess Skill Icon",
            Code = "@skill_prowess_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Stress Gain Icon",
            Code = "@stress_gain_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Stress Critical Icon",
            Code = "@stress_critical_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Stress Loss Icon",
            Code = "@stress_loss_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Death Icon",
            Code = "@death_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Scheme Icon",
            Code = "@scheme_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Crime Icon",
            Code = "@crime_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Intimidated Icon",
            Code = "@intimidated_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Terrified Icon",
            Code = "@terrified_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Realm Capital Icon",
            Code = "@realm_capital_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Alliance Icon",
            Code = "@alliance_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Prison Icon",
            Code = "@prison_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Titles Icon",
            Code = "@titles_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Domain Icon",
            Code = "@domain_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Building Icon",
            Code = "@building_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Faction Icon",
            Code = "@faction_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Friend Icon",
            Code = "@friend_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Best Friend Icon",
            Code = "@best_friend_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Rival Icon",
            Code = "@rival_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Nemesis Icon",
            Code = "@nemesis_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Lover Icon",
            Code = "@lover_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Soulmate Icon",
            Code = "@soulmate_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Catholic Icon",
            Code = "@catholic_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Orthodox Icon",
            Code = "@orthodox_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Virtue Icon",
            Code = "@virtue_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Sin Icon",
            Code = "@sin_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
        },
        new()
        {
            Name = "Fervor Icon",
            Code = "@fervor_icon!",
            Icon = PackIconKind.Image,
            Kind = Kind.LocalizationIcon,
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
