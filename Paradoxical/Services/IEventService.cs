using Paradoxical.Model;
using System.Collections.Generic;

namespace Paradoxical.Services;
public interface IEventService
{
    IEnumerable<Event> Get();

    void Insert(Event element);
    void Update(Event element);
    void Delete(Event element);

    IEnumerable<Trigger> GetTriggers(Event element);
    void SetTriggers(Event element, IEnumerable<Trigger> relations);

    IEnumerable<Effect> GetImmediateEffects(Event element);
    void SetImmediateEffects(Event element, IEnumerable<Effect> relations);

    IEnumerable<Effect> GetAfterEffects(Event element);
    void SetAfterEffects(Event element, IEnumerable<Effect> relations);

    IEnumerable<Option> GetOptions(Event element);
    void SetOptions(Event element, IEnumerable<Effect> relations);
}
