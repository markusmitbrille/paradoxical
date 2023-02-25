using Paradoxical.Model;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface ITriggerService
{
    IEnumerable<Trigger> Get();

    void Insert(Trigger element);
    void Update(Trigger element);
    void Delete(Trigger element);
}
