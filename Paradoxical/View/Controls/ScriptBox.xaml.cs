using Paradoxical.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Paradoxical.View;

/// <summary>
/// Interaction logic for ScriptBox.xaml
/// </summary>
public partial class ScriptBox : TextBox
{
    public bool AllowFormatting
    {
        get { return (bool)GetValue(AllowFormattingProperty); }
        set { SetValue(AllowFormattingProperty, value); }
    }

    public static readonly DependencyProperty AllowFormattingProperty =
        DependencyProperty.Register("AllowFormatting", typeof(bool), typeof(ScriptBox), new PropertyMetadata(false));

    private MatchCollection WordMatches { get; set; } = WordRegex.Matches(string.Empty);
    private Match? CurrentWord { get; set; } = null;

    private CompleteBox? Popup { get; set; }
    private int? PopupIndex { get; set; }

    public ScriptBox()
    {
        InitializeComponent();
    }

    private void OpenCompleteExecutedHandler(object sender, ExecutedRoutedEventArgs e)
    {
        OpenPopup();
    }

    private void OpenCompleteCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    private void ConfirmCompleteExecutedHandler(object sender, ExecutedRoutedEventArgs e)
    {
        if (Popup == null)
        { return; }

        Popup.Result = true;
        ClosePopup();
    }

    private void ConfirmCompleteCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
    {
        if (Popup == null)
        {
            e.CanExecute = false;
            return;
        }

        if (Popup.Selected == null)
        {
            e.CanExecute = false;
            return;
        }

        e.CanExecute = true;
    }

    private void CancelCompleteExecutedHandler(object sender, ExecutedRoutedEventArgs e)
    {
        if (Popup == null)
        { return; }

        Popup.Result = false;
        ClosePopup();
    }

    private void CancelCompleteCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
    {
        if (Popup == null)
        {
            e.CanExecute = false;
            return;
        }

        e.CanExecute = true;
    }

    private void FormatTextExecutedHandler(object sender, ExecutedRoutedEventArgs e)
    {
        FormatText();
    }

