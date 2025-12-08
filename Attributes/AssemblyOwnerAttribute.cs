namespace ScalarDemo.Attributes;

[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyOwnerAttribute(string ownerName) : Attribute
{
	public string OwnerName { get; } = ownerName;
}
