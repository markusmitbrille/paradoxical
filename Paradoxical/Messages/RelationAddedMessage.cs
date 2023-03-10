using Paradoxical.Core;

namespace Paradoxical.Messages;

public class RelationAddedMessage : IMessage
{
    public IRelationship Relation { get; }

    public RelationAddedMessage(IRelationship relation)
    {
        Relation = relation;
    }
}
