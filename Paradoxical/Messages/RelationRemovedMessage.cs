using Paradoxical.Core;

namespace Paradoxical.Messages;

public class RelationRemovedMessage : IMessage
{
    public IRelationship Relation { get; }

    public RelationRemovedMessage(IRelationship relation)
    {
        Relation = relation;
    }
}
