using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Paradoxical.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace Paradoxical.View
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ActiveModEventsFilterHandler(object sender, FilterEventArgs e)
        {
            if (e.Item is not ParadoxEvent evt)
            {
                e.Accepted = true;
                return;
            }

            if (string.IsNullOrEmpty(EventExplorerFilter.Text))
            {
                e.Accepted = true;
                return;
            }

            if (evt.Title.ToLowerInvariant().Contains(EventExplorerFilter.Text.ToLowerInvariant()))
            {
                e.Accepted = true;
                return;
            }

            e.Accepted = false;
            return;
        }

        private void EventExplorerFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(EventExplorerDataGrid.ItemsSource)?.Refresh();
        }

        private void ActiveModTriggersFilterHandler(object sender, FilterEventArgs e)
        {
            if (e.Item is not ParadoxTrigger evt)
            {
                e.Accepted = true;
                return;
            }

            if (string.IsNullOrEmpty(TriggerExplorerFilter.Text))
            {
                e.Accepted = true;
                return;
            }

            if (evt.Name.ToLowerInvariant().Contains(TriggerExplorerFilter.Text.ToLowerInvariant()))
            {
                e.Accepted = true;
                return;
            }

            e.Accepted = false;
            return;
        }

        private void TriggerExplorerFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(TriggerExplorerDataGrid.ItemsSource)?.Refresh();
        }

        private void ActiveModEffectsFilterHandler(object sender, FilterEventArgs e)
        {
            if (e.Item is not ParadoxEffect evt)
            {
                e.Accepted = true;
                return;
            }

            if (string.IsNullOrEmpty(EffectExplorerFilter.Text))
            {
                e.Accepted = true;
                return;
            }

            if (evt.Name.ToLowerInvariant().Contains(EffectExplorerFilter.Text.ToLowerInvariant()))
            {
                e.Accepted = true;
                return;
            }

            e.Accepted = false;
            return;
        }

        private void EffectExplorerFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(EffectExplorerDataGrid.ItemsSource)?.Refresh();
        }
    }
}
