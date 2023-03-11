using Paradoxical.Messages;
using Paradoxical.Services;
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

public abstract class RelationshipService<TOwner, TRelation, TRelationship> : IRelationshipService<TOwner, TRelation, TRelationship>
    where TOwner : IElement, new()
    where TRelation : IElement, new()
    where TRelationship : IRelationship
{
    public IDataService Data { get; }
    public IMediatorService Mediator { get; }

    protected abstract string OwnerTable { get; }
    protected abstract string OwnerPrimaryKey { get; }
    protected abstract string OwnerForeignKey { get; }

    protected abstract string RelationTable { get; }
    protected abstract string RelationPrimaryKey { get; }
    protected abstract string RelationForeignKey { get; }

    protected abstract string RelationshipTable { get; }

    public RelationshipService(
        IDataService data,
        IMediatorService mediator)
    {
        Data = data;
        Mediator = mediator;
    }

    public IEnumerable<TRelation> GetRelations(TOwner owner)
    {
        string query = ParadoxQuery.Collection(
            m: RelationTable,
            n: OwnerTable,
            mn: RelationshipTable,
            mfk: RelationForeignKey,
            nfk: OwnerForeignKey,
            mpk: RelationPrimaryKey,
            npk: OwnerPrimaryKey);

        return Data.Connection.Query<TRelation>(query, owner.Id);
    }
    public IEnumerable<TOwner> GetOwners(TRelation relation)
    {
        string query = ParadoxQuery.Collection(
            m: OwnerTable,
            n: RelationTable,
            mn: RelationshipTable,
            mfk: OwnerForeignKey,
            nfk: RelationForeignKey,
            mpk: OwnerPrimaryKey,
            npk: RelationPrimaryKey);

        return Data.Connection.Query<TOwner>(query, relation.Id);
    }

    public void Add(TRelationship relationship)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: RelationshipTable,
            mfk: OwnerForeignKey,
            nfk: RelationForeignKey);

        Data.Connection.Execute(query, relationship.OwnerID, relationship.RelationID);

        Mediator.Send<RelationAddedMessage>(new(relationship));
    }
    public void Remove(TRelationship relationship)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: RelationshipTable,
            mfk: OwnerForeignKey,
            nfk: RelationForeignKey);

        Data.Connection.Execute(query, relationship.OwnerID, relationship.RelationID);

        Mediator.Send<RelationRemovedMessage>(new(relationship));
    }
}
