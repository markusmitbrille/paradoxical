using Paradoxical.Core;

namespace Paradoxical.Messages;

public class ElementSelectedMessage : IMessage
{
    public IElement Element { get; }

    public ElementSelectedMessage(IElement element)
    {
        Element = element;
    }
}
