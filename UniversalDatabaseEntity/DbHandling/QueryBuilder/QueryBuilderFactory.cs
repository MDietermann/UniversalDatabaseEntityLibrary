namespace UniversalDatabaseEntity.DbHandling.QueryBuilder;

public class QueryBuilderFactory
{
    private Dictionary<DbTypeEnum, IQueryBuilder> _builders = new Dictionary<DbTypeEnum, IQueryBuilder>
    {
        { DbTypeEnum.MySql, new MySqlQueryBuilder() },
    };

    public IQueryBuilder Create(DbTypeEnum type)
    {
        return _builders[type];
    }
}