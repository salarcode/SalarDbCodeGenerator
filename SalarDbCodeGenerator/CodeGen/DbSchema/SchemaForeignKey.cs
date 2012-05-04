using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalarDbCodeGenerator.CodeGen.DbSchema
{
	/// <summary>
	/// Foreign key relation details for table
	/// </summary>
	public class SchemaForeignKey
	{
		public enum ForeignKeyMultiplicity { Many, One }
		#region properties
		///// <summary>
		///// Current table name
		///// </summary>
		//internal string ForeignTableName { get; set; }

		/// <summary>
		/// Name of ForeignKey
		/// </summary>
		public string ForeignKeyName { get; set; }

		/// <summary>
		/// Represents the multiplicity information about the End of a relationship type.
		/// </summary>
		public ForeignKeyMultiplicity Multiplicity { get; set; }

		/// <summary>
		/// The column which is used as ForeignKey in current table
		/// </summary>
		public SchemaColumn LocalColumn { get; set; }

		/// <summary>
		/// The column which is used as ForeignKey in current table
		/// </summary>
		public string LocalColumnName { get; set; }

		/// <summary>
		/// Foreign column used for relation
		/// </summary>
		public SchemaColumn ForeignColumn { get; set; }

		/// <summary>
		/// Foreign column name used for relation
		/// </summary>
		public string ForeignColumnName { get; set; }

		/// <summary>
		/// Foreign table
		/// </summary>
		public SchemaTable ForeignTable { get; set; }

		private string _foreignTableName;

		/// <summary>
		/// Foreign table name
		/// </summary>
		public string ForeignTableName
		{
			get { return _foreignTableName; }
			set
			{
				_foreignTableName = value;
				ForeignTableNameAsField = value;
			}
		}

		/// <summary>
		/// Foreign table name as field
		/// </summary>
		public string ForeignTableNameAsField { get; set; }

		public override string ToString()
		{
			return ForeignKeyName + " " + ForeignTableName;
		}
		#endregion
	}
}
