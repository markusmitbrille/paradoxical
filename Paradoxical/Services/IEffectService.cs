using Paradoxical.Model;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface IEffectService
{
    IEnumerable<Effect> Get();

    void Insert(Effect element);
    void Update(Effect element);
    void Delete(Effect element);
}
