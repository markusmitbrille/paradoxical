using Paradoxical.Model;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface IOptionService
{
    IEnumerable<Option> Get();

    void Insert(Option element);
    void Update(Option element);
    void Delete(Option element);

    Event GetOwner(Option element);
    Event? GetTriggeredEvent(Option element);

    IEnumerable<Trigger> GetTriggers(Option element);
    void SetTriggers(Option element, IEnumerable<Trigger> relations);

    IEnumerable<Effect> GetEffects(Option element);
    void SetEffects(Option element, IEnumerable<Effect> relations);
}