    private void FormatTextCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = AllowFormatting;
    }

    private void TextChangedHandler(object sender, TextChangedEventArgs e)
    {
        UpdateWordMatches();
        UpdateCurrentWord();

        UpdatePopup();
    }

    private void SelectionChangedHandler(object sender, RoutedEventArgs e)
    {
        UpdateCurrentWord();
        UpdatePopup();
    }

    private void LostFocusHandler(object sender, RoutedEventArgs e)
    {
        if (Popup == null)
        { return; }

        Popup.Result = false;
        ClosePopup();
    }

    private void LostKeyboardFocusHandler(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (Popup == null)
        { return; }

        Popup.Result = false;
        ClosePopup();
    }

    private void KeyDownHandler(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Up || e.Key == Key.Down)
        {
            RaisePopupEvent(sender, e);
        }

        if (e.Key == Key.Tab)
        {
            if (ScriptBoxCommands.ConfirmComplete.CanExecute(null, this) == true)
            {
                ScriptBoxCommands.ConfirmComplete.Execute(null, this);
                e.Handled = true;
            }
            else
            {
                InsertIndentation();
                e.Handled = true;
            }
        }
    }

    private void RaisePopupEvent(object sender, KeyEventArgs e)
    {
        if (Popup == null)
        { return; }

        var target = Popup;
        var keyboard = Keyboard.PrimaryDevice;
        var inputSource = PresentationSource.FromVisual(target);
        var timestamp = 0;
        var key = e.Key;

        var routedEvent = Keyboard.KeyDownEvent;

        var args = new KeyEventArgs(keyboard, inputSource, timestamp, key)
        {
            RoutedEvent = routedEvent
        };

        Popup.RaiseEvent(args);
        e.Handled = true;
    }

    private void PreviewTextInputHandler(object sender, TextCompositionEventArgs e)
    {
        if (e.Text == "\"")
        {
            InsertQuotationMarks();
            e.Handled = true;
        }
        if (e.Text == "(")
        {
            InsertBraces();
            e.Handled = true;
        }
        if (e.Text == "[")
        {
            InsertSquareBraces();
            e.Handled = true;
        }
        if (e.Text == "{")
        {
            InsertCurlyBraces();
            e.Handled = true;
        }
        if (e.Text == "\t")
        {
            InsertIndentation();
            e.Handled = true;
        }
    }

    private void InsertQuotationMarks()
    {
        var index = CaretIndex;
        var text = Text;

        text = text.Insert(CaretIndex, "\"\"");

        Text = text;
        CaretIndex = index + 1;
    }

    private void InsertBraces()
    {
        var index = CaretIndex;
        var text = Text;

        text = text.Insert(CaretIndex, "()");

        Text = text;
        CaretIndex = index + 1;
    }

    private void InsertSquareBraces()
    {
        var index = CaretIndex;
        var text = Text;

        text = text.Insert(CaretIndex, "[]");

        Text = text;
        CaretIndex = index + 1;
    }

    private void InsertCurlyBraces()
    {
        var index = CaretIndex;
        var text = Text;

        text = text.Insert(CaretIndex, "{  }");

        Text = text;
        CaretIndex = index + 2;
    }

    private void InsertIndentation()
    {
        var index = CaretIndex;
        var text = Text;

        text = text.Insert(CaretIndex, ParadoxText.Indentation);

        Text = text;
        CaretIndex = index + 4;
    }

    [GeneratedRegex(@"\w+")]
    private static partial Regex GetWordRegex();
    public static Regex WordRegex => GetWordRegex();

    private void UpdateWordMatches()
    {
        WordMatches = WordRegex.Matches(Text);
    }

    private void UpdateCurrentWord()
    {
        CurrentWord = WordMatches.SingleOrDefault(match => CaretIndex >= match.Index && CaretIndex <= match.Index + match.Length);
    }

    private void OpenPopup()
    {
        if (Popup != null)
        {
            ClosePopup();
        }

        var index = GetPopupIndex();
        var position = GetCharPosition(index);

        PopupIndex = index;

        Window window = Window.GetWindow(this);
        Popup = new()
        {
            Owner = window,
            Left = position.X,
            Top = position.Y,
            Filter = CurrentWord?.Value,
        };

        Popup.Closed += PopupClosedHandler;

        Popup.UpdateScores();
        Popup.UpdateView();
        Popup.UpdateSelection();
        Popup.Show();

        // refocus
        Focus();

        LostFocus += LostFocusHandler;
        LostKeyboardFocus += LostKeyboardFocusHandler;
    }

    private void ApplyPopup()
    {
        if (Popup == null)
        { return; }

        if (Popup.Selected == null)
        { return; }

        string text = Text;

        int index = CurrentWord?.Index ?? CaretIndex;
        string code = Popup.Selected.Code;

        if (CurrentWord != null)
        {
            text = text.Remove(index, CurrentWord.Length);
        }

        text = text.Insert(index, code);

        Text = text;
        CaretIndex = index + code.Length;
    }

    private void ClosePopup()
    {
        if (Popup == null)
        { return; }

        Popup.Close();
    }

    private void PopupClosedHandler(object? sender, EventArgs e)
    {
        if (Popup == null)
        { return; }

        if (Popup.Result == true)
        {
            ApplyPopup();
        }

        Popup = null;
        PopupIndex = null;

        LostFocus -= LostFocusHandler;
        LostKeyboardFocus -= LostKeyboardFocusHandler;
    }

    private void UpdatePopup()
    {
        if (Popup == null)
        { return; }

        ValidatePopupPlacement();

        if (Popup == null)
        { return; }

        Popup.Filter = CurrentWord?.Value;

        Popup.UpdateScores();
        Popup.UpdateView();
        Popup.UpdateSelection();
    }

    private void ValidatePopupPlacement()
    {
        if (Popup == null)
        { return; }

        if (PopupIndex == null)
        { return; }

        var index = GetPopupIndex();
        if (index != PopupIndex)
        {
            ClosePopup();
        }
    }

    private int GetPopupIndex()
    {
        if (CurrentWord != null)
        {
            return CurrentWord.Index;
        }

        return CaretIndex;
    }

    private const int OFFSET = 5;

    private Point GetCharPosition(int index)
    {
        var boxPos = TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
        var caretRect = GetRectFromCharacterIndex(index);
        var size = FontSize;

        return new() { X = boxPos.X + caretRect.X, Y = boxPos.Y + caretRect.Y + size + OFFSET };
    }

    private void FormatText()
    {
        FormatNewLines();
        FormatSpaces();
        FormatIndent();
    }

    [GeneratedRegex(@"\s*{\s*")]
    private static partial Regex GetBlockStartWhitespaceRegex();
    public static Regex BlockStartWhitespaceRegex => GetBlockStartWhitespaceRegex();

    [GeneratedRegex(@"\s*}\s*")]
    private static partial Regex GetBlockEndWhitespaceRegex();
    public static Regex BlockEndWhitespaceRegex => GetBlockEndWhitespaceRegex();

    [GeneratedRegex(@"\s*(?<statement>\w+(?:\.\w+)*\s*=)")]
    private static partial Regex GetStatementWhitespaceRegex();
    public static Regex StatementWhitespaceRegex => GetStatementWhitespaceRegex();

    private void FormatNewLines()
    {
        string text = Text;

        text = BlockStartWhitespaceRegex.Replace(text, " {\r\n");
        text = BlockEndWhitespaceRegex.Replace(text, "\r\n}");
        text = StatementWhitespaceRegex.Replace(text, "\r\n${statement}");

        text = text.Trim();
        text = text + "\r\n";

        Text = text;
    }

    [GeneratedRegex(@"{")]
    private static partial Regex GetBlockStartRegex();
    public static Regex BlockStartRegex => GetBlockStartRegex();

    [GeneratedRegex(@"}")]
    private static partial Regex GetBlockEndRegex();
    public static Regex BlockEndRegex => GetBlockEndRegex();

    private void FormatIndent()
    {
        string text = Text;

        int indentLevel = 0;

        string[] lines = text.Split(Environment.NewLine);
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            indentLevel -= BlockEndRegex.Matches(line).Count;

            line = line.Trim();
            line = string.Concat(Enumerable.Repeat(ParadoxText.Indentation, indentLevel)) + line;

            indentLevel += BlockStartRegex.Matches(line).Count;

            lines[i] = line;
        }

        text = string.Join(Environment.NewLine, lines);
        Text = text;
    }

    [GeneratedRegex(@"  +")]
    private static partial Regex GetWhitespaceRegex();
    public static Regex WhitespaceRegex => GetWhitespaceRegex();

    [GeneratedRegex(@" ?= ?")]
    private static partial Regex GetEqualsRegex();
    public static Regex EqualsRegex => GetEqualsRegex();

    [GeneratedRegex(@"\s*\r\n\s*(?:\r\n\s*)*")]
    private static partial Regex GetLineBreakRegex();
    public static Regex LineBreakRegex => GetLineBreakRegex();

    private void FormatSpaces()
    {
        var text = Text;

        text = WhitespaceRegex.Replace(text, " ");
        text = EqualsRegex.Replace(text, " = ");
        text = LineBreakRegex.Replace(text, "\r\n");

        Text = text;
    }
}