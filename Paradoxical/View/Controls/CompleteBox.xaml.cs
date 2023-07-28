﻿using FuzzySharp;
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

    private IEnumerable<Item> Items { get; } = new Item[]
    {
        #region UNIVERSAL SCOPES

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

        #endregion
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
        #region UNIVERSAL SCOPES

        new()
        {
            Name = "Culture → Culture Head",
            Code = "culture_head",
            Tags = new[] { "culture_head" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Culture → Culture Group",
            Code = "culture_group",
            Tags = new[] { "culture_group" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to culture group.",
        },
        new()
        {
            Name = "Landed Title → Title ID",
            Code = "title:%%",
            Offset = 7,
            Tags = new[] { "title id" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Landed Title → barony_controller",
            Code = "barony_controller",
            Tags = new[] { "barony_controller" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Landed Title → County Controller",
            Code = "county_controller",
            Tags = new[] { "county_controller" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Landed Title → Current Heir",
            Code = "current_heir",
            Tags = new[] { "current_heir" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Landed Title → Holder",
            Code = "holder",
            Tags = new[] { "holder" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Landed Title → Lessee",
            Code = "lessee",
            Tags = new[] { "lessee" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Landed Title → Lessee Title",
            Code = "lessee_title",
            Tags = new[] { "lessee_title" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Landed Title → Previous Holder",
            Code = "previous_holder",
            Tags = new[] { "previous_holder" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Landed Title → Culture",
            Code = "culture",
            Tags = new[] { "culture" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to culture.",
        },
        new()
        {
            Name = "Landed Title → Culture Group",
            Code = "culture_group",
            Tags = new[] { "culture_group" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to culture group.",
        },
        new()
        {
            Name = "Landed Title → Controlled Faith",
            Code = "controlled_faith",
            Tags = new[] { "controlled_faith" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to faith.",
        },
        new()
        {
            Name = "Landed Title → Faith",
            Code = "faith",
            Tags = new[] { "faith" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to faith.",
        },
        new()
        {
            Name = "Landed Title → Barony",
            Code = "barony",
            Tags = new[] { "barony" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Landed Title → County",
            Code = "county",
            Tags = new[] { "county" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Landed Title → Duchy",
            Code = "duchy",
            Tags = new[] { "duchy" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Landed Title → kingdom",
            Code = "kingdom",
            Tags = new[] { "kingdom" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Landed Title → Empire",
            Code = "empire",
            Tags = new[] { "empire" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Landed Title → De Facto Liege",
            Code = "de_facto_liege",
            Tags = new[] { "de_facto_liege" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Landed Title → De Jure Liege",
            Code = "de_jure_liege",
            Tags = new[] { "de_jure_liege" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Landed Title → Title Capital County",
            Code = "title_capital_county",
            Tags = new[] { "title_capital_county" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Landed Title → Title Province",
            Code = "title_province",
            Tags = new[] { "title_province" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to province.",
        },
        new()
        {
            Name = "Landed Title → Religion",
            Code = "religion",
            Tags = new[] { "religion" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to religion.",
        },
        new()
        {
            Name = "Dynasty House → House Head",
            Code = "house_head",
            Tags = new[] { "house_head" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to .",
        },
        new()
        {
            Name = "Dynasty → Dynast",
            Code = "dynast",
            Tags = new[] { "dynast" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Secret → Secret Owner",
            Code = "secret_owner",
            Tags = new[] { "secret_owner" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Secret → Secret Target",
            Code = "secret_target",
            Tags = new[] { "secret_target" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Holy Order → Holy Order Patron",
            Code = "holy_order_patron",
            Tags = new[] { "holy_order_patron" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Holy Order → Leader",
            Code = "leader",
            Tags = new[] { "leader" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Holy Order → Title",
            Code = "title",
            Tags = new[] { "title" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Character → Commanding Army",
            Code = "commanding_army",
            Tags = new[] { "commanding_army" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to army.",
        },
        new()
        {
            Name = "Character → Knight Army",
            Code = "knight_army",
            Tags = new[] { "knight_army" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to army.",
        },
        new()
        {
            Name = "Character → Betrothed",
            Code = "betrothed",
            Tags = new[] { "betrothed" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Concubinist",
            Code = "concubinist",
            Tags = new[] { "concubinist" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Court Owner",
            Code = "court_owner",
            Tags = new[] { "court_owner" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Designated Heir",
            Code = "designated_heir",
            Tags = new[] { "designated_heir" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Employer",
            Code = "employer",
            Tags = new[] { "employer" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Father",
            Code = "father",
            Tags = new[] { "father" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → GHW Beneficiary",
            Code = "ghw_beneficiary",
            Tags = new[] { "ghw_beneficiary" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Host",
            Code = "host",
            Tags = new[] { "host" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Imprisoner",
            Code = "imprisoner",
            Tags = new[] { "imprisoner" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Killer",
            Code = "killer",
            Tags = new[] { "killer" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Liege",
            Code = "liege",
            Tags = new[] { "liege" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Liege or Court Owner",
            Code = "liege_or_court_owner",
            Tags = new[] { "liege_or_court_owner" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Matchmaker",
            Code = "matchmaker",
            Tags = new[] { "matchmaker" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Mother",
            Code = "mother",
            Tags = new[] { "mother" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Player Heir",
            Code = "player_heir",
            Tags = new[] { "player_heir" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Pregnancy Assumed Father",
            Code = "pregnancy_assumed_father",
            Tags = new[] { "pregnancy_assumed_father" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Pregnancy Real Father",
            Code = "pregnancy_real_father",
            Tags = new[] { "pregnancy_real_father" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Primary Heir",
            Code = "primary_heir",
            Tags = new[] { "primary_heir" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Primary Partner",
            Code = "primary_partner",
            Tags = new[] { "primary_partner" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Primary Spouse",
            Code = "primary_spouse",
            Tags = new[] { "primary_spouse" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Prisoner",
            Code = "prisoner",
            Tags = new[] { "prisoner" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Real Father",
            Code = "real_father",
            Tags = new[] { "real_father" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Realm Priest",
            Code = "realm_priest",
            Tags = new[] { "realm_priest" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Top Liege",
            Code = "top_liege",
            Tags = new[] { "top_liege" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Character → Council Task",
            Code = "council_task",
            Tags = new[] { "council_task" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to council task.",
        },
        new()
        {
            Name = "Character → Culture",
            Code = "culture",
            Tags = new[] { "culture" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to culture.",
        },
        new()
        {
            Name = "Character → Culture Group",
            Code = "culture_group",
            Tags = new[] { "culture_group" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to culture group.",
        },
        new()
        {
            Name = "Character → Dynasty",
            Code = "dynasty",
            Tags = new[] { "dynasty" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to dynasty.",
        },
        new()
        {
            Name = "Character → House",
            Code = "house",
            Tags = new[] { "house" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to dynasty house.",
        },
        new()
        {
            Name = "Character → Joined Faction",
            Code = "joined_faction",
            Tags = new[] { "joined_faction" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to .",
        },
        new()
        {
            Name = "Character → Faith",
            Code = "faith",
            Tags = new[] { "faith" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to faith.",
        },
        new()
        {
            Name = "Character → Capital Barony",
            Code = "capital_barony",
            Tags = new[] { "capital_barony" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Character → Capital County",
            Code = "capital_county",
            Tags = new[] { "capital_county" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Character → Primary Title",
            Code = "primary_title",
            Tags = new[] { "primary_title" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Character → Capital Province",
            Code = "capital_province",
            Tags = new[] { "capital_province" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to province.",
        },
        new()
        {
            Name = "Character → Location",
            Code = "location",
            Tags = new[] { "location" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to province.",
        },
        new()
        {
            Name = "Character → Religion",
            Code = "religion",
            Tags = new[] { "religion" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to religion.",
        },
        new()
        {
            Name = "Army → Army Commander",
            Code = "army_commander",
            Tags = new[] { "army_commander" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Army → Army Owner",
            Code = "army_owner",
            Tags = new[] { "army_owner" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Army → Location",
            Code = "location",
            Tags = new[] { "location" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to province.",
        },
        new()
        {
            Name = "Province → Barony Controller",
            Code = "barony_controller",
            Tags = new[] { "barony_controller" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Province → County Controller",
            Code = "county_controller",
            Tags = new[] { "county_controller" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Province → Province Owner",
            Code = "province_owner",
            Tags = new[] { "province_owner" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Province → Culture",
            Code = "culture",
            Tags = new[] { "culture" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to culture.",
        },
        new()
        {
            Name = "Province → Culture Group",
            Code = "culture_group",
            Tags = new[] { "culture_group" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to culture group.",
        },
        new()
        {
            Name = "Province → Faith",
            Code = "faith",
            Tags = new[] { "faith" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to faith.",
        },
        new()
        {
            Name = "Province → Barony",
            Code = "barony",
            Tags = new[] { "barony" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Province → County",
            Code = "county",
            Tags = new[] { "county" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Province → Duchy",
            Code = "duchy",
            Tags = new[] { "duchy" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Province → Kingdom",
            Code = "kingdom",
            Tags = new[] { "kingdom" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Province → Empire",
            Code = "empire",
            Tags = new[] { "empire" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Province → Religion",
            Code = "religion",
            Tags = new[] { "religion" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to religion.",
        },
        new()
        {
            Name = "Faith → Religious Head",
            Code = "religious_head",
            Tags = new[] { "religious_head" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Faith → Great Holy War",
            Code = "great_holy_war",
            Tags = new[] { "great_holy_war" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to great holy war.",
        },
        new()
        {
            Name = "Faith → Religious Head Title",
            Code = "religious_head_title",
            Tags = new[] { "religious_head_title" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Faith → Religion",
            Code = "religion",
            Tags = new[] { "religion" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to religion.",
        },
        new()
        {
            Name = "Great Goly War → GHW War",
            Code = "ghw_war",
            Tags = new[] { "ghw_war" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to war.",
        },
        new()
        {
            Name = "Great Goly War → Religion",
            Code = "religion",
            Tags = new[] { "religion" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to religion.",
        },
        new()
        {
            Name = "Great Goly War → GHW Target Title",
            Code = "ghw_target_title",
            Tags = new[] { "ghw_target_title" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Great Goly War → Faith",
            Code = "faith",
            Tags = new[] { "faith" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to faith.",
        },
        new()
        {
            Name = "Great Goly War → GHW Designated Winner",
            Code = "ghw_designated_winner",
            Tags = new[] { "ghw_designated_winner" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Great Goly War → GHW Target Character",
            Code = "ghw_target_character",
            Tags = new[] { "ghw_target_character" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Great Goly War → GHW Title Recipient",
            Code = "ghw_title_recipient",
            Tags = new[] { "ghw_title_recipient" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Great Goly War → GHW War Declarer",
            Code = "ghw_war_declarer",
            Tags = new[] { "ghw_war_declarer" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Combat Side → Side Commander",
            Code = "side_commander",
            Tags = new[] { "side_commander" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Combat Side → Side Primary Participant",
            Code = "side_primary_participant",
            Tags = new[] { "side_primary_participant" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Combat Side → Combat",
            Code = "combat",
            Tags = new[] { "combat" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to combat.",
        },
        new()
        {
            Name = "Combat Side → Enemy Side",
            Code = "enemy_side",
            Tags = new[] { "enemy_side" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to combat side.",
        },
        new()
        {
            Name = "Scheme → Scheme Owner",
            Code = "scheme_owner",
            Tags = new[] { "scheme_owner" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Scheme → Scheme Target",
            Code = "scheme_target",
            Tags = new[] { "scheme_target" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Scheme → Scheme Defender",
            Code = "scheme_defender",
            Tags = new[] { "scheme_defender" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Council Task → Councillor",
            Code = "councillor",
            Tags = new[] { "councillor" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "War → Casus Belli",
            Code = "casus_belli",
            Tags = new[] { "casus_belli" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to casus belli.",
        },
        new()
        {
            Name = "War → Claimant",
            Code = "claimant",
            Tags = new[] { "claimant" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "War → Primary Attacker",
            Code = "primary_attacker",
            Tags = new[] { "primary_attacker" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "War → Primary Defender",
            Code = "primary_defender",
            Tags = new[] { "primary_defender" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Casus belli → Claimant",
            Code = "claimant",
            Tags = new[] { "claimant" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Casus belli → Primary Attacker",
            Code = "primary_attacker",
            Tags = new[] { "primary_attacker" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Casus belli → Primary Defender",
            Code = "primary_defender",
            Tags = new[] { "primary_defender" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Casus belli → War",
            Code = "war",
            Tags = new[] { "war" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to war.",
        },
        new()
        {
            Name = "Combat → Combat Attacker",
            Code = "combat_attacker",
            Tags = new[] { "combat_attacker" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to combat side.",
        },
        new()
        {
            Name = "Combat → Combat Defender",
            Code = "combat_defender",
            Tags = new[] { "combat_defender" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to combat side.",
        },
        new()
        {
            Name = "Combat → Location",
            Code = "location",
            Tags = new[] { "location" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to province.",
        },
        new()
        {
            Name = "Combat → Combat War",
            Code = "combat_war",
            Tags = new[] { "combat_war" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to war.",
        },
        new()
        {
            Name = "Story Cycle → Story Owner",
            Code = "story_owner",
            Tags = new[] { "story_owner" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Faction → Faction Leader",
            Code = "faction_leader",
            Tags = new[] { "faction_leader" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Faction → Faction Target",
            Code = "faction_target",
            Tags = new[] { "faction_target" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Faction → Special Character",
            Code = "special_character",
            Tags = new[] { "special_character" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Faction → Special Title",
            Code = "special_title",
            Tags = new[] { "special_title" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to landed title.",
        },
        new()
        {
            Name = "Faction → Faction War",
            Code = "faction_war",
            Tags = new[] { "faction_war" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to war.",
        },
        new()
        {
            Name = "Activity → Activity Owner",
            Code = "activity_owner",
            Tags = new[] { "activity_owner" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to character.",
        },
        new()
        {
            Name = "Activity → Activity Province",
            Code = "activity_province",
            Tags = new[] { "activity_province" },
            Icon = PackIconKind.ArrowRightBottom,
            Kind = Kind.Scope,
            Tooltip = "Scopes to province.",
        },

        #endregion
        #region CODE SNIPPETS

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
            Name = "AND",
            Code = "AND = {  }",
            Offset = 8,
            Icon = PackIconKind.CodeBraces,
            Kind = Kind.CodeSnippet,
        },
        new()
        {
            Name = "OR",
            Code = "OR = {  }",
            Offset = 7,
            Icon = PackIconKind.CodeBraces,
            Kind = Kind.CodeSnippet,
        },
        new()
        {
            Name = "NAND",
            Code = "NAND = {  }",
            Offset = 9,
            Icon = PackIconKind.CodeBraces,
            Kind = Kind.CodeSnippet,
        },
        new()
        {
            Name = "NOR",
            Code = "NOR = {  }",
            Offset = 8,
            Icon = PackIconKind.CodeBraces,
            Kind = Kind.CodeSnippet,
        },
        new()
        {
            Name = "NOT",
            Code = "NOT = {  }",
            Offset = 8,
            Icon = PackIconKind.CodeBraces,
            Kind = Kind.CodeSnippet,
        },
        new()
        {
            Name = "trigger_if",
            Code = "trigger_if = { limit = { always = yes }  }",
            Offset = 40,
            Icon = PackIconKind.CodeBraces,
            Kind = Kind.CodeSnippet,
            Tooltip = "Evaluates the triggers if the display_triggers of the limit are met.",
        },
        new()
        {
            Name = "trigger_else_if",
            Code = "trigger_else_if = { limit = { always = yes }  }",
            Offset = 45,
            Icon = PackIconKind.CodeBraces,
            Kind = Kind.CodeSnippet,
            Tooltip = "Evaluates the enclosed triggers if the display_triggers of the preceding `trigger_if` or `trigger_else_if` is not met and its own display_trigger of the limit is met.",
        },
        new()
        {
            Name = "trigger_else",
            Code = "trigger_else = {  }",
            Offset = 17,
            Icon = PackIconKind.CodeBraces,
            Kind = Kind.CodeSnippet,
            Tooltip = "Evaluates the triggers if the display_triggers of preceding 'trigger_if' or 'trigger_else_if' is not met.",
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
