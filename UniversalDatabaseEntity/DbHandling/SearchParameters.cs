namespace UniversalDatabaseEntity.DbHandling;

public class SearchParameters {
    public SearchParameters(string tableName, List<DynamicProperty<object>> parameters) {
        this.TableName = tableName;
        this.Parameters = parameters;
    }
    public string TableName { get; set; }
    public List<DynamicProperty<object>> Parameters { get; set; }
}
