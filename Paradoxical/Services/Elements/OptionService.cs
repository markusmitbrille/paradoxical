using Paradoxical.Core;
using Paradoxical.Model.Elements;

namespace Paradoxical.Services.Elements;

public interface IOptionService : IElementService<Option>
{
}

public class OptionService : ElementService<Option>, IOptionService
{
    public OptionService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}
