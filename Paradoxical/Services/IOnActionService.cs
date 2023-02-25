using Paradoxical.Model;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface IOnActionService
{
    IEnumerable<OnAction> Get();

    void Insert(OnAction element);
    void Update(OnAction element);
    void Delete(OnAction element);

    IEnumerable<Trigger> GetTriggers(OnAction element);
    void SetTriggers(OnAction element, IEnumerable<Trigger> relations);

    IEnumerable<Effect> GetEffects(OnAction element);
    void SetEffects(OnAction element, IEnumerable<Effect> relations);

    IEnumerable<Event> GetEvents(OnAction element);
    void SetEvents(OnAction element, IEnumerable<Event> relations);

    IEnumerable<OnAction> GetOnActions(OnAction element);
    void SetOnActions(OnAction element, IEnumerable<OnAction> relations);
}
