using FuzzySharp;
using MaterialDesignThemes.Wpf;
using Paradoxical.Core;
using Paradoxical.Extensions;
using Paradoxical.Info;
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
        Trigger =
            0b0000000000000100,
        Effect =
            0b0000000000001000,

        LocalizationScope =
            0b0000000000010000,
        Command =
            0b0000000000100000,
        Argument =
            0b0000000001000000,
        Style =
            0b0000000010000000,
        Icon =
            0b0000000100000000,

        Theme =
            0b0000001000000000,

        Animation =
            0b0000010000000000,

        Outfit =
            0b0000100000000000,

        Picture =
            0b0001000000000000,

        Code =
            Scope
            | CodeScope
            | Trigger
            | Effect,

        Localization =
            Scope
            | LocalizationScope
            | Argument
            | Style
            | Icon
            | Command,

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
        #region ARGUMENTS

        new()
        {
            Name = "Uppercase Argument",
            Code = "|U",
            Offset = 0,
            Tags = new[] { "argumentupper", "argumentupper" },
            Icon = PackIconKind.FormatPaint,
            Kind = Kind.Argument,
        },
        new()
        {
            Name = "Lowercase Argument",
            Code = "|L",
            Offset = 0,
            Tags = new[] { "argumentlower", "lowerargument" },
            Icon = PackIconKind.FormatPaint,
            Kind = Kind.Argument,
        },
        new()
        {
            Name = "Positive Argument",
            Code = "|P",
            Offset = 0,
            Tags = new[] { "argumentpos", "posargument" },
            Icon = PackIconKind.FormatPaint,
            Kind = Kind.Argument,
        },
        new()
        {
            Name = "Negative Argument",
            Code = "|N",
            Offset = 0,
            Tags = new[] { "argumentneg", "negargument" },
            Icon = PackIconKind.FormatPaint,
            Kind = Kind.Argument,
        },
        new()
        {
            Name = "Game Concept Argument",
            Code = "|E",
            Offset = 0,
            Tags = new[] { "argumentgameconcept", "gameconceptargument" },
            Icon = PackIconKind.FormatPaint,
            Kind = Kind.Argument,
        },
        new()
        {
            Name = "White Argument",
            Code = "|V",
            Offset = 0,
            Tags = new[] { "argument", "white" },
            Icon = PackIconKind.FormatPaint,
            Kind = Kind.Argument,
        },

        #endregion
        #region STYLES

        new()
        {
            Name = "Positive Style",
            Code = "#P  #!",
            Offset = 3,
            Tags = new[] { "stylepos", "posstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.Style,
        },
        new()
        {
            Name = "Negative Style",
            Code = "#N  #!",
            Offset = 3,
            Tags = new[] { "styleneg", "negstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.Style,
        },
        new()
        {
            Name = "Help Style",
            Code = "#help  #!",
            Offset = 6,
            Tags = new[] { "stylehelp", "helpstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.Style,
        },
        new()
        {
            Name = "Informational Style",
            Code = "#I  #!",
            Offset = 3,
            Tags = new[] { "styleinfo", "infostyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.Style,
        },
        new()
        {
            Name = "Warning Style",
            Code = "#warning  #!",
            Offset = 3,
            Tags = new[] { "stylewarning", "warningstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.Style,
        },
        new()
        {
            Name = "Title Style",
            Code = "#T  #!",
            Offset = 3,
            Tags = new[] { "styletitle", "titlestyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.Style,
        },
        new()
        {
            Name = "Game Concept Style",
            Code = "#E  #!",
            Offset = 3,
            Tags = new[] { "stylegameconcept", "gameconceptstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.Style,
        },
        new()
        {
            Name = "Italic Warning Style",
            Code = "#X  #!",
            Offset = 3,
            Tags = new[] { "styleitalicwarning", "italicwarningstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.Style,
        },
        new()
        {
            Name = "Bold and Italic Style",
            Code = "#S  #!",
            Offset = 3,
            Tags = new[] { "stylebolditalic", "bolditalicstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.Style,
        },
        new()
        {
            Name = "White Style",
            Code = "#V  #!",
            Offset = 3,
            Tags = new[] { "stylewhite", "whitestyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.Style,
        },
        new()
        {
            Name = "Emphasized Style",
            Code = "#EMP  #!",
            Offset = 5,
            Tags = new[] { "styleemph", "emphstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.Style,
        },
        new()
        {
            Name = "Weak Style",
            Code = "#weak  #!",
            Offset = 6,
            Tags = new[] { "styleweak", "weakstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.Style,
        },
        new()
        {
            Name = "Bold Style",
            Code = "#bold  #!",
            Offset = 6,
            Tags = new[] { "stylebold", "boldstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.Style,
        },
        new()
        {
            Name = "Italic Style",
            Code = "#italic  #!",
            Offset = 8,
            Tags = new[] { "styleitalic", "italicstyle" },
            Icon = PackIconKind.Style,
            Kind = Kind.Style,
        },

        #endregion
        #region ICONS

        new()
        {
            Name = "Icon",
            Code = "@!",
            Offset = 1,
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Warning Icon",
            Code = "@warning_icon!",
            Tags = new[] { "iconwarning", "warningicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Gold Icon",
            Code = "@gold_icon!",
            Tags = new[] { "icon", "gold" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Prestige Icon",
            Code = "@prestige_icon!",
            Tags = new[] { "iconprestige", "prestigeicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Piety Icon",
            Code = "@piety_icon!",
            Tags = new[] { "iconpiety", "pietyicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Christian Piety Icon",
            Code = "@piety_icon_christian!",
            Tags = new[] { "iconpietychristianity", "pietychristianityicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Islam Piety Icon",
            Code = "@piety_icon_islam!",
            Tags = new[] { "iconpietyislam", "pietyislamicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Pagan Piety Icon",
            Code = "@piety_icon_pagan!",
            Tags = new[] { "iconpietypagan", "pietypaganicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Eastern Piety Icon",
            Code = "@piety_icon_eastern!",
            Tags = new[] { "iconpietyeastern", "pietyeasternicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Judaism Piety Icon",
            Code = "@piety_icon_judaism!",
            Tags = new[] { "iconpietyjudaism", "pietyjudaismicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Zoroastrian Piety Icon",
            Code = "@piety_icon_zoroastrian!",
            Tags = new[] { "iconpietyzoroastrian", "pietyzoroastrianicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Time Icon",
            Code = "@time_icon!",
            Tags = new[] { "icontime", "timeicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Cross Icon",
            Code = "@cross_icon!",
            Tags = new[] { "iconcross", "crossicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Stress Icon",
            Code = "@stress_icon!",
            Tags = new[] { "iconstress", "stressicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Dread Icon",
            Code = "@dread_icon!",
            Tags = new[] { "icondread", "dreadicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Exposed Icon",
            Code = "@exposed_icon!",
            Tags = new[] { "iconexposed", "exposedicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Diplomacy Skill Icon",
            Code = "@skill_diplomacy_icon!",
            Tags = new[] { "iconskilldiplomacy", "skilldiplomacyicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Martial Skill Icon",
            Code = "@skill_martial_icon!",
            Tags = new[] { "iconskillmartial", "skillmartialicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Stewardship Skill Icon",
            Code = "@skill_stewardship_icon!",
            Tags = new[] { "iconskillstewardship", "skillstewardshipicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Intrigue Skill Icon",
            Code = "@skill_intrigue_icon!",
            Tags = new[] { "iconskillintrigue", "skillintrigueicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Learning Skill Icon",
            Code = "@skill_learning_icon!",
            Tags = new[] { "iconskilllearning", "skilllearningicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Prowess Skill Icon",
            Code = "@skill_prowess_icon!",
            Tags = new[] { "iconskillprowess", "skillprowessicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Stress Gain Icon",
            Code = "@stress_gain_icon!",
            Tags = new[] { "iconstressgain", "stressgainicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Stress Critical Icon",
            Code = "@stress_critical_icon!",
            Tags = new[] { "iconstresscritical", "stresscriticalicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Stress Loss Icon",
            Code = "@stress_loss_icon!",
            Tags = new[] { "iconstressloss", "stresslossicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Death Icon",
            Code = "@death_icon!",
            Tags = new[] { "icondeath", "deathicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Scheme Icon",
            Code = "@scheme_icon!",
            Tags = new[] { "iconscheme", "schemeicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Crime Icon",
            Code = "@crime_icon!",
            Tags = new[] { "iconcrime", "crimeicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Intimidated Icon",
            Code = "@intimidated_icon!",
            Tags = new[] { "iconintimidated", "intimidatedicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Terrified Icon",
            Code = "@terrified_icon!",
            Tags = new[] { "iconterrified", "terrifiedicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Realm Capital Icon",
            Code = "@realm_capital_icon!",
            Tags = new[] { "iconrealmcapital", "realmcapitalicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Alliance Icon",
            Code = "@alliance_icon!",
            Tags = new[] { "iconalliance", "allianceicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Prison Icon",
            Code = "@prison_icon!",
            Tags = new[] { "iconprison", "prisonicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Titles Icon",
            Code = "@titles_icon!",
            Tags = new[] { "icontitles", "titlesicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Domain Icon",
            Code = "@domain_icon!",
            Tags = new[] { "icondomain", "domainicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Building Icon",
            Code = "@building_icon!",
            Tags = new[] { "iconbuilding", "buildingicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Faction Icon",
            Code = "@faction_icon!",
            Tags = new[] { "iconfaction", "factionicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Friend Icon",
            Code = "@friend_icon!",
            Tags = new[] { "iconfriend", "friendicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Best Friend Icon",
            Code = "@best_friend_icon!",
            Tags = new[] { "iconbestfriend", "bestfriendicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Rival Icon",
            Code = "@rival_icon!",
            Tags = new[] { "iconrival", "rivalicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Nemesis Icon",
            Code = "@nemesis_icon!",
            Tags = new[] { "iconnemesis", "nemesisicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Lover Icon",
            Code = "@lover_icon!",
            Tags = new[] { "iconlover", "lovericon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Soulmate Icon",
            Code = "@soulmate_icon!",
            Tags = new[] { "iconsoulmate", "soulmateicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Catholic Icon",
            Code = "@catholic_icon!",
            Tags = new[] { "iconcatholic", "catholicicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Orthodox Icon",
            Code = "@orthodox_icon!",
            Tags = new[] { "iconorthodox", "orthodoxicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Virtue Icon",
            Code = "@virtue_icon!",
            Tags = new[] { "iconvirtue", "virtueicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Sin Icon",
            Code = "@sin_icon!",
            Tags = new[] { "iconsin", "sinicon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
        },
        new()
        {
            Name = "Fervor Icon",
            Code = "@fervor_icon!",
            Tags = new[] { "iconfervor", "fervoricon" },
            Icon = PackIconKind.Image,
            Kind = Kind.Icon,
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
        foreach (var info in triggers)
        {
            Suggestions.Add(new()
            {
                Name = info.Name,
                Code = info.Name,
                Tooltip = info.Tooltip,
                Icon = PackIconKind.CodeBraces,
                Kind = Kind.Trigger,
            });
        }

        var effects = EffectInfo.ParseLog();
        foreach (var info in effects)
        {
            Suggestions.Add(new()
            {
                Name = info.Name,
                Code = info.Name,
                Tooltip = info.Tooltip,
                Icon = PackIconKind.CodeBraces,
                Kind = Kind.Effect,
            });
        }

        var scopes = ScopeInfo.ParseLog();
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

        var commands = CommandInfo.ParseText();
        foreach (var info in commands)
        {
            Suggestions.Add(new()
            {
                Name = info.Name,
                Code = info.Name,
                Icon = PackIconKind.Function,
                Kind = Kind.Command,
            });
        }

        var animations = AnimationInfo.ParseText();
        foreach (var info in animations)
        {
            Suggestions.Add(new()
            {
                Name = info.Name,
                Code = info.Name,
                Icon = PackIconKind.HandWave,
                Kind = Kind.Animation,
            });
        }

        var outfits = OutfitInfo.ParseText();
        foreach (var info in outfits)
        {
            Suggestions.Add(new()
            {
                Name = info.Name,
                Code = info.Name,
                Icon = PackIconKind.Hanger,
                Kind = Kind.Outfit,
            });
        }

        var pictures = PictureInfo.ParseText();
        foreach (var info in pictures)
        {
            Suggestions.Add(new()
            {
                Name = info.Name,
                Code = info.Name,
                Icon = PackIconKind.FileImage,
                Kind = Kind.Picture,
            });
        }

        var themes = ThemeInfo.ParseText();
        foreach (var info in themes)
        {
            Suggestions.Add(new()
            {
                Name = info.Name,
                Code = info.Name,
                Icon = PackIconKind.Landscape,
                Kind = Kind.Theme,
            });
        }
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

        if (item.Score > 10)
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
