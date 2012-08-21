using SalarDbCodeGenerator.Schema.Database;

namespace SalarDbCodeGenerator.Schema
{
	public class DotNetSchemaDataInfo
	{
		public const string DotNetArrayIdenticator = "[]";

		public static DbColumn.ColumnCondensedType DetermineColumnCondensedType(string dotNetDataType)
		{
			switch (dotNetDataType)
			{
				case "System.String":
					return DbColumn.ColumnCondensedType.String;
				case "System.Int16":
					return DbColumn.ColumnCondensedType.Integer;
				case "System.Int32":
					return DbColumn.ColumnCondensedType.Integer;
				case "System.Int64":
					return DbColumn.ColumnCondensedType.Integer;
				case "System.Byte":
					return DbColumn.ColumnCondensedType.Integer;
				case "System.Decimal":
					return DbColumn.ColumnCondensedType.Decimal;
				case "System.Single":
					return DbColumn.ColumnCondensedType.Decimal;
				case "System.Double":
					return DbColumn.ColumnCondensedType.Decimal;
			}
			return DbColumn.ColumnCondensedType.None;
		}
	}
}
