using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Model;
using CommunityToolkit.Mvvm.Input;
using System.Linq;

namespace Paradoxical.ViewModel
{
    public partial class EffectPageViewModel : PageViewModel
    {
        public override string PageName => "Effects";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddEffectCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveEffectCommand))]
        private ParadoxMod? activeMod;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveEffectCommand))]
        private ParadoxEffect? selectedEffect;

        [RelayCommand(CanExecute = nameof(CanAddEffect))]
        private void AddEffect()
        {
            if (ActiveMod == null)
            { return; }

            ParadoxEffect eff = new();
            eff.Name = "New Effect";

            ActiveMod.Effects.Add(eff);
            SelectedEffect = eff;
        }
        private bool CanAddEffect()
        {
            return ActiveMod != null;
        }

        [RelayCommand(CanExecute = nameof(CanRemoveEffect))]
        private void RemoveEffect()
        {
            if (ActiveMod == null)
            { return; }
            if (SelectedEffect == null)
            { return; }

            foreach (ParadoxEvent evt in ActiveMod.Events)
            {
                evt.ImmediateEffects.Remove(SelectedEffect);
                evt.AfterEffects.Remove(SelectedEffect);

                foreach (ParadoxEventOption opt in evt.Options)
                {
                    opt.Effects.Remove(SelectedEffect);
                }
            }

            ActiveMod.Effects.Remove(SelectedEffect);
            SelectedEffect = ActiveMod.Effects.FirstOrDefault();
        }
        private bool CanRemoveEffect()
        {
            return ActiveMod != null && SelectedEffect != null;
        }
    }
}