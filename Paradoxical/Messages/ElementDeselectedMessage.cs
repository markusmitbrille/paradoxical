using Paradoxical.Core;

namespace Paradoxical.Messages;

public class ElementDeselectedMessage : IMessage
{
    public IElement Element { get; }

    public ElementDeselectedMessage(IElement element)
    {
        Element = element;
    }
}
