using Paradoxical.Core;
using Paradoxical.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
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

        [GeneratedRegex(@"(?<!(?: |\?|\<|\>))(?==)")]
        private static partial Regex GetEqualsSignWithoutSpaceBeforePattern();
        private static Regex EqualsSignWithoutSpaceBeforePattern => GetEqualsSignWithoutSpaceBeforePattern();
        private static string EqualsSignWithoutSpaceBeforeReplacement => " ";

        [GeneratedRegex(@"(?<==)(?! )")]
        private static partial Regex GetEqualsSignWithoutSpaceAfterPattern();
        private static Regex EqualsSignWithoutSpaceAfterPattern => GetEqualsSignWithoutSpaceAfterPattern();
        private static string EqualsSignWithoutSpaceAfterReplacement => " ";

        [GeneratedRegex(@"(?<! )(?=(?:\<|\>))")]
        private static partial Regex GetComparisonWithoutSpaceBeforePattern();
        private static Regex ComparisonWithoutSpaceBeforePattern => GetComparisonWithoutSpaceBeforePattern();
        private static string ComparisonWithoutSpaceBeforeReplacement => " ";

        [GeneratedRegex(@"(?<=(?:\<|\>))(?!(?: |\=))")]
        private static partial Regex GetComparisonWithoutSpaceAfterPattern();
        private static Regex ComparisonWithoutSpaceAfterPattern => GetComparisonWithoutSpaceAfterPattern();
        private static string ComparisonWithoutSpaceAfterReplacement => " ";

        [GeneratedRegex(@"(?<!(?: ))(?=\?)")]
        private static partial Regex GetQuestionMarkWithoutSpaceBeforePattern();
        private static Regex QuestionMarkWithoutSpaceBeforePattern => GetQuestionMarkWithoutSpaceBeforePattern();
        private static string QuestionMarkWithoutSpaceBeforeReplacement => " ";

        [GeneratedRegex(@"(?<!(?:\r\n[\s-[\r\n]]*|[.:]))(?=\b\w+\b(?:[.:]\b\w+\b)*[\s-[\r\n]]*(?:\=|\?\=|\<|\>|\<\=|\>\=))")]
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

            ReplaceRecursive(ref text, ref index, EqualsSignWithoutSpaceBeforePattern, EqualsSignWithoutSpaceBeforeReplacement);
            ReplaceRecursive(ref text, ref index, EqualsSignWithoutSpaceAfterPattern, EqualsSignWithoutSpaceAfterReplacement);

            ReplaceRecursive(ref text, ref index, ComparisonWithoutSpaceBeforePattern, ComparisonWithoutSpaceBeforeReplacement);
            ReplaceRecursive(ref text, ref index, ComparisonWithoutSpaceAfterPattern, ComparisonWithoutSpaceAfterReplacement);

            ReplaceRecursive(ref text, ref index, QuestionMarkWithoutSpaceBeforePattern, QuestionMarkWithoutSpaceBeforeReplacement);

            ReplaceRecursive(ref text, ref index, StatementWithoutNewLineBeforePattern, StatementWithoutNewLineBeforeReplacement);

            ReplaceRecursive(ref text, ref index, WhitespaceAtLineStartPattern, WhitespaceAtLineStartReplacement);
            ReplaceRecursive(ref text, ref index, WhitespaceAtLineEndPattern, WhitespaceAtLineEndReplacement);

            ReplaceRecursive(ref text, ref index, RedundantWhitespacePattern, RedundantWhitespaceReplacement);

            ReplaceFirst(ref text, ref index, WhitespaceAtTextStartPattern, WhitespaceAtTextStartReplacement);
            ReplaceFirst(ref text, ref index, WhitespaceAtTextEndPattern, WhitespaceAtTextEndReplacement);

            IndentLines(ref text, ref index);
        }

        [GeneratedRegex(@"{")]
        private static partial Regex GetBlockStartPattern();
        public static Regex BlockStartPattern => GetBlockStartPattern();

        [GeneratedRegex(@"}")]
        private static partial Regex GetBlockEndPattern();
        public static Regex BlockEndPattern => GetBlockEndPattern();

        [GeneratedRegex(@"^}")]
        private static partial Regex GetStartsWithBlockEndPattern();
        public static Regex StartsWithBlockEndPattern => GetStartsWithBlockEndPattern();

        [GeneratedRegex(@"^\r\n")]
        private static partial Regex GetStartsWithNewLinePattern();
        public static Regex StartsWithNewLinePattern => GetStartsWithNewLinePattern();

        private static void IndentLines(ref string text, ref int index)
        {
            int current = 0;
            while (current >= 0 && current < text.Length)
            {
                string before = text[..current];
                string after = text[current..];

                int level = 0;

                level += BlockStartPattern.Matches(before).Count;
                level -= BlockEndPattern.Matches(before).Count;

                // line starts with block end
                if (StartsWithBlockEndPattern.IsMatch(after) == true)
                {
                    // outdent by one level
                    level -= 1;
                }

                // non-current line is empty
                if (StartsWithNewLinePattern.IsMatch(after) == true && index != current)
                {
                    // do not indent
                    level = 0;
                }

                level = Math.Clamp(level, 0, 16);

                string indent = string.Concat(Enumerable.Repeat(ParadoxText.Indentation, level));

                text = text.Insert(current, indent);

                // index on or after line start index
                if (index >= current)
                {
                    // move index by indentation length
                    index += indent.Length;
                }

                current = text.IndexOf(Environment.NewLine, current);

                if (current == -1)
                { break; }

                current += Environment.NewLine.Length;
            }
        }

        private static void ReplaceFirst(ref string text, ref int index, Regex pattern, string replacement)
        {
            Match match = pattern.Match(text);

            if (match.Success == false)
            { return; }

            ReplaceMatch(ref text, ref index, match, replacement);
        }

        private static void ReplaceRecursive(ref string text, ref int index, Regex pattern, string replacement)
        {
            for (Match match = pattern.Match(text); match.Success == true; match = pattern.Match(text))
            {
                ReplaceMatch(ref text, ref index, match, replacement);
            }
        }

        private static void ReplaceMatch(ref string text, ref int index, Match match, string replacement)
        {
            string original = match.Value;

            text = text.Remove(match.Index, match.Length);
            text = text.Insert(match.Index, replacement);

            // index after match
            if (index >= match.Index + match.Length)
            {
                // move index by length delta
                index = index - original.Length + replacement.Length;
            }

            // index in match
            else if (index >= match.Index && index < match.Index + match.Length)
            {
                // keep index within bounds
                index = Math.Clamp(index, match.Index, match.Index + replacement.Length);
            }

            // index before match
            else if (index < match.Index)
            {
                // index remains the same
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

    public bool AcceptsDot
    {
        get { return (bool)GetValue(AcceptsDotProperty); }
        set { SetValue(AcceptsDotProperty, value); }
    }

    public static readonly DependencyProperty AcceptsDotProperty =
        DependencyProperty.Register("AcceptsDot", typeof(bool), typeof(ScriptBox), new PropertyMetadata(false));

    public CompleteBox.Kind AllowedCompleteItems
    {
        get { return (CompleteBox.Kind)GetValue(AllowedCompleteItemsProperty); }
        set { SetValue(AllowedCompleteItemsProperty, value); }
    }

    public static readonly DependencyProperty AllowedCompleteItemsProperty =
        DependencyProperty.Register("AllowedCompleteItems", typeof(CompleteBox.Kind), typeof(ScriptBox), new PropertyMetadata(CompleteBox.Kind.All));

    [GeneratedRegex(@"\w+")]
    private static partial Regex GetWordRegex();
    public static Regex WordRegex => GetWordRegex();

    [GeneratedRegex(@"[\s-[\r\n]]+")]
    private static partial Regex GetWhiteSpaceRegex();
    public static Regex WhiteSpaceRegex => GetWhiteSpaceRegex();

    [GeneratedRegex(@"\r\n")]
    private static partial Regex GetNewLineRegex();
    public static Regex NewLineRegex => GetNewLineRegex();

    [GeneratedRegex(@"(\?\=|\!\=|\+\=|\-\=|\*\=|\\\=|\<\=|\>\=)")]
    private static partial Regex GetCompoundSymbolRegex();
    public static Regex CompoundSymbolRegex => GetCompoundSymbolRegex();

    [GeneratedRegex(@"[^\s\w]")]
    private static partial Regex GetSymbolRegex();
    public static Regex SymbolRegex => GetSymbolRegex();

    [GeneratedRegex(@"^.*$", RegexOptions.Multiline)]
    private static partial Regex GetLineRegex();
    public static Regex LineRegex => GetLineRegex();

    [GeneratedRegex(@"{")]
    private static partial Regex GetBlockStartRegex();
    public static Regex BlockStartRegex => GetBlockStartRegex();

    [GeneratedRegex(@"}")]
    private static partial Regex GetBlockEndRegex();
    public static Regex BlockEndRegex => GetBlockEndRegex();

    private MatchCollection WordMatches { get; set; } = WordRegex.Matches(string.Empty);
    private MatchCollection WhiteSpaceMatches { get; set; } = WhiteSpaceRegex.Matches(string.Empty);
    private MatchCollection NewLineMatches { get; set; } = NewLineRegex.Matches(string.Empty);
    private MatchCollection SymbolMatches { get; set; } = SymbolRegex.Matches(string.Empty);
    private MatchCollection CompoundSymbolMatches { get; set; } = CompoundSymbolRegex.Matches(string.Empty);
    private MatchCollection LineMatches { get; set; } = LineRegex.Matches(string.Empty);

    private Match? CurrentWord { get; set; } = null;

    private CompleteBox? Popup { get; set; }
    private int? PopupIndex { get; set; }

    public ScriptBox()
    {
        InitializeComponent();
    }

    private void UpdateMatches()
    {
        WordMatches = WordRegex.Matches(Text);
        WhiteSpaceMatches = WhiteSpaceRegex.Matches(Text);
        NewLineMatches = NewLineRegex.Matches(Text);
        SymbolMatches = SymbolRegex.Matches(Text);
        CompoundSymbolMatches = CompoundSymbolRegex.Matches(Text);
        LineMatches = LineRegex.Matches(Text);
    }

    private void UpdateCurrentWord()
    {
        CurrentWord = WordMatches.SingleOrDefault(match => CaretIndex >= match.Index && CaretIndex <= match.Index + match.Length);
    }

    #region Event Handlers

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
        UpdateMatches();
        UpdateCurrentWord();

        ValidatePopup();
        UpdatePopup();
    }

    private void SelectionChangedHandler(object sender, RoutedEventArgs e)
    {
        UpdateCurrentWord();

        ValidatePopup();
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

    private void MouseDownHandler(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2 && e.LeftButton == MouseButtonState.Pressed)
        {
            var pos = e.GetPosition(this);
            var index = CaretIndex;

            var (start, length) = GetDoubleClickSelection(pos, index);
            Select(start, length);

            StartDragSelection(SelectionMode.Double, pos, index);

            e.Handled = true;
            return;
        }

        if (e.ClickCount == 3 && e.LeftButton == MouseButtonState.Pressed)
        {
            var pos = e.GetPosition(this);
            var index = CaretIndex;

            var (start, length) = GetTripleClickSelection(pos, index);
            Select(start, length);

            StartDragSelection(SelectionMode.Triple, pos, index);

            e.Handled = true;
            return;
        }
    }

    private void MouseUpHandler(object sender, MouseButtonEventArgs e)
    {
        if (CurrentSelection != null)
        {
            StopDragSelection();
        }
    }

    private void MouseMoveHandler(object sender, MouseEventArgs e)
    {
        if (CurrentSelection != null && e.LeftButton == MouseButtonState.Pressed)
        {
            var pos = e.GetPosition(this);

            ApplySelection(pos);

            e.Handled = true;
            return;
        }
    }

    #endregion

    #region Selection

    private class Selection
    {
        public SelectionMode Mode { get; init; }
        public Point StartPos { get; init; }
        public int StartIndex { get; init; }
    }

    private enum SelectionMode
    {
        None,
        Single,
        Double,
        Triple,
    }

    private Selection? CurrentSelection { get; set; }

    private void StartDragSelection(SelectionMode mode, Point pos, int index)
    {
        if (CurrentSelection != null)
        {
            StopDragSelection();
        }

        CurrentSelection = new()
        {
            Mode = mode,
            StartPos = pos,
            StartIndex = index,
        };
    }

    private void StopDragSelection()
    {
        CurrentSelection = null;
    }

    private void ApplySelection(Point pos)
    {
        if (CurrentSelection == null)
        { return; }

        if (CurrentSelection.Mode == SelectionMode.Double)
        {
            var index = GetCharacterIndexFromPoint(pos, true);

            var (startOld, lengthOld) = GetDoubleClickSelection(CurrentSelection.StartPos, CurrentSelection.StartIndex);
            var (startNew, lengthNew) = GetDoubleClickSelection(pos, index);

            Select(Math.Min(startOld, startNew), Math.Max(startOld + lengthOld, startNew + lengthNew) - Math.Min(startOld, startNew));
        }

        if (CurrentSelection.Mode == SelectionMode.Triple)
        {
            var index = GetCharacterIndexFromPoint(pos, true);

            var (startOld, lengthOld) = GetTripleClickSelection(CurrentSelection.StartPos, CurrentSelection.StartIndex);
            var (startNew, lengthNew) = GetTripleClickSelection(pos, index);

            Select(Math.Min(startOld, startNew), Math.Max(startOld + lengthOld, startNew + lengthNew) - Math.Min(startOld, startNew));
        }
    }

    private (int, int) GetDoubleClickSelection(Point pos, int index)
    {
        var wordMatch = WordMatches.FirstOrDefault(match => index >= match.Index && index <= match.Index + match.Length);
        if (wordMatch != null)
        {
            return (wordMatch.Index, wordMatch.Length);
        }

        var comsymMatch = CompoundSymbolMatches.FirstOrDefault(match => index >= match.Index && index <= match.Index + match.Length);
        if (comsymMatch != null)
        {
            return (comsymMatch.Index, comsymMatch.Length);
        }

        var symMatch = SymbolMatches.FirstOrDefault(match => index >= match.Index && index <= match.Index + match.Length);
        if (symMatch != null)
        {
            return (symMatch.Index, symMatch.Length);
        }

        var wsMatch = WhiteSpaceMatches.FirstOrDefault(match => index >= match.Index && index <= match.Index + match.Length);
        if (wsMatch != null)
        {
            return (wsMatch.Index, wsMatch.Length);
        }

        var nlMatch = NewLineMatches.FirstOrDefault(match => index >= match.Index && index <= match.Index + match.Length);
        if (nlMatch != null)
        {
            return (index, 0);
        }

        var charIndex = GetCharacterIndexFromPoint(pos, true);
        return (charIndex, 1);
    }

    private (int, int) GetTripleClickSelection(Point pos, int index)
    {
        var lineMatch = LineMatches.FirstOrDefault(match => index >= match.Index && index < match.Index + match.Length);
        if (lineMatch != null)
        {
            return (lineMatch.Index, lineMatch.Length);
        }

        return GetDoubleClickSelection(pos, index);
    }

    #endregion

    #region Text Input

    private void TextInputHandler(object sender, TextCompositionEventArgs e)
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
        if (e.Text == "\t" && AcceptsTab == true)
        {
            InsertIndentation();
            e.Handled = true;
        }
        if (e.Text == "\r\n" && AcceptsReturn == true)
        {
            InsertNewLine();
            e.Handled = true;
        }
        if (e.Text == "." && AcceptsDot == true)
        {
            InsertDot();
            e.Handled = true;
        }
    }

    private void InsertQuotationMarks()
    {
        var index = CaretIndex;
        var length = SelectionLength;
        var text = Text;

        text = text.Remove(index, length);
        text = text.Insert(index, "\"\"");
        index += 1;

        Text = text;
        CaretIndex = index;
    }

    private void InsertBraces()
    {
        var index = CaretIndex;
        var length = SelectionLength;
        var text = Text;

        text = text.Remove(index, length);
        text = text.Insert(index, "()");
        index += 1;

        Text = text;
        CaretIndex = index;
    }

    private void InsertSquareBraces()
    {
        var index = CaretIndex;
        var length = SelectionLength;
        var text = Text;

        text = text.Remove(index, length);
        text = text.Insert(index, "[]");
        index += 1;

        Text = text;
        CaretIndex = index;
    }

    private void InsertCurlyBraces()
    {
        var index = CaretIndex;
        var length = SelectionLength;
        var text = Text;

        text = text.Remove(index, length);
        text = text.Insert(index, "{  }");
        index += 2;

        Text = text;
        CaretIndex = index;
    }

    private void InsertIndentation()
    {
        var index = CaretIndex;
        var length = SelectionLength;
        var text = Text;

        text = text.Remove(index, length);
        text = text.Insert(index, ParadoxText.Indentation);
        index += ParadoxText.Indentation.Length;

        Text = text;
        CaretIndex = index;
    }

    private void InsertNewLine()
    {
        var text = Text;
        var length = SelectionLength;
        var index = CaretIndex;

        text = text.Remove(index, length);
        text = text.Insert(index, Environment.NewLine);
        index += Environment.NewLine.Length;

        var after = text[index..];
        after = after.TrimStart();
        after = after[..1];

        if (BlockEndRegex.IsMatch(after))
        {
            text = text.Insert(index, Environment.NewLine);
        }

        if (AllowFormatting == true)
        {
            Formatter.Format(ref text, ref index);
        }

        Text = text;
        CaretIndex = index;
    }

    private void InsertDot()
    {
        var popup = Popup;
        if (popup != null)
        {
            popup.Result = true;
            ClosePopup();
        }

        var index = CaretIndex;
        var length = SelectionLength;
        var text = Text;

        text = text.Remove(index, length);
        text = text.Insert(index, ".");
        index += 1;

        Text = text;
        CaretIndex = index;

        if (popup != null)
        {
            OpenPopup();
        }
    }

    #endregion

    #region Popup

    private void OpenPopup()
    {
        if (Popup != null)
        {
            ClosePopup();
        }

        var index = GetPopupIndex();
        var pos = GetPopupPosition(index);

        PopupIndex = index;

        Window window = Window.GetWindow(this);
        Popup = new()
        {
            Owner = window,
            Left = pos.X,
            Top = pos.Y,
            Filter = CurrentWord?.Value ?? string.Empty,
            AllowedItems = AllowedCompleteItems,
            MaxItems = 10,
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
        int offset = Popup.Selected.Offset ?? code.Length;

        if (CurrentWord != null)
        {
            text = text.Remove(index, CurrentWord.Length);
        }

        text = text.Insert(index, code);
        index += offset;

        Text = text;
        CaretIndex = index;
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

    private void UpdatePopup()
    {
        if (Popup == null)
        { return; }

        Popup.Filter = CurrentWord?.Value ?? string.Empty;

        Popup.UpdateScores();
        Popup.UpdateView();
        Popup.UpdateSelection();
    }

    private void ValidatePopup()
    {
        if (Popup == null)
        { return; }

        ValidatePopupPlacement();
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

    private Point GetPopupPosition(int index)
    {
        var boxPos = TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
        var caretRect = GetRectFromCharacterIndex(index);
        var size = FontSize;

        return new() { X = boxPos.X + caretRect.X, Y = boxPos.Y + caretRect.Y + size + POPUP_OFFSET };
    }

    #endregion
}