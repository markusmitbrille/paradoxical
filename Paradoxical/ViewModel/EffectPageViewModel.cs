using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Paradoxical.Data;
using Paradoxical.Model;
using System.Linq;

namespace Paradoxical.ViewModel
{
    public partial class EffectPageViewModel : PageViewModel
    {
        public override string PageName => "Effects";

        public ModContext Context { get; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveEffectCommand))]
        private ParadoxEffect? selectedEffect;

        public EffectPageViewModel(ModContext context)
        {
            Context = context;
        }

        [RelayCommand]
        private void AddEffect()
        {
            ParadoxEffect eff = new(Context);
            eff.Name = "New Effect";

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
    }
}