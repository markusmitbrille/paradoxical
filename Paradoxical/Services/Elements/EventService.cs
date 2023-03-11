using Paradoxical.Core;
using Paradoxical.Model.Elements;

namespace Paradoxical.Services.Elements;

public interface IEventService : IElementService<Event>
{
}

public class EventService : ElementService<Event>, IEventService
{
    public EventService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}
