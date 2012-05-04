using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using SalarDbCodeGenerator.CodeGen.DbSchema;
using SalarDbCodeGenerator.DbProject;

namespace SalarDbCodeGenerator.CodeGen.SchemaEngines
{
	public class SQLCeSchemaEngine : ExSchemaEngine
	{
		/* http://arjunachith.blogspot.com/2007/08/retrieving-schema-information-of-sql-ce.html
		 * Following are some views available in SQL CE which you can use to retrieve information about the tables in the database.

		-- Gets the list of Columns with their associated Tables. You will be able to retrieve the column properties such as Data Type, Allow Null, Default Values, etc.
		SELECT * FROM INFORMATION_SCHEMA.COLUMNS

		-- Retrieves information about the indexes contained in the database.
		SELECT * FROM INFORMATION_SCHEMA.INDEXES

		-- Retrieves information about the Primary Keys used in the tables.
		SELECT * FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE

		-- Retrieves all the Tables in the database including the System tables.
		SELECT * FROM INFORMATION_SCHEMA.TABLES

		-- Retrieves all the Constrains in the database.
		SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS

		-- Retrieves all the Data Types available in the database.
		SELECT * FROM INFORMATION_SCHEMA.PROVIDER_TYPES

		-- If you have any referential integrity constraints in your database defined, those will be returned by this.
		SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS 

		 */


		struct SqlCeDataTypes
		{
			public string TypeName;
			public long ColumnSize;
			public string CreateFormat;
			public string DotNetDataType;
			public bool IsAutoincrementable;
		}
		#region local variables
		private SqlCeConnection _dbConnection;
		private List<SqlCeDataTypes> _sqlCeDataTypes;
		#endregion

		#region properties
		#endregion

		#region public methods
		public SQLCeSchemaEngine(DbConnection dbConnection)
		{
			_dbConnection = (SqlCeConnection)dbConnection;
		}

		/// <summary>
		/// Reads list of tables and views
		/// </summary>
		public override void ReadViewsTablesList(out StringCollection tables, out StringCollection views)
		{
			views = ReadViewsList();
			tables = ReadTablesList(views);
		}

		/// <summary>
		/// Reads full database schema
		/// </summary>
		public override void FillSchema(SchemaDatabase schemaDatabase)
		{
			if (schemaDatabase == null)
				throw new ArgumentNullException("schemaDatabase", "Database is not specifed.");

			try
			{
				if (_sqlCeDataTypes == null)
					_sqlCeDataTypes = ReadSqlCeDataTypes();

				schemaDatabase.SchemaViews = ReadViews();
				schemaDatabase.SchemaTables = ReadTables(schemaDatabase.SchemaViews);

			}
			finally
			{
				// be sure the connection is closed
				_dbConnection.Close();
			}
		}

		public override string GetDataProviderClassName(DataProviderClassNames providerClassName)
		{
			switch (providerClassName)
			{
				case DataProviderClassNames.ClassPrefix:
					return "SqlCe";
				case DataProviderClassNames.ClassCommand:
					return "SqlCeCommand";
				case DataProviderClassNames.ClassConnection:
					return "SqlCeConnection";
				case DataProviderClassNames.ClassDataAdapter:
					return "SqlCeDataAdapter";
				case DataProviderClassNames.ClassDataReader:
					return "SqlCeDataReader";
				case DataProviderClassNames.ClassParameter:
					return "SqlCeParameter";
				case DataProviderClassNames.ClassTransaction:
					return "SqlCeTransaction";
				case DataProviderClassNames.ClassNamespace:
					return "System.Data.SqlServerCe";
				case DataProviderClassNames.AssemblyReference:
					return "System.Data.SqlServerCe";
				case DataProviderClassNames.StoredProcParamPrefix:
					return "@";
				default:
					return "";
			}
		}
		#endregion

