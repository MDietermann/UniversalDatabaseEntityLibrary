using System.Data;
using MySqlConnector;
using Exception = System.Exception;

namespace UniversalDatabaseEntity.DbHandling;

public class MySqlHandler : IDbHandler {
    private MySqlConnection? _connection;

    /// <summary>
    ///     Retrieve a single entity from the database based on the given search parameters.
    /// </summary>
    /// <param name="factory">The factory to use for database operations.</param>
    /// <param name="searchParameters">The parameters to use for the search.</param>
    /// <returns>The retrieved entity, or null if no entity was found.</returns>
    public async Task<UniversalEntity?>? GetEntity(
        UdeFactory factory,
        SearchParameters searchParameters) {
        await this.Connect(factory: factory);

        try {
            string query =
                $"SELECT * FROM {searchParameters.TableName} WHERE ";

            if (searchParameters.Parameters == null) return null;
            
            // Build the WHERE clause of the query
            for ( int index = 0; index < searchParameters.Parameters.Count; index++ ) {
                DynamicProperty<object> parameter = searchParameters.Parameters[index: index];
                query += $"{parameter.GetKey()} = @value{index}";

                if ( index < searchParameters.Parameters.Count - 1 ) query += " AND ";
            }

            query += ";";

            // Create a MySqlCommand object to execute the query
            await using MySqlCommand command = new MySqlCommand(
                commandText: query,
                connection: this._connection
            );

            // Add the parameter values to the command
            for ( int index = 0; index < searchParameters.Parameters.Count; index++ ) {
                DynamicProperty<object> parameter = searchParameters.Parameters[index: index];
                command.Parameters.AddWithValue(
                    parameterName: $"value{index}",
                    value: parameter.GetValue()
                );
            }

            // Execute the query and read the result
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();

            List<DynamicProperty<object>> properties = [];
            if ( !reader.HasRows ) return null;

            while ( await reader.ReadAsync() ) {
                for ( int index = 0; index < reader.FieldCount; index++ ) {
                    properties.Add(
                        item: new DynamicProperty<object>(
                            key: reader.GetName(ordinal: index),
                            value: reader.GetValue(ordinal: index)
                        ));
                }
            }

            await this._connection!.CloseAsync();
            // Return the retrieved entity
            return properties.Count <= 0 ? null : new UniversalEntity(entityProperties: properties);
        }
        catch ( Exception exception ) {
            Console.Error.WriteLineAsync(value: exception.Message).GetAwaiter().GetResult();
            return null;
        }
    }

    public async Task<List<UniversalEntity>?> GetEntities(UdeFactory factory, SearchParameters searchParameters) {
        await this.Connect(factory: factory);

        try {
            string query = $"SELECT * FROM {searchParameters!.TableName}";

            if ( searchParameters.Parameters != null ) {
                query += " WHERE ";
                for ( int index = 0; index < searchParameters.Parameters.Count; index++ ) {
                    DynamicProperty<object> parameter = searchParameters.Parameters[index: index];
                    query += $"{parameter.GetKey()} = @value{index}";

                    if ( index < searchParameters.Parameters.Count - 1 ) query += " AND ";
                }
            }

            query += ";";

            await using MySqlCommand command = new MySqlCommand(
                commandText: query,
                connection: this._connection
            );

            if ( searchParameters.Parameters != null ) {
                for ( int index = 0; index < searchParameters.Parameters.Count; index++ ) {
                    DynamicProperty<object> parameter = searchParameters.Parameters[index: index];
                    command.Parameters.AddWithValue(
                        parameterName: $"value{index}",
                        value: parameter.GetValue()
                    );
                }
            }

            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            List<UniversalEntity>? entities = [];
            while ( await reader.ReadAsync() ) {
                List<DynamicProperty<object>> properties = [];
                for ( int index = 0; index < reader.FieldCount; index++ ) {
                    properties.Add(
                        item: new DynamicProperty<object>(
                            key: reader.GetName(ordinal: index),
                            value: reader.GetValue(ordinal: index)
                        ));
                }

                entities.Add(item: new UniversalEntity(entityProperties: properties));
            }

            await this._connection!.CloseAsync();
            return entities;
        }
        catch ( Exception exception ) {
            Console.Error.WriteLineAsync(value: $"{exception.Message}: {exception.Source}").GetAwaiter().GetResult();
            return null;
        }
    }

    public async Task AddEntity(UdeFactory factory, UniversalEntity entity, string tableName) {
        await this.Connect(factory: factory);

        try {
            string query = $"INSERT INTO {tableName} (";
            for ( int index = 0; index < entity.GetEntityProperties().Count; index++ ) {
                DynamicProperty<object> property = entity.GetEntityProperties()[index: index];
                query += $"{property.GetKey()}";
                if ( index < entity.GetEntityProperties().Count - 1 ) {
                    query += ", ";
                }
            }

            query += ") VALUES (";
            for ( int index = 0; index < entity.GetEntityProperties().Count; index++ ) {
                DynamicProperty<object> property = entity.GetEntityProperties()[index: index];
                query += $"@value{index}";
                if ( index < entity.GetEntityProperties().Count - 1 ) {
                    query += ", ";
                }
            }

            query += ");";
            await using MySqlCommand command = new MySqlCommand(
                commandText: query,
                connection: this._connection
            );

            for ( int index = 0; index < entity.GetEntityProperties().Count; index++ ) {
                DynamicProperty<object> property = entity.GetEntityProperties()[index: index];
                command.Parameters.AddWithValue(
                    parameterName: $"value{index}",
                    value: property.GetValue()
                );
            }

            await command.ExecuteNonQueryAsync();
            await this._connection!.CloseAsync();
        }
        catch ( Exception exception ) {
            Console.Error.WriteLineAsync(value: exception.Message).GetAwaiter().GetResult();
        }
    }

    public Task UpdateEntity(UdeFactory factory, UniversalEntity entity) {
        throw new NotImplementedException();
    }

    public Task DeleteEntity(UdeFactory factory, UniversalEntity entity) {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Establishes a connection to the database. If a connection has already been established,
    ///     this method will do nothing.
    /// </summary>
    /// <param name="factory">The factory to use for database operations.</param>
    private async Task Connect(UdeFactory factory) {
        // Check if a connection has already been established
        if ( this._connection is not null && this._connection.State == ConnectionState.Open ) return;
        try {
            // Create a connection string builder
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder {
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
        catch ( Exception exception ) {
            await Console.Error.WriteLineAsync(value: exception.Message);
        }
    }
}
