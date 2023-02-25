using Paradoxical.Model;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface IDecisionService
{
    IEnumerable<Decision> Get();

    void Insert(Decision element);
    void Update(Decision element);
    void Delete(Decision element);

    Event? GetTriggeredEvent(Decision element);

    IEnumerable<Trigger> GetIsShownTriggers(Decision element);
    void SetIsShownTriggers(Decision element, IEnumerable<Trigger> relations);

    IEnumerable<Trigger> GetIsValidTriggers(Decision element);
    void SetIsValidTriggers(Decision element, IEnumerable<Trigger> relations);

    IEnumerable<Trigger> GetIsValidFailureTriggers(Decision element);
    void SetIsValidFailureTriggers(Decision element, IEnumerable<Trigger> relations);

    IEnumerable<Effect> GetEffects(Decision element);
    void SetEffects(Decision element, IEnumerable<Effect> relations);
}
