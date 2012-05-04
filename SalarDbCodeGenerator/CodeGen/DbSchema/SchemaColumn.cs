using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2009-9-30
// ====================================
namespace SalarDbCodeGenerator.CodeGen.DbSchema
{
	public class SchemaColumn
	{
		#region local variables
		#endregion

		#region field variables
		public const string DotNetArrayIdenticator = "[]";
		#endregion

		#region properties
		public bool AutoIncrement { get; set; }
		public string DbType { get; set; }
		public string DotNetType { get; set; }
		public string FieldName { get; set; }
		public int Length { get; set; }
		public bool Nullable { get; set; }
		public bool PrimaryKey { get; set; }

		/// <summary>
		/// CHARACTER_MAXIMUM_LENGTH is actual len of the column, how many bytes it can save
		/// </summary>
		public int CharacterMaxLength { get; set; }

		/// <summary>
		/// Len is maximum possible!?
		/// </summary>
		public bool LengthIsMax { get; set; }


		/// <summary>
		/// Is referenced to other tables
		/// </summary>
		public bool IsReferenceKey { get; set; }

		/// <summary>
		/// Referenced to other tables
		/// </summary>
		public SchemaTable IsReferenceKeyTable { get; set; }

		public string Owner { get; set; }

		public int ColumnOrdinal { get; set; }
		public int NumericScale { get; set; }
		public int NumericPrecision { get; set; }

		/// <summary>
		/// Description which is applied in tables
		/// </summary>
		public string UserDescription { get; set; }

		public string DotNetTypeClean
		{
			get
			{
				string _dotNetType = DotNetType;
				if (string.IsNullOrEmpty(_dotNetType))
					return _dotNetType;
				if (_dotNetType.StartsWith("System."))
					return _dotNetType.Remove(0, "System.".Length);
				else
					return _dotNetType;
			}
		}
		#endregion

		#region public methods
		public SchemaColumn(string fieldName)
		{
			FieldName = fieldName;
		}
		public bool IsArray()
		{
			return this.DotNetType.Contains(DotNetArrayIdenticator);
		}
		#endregion

		#region protected methods
		public override string ToString()
		{
			return FieldName + ", " + DotNetTypeClean;
		}
		#endregion

		#region private methods
		#endregion

	}
}
