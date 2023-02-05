using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Data;
using Paradoxical.Model;
using Paradoxical.View;
using System.Linq;
using System.Windows;

namespace Paradoxical.ViewModel
{
    public partial class EffectPageViewModel : PageViewModelBase
    {
        public override string PageName => "Effects";

        public ModContext Context { get; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEffectSelected))]
        [NotifyCanExecuteChangedFor(nameof(DuplicateEffectCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveEffectCommand))]
        [NotifyCanExecuteChangedFor(nameof(PreviousEffectCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextEffectCommand))]
        private ParadoxEffect? selectedEffect;
        public bool IsEffectSelected => SelectedEffect != null;

        public EffectPageViewModel(ModContext context)
        {
            Context = context;
        }

        [RelayCommand]
        private void AddEffect()
        {
            ParadoxEffect eff = new(Context);

            Context.Effects.Add(eff);
            SelectedEffect = eff;

            FindEffectCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanDuplicateEffect))]
        private void DuplicateEffect()
        {
            if (SelectedEffect == null)
            { return; }

            ParadoxEffect evt = new(Context, SelectedEffect);

            Context.Effects.Add(evt);
            SelectedEffect = evt;

            FindEffectCommand.NotifyCanExecuteChanged();
        }
        private bool CanDuplicateEffect()
        {
            return SelectedEffect != null;
        }

        [RelayCommand(CanExecute = nameof(CanRemoveEffect))]
        private void RemoveEffect()
        {
            if (SelectedEffect == null)
            { return; }

            if (MessageBox.Show(
                "Are you sure?",
                "Remove Trigger",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.Yes) != MessageBoxResult.Yes)
            { return; }

            foreach (ParadoxEvent evt in Context.Events)
            {
                evt.ImmediateEffects.Remove(SelectedEffect);
                evt.AfterEffects.Remove(SelectedEffect);

                foreach (ParadoxEventOption opt in evt.Options)
                {
                    opt.Effects.Remove(SelectedEffect);
                }
            }

            Context.Effects.Remove(SelectedEffect);
            SelectedEffect = Context.Effects.FirstOrDefault();

            FindEffectCommand.NotifyCanExecuteChanged();
        }
        private bool CanRemoveEffect()
        {
            return SelectedEffect != null;
        }

        [RelayCommand(CanExecute = nameof(CanFindEffect))]
        private async void FindEffect()
        {
            FindEffectDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Effects,
                Selected = SelectedEffect,
            };
            FindEffectDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, "RootDialog");

            if (vm.DialogResult != true)
            { return; }

            if (vm.Selected == null)
            { return; }

            SelectedEffect = vm.Selected;
        }
        private bool CanFindEffect()
        {
            return Context.Effects.Any();
        }

        [RelayCommand(CanExecute = nameof(CanPreviousEffect))]
        private void PreviousEffect()
        {
            if (SelectedEffect == null)
            { return; }

            int index = Context.Effects.IndexOf(SelectedEffect);
            SelectedEffect = Context.Effects[index - 1];
        }
        private bool CanPreviousEffect()
        {
            return SelectedEffect != null
                && Context.Effects.Any()
                && SelectedEffect != Context.Effects.First();
        }

        [RelayCommand(CanExecute = nameof(CanNextEffect))]
        private void NextEffect()
        {
            if (SelectedEffect == null)
            { return; }

            int index = Context.Effects.IndexOf(SelectedEffect);
            SelectedEffect = Context.Effects[index + 1];
        }
        private bool CanNextEffect()
        {
            return SelectedEffect != null
                && Context.Effects.Any()
                && SelectedEffect != Context.Effects.Last();
        }
    }
}