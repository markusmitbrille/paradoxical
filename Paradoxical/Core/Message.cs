namespace Paradoxical.Core;

public interface IMessage
{
}

public interface IMessageHandler<T> where T : IMessage
{
    void Handle(T message);
}
