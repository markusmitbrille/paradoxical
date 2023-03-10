using Paradoxical.Core;

namespace Paradoxical.Messages;

public class ElementDeletedMessage : IMessage
{
    public IElement Model { get; }

    public ElementDeletedMessage(IElement model)
    {
        Model = model;
    }
}
