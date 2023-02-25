using Paradoxical.Model;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface IPortraitService
{
    IEnumerable<Portrait> Get();

    Portrait? Get(Event owner, PortraitPosition position);

    void Insert(Portrait element);
    void Update(Portrait element);
    void Delete(Portrait element);
}
