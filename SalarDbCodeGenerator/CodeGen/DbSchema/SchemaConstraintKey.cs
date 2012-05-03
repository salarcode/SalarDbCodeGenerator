using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalarSoft.DbCodeGenerator.CodeGen.DbSchema
{
	/// <summary>
	/// Constraint key details for table
	/// </summary>
	public class SchemaConstraintKey
	{
		#region properties
		/// <summary>
		/// Name of ForeignKey
		/// </summary>
		public string KeyName { get; set; }

		/// <summary>
		/// The column name used 
		/// </summary>
		public string KeyColumnName { get; set; }

		/// <summary>
		/// The column name used 
		/// </summary>
		public SchemaColumn KeyColumn { get; set; }

		/// <summary>
		/// Is constraint key
		/// </summary>
		public bool IsUnique { get; set; }
		#endregion

		public override string ToString()
		{
			return KeyName + ", " + KeyColumnName;
		}
	}
}
