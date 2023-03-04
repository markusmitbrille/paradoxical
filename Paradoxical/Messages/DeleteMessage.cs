using Paradoxical.Core;

namespace Paradoxical.Messages;

public class DeleteMessage : IMessage
{
    public IModel Model { get; }

    public DeleteMessage(IModel model)
    {
        Model = model;
    }
}
