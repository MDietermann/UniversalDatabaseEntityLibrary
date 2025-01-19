namespace UniversalDatabaseEntity;

public class UniversalEntity(List<DynamicProperty<object>> entityProperties) {
    private List<DynamicProperty<object>> EntityProperties { get; } = entityProperties;

    public DynamicProperty<object>? GetEntityProperty(string propertyName) {
        return this.EntityProperties.FirstOrDefault(predicate: x => x.GetKey() == propertyName);
    }

    public void SetEntityProperty(string propertyName, object propertyValue) {
        DynamicProperty<object>? property =
            this.EntityProperties.FirstOrDefault(predicate: x => x.GetKey() == propertyName);
        property?.SetValue(value: propertyValue);
    }

    public override string ToString() => string.Join(separator: ", ", values: this.EntityProperties);
}
