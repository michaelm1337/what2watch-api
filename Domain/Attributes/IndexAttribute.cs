namespace Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IndexAttribute : Attribute
    {
        public bool IsUnique { get; set; } = false;
    }
}
