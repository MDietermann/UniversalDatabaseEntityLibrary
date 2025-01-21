namespace UniversalDatabaseEntity.DbHandling.QueryBuilder;

public interface IQueryBuilder
{
    public string SelectQuery(SearchParameters searchParameters);
    public string InsertQuery(UniversalEntity entity, string tableName);
    public string UpdateQuery(UniversalEntity entity, string tableName);
    public string DeleteQuery(UniversalEntity entity, string tableName);
}