using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Data;
using Paradoxical.Model;
using Paradoxical.View;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Paradoxical.ViewModel
{
    public partial class EffectPageViewModel : PageViewModel
    {
        public override string PageName => "Effects";

        public ModContext Context { get; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEffectSelected))]
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
            ParadoxEffect eff = new(Context)
            {
                Name = $"Effect [{Guid.NewGuid().ToString()[0..4]}]",
            };

            Context.Effects.Add(eff);
            SelectedEffect = eff;
        }

        [RelayCommand(CanExecute = nameof(CanRemoveEffect))]
        private void RemoveEffect()
        {
            if (SelectedEffect == null)
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
        }
        private bool CanRemoveEffect()
        {
            return SelectedEffect != null;
        }

        [RelayCommand]
        private async void FindEffect()
        {
            FindEffectDialogViewModel vm = new()
            {
                Items = Context.Effects,
            };
            FindEffectDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, "RootDialog");

            if (vm.Selected == null)
            { return; }

            SelectedEffect = vm.Selected;
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