using System.Data;
using MySqlConnector;
using Exception = System.Exception;
using UniversalDatabaseEntity.DbHandling;

namespace UniversalDatabaseEntity.DbHandling;

public class MySqlHandler : IDbHandler {
    private MySqlConnection? _connection;

    /// <summary>
    ///     Establishes a connection to the database. If a connection has already been established,
    ///     this method will do nothing.
    /// </summary>
    /// <param name="factory">The factory to use for database operations.</param>
    public void Connect(UdeFactory factory) {
        // Check if a connection has already been established
        if (this._connection is null || this._connection.State != ConnectionState.Open) {
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
                this._connection.Open();
            }
            catch (Exception exception) {
                Console.Error.WriteLine(value: exception.Message);
            }
        }
    }

    /// <summary>
    ///     Retrieve a single entity from the database based on the given search parameters.
    /// </summary>
    /// <param name="factory">The factory to use for database operations.</param>
    /// <param name="searchParameters">The parameters to use for the search.</param>
    /// <returns>The retrieved entity, or null if no entity was found.</returns>
    public UniversalEntity? GetEntity(
        UdeFactory factory,
        SearchParameters searchParameters) {
        this.Connect(factory: factory);

        string query =
            $"SELECT * FROM {searchParameters.TableName} WHERE ";

        // Build the WHERE clause of the query
        for ( int index = 0; index < searchParameters.Parameters.Count; index++ ) {
            DynamicProperty<object> parameter = searchParameters.Parameters[index];
            query += $"{parameter.GetKey()} = '@value{index}'";

            if ( index < searchParameters.Parameters.Count - 1 ) {
                query += " AND ";
            }
        }

        query += ";";
        
        // Create a MySqlCommand object to execute the query
        using MySqlCommand command = new MySqlCommand(
            commandText: query,
            connection: this._connection
        );

        // Add the parameter values to the command
        for ( int index = 0; index < searchParameters.Parameters.Count; index++ ) {
            DynamicProperty<object> parameter = searchParameters.Parameters[index];
            command.Parameters.AddWithValue(
                parameterName: $"value{index}",
                value: parameter.GetValue()
            );
        }
        
        // Execute the query and read the result
        using MySqlDataReader reader = command.ExecuteReader();
        if ( !reader.HasRows ) return null;

        List<DynamicProperty<object>> properties = new List<DynamicProperty<object>>();
        while ( reader.Read() ) {
            for ( int index = 0; index < reader.FieldCount; index++ ) {
                properties.Add(
                    new DynamicProperty<object>(
                        key: reader.GetName(index),
                        value: reader.GetValue(index)
                ));
            }           
        }
        
        this._connection!.Close();
        // Return the retrieved entity
        return properties.Count <= 0 ? null : new UniversalEntity(properties);
    }

    public List<UniversalEntity> GetEntities(UdeFactory factory, SearchParameters searchParameters) => throw new NotImplementedException();

    public void AddEntity(UdeFactory factory, UniversalEntity entity) {
        throw new NotImplementedException();
    }

    public void UpdateEntity(UdeFactory factory, UniversalEntity entity) {
        throw new NotImplementedException();
    }

    public void DeleteEntity(UdeFactory factory, UniversalEntity entity) {
        throw new NotImplementedException();
    }
}
