using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using Paradoxical.Services.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Paradoxical.ViewModel;

public class DecisionDetailsViewModel : PageViewModel
    , IEquatable<DecisionDetailsViewModel?>
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => $"Decision {Selected?.Id.ToString() ?? "Details"}";

    private void RefreshPageName() => OnPropertyChanged(nameof(PageName));

    public override bool IsValid
    {
        get
        {
            if (Selected == null)
            { return false; }

            var model = DecisionService.Find(Selected.Model);

            if (model == null)
            { return false; }

            return true;
        }
    }

    public IFinder Finder { get; }

    public IDataService DataService { get; }
    public IModService ModService { get; }
    public IDecisionService DecisionService { get; }
    public ITriggerService TriggerService { get; }
    public IEffectService EffectService { get; }
    public IEventService EventService { get; }
    public IPortraitService PortraitService { get; }

    private int selectedTab;
    public int SelectedTab
    {
        get => selectedTab;
        set => SetProperty(ref selectedTab, value);
    }

    private DecisionViewModel? selected;
    public DecisionViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    private ObservableCollection<TriggerViewModel>? shownTriggers;
    public ObservableCollection<TriggerViewModel> ShownTriggers
    {
        get => shownTriggers ??= new();
        set => SetProperty(ref shownTriggers, value);
    }

    private ObservableCollection<TriggerViewModel>? failureTriggers;
    public ObservableCollection<TriggerViewModel> FailureTriggers
    {
        get => failureTriggers ??= new();
        set => SetProperty(ref failureTriggers, value);
    }

    private ObservableCollection<TriggerViewModel>? validTriggers;
    public ObservableCollection<TriggerViewModel> ValidTriggers
    {
        get => validTriggers ??= new();
        set => SetProperty(ref validTriggers, value);
    }

    private ObservableCollection<EffectViewModel>? effects;
    public ObservableCollection<EffectViewModel> Effects
    {
        get => effects ??= new();
        set => SetProperty(ref effects, value);
    }

    private ObservableCollection<EventViewModel>? allEvents;
    public ObservableCollection<EventViewModel> AllEvents
    {
        get => allEvents ??= new();
        set => SetProperty(ref allEvents, value);
    }

    private ObservableCollection<TriggerViewModel>? aiPotentialTriggers;
    public ObservableCollection<TriggerViewModel> AiPotentialTriggers
    {
        get => aiPotentialTriggers ??= new();
        set => SetProperty(ref aiPotentialTriggers, value);
    }

    public string Output
    {
        get
        {
            if (Selected == null)
            { return string.Empty; }

            using StringWriter writer = new();

            Selected.Model.Write(writer, ModService, DecisionService);

            return writer.ToString();
        }
    }

    public DecisionDetailsViewModel(
        IShell shell,
        IMediatorService mediator,
        IFinder finder,
        IDataService dataService,
        IModService modService,
        IDecisionService decisionService,
        ITriggerService triggerService,
        IEffectService effectService,
        IEventService eventService,
        IPortraitService portraitService)
        : base(shell, mediator)
    {
        Finder = finder;

        DataService = dataService;
        ModService = modService;
        DecisionService = decisionService;
        TriggerService = triggerService;
        EffectService = effectService;
        EventService = eventService;
        PortraitService = portraitService;
    }

    public override void OnNavigatedTo()
    {
        if (DataService.IsInTransaction)
        {
            DataService.RollbackTransaction();
        }

        Reload();

        Mediator.Register<SaveMessage>(this);
        Mediator.Register<ShutdownMessage>(this);
    }

    public override void OnNavigatingFrom()
    {
        Save();

        if (DataService.IsInTransaction)
        {
            DataService.RollbackTransaction();
        }

        Mediator.Unregister<SaveMessage>(this);
        Mediator.Unregister<ShutdownMessage>(this);
    }

    void IMessageHandler<SaveMessage>.Handle(SaveMessage message)
    {
        Save();
    }

    void IMessageHandler<ShutdownMessage>.Handle(ShutdownMessage message)
    {
        Save();
    }

    public void Load(Decision model)
    {
        model = DecisionService.Get(model);
        Selected = new() { Model = model };

        Selected.PropertyChanged += TriggeredEventId_PropertyChanged;

        LoadShownTriggers();
        LoadFailureTriggers();
        LoadValidTriggers();
        LoadEffects();
        LoadAllEvents();
        LoadAiPotentialTriggers();

        RefreshPageName();
        RefreshOutput();

        DataService.BeginTransaction();
    }

    private void LoadShownTriggers()
    {
        if (Selected == null)
        { return; }

        var triggers = DecisionService.GetShownTriggers(Selected.Model)
            .Select(model => new TriggerViewModel() { Model = model });

        ShownTriggers = new(triggers);
    }

    private void LoadFailureTriggers()
    {
        if (Selected == null)
        { return; }

        var triggers = DecisionService.GetFailureTriggers(Selected.Model)
            .Select(model => new TriggerViewModel() { Model = model });

        FailureTriggers = new(triggers);
    }

    private void LoadValidTriggers()
    {
        if (Selected == null)
        { return; }

        var triggers = DecisionService.GetValidTriggers(Selected.Model)
            .Select(model => new TriggerViewModel() { Model = model });

        ValidTriggers = new(triggers);
    }

    private void LoadEffects()
    {
        if (Selected == null)
        { return; }

        var effects = DecisionService.GetEffects(Selected.Model)
            .Select(model => new EffectViewModel() { Model = model });

        Effects = new(effects);
    }

    private void LoadAllEvents()
    {
        if (Selected == null)
        { return; }

        var allEvents = EventService.Get()
            .Select(model => new EventViewModel() { Model = model });

        AllEvents = new(allEvents);
    }

    private void LoadAiPotentialTriggers()
    {
        if (Selected == null)
        { return; }

        var triggers = DecisionService.GetAiPotentialTriggers(Selected.Model)
            .Select(model => new TriggerViewModel() { Model = model });

        AiPotentialTriggers = new(triggers);
    }

    private RelayCommand? reloadCommand;
    public RelayCommand ReloadCommand => reloadCommand ??= new(Reload);

    private void Reload()
    {
        if (Selected == null)
        { return; }

        if (DataService.IsInTransaction)
        {
            DataService.RollbackTransaction();
        }

        var model = Selected.Model;
        Selected = null;

        Load(model);
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save);

    private void Save()
    {
        if (Selected == null)
        { return; }

        DecisionService.Update(Selected.Model);

        RefreshPageName();
        RefreshOutput();

        DataService.CommitTransaction();
        DataService.BeginTransaction();
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        Decision model = new();
        DecisionService.Insert(model);

        var page = Shell.Navigate<DecisionDetailsViewModel>();
        page.Load(model);
    }

    private RelayCommand? duplicateCommand;
    public RelayCommand DuplicateCommand => duplicateCommand ??= new(Duplicate);

    private void Duplicate()
    {
        if (Selected == null)
        { return; }

        Decision model = new(Selected.Model);

        DecisionService.Insert(model);

        var page = Shell.Navigate<DecisionDetailsViewModel>();
        page.Load(model);
    }

    private RelayCommand? deleteCommand;
    public RelayCommand DeleteCommand => deleteCommand ??= new(Delete);

    private void Delete()
    {
        if (Selected == null)
        { return; }

        DecisionService.Delete(Selected.Model);

        Shell.Navigate<DecisionTableViewModel>();
        Shell.InvalidatePage(this);
    }

    private RelayCommand? refreshOutputCommand;
    public RelayCommand RefreshOutputCommand => refreshOutputCommand ??= new(RefreshOutput);

    private void RefreshOutput()
    {
        if (Selected == null)
        { return; }

        OnPropertyChanged(nameof(Output));
    }

    private RelayCommand<DecisionViewModel>? editPreviousCommand;
    public RelayCommand<DecisionViewModel> EditPreviousCommand => editPreviousCommand ??= new(EditPrevious, CanEditPrevious);

    private void EditPrevious(DecisionViewModel? observable)
    {
        if (observable == null)
        { return; }

        var siblings = DecisionService.Get().ToList();
        siblings.Sort();

        int index = siblings.IndexOf(observable.Model) - 1;
        if (index < 0)
        {
            return;
        }

        var model = siblings[index];

        var page = Shell.Navigate<DecisionDetailsViewModel>();
        page.Load(model);
    }
    private bool CanEditPrevious(DecisionViewModel? observable)
    {
        if (observable == null)
        { return false; }

        var siblings = DecisionService.Get().ToList();
        siblings.Sort();

        int index = siblings.IndexOf(observable.Model) - 1;

        return index >= 0;
    }

    private RelayCommand<DecisionViewModel>? editNextCommand;
    public RelayCommand<DecisionViewModel> EditNextCommand => editNextCommand ??= new(EditNext, CanEditNext);

    private void EditNext(DecisionViewModel? observable)
    {
        if (observable == null)
        { return; }

        var siblings = DecisionService.Get().ToList();
        siblings.Sort();

        int index = siblings.IndexOf(observable.Model) + 1;
        if (index >= siblings.Count)
        {
            return;
        }

        var model = siblings[index];

        var page = Shell.Navigate<DecisionDetailsViewModel>();
        page.Load(model);
    }
    private bool CanEditNext(DecisionViewModel? observable)
    {
        if (observable == null)
        { return false; }

        var siblings = DecisionService.Get().ToList();
        siblings.Sort();

        int index = siblings.IndexOf(observable.Model) + 1;

        return index < siblings.Count;
    }

    #region Flow Commands

    private RelayCommand? createTriggeredEventCommand;
    public RelayCommand CreateTriggeredEventCommand => createTriggeredEventCommand ??= new(CreateTriggeredEvent);

    private void CreateTriggeredEvent()
    {
        if (Selected == null)
        { return; }

        Event model = new();
        EventService.Insert(model);

        Portrait leftPortrait = new()
        {
            EventId = model.Id,
            Position = PortraitPosition.Left,
        };
        Portrait rightPortrait = new()
        {
            EventId = model.Id,
            Position = PortraitPosition.Right,
        };
        Portrait lowerLeftPortrait = new()
        {
            EventId = model.Id,
            Position = PortraitPosition.LowerLeft,
        };
        Portrait lowerCenterPortrait = new()
        {
            EventId = model.Id,
            Position = PortraitPosition.LowerCenter,
        };
        Portrait lowerRightPortrait = new()
        {
            EventId = model.Id,
            Position = PortraitPosition.LowerRight,
        };

        PortraitService.Insert(leftPortrait);
        PortraitService.Insert(rightPortrait);
        PortraitService.Insert(lowerLeftPortrait);
        PortraitService.Insert(lowerCenterPortrait);
        PortraitService.Insert(lowerRightPortrait);

        EventViewModel observable = new() { Model = model };
        AllEvents.Add(observable);

        Selected.TriggeredEventId = model.Id;
    }

    private AsyncRelayCommand? addTriggeredEventCommand;
    public AsyncRelayCommand AddTriggeredEventCommand => addTriggeredEventCommand ??= new(AddTriggeredEvent);

    private async Task AddTriggeredEvent()
    {
        if (Selected == null)
        { return; }

        Save();

        Finder.Items = EventService.Get()
            .Select(model => new EventViewModel() { Model = model });

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Selected.TriggeredEventId = Finder.Selected.Id;
    }

    private RelayCommand? removeTriggeredEventCommand;
    public RelayCommand RemoveTriggeredEventCommand => removeTriggeredEventCommand ??= new(RemoveTriggeredEvent, CanRemoveTriggeredEvent);

    private void RemoveTriggeredEvent()
    {
        if (Selected == null)
        { return; }

        if (Selected.TriggeredEventId == null)
        { return; }

        Selected.TriggeredEventId = null;
    }
    private bool CanRemoveTriggeredEvent()
    {
        return Selected != null && Selected.TriggeredEventId != null;
    }

    private RelayCommand? editTriggeredEventCommand;
    public RelayCommand EditTriggeredEventCommand => editTriggeredEventCommand ??= new(EditTriggeredEvent, CanEditTriggeredEvent);

    private void EditTriggeredEvent()
    {
        if (Selected == null)
        { return; }

        if (Selected.TriggeredEventId == null)
        { return; }

        Event model = EventService.Get(Selected.TriggeredEventId.Value);

        var page = Shell.Navigate<EventDetailsViewModel>();
        page.Load(model);
    }
    private bool CanEditTriggeredEvent()
    {
        return Selected != null && Selected.TriggeredEventId != null;
    }

    private void TriggeredEventId_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(OptionViewModel.TriggeredEventId))
        { return; }

        RemoveTriggeredEventCommand.NotifyCanExecuteChanged();
        EditTriggeredEventCommand.NotifyCanExecuteChanged();
    }

    #endregion

    #region Shown Trigger Commands

    private RelayCommand? createShownTriggerCommand;
    public RelayCommand CreateShownTriggerCommand => createShownTriggerCommand ??= new(CreateShownTrigger);

    private void CreateShownTrigger()
    {
        if (Selected == null)
        { return; }

        Decision owner = Selected.Model;
        Trigger relation = new();

        TriggerService.Insert(relation);
        DecisionService.AddShownTrigger(owner, relation);

        TriggerViewModel observable = new() { Model = relation };
        ShownTriggers.Add(observable);
    }

    private AsyncRelayCommand? addShownTriggerCommand;
    public AsyncRelayCommand AddShownTriggerCommand => addShownTriggerCommand ??= new(AddShownTrigger);

    private async Task AddShownTrigger()
    {
        if (Selected == null)
        { return; }

        Save();

        Finder.Items = TriggerService.Get()
            .Select(model => new TriggerViewModel() { Model = model });

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Decision owner = Selected.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        DecisionService.AddShownTrigger(owner, relation);

        TriggerViewModel observable = new() { Model = relation };
        ShownTriggers.Add(observable);
    }

    private RelayCommand<TriggerViewModel>? removeShownTriggerCommand;
    public RelayCommand<TriggerViewModel> RemoveShownTriggerCommand => removeShownTriggerCommand ??= new(RemoveShownTrigger, CanRemoveShownTrigger);

    private void RemoveShownTrigger(TriggerViewModel? observable)
    {
        if (Selected == null)
        { return; }

        if (observable == null)
        { return; }

        DecisionService.RemoveShownTrigger(Selected.Model, observable.Model);
        ShownTriggers.Remove(observable);
    }
    private bool CanRemoveShownTrigger(TriggerViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<TriggerViewModel>? editShownTriggerCommand;
    public RelayCommand<TriggerViewModel> EditShownTriggerCommand => editShownTriggerCommand ??= new(EditShownTrigger, CanEditShownTrigger);

    private void EditShownTrigger(TriggerViewModel? observable)
    {
        if (observable == null)
        { return; }

        Trigger model = observable.Model;

        var page = Shell.Navigate<TriggerDetailsViewModel>();
        page.Load(model);
    }
    private bool CanEditShownTrigger(TriggerViewModel? observable)
    {
        return observable != null;
    }

    #endregion

    #region Failure Trigger Commands

    private RelayCommand? createFailureTriggerCommand;
    public RelayCommand CreateFailureTriggerCommand => createFailureTriggerCommand ??= new(CreateFailureTrigger);

    private void CreateFailureTrigger()
    {
        if (Selected == null)
        { return; }

        Decision owner = Selected.Model;
        Trigger relation = new();

        TriggerService.Insert(relation);
        DecisionService.AddFailureTrigger(owner, relation);

        TriggerViewModel observable = new() { Model = relation };
        FailureTriggers.Add(observable);
    }

    private AsyncRelayCommand? addFailureTriggerCommand;
    public AsyncRelayCommand AddFailureTriggerCommand => addFailureTriggerCommand ??= new(AddFailureTrigger);

    private async Task AddFailureTrigger()
    {
        if (Selected == null)
        { return; }

        Save();

        Finder.Items = TriggerService.Get()
            .Select(model => new TriggerViewModel() { Model = model });

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Decision owner = Selected.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        DecisionService.AddFailureTrigger(owner, relation);

        TriggerViewModel observable = new() { Model = relation };
        FailureTriggers.Add(observable);
    }

    private RelayCommand<TriggerViewModel>? removeFailureTriggerCommand;
    public RelayCommand<TriggerViewModel> RemoveFailureTriggerCommand => removeFailureTriggerCommand ??= new(RemoveFailureTrigger, CanRemoveFailureTrigger);

    private void RemoveFailureTrigger(TriggerViewModel? observable)
    {
        if (Selected == null)
        { return; }

        if (observable == null)
        { return; }

        DecisionService.RemoveFailureTrigger(Selected.Model, observable.Model);
        FailureTriggers.Remove(observable);
    }
    private bool CanRemoveFailureTrigger(TriggerViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<TriggerViewModel>? editFailureTriggerCommand;
    public RelayCommand<TriggerViewModel> EditFailureTriggerCommand => editFailureTriggerCommand ??= new(EditFailureTrigger, CanEditFailureTrigger);

    private void EditFailureTrigger(TriggerViewModel? observable)
    {
        if (observable == null)
        { return; }

        Trigger model = observable.Model;

        var page = Shell.Navigate<TriggerDetailsViewModel>();
        page.Load(model);
    }
    private bool CanEditFailureTrigger(TriggerViewModel? observable)
    {
        return observable != null;
    }

    #endregion

    #region Valid Trigger Commands

    private RelayCommand? createValidTriggerCommand;
    public RelayCommand CreateValidTriggerCommand => createValidTriggerCommand ??= new(CreateValidTrigger);

    private void CreateValidTrigger()
    {
        if (Selected == null)
        { return; }

        Decision owner = Selected.Model;
        Trigger relation = new();

        TriggerService.Insert(relation);
        DecisionService.AddValidTrigger(owner, relation);

        TriggerViewModel observable = new() { Model = relation };
        ValidTriggers.Add(observable);
    }

    private AsyncRelayCommand? addValidTriggerCommand;
    public AsyncRelayCommand AddValidTriggerCommand => addValidTriggerCommand ??= new(AddValidTrigger);

    private async Task AddValidTrigger()
    {
        if (Selected == null)
        { return; }

        Save();

        Finder.Items = TriggerService.Get()
            .Select(model => new TriggerViewModel() { Model = model });

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Decision owner = Selected.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        DecisionService.AddValidTrigger(owner, relation);

        TriggerViewModel observable = new() { Model = relation };
        ValidTriggers.Add(observable);
    }

    private RelayCommand<TriggerViewModel>? removeValidTriggerCommand;
    public RelayCommand<TriggerViewModel> RemoveValidTriggerCommand => removeValidTriggerCommand ??= new(RemoveValidTrigger, CanRemoveValidTrigger);

    private void RemoveValidTrigger(TriggerViewModel? observable)
    {
        if (Selected == null)
        { return; }

        if (observable == null)
        { return; }

        DecisionService.RemoveValidTrigger(Selected.Model, observable.Model);
        ValidTriggers.Remove(observable);
    }
    private bool CanRemoveValidTrigger(TriggerViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<TriggerViewModel>? editValidTriggerCommand;
    public RelayCommand<TriggerViewModel> EditValidTriggerCommand => editValidTriggerCommand ??= new(EditValidTrigger, CanEditValidTrigger);

    private void EditValidTrigger(TriggerViewModel? observable)
    {
        if (observable == null)
        { return; }

        Trigger model = observable.Model;

        var page = Shell.Navigate<TriggerDetailsViewModel>();
        page.Load(model);
    }
    private bool CanEditValidTrigger(TriggerViewModel? observable)
    {
        return observable != null;
    }

    #endregion

    #region Effect Commands

    private RelayCommand? createEffectCommand;
    public RelayCommand CreateEffectCommand => createEffectCommand ??= new(CreateEffect);

    private void CreateEffect()
    {
        if (Selected == null)
        { return; }

        Decision owner = Selected.Model;
        Effect relation = new();

        EffectService.Insert(relation);
        DecisionService.AddEffect(owner, relation);

        EffectViewModel observable = new() { Model = relation };
        Effects.Add(observable);
    }

    private AsyncRelayCommand? addEffectCommand;
    public AsyncRelayCommand AddEffectCommand => addEffectCommand ??= new(AddEffect);

    private async Task AddEffect()
    {
        if (Selected == null)
        { return; }

        Save();

        Finder.Items = EffectService.Get()
            .Select(model => new EffectViewModel() { Model = model });

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Decision owner = Selected.Model;
        Effect relation = ((EffectViewModel)Finder.Selected).Model;

        DecisionService.AddEffect(owner, relation);

        EffectViewModel observable = new() { Model = relation };
        Effects.Add(observable);
    }

    private RelayCommand<EffectViewModel>? removeEffectCommand;
    public RelayCommand<EffectViewModel> RemoveEffectCommand => removeEffectCommand ??= new(RemoveEffect, CanRemoveEffect);

    private void RemoveEffect(EffectViewModel? observable)
    {
        if (Selected == null)
        { return; }

        if (observable == null)
        { return; }

        DecisionService.RemoveEffect(Selected.Model, observable.Model);
        Effects.Remove(observable);
    }
    private bool CanRemoveEffect(EffectViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<EffectViewModel>? editEffectCommand;
    public RelayCommand<EffectViewModel> EditEffectCommand => editEffectCommand ??= new(EditEffect, CanEditEffect);

    private void EditEffect(EffectViewModel? observable)
    {
        if (observable == null)
        { return; }

        Effect model = observable.Model;

        var page = Shell.Navigate<EffectDetailsViewModel>();
        page.Load(model);
    }
    private bool CanEditEffect(EffectViewModel? observable)
    {
        return observable != null;
    }

    #endregion

    #region Shown Trigger Commands

    private RelayCommand? createAiPotentialTriggerCommand;
    public RelayCommand CreateAiPotentialTriggerCommand => createAiPotentialTriggerCommand ??= new(CreateAiPotentialTrigger);

    private void CreateAiPotentialTrigger()
    {
        if (Selected == null)
        { return; }

        Decision owner = Selected.Model;
        Trigger relation = new();

        TriggerService.Insert(relation);
        DecisionService.AddAiPotentialTrigger(owner, relation);

        TriggerViewModel observable = new() { Model = relation };
        AiPotentialTriggers.Add(observable);
    }

    private AsyncRelayCommand? addAiPotentialTriggerCommand;
    public AsyncRelayCommand AddAiPotentialTriggerCommand => addAiPotentialTriggerCommand ??= new(AddAiPotentialTrigger);

    private async Task AddAiPotentialTrigger()
    {
        if (Selected == null)
        { return; }

        Save();

        Finder.Items = TriggerService.Get()
            .Select(model => new TriggerViewModel() { Model = model });

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Decision owner = Selected.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        DecisionService.AddAiPotentialTrigger(owner, relation);

        TriggerViewModel observable = new() { Model = relation };
        AiPotentialTriggers.Add(observable);
    }

    private RelayCommand<TriggerViewModel>? removeAiPotentialTriggerCommand;
    public RelayCommand<TriggerViewModel> RemoveAiPotentialTriggerCommand => removeAiPotentialTriggerCommand ??= new(RemoveAiPotentialTrigger, CanRemoveAiPotentialTrigger);

    private void RemoveAiPotentialTrigger(TriggerViewModel? observable)
    {
        if (Selected == null)
        { return; }

        if (observable == null)
        { return; }

        DecisionService.RemoveAiPotentialTrigger(Selected.Model, observable.Model);
        AiPotentialTriggers.Remove(observable);
    }
    private bool CanRemoveAiPotentialTrigger(TriggerViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<TriggerViewModel>? editAiPotentialTriggerCommand;
    public RelayCommand<TriggerViewModel> EditAiPotentialTriggerCommand => editAiPotentialTriggerCommand ??= new(EditAiPotentialTrigger, CanEditAiPotentialTrigger);

    private void EditAiPotentialTrigger(TriggerViewModel? observable)
    {
        if (observable == null)
        { return; }

        Trigger model = observable.Model;

        var page = Shell.Navigate<TriggerDetailsViewModel>();
        page.Load(model);
    }
    private bool CanEditAiPotentialTrigger(TriggerViewModel? observable)
    {
        return observable != null;
    }

    #endregion

    #region Equality

    public override bool Equals(object? obj)
    {
        return Equals(obj as DecisionDetailsViewModel);
    }

    public bool Equals(DecisionDetailsViewModel? other)
    {
        return other is not null &&
               EqualityComparer<DecisionViewModel?>.Default.Equals(selected, other.selected);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(selected);
    }

    public static bool operator ==(DecisionDetailsViewModel? left, DecisionDetailsViewModel? right)
    {
        return EqualityComparer<DecisionDetailsViewModel>.Default.Equals(left, right);
    }

    public static bool operator !=(DecisionDetailsViewModel? left, DecisionDetailsViewModel? right)
    {
        return !(left == right);
    }

    #endregion
}
