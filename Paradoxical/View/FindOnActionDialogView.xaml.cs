﻿using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View
{
    public partial class FindOnActionDialogView : UserControl
    {
        public FindOnActionDialogView()
        {
            InitializeComponent();

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}
