using System.Collections.Generic;

namespace Paradoxical.Core;

public interface IElementService<TElement> where TElement : IElement
{
    TElement Get(int id);
    TElement Get(TElement model);

    IEnumerable<TElement> Get();

    void Insert(TElement model);
    void Delete(TElement model);

    void Update(TElement model);
    void UpdateAll(IEnumerable<TElement> models);
}