		#region private methods
		/// <summary>
		/// Reads tables list. This method requires views list to prevent from conflict!
		/// </summary>
		private StringCollection ReadTablesList(StringCollection viewsList)
		{
			StringCollection result = new StringCollection();

			if (_dbConnection.State != ConnectionState.Open)
				_dbConnection.Open();
			try
			{
				using (DataTable views = _dbConnection.GetSchema("TABLES"))
				{
					foreach (DataRow row in views.Rows)
					{
						string tableName = row["TABLE_NAME"].ToString();

						// search in views about this
						if (viewsList.Contains(tableName))
							continue;

						// add to results
						result.Add(tableName);
					}
				}
			}
			finally
			{
				_dbConnection.Close();
			}
			return result;
		}

		/// <summary>
		/// Reads views list
		/// </summary>
		private StringCollection ReadViewsList()
		{
			return new StringCollection();
		}

		private List<SqlCeDataTypes> ReadSqlCeDataTypes()
		{
			List<SqlCeDataTypes> result = new List<SqlCeDataTypes>();

			if (_dbConnection.State != ConnectionState.Open)
				_dbConnection.Open();
			try
			{
				using (DataTable types = _dbConnection.GetSchema("DataTypes"))
				{
					foreach (DataRow row in types.Rows)
					{
						var type = new SqlCeDataTypes()
									{
										ColumnSize = Common.TryConvertInt64(row["ColumnSize"].ToString(), 0),
										TypeName = row["TypeName"].ToString(),
										CreateFormat = row["CreateFormat"].ToString(),
										DotNetDataType = row["DataType"].ToString(),
										IsAutoincrementable = Convert.ToBoolean(row["IsAutoincrementable"])
									};
						result.Add(type);
					}
				}
			}
			finally
			{
				_dbConnection.Close();
			}
			return result;
		}



		/// <summary>
		/// Reads tables schema from database
		/// </summary>
		private List<SchemaTable> ReadTables(List<SchemaView> viewList)
		{
			List<SchemaTable> result = new List<SchemaTable>();

			if (_dbConnection.State != ConnectionState.Open)
				_dbConnection.Open();
			try
			{
				using (DataTable tables = _dbConnection.GetSchema("Tables"))
				{
					foreach (DataRow row in tables.Rows)
					{
						string tableName = row["TABLE_NAME"].ToString();

						// search in views about this
						foreach (var view in viewList)
						{
							if (view.TableName == tableName)
							{
								// we ignore view here
								continue;
							}
						}

						// View columns
						List<SchemaColumn> columns = ReadColumns(tableName);

						// read columns description
						//if (ReadColumnsDescription)
						//	ApplyColumnsDescription(tableName, columns);

						// new table
						SchemaTable dbTable = new SchemaTable(tableName, columns);

						// table schema
						dbTable.TableSchemaName = row["TABLE_SCHEMA"].ToString();

						// add to results
						result.Add(dbTable);
					}

					// it is time to read foreign keys!
					// foreign keys are requested?
					if (ReadTablesForeignKeys)
						ApplyTablesForeignKeys(result);

				}
			}
			finally
			{
				_dbConnection.Close();
			}
			return result;
		}

		/// <summary>
		/// Reads views schema from database
		/// </summary>
		private List<SchemaView> ReadViews()
		{
			return new List<SchemaView>();
		}

		/// <summary>
		/// Read columns schema from database
		/// </summary>
		private List<SchemaColumn> ReadColumns(string tableName)
		{
			List<SchemaColumn> result = new List<SchemaColumn>();

			if (_dbConnection.State != ConnectionState.Open)
				_dbConnection.Open();

			var cmd = string.Format("SELECT * FROM information_schema.columns WHERE TABLE_NAME='{0}' ", tableName);
			using (var adapter = new SqlCeDataAdapter(cmd, _dbConnection.ConnectionString))
			using (var columnsDbTypeTable = new DataTable("Columns"))
			{
				adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

				// Used to get columns Sql DataType
				// readd the columns info
				adapter.Fill(columnsDbTypeTable);

				// Fetch the rows
				foreach (DataRow dr in columnsDbTypeTable.Rows)
				{
					string columnName = dr["COLUMN_NAME"].ToString();
					var column = new SchemaColumn(columnName)
					{
						DbType = dr["DATA_TYPE"].ToString(),
						Length = Common.TryConvertInt32(dr["CHARACTER_MAXIMUM_LENGTH"].ToString(), 0),
						Nullable = dr["IS_NULLABLE"].ToString().ToUpper().StartsWith("Y"),
						ColumnOrdinal = Convert.ToInt32(dr["ORDINAL_POSITION"]),
						NumericPrecision = Common.TryConvertInt32(dr["NUMERIC_PRECISION"].ToString(), 0),
						NumericScale = Common.TryConvertInt32(dr["NUMERIC_SCALE"].ToString(), 0),

						AutoIncrement = Common.TryConvertBoolean(dr["AUTOINC_INCREMENT"], false),
						//PrimaryKey = Convert.ToBoolean(dr["IsKey"]),
					};

					var dotNetType = _sqlCeDataTypes.First(x => x.TypeName == column.DbType);
					column.DotNetType = dotNetType.DotNetDataType;
					column.CharacterMaxLength = (int)dotNetType.ColumnSize;

					// Columns which needs additional fetch
					FillColumnAdditionalInfo(column, tableName, columnName);

					// Add to result
					result.Add(column);
				}
			}

			return result;
		}

