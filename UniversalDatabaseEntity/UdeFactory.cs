using UniversalDatabaseEntity.DbHandling;

namespace UniversalDatabaseEntity;

/// <summary>
///     This class provides a factory for creating and manipulating database entities.
/// </summary>
public class UdeFactory {
    /// <summary>
    ///     Constructor for the UdeFactory class.
    /// </summary>
    /// <param name="databaseType">The type of database to connect to.</param>
    /// <param name="server">The server to connect to.</param>
    /// <param name="userId">The user ID to use for the connection.</param>
    /// <param name="password">The password to use for the connection.</param>
    /// <param name="databaseName">The name of the database to connect to.</param>
    /// <param name="port">The port number to use for the connection.</param>
    public UdeFactory(
        DbTypeEnum databaseType,
        string server,
        string userId,
        string password,
        string databaseName,
        string port) {
        this.DatabaseType = databaseType;
        this.Server = server;
        this.UserId = userId;
        this.Password = password;
        this.DatabaseName = databaseName;
        this.Port = port;

        this.DbHandler.Add(key: DbTypeEnum.MySql, value: new MySqlHandler());
    }

    /// <summary>
    ///     The type of database to connect to.
    /// </summary>
    private DbTypeEnum DatabaseType { get; }

    /// <summary>
    ///     The server to connect to.
    /// </summary>
    public string Server { get; }

    /// <summary>
    ///     The user ID to use for the connection.
    /// </summary>
    public string UserId { get; }

    /// <summary>
    ///     The password to use for the connection.
    /// </summary>
    public string Password { get; }

    /// <summary>
    ///     The name of the database to connect to.
    /// </summary>
    public string DatabaseName { get; }

    /// <summary>
    ///     The port number to use for the connection.
    /// </summary>
    public string Port { get; }

    /// <summary>
    ///     A dictionary of database handlers.
    /// </summary>
    private Dictionary<DbTypeEnum, IDbHandler> DbHandler { get; } = new Dictionary<DbTypeEnum, IDbHandler>();

    /// <summary>
    ///     Get an entity from the database.
    /// </summary>
    /// <param name="searchParameters">The search parameters to use when retrieving the entity.</param>
    /// <returns>The entity retrieved from the database.</returns>
    public UniversalEntity? GetEntity(SearchParameters searchParameters) 
        => this.DbHandler[key: this.DatabaseType].GetEntity(factory: this, searchParameters: searchParameters)!.Result;

    /// <summary>
    ///     Get a list of entities from the database.
    /// </summary>
    /// <param name="searchParameters">The search parameters to use when retrieving the entities.</param>
    /// <returns>A list of entities retrieved from the database.</returns>
    public List<UniversalEntity> GetEntities(SearchParameters searchParameters) => this
        .DbHandler[key: this.DatabaseType].GetEntities(factory: this, searchParameters: searchParameters);

    /// <summary>
    ///     Add an entity to the database.
    /// </summary>
    /// <param name="entity">The entity to add to the database.</param>
    public void AddEntity(UniversalEntity entity) {
        this.DbHandler[key: this.DatabaseType].AddEntity(factory: this, entity: entity);
    }

    /// <summary>
    ///     Update an entity in the database.
    /// </summary>
    /// <param name="entity">The entity to update in the database.</param>
    public void UpdateEntity(UniversalEntity entity) {
        this.DbHandler[key: this.DatabaseType].UpdateEntity(factory: this, entity: entity);
    }

    /// <summary>
    ///     Delete an entity from the database.
    /// </summary>
    /// <param name="entity">The entity to delete from the database.</param>
    public void DeleteEntity(UniversalEntity entity) {
        this.DbHandler[key: this.DatabaseType].DeleteEntity(factory: this, entity: entity);
    }
}
