using Paradoxical.Core;
using Paradoxical.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

public partial class ElementList : UserControl
{
    public IEnumerable ItemsSource
    {
        get { return (IEnumerable)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(object), typeof(ElementList), new PropertyMetadata(null));


    public IElementWrapper SelectedItem
    {
        get { return (IElementWrapper)GetValue(SelectedItemProperty); }
        set { SetValue(SelectedItemProperty, value); }
    }

    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register("SelectedItem", typeof(object), typeof(ElementList), new PropertyMetadata(null));


    public ICommand AddCommand
    {
        get { return (ICommand)GetValue(AddCommandProperty); }
        set { SetValue(AddCommandProperty, value); }
    }

    public static readonly DependencyProperty AddCommandProperty =
        DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(ElementList), new PropertyMetadata(null));


    public ICommand CreateCommand
    {
        get { return (ICommand)GetValue(CreateCommandProperty); }
        set { SetValue(CreateCommandProperty, value); }
    }

    public static readonly DependencyProperty CreateCommandProperty =
        DependencyProperty.Register("CreateCommand", typeof(ICommand), typeof(ElementList), new PropertyMetadata(null));


    public ICommand RemoveCommand
    {
        get { return (ICommand)GetValue(RemoveCommandProperty); }
        set { SetValue(RemoveCommandProperty, value); }
    }

    public static readonly DependencyProperty RemoveCommandProperty =
        DependencyProperty.Register("RemoveCommand", typeof(ICommand), typeof(ElementList), new PropertyMetadata(null));


    public ICommand EditCommand
    {
        get { return (ICommand)GetValue(EditCommandProperty); }
        set { SetValue(EditCommandProperty, value); }
    }

    public static readonly DependencyProperty EditCommandProperty =
        DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(ElementList), new PropertyMetadata(null));


    public string PlaceHolderText
    {
        get { return (string)GetValue(PlaceHolderTextProperty); }
        set { SetValue(PlaceHolderTextProperty, value); }
    }

    public static readonly DependencyProperty PlaceHolderTextProperty =
        DependencyProperty.Register("PlaceHolderText", typeof(string), typeof(ElementList), new PropertyMetadata("No elements selected…"));


    public ElementList()
    {
        InitializeComponent();
    }
}
