using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Salar Khalilzadeh <salar2k@gmail.com>
// © 2012, All rights reserved
// 2012/07/07
// ====================================
namespace SalarDbCodeGenerator.Schema.Database
{
	public class DbColumn
	{
		public enum ColumnCondensedType
		{
			None,
			String,
			Decimal,
			Integer
		}

		#region properties
		public bool AutoIncrement { get; set; }
		public string DataTypeDb { get; set; }
		public string DataTypeDotNet { get; set; }

		/// <summary>
		/// Determined special type of column
		/// </summary>
		public ColumnCondensedType DataCondensedType { get; set; }
		public int Length { get; set; }
		public bool PrimaryKey { get; set; }
		private bool _allowNull;
		public bool AllowNull
		{
			get { return _allowNull; }
			set
			{
				_allowNull = value;
				DataTypeNullable = value;
			}
		}
		public bool DataTypeNullable { get; set; }

		/// <summary>
		/// Datatype can not be converted by simple implitic converts
		/// </summary>
		public bool ExplicitCastDataType { get; set; }

		private string _fieldNameDb;

		/// <summary>
		/// Exact field name in database
		/// </summary>
		public string FieldNameDb
		{
			get { return _fieldNameDb; }
			set
			{
				_fieldNameDb = value;
				FieldNameSchema = _fieldNameDb;
			}
		}

		/// <summary>
		/// Field name in the schema, after duplicate checks. Renamed name.
		/// </summary>
		public string FieldNameSchema { get; set; }

		/// <summary>
		/// CHARACTER_MAXIMUM_LENGTH is actual len of the column, how many bytes it can save
		/// </summary>
		public int DataTypeMaxLength { get; set; }

		/// <summary>
		/// Len is maximum possible!?
		/// </summary>
		//public bool LengthIsMax { get; set; }

		/// <summary>
		/// Is referenced to other tables
		/// </summary>
		public bool IsReferenceKey { get; set; }

		/// <summary>
		/// Referenced to other tables
		/// </summary>
		public DbTable IsReferenceKeyTable { get; set; }

		public string Owner { get; set; }

		public int ColumnOrdinal { get; set; }
		public int NumericScale { get; set; }
		public int NumericPrecision { get; set; }

		/// <summary>
		/// Description which is applied in tables
		/// </summary>
		public string UserDescription { get; set; }

		//public string DataTypeDotNetClean { get; set; }

        public string SequenceName { get; set; }
		#endregion

		#region public methods
		public DbColumn(string fieldName)
		{
			FieldNameDb = fieldName;
		}
		public bool IsArray(string arrayIndicator)
		{
			return this.DataTypeDotNet.Contains(arrayIndicator);
		}
		#endregion

		#region protected methods
		public override string ToString()
		{
			return FieldNameDb + ", " + DataTypeDotNet;
		}
		#endregion

	}
}
