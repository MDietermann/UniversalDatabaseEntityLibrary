namespace UniversalDatabaseEntity.DbHandling;

public interface IDbHandler {
    public void Connect(UdeFactory factory);
    public UniversalEntity? GetEntity(UdeFactory factory, SearchParameters searchParameters);
    public List<UniversalEntity> GetEntities(UdeFactory factory, SearchParameters searchParameters);
    public void AddEntity(UdeFactory factory, UniversalEntity entity);
    public void UpdateEntity(UdeFactory factory, UniversalEntity entity);
    public void DeleteEntity(UdeFactory factory, UniversalEntity entity);
}
