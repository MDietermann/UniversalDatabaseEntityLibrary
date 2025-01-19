namespace UniversalDatabaseEntity;

public class DynamicProperty<T>(string key, T value) {
    private string Key { get; set; } = key;
    private T Value { get; set; } = value;

    public string GetKey() => this.Key;
    public T GetValue() => this.Value;

    public void SetKey(string key) => this.Key = key;
    public void SetValue(T value) => this.Value = value;

    public override string ToString() => $"{this.Key}: {this.Value}";
}
