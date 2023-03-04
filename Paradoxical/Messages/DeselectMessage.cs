using Paradoxical.Core;

namespace Paradoxical.Messages;

public class DeselectMessage : IMessage
{
    public IElement Element { get; }

    public DeselectMessage(IElement element)
    {
        Element = element;
    }
}
