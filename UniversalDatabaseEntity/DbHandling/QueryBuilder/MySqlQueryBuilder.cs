

using System.Text;

namespace UniversalDatabaseEntity.DbHandling.QueryBuilder;

public class MySqlQueryBuilder : IQueryBuilder
{
    public string SelectQuery(SearchParameters searchParameters)
    {
        string tableName = searchParameters.TableName;
        List<DynamicProperty<object>>? parameters = searchParameters.Parameters;

        StringBuilder queryBuilder = new($"SELECT * FROM {tableName}");

        if (parameters == null) return $"{queryBuilder};";

        queryBuilder.Append(" WHERE ");

        for (int index = 0; index < parameters.Count; index++)
        {
            DynamicProperty<object> parameter = parameters[index];
            queryBuilder.Append($"{parameter.GetKey()} = @value{index}");

            if (index < parameters.Count - 1) queryBuilder.Append(" AND ");
        }

        return $"{queryBuilder};";
    }

    public string InsertQuery(UniversalEntity entity, string tableName)
    {
        string columnNames = string.Join(", ", 
            entity.GetEntityProperties().Select(p => p.GetKey()));
        
        string parameterNames = string.Join(", ", 
            Enumerable.Range(0, entity.GetEntityProperties().Count).Select(i => $"@value{i}"));

        return $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameterNames});";
    }

    public string UpdateQuery(UniversalEntity entity, string tableName)
    {
        throw new NotImplementedException();
    }

    public string DeleteQuery(UniversalEntity entity, string tableName)
    {
        throw new NotImplementedException();
    }
}