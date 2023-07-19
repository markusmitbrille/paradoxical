using Paradoxical.Core;
using Paradoxical.Extensions;
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
    private static partial class Formatter
    {
        [GeneratedRegex(@"(?<!\r\n[\s-[\r\n]]*)(?={)")]
        private static partial Regex GetBlockStartWithoutNewLineBeforePattern();
        private static Regex BlockStartWithoutNewLineBeforePattern => GetBlockStartWithoutNewLineBeforePattern();
        private static string BlockStartWithoutNewLineBeforeReplacement => "\r\n";

        [GeneratedRegex(@"(?<!\r\n[\s-[\r\n]]*)(?=})")]
        private static partial Regex GetBlockEndWithoutNewLineBeforePattern();
        private static Regex BlockEndWithoutNewLineBeforePattern => GetBlockEndWithoutNewLineBeforePattern();
        private static string BlockEndWithoutNewLineBeforeReplacement => "\r\n";

        [GeneratedRegex(@"(?<={)(?![\s-[\r\n]]*\r\n)")]
        private static partial Regex GetBlockStartWithoutNewLineAfterPattern();
        private static Regex BlockStartWithoutNewLineAfterPattern => GetBlockStartWithoutNewLineAfterPattern();
        private static string BlockStartWithoutNewLineAfterReplacement => "\r\n";

        [GeneratedRegex(@"(?<=})(?![\s-[\r\n]]*\r\n)")]
        private static partial Regex GetBlockEndWithoutNewLineAfterPattern();
        private static Regex BlockEndWithoutNewLineAfterPattern => GetBlockEndWithoutNewLineAfterPattern();
        private static string BlockEndWithoutNewLineAfterReplacement => "\r\n";

        [GeneratedRegex(@"(?<!(?: |\?|\+|\-|\*|\\))(?==)")]
        private static partial Regex GetAssignmentWithoutSpaceBeforePattern();
        private static Regex AssignmentWithoutSpaceBeforePattern => GetAssignmentWithoutSpaceBeforePattern();
        private static string EqualsWithoutSpaceBeforeReplacement => " ";

        [GeneratedRegex(@"(?<==)(?! )")]
        private static partial Regex GetAssignmentWithoutSpaceAfterPattern();
        private static Regex AssignmentWithoutSpaceAfterPattern => GetAssignmentWithoutSpaceAfterPattern();
        private static string EqualsWithoutSpaceAfterReplacement => " ";

        [GeneratedRegex(@"(?<!(?:\r\n[\s-[\r\n]]*|[.:]))(?=\b\w+\b(?:[.:]\b\w+\b)*[\s-[\r\n]]*(?:\?|\+|\-|\*|\\)?=)")]
        private static partial Regex GetStatementWithoutNewLineBeforePattern();
        private static Regex StatementWithoutNewLineBeforePattern => GetStatementWithoutNewLineBeforePattern();
        private static string StatementWithoutNewLineBeforeReplacement => "\r\n";

        [GeneratedRegex(@"(?<=\r\n)[\s-[\r\n]]+")]
        private static partial Regex GetWhitespaceAtLineStartPattern();
        private static Regex WhitespaceAtLineStartPattern => GetWhitespaceAtLineStartPattern();
        private static string WhitespaceAtLineStartReplacement => "";

        [GeneratedRegex(@"[\s-[\r\n]]+(?=\r\n)")]
        private static partial Regex GetWhitespaceAtLineEndPattern();
        private static Regex WhitespaceAtLineEndPattern => GetWhitespaceAtLineEndPattern();
        private static string WhitespaceAtLineEndReplacement => "";

        [GeneratedRegex(@"^\s*")]
        private static partial Regex GetWhitespaceAtTextStartPattern();
        private static Regex WhitespaceAtTextStartPattern => GetWhitespaceAtTextStartPattern();
        private static string WhitespaceAtTextStartReplacement => "";

        [GeneratedRegex(@"\s*$")]
        private static partial Regex GetWhitespaceAtTextEndPattern();
        private static Regex WhitespaceAtTextEndPattern => GetWhitespaceAtTextEndPattern();
        private static string WhitespaceAtTextEndReplacement => "\r\n";

        [GeneratedRegex(@"[\s-[\r\n]][\s-[\r\n]]+")]
        private static partial Regex GetRedundantWhitespacePattern();
        private static Regex RedundantWhitespacePattern => GetRedundantWhitespacePattern();
        private static string RedundantWhitespaceReplacement => " ";

        public static void Format(ref string text, ref int index)
        {
            //ReplaceRecursive(text, index, BlockStartWithoutNewLineBeforePattern, BlockStartWithoutNewLineBeforeReplacement);
            ReplaceRecursive(ref text, ref index, BlockEndWithoutNewLineBeforePattern, BlockEndWithoutNewLineBeforeReplacement);

            ReplaceRecursive(ref text, ref index, BlockStartWithoutNewLineAfterPattern, BlockStartWithoutNewLineAfterReplacement);
            ReplaceRecursive(ref text, ref index, BlockEndWithoutNewLineAfterPattern, BlockEndWithoutNewLineAfterReplacement);

            ReplaceRecursive(ref text, ref index, AssignmentWithoutSpaceBeforePattern, EqualsWithoutSpaceBeforeReplacement);
            ReplaceRecursive(ref text, ref index, AssignmentWithoutSpaceAfterPattern, EqualsWithoutSpaceAfterReplacement);

            ReplaceRecursive(ref text, ref index, StatementWithoutNewLineBeforePattern, StatementWithoutNewLineBeforeReplacement);

            ReplaceRecursive(ref text, ref index, WhitespaceAtLineStartPattern, WhitespaceAtLineStartReplacement);
            ReplaceRecursive(ref text, ref index, WhitespaceAtLineEndPattern, WhitespaceAtLineEndReplacement);

            ReplaceRecursive(ref text, ref index, RedundantWhitespacePattern, RedundantWhitespaceReplacement);

            ReplaceFirst(ref text, ref index, WhitespaceAtTextStartPattern, WhitespaceAtTextStartReplacement);
            ReplaceFirst(ref text, ref index, WhitespaceAtTextEndPattern, WhitespaceAtTextEndReplacement);

            //IndentLines(ref text, ref index);
        }

        //[GeneratedRegex(@"{")]
        //private static partial Regex GetBlockStartPattern();
        //public static Regex BlockStartPattern => GetBlockStartPattern();

        //[GeneratedRegex(@"}")]
        //private static partial Regex GetBlockEndPattern();
        //public static Regex BlockEndPattern => GetBlockEndPattern();

        //[GeneratedRegex(@"^\s*}\s*")]
        //private static partial Regex GetStartsWithBlockEndRegex();
        //public static Regex StartsWithBlockEndRegex => GetStartsWithBlockEndRegex();

        //private static void IndentLines(ref string text, ref int index)
        //{
        //    int level = 0;
        //    int linestart = 0;

        //    string[] lines = text.Split(Environment.NewLine);
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        string line = lines[i];

        //        int indents = level;
        //        indents = StartsWithBlockEndRegex.IsMatch(line) == true ? indents - 1 : indents;
        //        indents = Math.Clamp(indents, 0, 16);

        //        string indent = string.Concat(Enumerable.Repeat(ParadoxText.Indentation, indents));

        //        line = indent + line;
        //        lines[i] = line;

        //        if (index >= linestart)
        //        {
        //            index += indent.Length;
        //        }

        //        level += BlockStartPattern.Matches(line).Count;
        //        level -= BlockEndPattern.Matches(line).Count;
        //        linestart += line.Length;
        //    }

        //    text = string.Join(Environment.NewLine, lines);
        //}

        private static void ReplaceFirst(ref string text, ref int index, Regex pattern, string replacement)
        {
            Match match = pattern.Match(text);

            if (match.Success == false)
            { return; }

            string originalText = match.Value;
            string replacedText = pattern.Replace(originalText, replacement);

            text = text.Remove(match.Index, match.Length);
            text = text.Insert(match.Index, replacedText);

            if (index >= match.Index + match.Length)
            {
                index = index - originalText.Length + replacedText.Length;
            }
        }

        private static void ReplaceRecursive(ref string text, ref int index, Regex pattern, string replacement)
        {
            for (Match match = pattern.Match(text); match.Success == true; match = pattern.Match(text))
            {
                string original = match.Value;

                text = text.Remove(match.Index, match.Length);
                text = text.Insert(match.Index, replacement);

                if (index >= match.Index + match.Length)
                {
                    index = index - original.Length + replacement.Length;
                }
            }
        }
    }

    public bool AllowFormatting
    {
        get { return (bool)GetValue(AllowFormattingProperty); }
        set { SetValue(AllowFormattingProperty, value); }
    }

    public static readonly DependencyProperty AllowFormattingProperty =
        DependencyProperty.Register("AllowFormatting", typeof(bool), typeof(ScriptBox), new PropertyMetadata(false));

    [GeneratedRegex(@"\w+")]
    private static partial Regex GetWordRegex();
    public static Regex WordRegex => GetWordRegex();

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
        var text = Text;
        var index = CaretIndex;

        Formatter.Format(ref text, ref index);

        Text = text;
        CaretIndex = index;
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
            else if (AcceptsTab == true)
            {
                InsertIndentation();
                e.Handled = true;
            }
        }

        if (e.Key == Key.Enter)
        {
            if (ScriptBoxCommands.ConfirmComplete.CanExecute(null, this) == true)
            {
                ScriptBoxCommands.ConfirmComplete.Execute(null, this);
                e.Handled = true;
            }
            else if (AcceptsReturn)
            {
                InsertNewLine();
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
        if (e.Text == "\r\n")
        {
            InsertNewLine();
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

    private void InsertNewLine()
    {
        var text = Text;
        var index = CaretIndex;

        text = text.Insert(index, Environment.NewLine);
        index += Environment.NewLine.Length;

        Formatter.Format(ref text, ref index);

        Text = text;
        CaretIndex = index;
    }

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

    private const int POPUP_OFFSET = 5;

    private Point GetCharPosition(int index)
    {
        var boxPos = TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
        var caretRect = GetRectFromCharacterIndex(index);
        var size = FontSize;

        return new() { X = boxPos.X + caretRect.X, Y = boxPos.Y + caretRect.Y + size + POPUP_OFFSET };
    }
}