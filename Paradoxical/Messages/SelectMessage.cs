using Paradoxical.Core;

namespace Paradoxical.Messages;

public class SelectMessage : IMessage
{
    public IElement Element { get; }

    public SelectMessage(IElement element)
    {
        Element = element;
    }
}
