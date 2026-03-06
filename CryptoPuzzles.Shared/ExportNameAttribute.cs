namespace CryptoPuzzles.Shared
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExportNameAttribute : Attribute
    {
        public string Name { get; }

        public ExportNameAttribute(string name)
        {
            Name = name;
        }
    }
}
