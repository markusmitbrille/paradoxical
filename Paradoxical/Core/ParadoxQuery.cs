namespace Paradoxical.Core;

public static class ParadoxQuery
{
    /// <summary>
    /// For two tables with an m:n relationship, this will generate a query that selects all m for one specific n.
    /// The query expects an additional argument for the primary key of the specific n.
    /// </summary>
    /// <param name="m">Name of the m-table.</param>
    /// <param name="n">Name of the n-table.</param>
    /// <param name="mn">Name of the m:n intermediary table.</param>
    /// <param name="mfk">Name of the foreign key, pointing to the m-table primary key, in the m:n intermediary table.</param>
    /// <param name="nfk">Name of the foreign key, pointing to the n-table primary key, in the m:n intermediary table.</param>
    /// <param name="mpk">Name of the m-table primary key.</param>
    /// <param name="npk">Name of the n-table primary key.</param>
    /// <returns></returns>
    public static string Collection(
        string m,
        string n,
        string mn,
        string mfk,
        string nfk,
        string mpk,
        string npk)
    {
        return $@"SELECT {m}.* FROM {m} JOIN {mn} on {m}.{mpk} = {mn}.{mfk} JOIN {n} on {n}.{npk} = {mn}.{nfk} WHERE {n}.{npk} = ?";
    }

    /// <summary>
    /// For two tables with an m:n relationship, this will generate a query that adds a relationship.
    /// The query expects additional arguments for the foreign key values pointing to the m-table, and n-table primary keys.
    /// </summary>
    /// <param name="mn">Name of the m:n intermediary table.</param>
    /// <param name="mfk">Name of the foreign key, pointing to the m-table primary key, in the m:n intermediary table.</param>
    /// <param name="nfk">Name of the foreign key, pointing to the n-table primary key, in the m:n intermediary table.</param>
    /// <returns></returns>
    public static string CollectionAdd(
        string mn,
        string mfk,
        string nfk)
    {
        return $@"INSERT INTO {mn} ({mfk}, {nfk}) VALUES (?, ?)";
    }

    /// <summary>
    /// For two tables with an m:n relationship, this will generate a query that removes a relationship.
    /// The query expects additional arguments for the foreign key values pointing to the m-table, and n-table primary keys.
    /// </summary>
    /// <param name="mn">Name of the m:n intermediary table.</param>
    /// <param name="mfk">Name of the foreign key, pointing to the m-table primary key, in the m:n intermediary table.</param>
    /// <param name="nfk">Name of the foreign key, pointing to the n-table primary key, in the m:n intermediary table.</param>
    /// <returns></returns>
    public static string CollectionRemove(
        string mn,
        string mfk,
        string nfk)
    {
        return $@"DELETE FROM {mn} WHERE {mfk} = ? AND {nfk} = ?";
    }

    /// <summary>
    /// For two tables with a 1:n relationship, this will generate a query that selects all components for a specific owner.
    /// The query expects an additional argument for the owner primary key.
    /// </summary>
    /// <param name="c">Name of the component table.</param>
    /// <param name="o">Name of the owner table.</param>
    /// <param name="fk">Name of the foreign key, pointing to the owner table primary key, in the component table.</param>
    /// <param name="pk">Name of the owner table primary key.</param>
    /// <returns></returns>
    public static string Composition(
        string c,
        string o,
        string fk,
        string pk)
    {
        return $@"SELECT {c}.* FROM {c} JOIN {o} on {c}.{fk} = {o}.{pk} WHERE {o}.{pk} = ?";
    }

    /// <summary>
    /// For two tables with a 1:n relationship, this will generate a query that selects the owner of a specific component.
    /// The query expects an additional argument for the component primary key.
    /// </summary>
    /// <param name="c">Name of the component table.</param>
    /// <param name="o">Name of the owner table.</param>
    /// <param name="fk">Name of the foreign key, pointing to the owner table primary key, in the component table.</param>
    /// <param name="cpk">Name of the component table primary key.</param>
    /// <param name="opk">Name of the owner table primary key.</param>
    /// <returns></returns>
    public static string Composite(
        string c,
        string o,
        string fk,
        string cpk,
        string opk)
    {
        return $@"SELECT {o}.* FROM {c} JOIN {o} on {c}.{fk} = {o}.{opk} WHERE {c}.{cpk} = ? LIMIT 1";
    }
}
