using Paradoxical.Core;

namespace Paradoxical.Messages;

public class SelectMessage : IMessage
{
    public IModel Model { get; }

    public SelectMessage(IModel model)
    {
        Model = model;
    }
}
