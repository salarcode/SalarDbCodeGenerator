
namespace SalarDbCodeGenerator.Schema.DbSchemaReaders
{
	public class DbSchemaNames
	{
		public static string FieldName_RemoveInvalidChars(string name)
		{
			if (string.IsNullOrEmpty(name))
				return name;
			return name.Replace(' ', '_').Replace('.', '_');
		}
	}
}
