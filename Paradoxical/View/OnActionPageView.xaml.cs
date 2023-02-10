﻿using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View
{
    public partial class OnActionPageView : UserControl
    {
        public OnActionPageView()
        {
            InitializeComponent();

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}
