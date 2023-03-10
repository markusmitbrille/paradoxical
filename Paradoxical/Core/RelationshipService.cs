using System.Collections.Generic;

namespace Paradoxical.Core;

public interface IRelationshipService<TOwner, TRelation, TRelationship>
    where TOwner : IElement
    where TRelation : IElement
    where TRelationship : IRelationship
{
    IEnumerable<TRelation> GetRelations(TOwner owner);
    IEnumerable<TOwner> GetOwners(TRelation relation);

    void Add(TRelationship relationship);
    void Remove(TRelationship relationship);
}
