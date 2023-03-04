using Paradoxical.Core;

namespace Paradoxical.Messages;

public class InsertMessage : IMessage
{
    public IModel Model { get; }

    public InsertMessage(IModel model)
    {
        Model = model;
    }
}
