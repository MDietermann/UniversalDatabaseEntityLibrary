namespace UniversalDatabaseEntity.DbHandling;

public interface IDbHandler {
    public Task<UniversalEntity?>? GetEntity(UdeFactory factory, SearchParameters searchParameters);
    public Task<List<UniversalEntity>?> GetEntities(UdeFactory factory, SearchParameters searchParameters);
    public Task AddEntity(UdeFactory factory, UniversalEntity entity, string tableName);
    public Task UpdateEntity(UdeFactory factory, UniversalEntity entity);
    public Task DeleteEntity(UdeFactory factory, UniversalEntity entity);
}
