﻿using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View
{
    public partial class FindDialogView : UserControl
    {
        public FindDialogView()
        {
            InitializeComponent();

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}