		/// <summary>
		/// Column additional information
		/// </summary>
		private void FillColumnAdditionalInfo(SchemaColumn toSetColumn, string tableName, string columnName)
		{
			string cmd = string.Format("SELECT CONSTRAINT_TYPE FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE A inner join  INFORMATION_SCHEMA.TABLE_CONSTRAINTS B ON A.CONSTRAINT_NAME  = B.CONSTRAINT_NAME   WHERE A.TABLE_NAME='{0}' AND A.COLUMN_NAME='{1}'",
				tableName, columnName);

			using (SqlCeDataAdapter adapter = new SqlCeDataAdapter(cmd, _dbConnection.ConnectionString))
			{
				adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

				using (var infoTbl = new DataTable())
				{
					adapter.Fill(infoTbl);
					toSetColumn.PrimaryKey = false;

					foreach (DataRow row in infoTbl.Rows)
					{
						toSetColumn.PrimaryKey = row["CONSTRAINT_TYPE"].ToString() == "PRIMARY KEY";
						if (toSetColumn.PrimaryKey)
							break;
					}
				}
			}
		}

		/// <summary>
		/// Reads columns description from SqlCe
		/// </summary>
		private void ApplyColumnsDescription(string tableName, List<SchemaColumn> columns)
		{
			throw new NotSupportedException("SqlCE doesn't have description field for columns.");
		}

