namespace L2Dn.Packages;

public class UObject: ISerializableObject
{
    public Dictionary<string, UProperty> Properties { get; } = new(StringComparer.OrdinalIgnoreCase);

    public void Read(UBinaryReader reader)
    {
        _typeRegistrations.TryGetValue(GetType(), out var typeRegistration);

        Properties.Clear();
        for (UProperty property = reader.ReadObject<UProperty>();
             property.Name != UName.NoneString;
             property = reader.ReadObject<UProperty>())
        {
            Properties[property.Name] = property;
            typeRegistration?.SetProperty(this, property.Name, property.Value);
        }
    }

    protected static void RegisterProperty<TObject>(string propertyName, Action<TObject, object> action)
        where TObject: UObject
    {
        TypeRegistration<TObject>.Properties.Add(propertyName, action);
        _typeRegistrations.TryAdd(typeof(TObject), new TypeRegistration<TObject>());
    }

    private static readonly Dictionary<Type, TypeRegistration> _typeRegistrations = new();

    private abstract class TypeRegistration
    {
        public abstract void SetProperty(UObject obj, string name, object value);
    }

    private class TypeRegistration<TObject>: TypeRegistration
        where TObject: UObject
    {
        public static readonly Dictionary<string, Action<TObject, object>> Properties =
            new(StringComparer.OrdinalIgnoreCase);

        public override void SetProperty(UObject obj, string name, object value)
        {
            if (Properties.TryGetValue(name, out Action<TObject, object>? action))
                action((TObject)obj, value);
        }
    }
}
