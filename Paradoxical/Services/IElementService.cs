using Paradoxical.Model;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface IElementService
{
    IEnumerable<IElement> Get();
}
