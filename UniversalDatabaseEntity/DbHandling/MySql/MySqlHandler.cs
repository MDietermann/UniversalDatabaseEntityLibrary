using System.Data;
using MySqlConnector;
using UniversalDatabaseEntity.DbHandling.QueryBuilder;
using Exception = System.Exception;

namespace UniversalDatabaseEntity.DbHandling.MySql;

public class MySqlHandler : IDbHandler
{
    private MySqlConnection? _connection;

    /// <summary>
    ///     Retrieve a single entity from the database based on the given search parameters.
    /// </summary>
    /// <param name="factory">The factory to use for database operations.</param>
    /// <param name="searchParameters">The parameters to use for the search.</param>
    /// <returns>The retrieved entity, or null if no entity was found.</returns>
    public async Task<UniversalEntity?> GetEntity(UdeFactory factory, SearchParameters searchParameters)
    {
        await this.Connect(factory);
        IQueryBuilder queryBuilder = new QueryBuilderFactory().Create(factory.DatabaseType);

        try
        {
            // Build the query
            string sqlQuery = queryBuilder.SelectQuery(searchParameters);

            // Execute the query
            await using MySqlCommand command = new MySqlCommand(sqlQuery, this._connection);

            if (searchParameters.Parameters != null)
                foreach (
                    (int index, DynamicProperty<object> parameter) in searchParameters.Parameters.Select(
                        (p, i) => (i, p)))
                    command.Parameters.AddWithValue($"value{index}", parameter.GetValue());

            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows) return null;

            List<DynamicProperty<object>> properties = new List<DynamicProperty<object>>();

            while (await reader.ReadAsync())
                for (int index = 0; index < reader.FieldCount; index++)
                    properties.Add(new DynamicProperty<object>(reader.GetName(index), reader.GetValue(index)));

            return new UniversalEntity(properties);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLineAsync(ex.Message).GetAwaiter().GetResult();
            return null;
        }
    }

    /// <summary>
    ///     Retrieve a list of entities from the database based on the given search parameters.
    /// </summary>
    /// <param name="factory">The factory to use for database operations.</param>
    /// <param name="searchParameters">The parameters to use for the search.</param>
    /// <returns>A list of retrieved entities, or null if an error occurs.</returns>
    public async Task<List<UniversalEntity>?> GetEntities(UdeFactory factory, SearchParameters searchParameters)
    {
        // Ensure the database connection is established
        await this.Connect(factory: factory);

        try
        {
            // Build the SQL query using the query builder
            string query = new QueryBuilderFactory().Create(factory.DatabaseType).SelectQuery(searchParameters);

            // Execute the query
            await using MySqlCommand command = new MySqlCommand(query, _connection);

            // Add query parameters if any are defined in the search parameters
            if (searchParameters.Parameters != null)
            {
                foreach ((int index, DynamicProperty<object> parameter) in searchParameters.Parameters.Select((p, i) => (i, p)))
                {
                    command.Parameters.AddWithValue($"value{index}", parameter.GetValue());
                }
            }
        
            // Execute the command and read the results
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();

            List<UniversalEntity> entities = new List<UniversalEntity>();

            // Process each row in the result set
            while (await reader.ReadAsync())
            {
                List<DynamicProperty<object>> properties = new List<DynamicProperty<object>>();

                // Add each column as a dynamic property to the entity
                for (int index = 0; index < reader.FieldCount; index++)
                {
                    properties.Add(new DynamicProperty<object>(reader.GetName(index), reader.GetValue(index)));
                }

                // Add the entity to the list
                entities.Add(new UniversalEntity(properties));
            }

            // Close the connection
            await this._connection.CloseAsync();

            return entities;
        }
        catch (Exception exception)
        {
            // Log the error message and return null
            Console.Error.WriteLineAsync(exception.Message).GetAwaiter().GetResult();
            return null;
        }
    }

    public async Task AddEntity(UdeFactory factory, UniversalEntity entity, string tableName)
    {
        await this.Connect(factory: factory);
        IQueryBuilder queryBuilder = new QueryBuilderFactory().Create(factory.DatabaseType);

        try
        {
            string query = queryBuilder.InsertQuery(entity: entity, tableName: tableName);

            await using MySqlCommand command = new MySqlCommand(
                commandText: query,
                connection: this._connection
            );

            for (int index = 0; index < entity.GetEntityProperties().Count; index++)
            {
                DynamicProperty<object> property = entity.GetEntityProperties()[index: index];
                command.Parameters.AddWithValue(
                    parameterName: $"value{index}",
                    value: property.GetValue()
                );
            }

            await command.ExecuteNonQueryAsync();
            await this._connection!.CloseAsync();
        }
        catch (Exception exception)
        {
            Console.Error.WriteLineAsync(value: exception.Message).GetAwaiter().GetResult();
        }
    }

    public Task UpdateEntity(UdeFactory factory, UniversalEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteEntity(UdeFactory factory, UniversalEntity entity)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Establishes a connection to the database. If a connection has already been established,
    ///     this method will do nothing.
    /// </summary>
    /// <param name="factory">The factory to use for database operations.</param>
    private async Task Connect(UdeFactory factory)
    {
        // Check if a connection has already been established
        if (this._connection is not null && this._connection.State == ConnectionState.Open) return;
        try
        {
            // Create a connection string builder
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
            {
                Server = factory.Server,
                UserID = factory.UserId,
                Password = factory.Password,
                Database = factory.DatabaseName,
                Port = uint.Parse(s: factory.Port)
            };

            // Create a new MySqlConnection object
            this._connection = new MySqlConnection(connectionString: builder.ConnectionString);

            // Open the connection
            await this._connection.OpenAsync();
        }
        catch (Exception exception)
        {
            await Console.Error.WriteLineAsync(value: exception.Message);
        }
    }
}