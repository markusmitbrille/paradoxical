﻿using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View;

public partial class OnActionTableView : UserControl
{
    public OnActionTableView()
    {
        InitializeComponent();

        Focusable = true;
        Loaded += (s, e) => Keyboard.Focus(this);
    }
}
