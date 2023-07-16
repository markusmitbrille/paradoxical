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

    private void KeyDownHandler(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Up || e.Key == Key.Down)
        {
            RaisePopupEvent(sender, e);
        }

        if (e.Key == Key.Tab)
        {
            ScriptBoxCommands.ConfirmComplete.Execute(null, this);
            e.Handled = true;
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

        Popup.UpdateView();
        Popup.UpdateSelection();
        Popup.Show();

        // refocus
        Focus();
    }

    private void ApplyPopup()
    {
        if (Popup == null)
        { return; }

        if (Popup.Selected == null)
        { return; }

        int index = CaretIndex;
        string code = Popup.Selected.Code;

        if (CurrentWord != null)
        {
            string text = Text;
            text = text.Remove(CurrentWord.Index, CurrentWord.Length);
            text = text.Insert(CurrentWord.Index, code);
            Text = text;
        }
        else
        {
            string text = Text;
            text = text.Insert(CaretIndex, code);
            Text = text;
        }

        // reset selection
        CaretIndex = index;

        // set caret to end of word
        CaretIndex = CurrentWord?.Index + CurrentWord?.Length ?? index;
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
    }

    private void UpdatePopup()
    {
        if (Popup == null)
        { return; }

        ValidatePopupPlacement();

        if (Popup == null)
        { return; }

        Popup.Filter = CurrentWord?.Value;
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
}