		/// <summary>
		/// Reads specified table foreign keys.
		/// </summary>
		private void ApplyTablesForeignKeys(List<SchemaTable> tables)
		{
			if (_dbConnection.State != ConnectionState.Open)
				_dbConnection.Open();
			try
			{
				// Used to get columns Sql DataType
				const string cmd = "SELECT  A.COLUMN_NAME as UNIQUE_COLUMN_NAME,C.COLUMN_NAME as CONSTRAINT_COLUMN_NAME , B.* FROM  INFORMATION_SCHEMA.indexes A INNER JOIN  INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS B ON UNIQUE_CONSTRAINT_NAME = INDEX_NAME  inner join INFORMATION_SCHEMA.KEY_COLUMN_USAGE C ON C.CONSTRAINT_NAME=B.CONSTRAINT_NAME";

				using (var adapter = new SqlCeDataAdapter(cmd, _dbConnection.ConnectionString))
				using (var foreignKeysTable = new DataTable())
				{
					adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
					adapter.Fill(foreignKeysTable);

					// nothing found!)
					if (foreignKeysTable.Rows.Count == 0)
						return;

					foreach (DataRow keyRow in foreignKeysTable.Rows)
					{
						var foreignKeyTableName = keyRow["CONSTRAINT_TABLE_NAME"].ToString();
						var primaryKeyTableName = keyRow["UNIQUE_CONSTRAINT_TABLE_NAME"].ToString();

						var foreignKeyTable = FindTable(tables, foreignKeyTableName);
						var primaryKeyTable = FindTable(tables, primaryKeyTableName);


						// one-to-many foreign relation will be added
						if (primaryKeyTable != null)
						{
							// foreign key many end
							var manyMultiplicityKey_Local = new SchemaForeignKey()
							{
								ForeignKeyName = keyRow["CONSTRAINT_NAME"].ToString(),
								LocalColumnName = keyRow["UNIQUE_COLUMN_NAME"].ToString(),
								ForeignColumnName = keyRow["CONSTRAINT_COLUMN_NAME"].ToString(),
								ForeignTableName = keyRow["CONSTRAINT_TABLE_NAME"].ToString(),
								Multiplicity = SchemaForeignKey.ForeignKeyMultiplicity.Many
							};

							// check if it is already there
							if (primaryKeyTable.ForeignKeys.Exists(x => x.ForeignKeyName == manyMultiplicityKey_Local.ForeignKeyName))
								continue;

							// to the list
							primaryKeyTable.ForeignKeys.Add(manyMultiplicityKey_Local);

							// apply local column
							SchemaColumn localColumn = primaryKeyTable.FindColumn(manyMultiplicityKey_Local.LocalColumnName);
							manyMultiplicityKey_Local.LocalColumn = localColumn;
							if (!localColumn.PrimaryKey)
							{
								localColumn.IsReferenceKey = true;
								localColumn.IsReferenceKeyTable = primaryKeyTable;
							}

							if (foreignKeyTable != null)
							{
								// foreign table of that!
								manyMultiplicityKey_Local.ForeignTable = foreignKeyTable;

								// apply foreign column
								SchemaColumn foreignColumn = foreignKeyTable.FindColumn(manyMultiplicityKey_Local.ForeignColumnName);
								manyMultiplicityKey_Local.ForeignColumn = foreignColumn;
							}
							else
							{
								manyMultiplicityKey_Local.ForeignTable = null;
								manyMultiplicityKey_Local.ForeignColumn = null;
							}
						}

						// one-to-one foreign relation will be added
						if (foreignKeyTable != null)
						{
							// foreign key many end
							var oneMultiplicityKey_Foreign = new SchemaForeignKey()
							{
								ForeignKeyName = keyRow["CONSTRAINT_NAME"].ToString(),
								LocalColumnName = keyRow["CONSTRAINT_COLUMN_NAME"].ToString(),
								ForeignColumnName = keyRow["UNIQUE_COLUMN_NAME"].ToString(),
								ForeignTableName = keyRow["UNIQUE_CONSTRAINT_TABLE_NAME"].ToString(),
								Multiplicity = SchemaForeignKey.ForeignKeyMultiplicity.One
							};

							// check if it is already there
							if (foreignKeyTable.ForeignKeys.Exists(x => x.ForeignKeyName == oneMultiplicityKey_Foreign.ForeignKeyName))
								continue;

							// to the list
							foreignKeyTable.ForeignKeys.Add(oneMultiplicityKey_Foreign);

							// apply local column
							SchemaColumn localColumn = foreignKeyTable.FindColumn(oneMultiplicityKey_Foreign.LocalColumnName);
							oneMultiplicityKey_Foreign.LocalColumn = localColumn;
							if (!localColumn.PrimaryKey)
							{
								localColumn.IsReferenceKey = true;
								localColumn.IsReferenceKeyTable = primaryKeyTable;
							}

							if (primaryKeyTable != null)
							{
								// foreign table of that!
								oneMultiplicityKey_Foreign.ForeignTable = primaryKeyTable;

								// apply foreign column
								SchemaColumn foreignColumn = primaryKeyTable.FindColumn(oneMultiplicityKey_Foreign.ForeignColumnName);
								oneMultiplicityKey_Foreign.ForeignColumn = foreignColumn;
							}
							else
							{
								oneMultiplicityKey_Foreign.ForeignTable = null;
								oneMultiplicityKey_Foreign.ForeignColumn = null;
							}
						}
					} // all keys
				}
			}
			finally
			{
				_dbConnection.Close();
			}
		}

		/// <summary>
		/// Finds table from list
		/// </summary>
		private SchemaTable FindTable(List<SchemaTable> tables, string tableName)
		{
			foreach (var table in tables)
			{
				if (table.TableName == tableName)
					return table;
			}
			return null;
		}
		#endregion

	}
}
