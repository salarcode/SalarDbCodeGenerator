using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Salar Khalilzadeh <salar2k@gmail.com>
// © 2012, All rights reserved
// 2012/07/06
// ====================================
namespace SalarDbCodeGenerator.Schema.Database
{
	public class DbTable
	{
		public enum TableTypeInfo { Table, View }

		#region field variables
		private string _tableName;
		#endregion

		#region properties
		public bool Enabled { get; set; }
		public bool ReadOnly { get; set; }
		public TableTypeInfo TableType { get; set; }
		public List<DbColumn> SchemaColumns { get; private set; }
		public List<DbForeignKey> ForeignKeys { get; private set; }
		public List<DbConstraintKey> ConstraintKeys { get; private set; }
		public string OwnerName { get; set; }
		public string TableNameSchema { get; set; }
		public string TableName
		{
			get { return _tableName; }
			private set
			{
				_tableName = value;
				TableNameSchema = _tableName;
				TableNameCS = _tableName;
			}
		}

		private string _tableNameCs;

		/// <summary>
		/// TableName Case Sensitive.
		/// Some databases (e.g Oracle) allow case sensitive table names which results two table with same name.
		/// </summary>
		public string TableNameCS
		{
			get { return _tableNameCs; }
			set
			{
				_tableNameCs = value;
				TableNameSchemaCS = value;
			}
		}

		public string TableNameSchemaCS { get; set; }
		#endregion

		#region public methods
		public DbTable(string tableName)
		{
			TableName = tableName;
			SchemaColumns = new List<DbColumn>();
			ForeignKeys = new List<DbForeignKey>();
			ConstraintKeys = new List<DbConstraintKey>();
			TableType = TableTypeInfo.Table;
		}
		public DbTable(string tableName, List<DbColumn> schemaColumns)
		{
			TableName = tableName;
			SchemaColumns = schemaColumns;
			ForeignKeys = new List<DbForeignKey>();
			ConstraintKeys = new List<DbConstraintKey>();
			TableType = TableTypeInfo.Table;
		}
		public DbTable(string tableName, List<DbColumn> schemaColumns, List<DbForeignKey> foreignKeys)
		{
			TableName = tableName;
			SchemaColumns = schemaColumns;
			ForeignKeys = foreignKeys;
			ConstraintKeys = new List<DbConstraintKey>();
			TableType = TableTypeInfo.Table;
		}

		public DbTable(string tableName, List<DbColumn> schemaColumns, List<DbForeignKey> foreignKeys, List<DbConstraintKey> constraintKeys)
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
		public DbColumn GetFirstAutoIncrementField()
		{
			foreach (var column in SchemaColumns)
			{
				if (column.AutoIncrement)
					return column;
			}
			return null;
		}

		public DbColumn GetPrimaryKey()
		{
			foreach (var column in SchemaColumns)
			{
				if (column.PrimaryKey)
					return column;
			}
			return null;
		}

		public DbColumn FindColumnDb(string fieldName)
		{
			foreach (var column in SchemaColumns)
			{
				if (column.FieldNameDb == fieldName)
					return column;
			}
			return null;
		}

		public DbColumn FindColumnSchema(string fieldName)
		{
			foreach (var column in SchemaColumns)
			{
				if (column.FieldNameSchema == fieldName)
					return column;
			}
			return null;
		}

		List<DbColumn> FindColumns__(string fieldName)
		{
			var result = new List<DbColumn>();
			foreach (var column in SchemaColumns)
			{
				if (column.FieldNameDb == fieldName)
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
