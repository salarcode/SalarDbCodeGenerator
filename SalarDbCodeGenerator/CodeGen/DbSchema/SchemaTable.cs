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
namespace SalarSoft.DbCodeGenerator.CodeGen.DbSchema
{
	public class SchemaTable
	{
		public enum TableTypeInfo { Table, View }
		#region local variables
		#endregion

		#region field variables
		private string _tableName;
		private string _tableNameCaseSensitive;
		#endregion

		#region properties
		public bool Enabled { get; set; }
		public bool ReadOnly { get; set; }
		public TableTypeInfo TableType { get; set; }
		public List<SchemaColumn> SchemaColumns { get; private set; }
		public List<SchemaForeignKey> ForeignKeys { get; private set; }
		public List<SchemaConstraintKey> ConstraintKeys { get; private set; }
		public string TableSchemaName { get; set; }
		public string TableName
		{
			get { return _tableName; }
			private set
			{
				_tableName = value;
				TableNameCS = _tableName;
			}
		}

		/// <summary>
		/// TableName Case Sensitive.
		/// Some databases (e.g Oracle) allow case sensitive table names which results two table with same name.
		/// </summary>
		public string TableNameCS { get; set; }
		#endregion

		#region public methods
		public SchemaTable(string tableName)
		{
			TableName = tableName;
			SchemaColumns = new List<SchemaColumn>();
			ForeignKeys = new List<SchemaForeignKey>();
			ConstraintKeys = new List<SchemaConstraintKey>();
			TableType = TableTypeInfo.Table;
		}
		public SchemaTable(string tableName, List<SchemaColumn> schemaColumns)
		{
			TableName = tableName;
			SchemaColumns = schemaColumns;
			ForeignKeys = new List<SchemaForeignKey>();
			ConstraintKeys = new List<SchemaConstraintKey>();
			TableType = TableTypeInfo.Table;
		}
		public SchemaTable(string tableName, List<SchemaColumn> schemaColumns, List<SchemaForeignKey> foreignKeys)
		{
			TableName = tableName;
			SchemaColumns = schemaColumns;
			ForeignKeys = foreignKeys;
			ConstraintKeys = new List<SchemaConstraintKey>();
			TableType = TableTypeInfo.Table;
		}

		public SchemaTable(string tableName, List<SchemaColumn> schemaColumns, List<SchemaForeignKey> foreignKeys, List<SchemaConstraintKey> constraintKeys)
		{
			TableName = tableName;
			SchemaColumns = schemaColumns;
			ForeignKeys = foreignKeys;
			ConstraintKeys = constraintKeys;
			TableType = TableTypeInfo.Table;
		}

		/// <summary>
		/// Checks if this table has primary key or not
		/// </summary>
		public bool HasPrimaryKey()
		{
			foreach (var column in SchemaColumns)
			{
				if (column.PrimaryKey)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Checks if this table has auto increment column or not
		/// </summary>
		public bool HasAutoIncrement()
		{
			foreach (var column in SchemaColumns)
			{
				if (column.AutoIncrement)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Returns number of auto increment columns
		/// </summary>
		public int GetAutoIncrementCount()
		{
			int result = 0;
			foreach (var column in SchemaColumns)
			{
				if (column.AutoIncrement)
					result++;
			}
			return result;
		}
		/// <summary>
		/// Returns number of auto increment columns
		/// </summary>
		public SchemaColumn GetFirstAutoIncrementField()
		{
			foreach (var column in SchemaColumns)
			{
				if (column.AutoIncrement)
					return column;
			}
			return null;
		}

		public SchemaColumn GetPrimaryKey()
		{
			foreach (var column in SchemaColumns)
			{
				if (column.PrimaryKey)
					return column;
			}
			return null;
		}

		public SchemaColumn FindColumn(string fieldName)
		{
			foreach (var column in SchemaColumns)
			{
				if (column.FieldName == fieldName)
					return column;
			}
			return null;
		}
		public List<SchemaColumn> FindColumns(string fieldName)
		{
			var result = new List<SchemaColumn>();
			foreach (var column in SchemaColumns)
			{
				if (column.FieldName == fieldName)
					result.Add(column);
			}
			return result;
		}
		#endregion

		#region protected methods
		public override string ToString()
		{
			return this.TableType.ToString() + " " + this.TableName;
		}
		#endregion

		#region private methods
		#endregion

	}
}
