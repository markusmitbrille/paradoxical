using Paradoxical.Core;

namespace Paradoxical.Messages;

public class ElementInsertedMessage : IMessage
{
    public IElement Model { get; }

    public ElementInsertedMessage(IElement model)
    {
        Model = model;
    }
}
