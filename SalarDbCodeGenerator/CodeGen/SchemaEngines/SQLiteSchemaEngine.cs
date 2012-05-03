using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using SalarSoft.DbCodeGenerator.CodeGen.DbSchema;

namespace SalarSoft.DbCodeGenerator.CodeGen.SchemaEngines
{
	public class SQLiteSchemaEngine : ExSchemaEngine
	{
		#region local variables
		private SQLiteConnection _dbConnection;
		#endregion

		#region properties
		#endregion

		#region public methods
		public SQLiteSchemaEngine(DbConnection dbConnection)
		{
			_dbConnection = (SQLiteConnection)dbConnection;
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
					return "SQLite";
				case DataProviderClassNames.ClassCommand:
					return "SQLiteCommand";
				case DataProviderClassNames.ClassConnection:
					return "SQLiteConnection";
				case DataProviderClassNames.ClassDataAdapter:
					return "SQLiteDataAdapter";
				case DataProviderClassNames.ClassDataReader:
					return "SQLiteDataReader";
				case DataProviderClassNames.ClassParameter:
					return "SQLiteParameter";
				case DataProviderClassNames.ClassTransaction:
					return "SQLiteTransaction";
				case DataProviderClassNames.ClassNamespace:
					return "System.Data.SQLite";
				case DataProviderClassNames.AssemblyReference:
					return "System.Data.SQLite";
				case DataProviderClassNames.StoredProcParamPrefix:
					return "?";
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
			StringCollection result = new StringCollection();

			if (_dbConnection.State != ConnectionState.Open)
				_dbConnection.Open();
			try
			{
				using (DataTable views = _dbConnection.GetSchema("Views"))
				{
					foreach (DataRow row in views.Rows)
					{
						string viewName = row["TABLE_NAME"].ToString();

						// add to results
						result.Add(viewName);
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
				using (DataTable tables = _dbConnection.GetSchema("TABLES"))
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
			List<SchemaView> result = new List<SchemaView>();

			if (_dbConnection.State != ConnectionState.Open)
				_dbConnection.Open();
			try
			{
				using (DataTable views = _dbConnection.GetSchema("Views"))
				{
					foreach (DataRow row in views.Rows)
					{
						string viewName = row["TABLE_NAME"].ToString();

						// View columns
						List<SchemaColumn> columns = ReadColumns(viewName);

						// read columns description
						//if (ReadColumnsDescription)
						//	ApplyColumnsDescription(viewName, columns);

						// new view
						SchemaView view = new SchemaView(viewName, columns);

						// view schema
						view.TableSchemaName = row["TABLE_SCHEMA"].ToString();

						// add to results
						result.Add(view);
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
		/// Read columns schema from database
		/// </summary>
		private List<SchemaColumn> ReadColumns(string tableName)
		{
			List<SchemaColumn> result = new List<SchemaColumn>();

			if (_dbConnection.State != ConnectionState.Open)
				_dbConnection.Open();
			try
			{

				using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(String.Format("SELECT * FROM {0} LIMIT 1 ", tableName), _dbConnection.ConnectionString))
				{
					adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

					DataTable columnsSchema;

					// Jjust to avoid stupid "Failed to enable constraints" error!
					using (DataSet tempDs = new DataSet())
					{
						// Avoiding stupid "Failed to enable constraints" error!
						tempDs.EnforceConstraints = false;

						using (DataTable columnsList = new DataTable())
						{
							tempDs.Tables.Add(columnsList);

							// Get from db
							adapter.Fill(columnsList);

							// Get schema
							using (DataTableReader reader = new DataTableReader(columnsList))
								columnsSchema = reader.GetSchemaTable();
						}
					}


					// Used to get columns Sql DataType
					using (DataTable columnsDbTypeTable = _dbConnection.GetSchema("COLUMNS"))
					{
						// Fetch the rows
						foreach (DataRow dr in columnsSchema.Rows)
						{
							string columnName = dr["ColumnName"].ToString();
							SchemaColumn column = new SchemaColumn(columnName)
							{
								DotNetType = dr["DataType"].ToString(),
								Length = Convert.ToInt32(dr["ColumnSize"]),
								PrimaryKey = Convert.ToBoolean(dr["IsKey"]),
								AutoIncrement = Convert.ToBoolean(dr["IsAutoIncrement"]),
								Nullable = Convert.ToBoolean(dr["AllowDBNull"]),
								ColumnOrdinal = Convert.ToInt32(dr["ColumnOrdinal"]),
							};

							// Columns which needs additional fetch
							FillColumnAdditionalInfo(column, columnsDbTypeTable, tableName, columnName);

							// Add to result
							result.Add(column);
						}
					}
				}
			}
			finally
			{
			}
			return result;
		}

		/// <summary>
		/// Column additional information
		/// </summary>
		private void FillColumnAdditionalInfo(SchemaColumn toSetColumn, DataTable columnsDbTypeTable, string tableName, string columnName)
		{
			DataRow[] addInfo = columnsDbTypeTable.Select(String.Format("TABLE_NAME='{0}' AND COLUMN_NAME='{1}'",
											  tableName,
											  columnName));
			object tempInfo = null;
			DataRow columnInfo = addInfo[0];

			toSetColumn.DbType = columnInfo["DATA_TYPE"].ToString();
			toSetColumn.Owner = columnInfo["TABLE_SCHEMA"].ToString();

			tempInfo = columnInfo["NUMERIC_SCALE"];
			if (tempInfo != null && tempInfo != DBNull.Value)
				toSetColumn.NumericScale = Convert.ToInt32(tempInfo);
			else toSetColumn.NumericScale = -1;

			tempInfo = columnInfo["NUMERIC_PRECISION"];
			if (tempInfo != null && tempInfo != DBNull.Value)
				toSetColumn.NumericPrecision = Convert.ToInt32(tempInfo);
			else toSetColumn.NumericPrecision = -1;
		}

		/// <summary>
		/// Reads columns description from SQLite
		/// </summary>
		private void ApplyColumnsDescription(string tableName, List<SchemaColumn> columns)
		{
			throw new NotSupportedException("SQLite doesn't have description field for columns.");
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
				using (DataTable foreignKeysTable = _dbConnection.GetSchema("ForeignKeys"))
				{
					// nothing found!
					if (foreignKeysTable.Rows.Count == 0)
						return;

					// find description if there is any
					foreach (var table in tables)
					{
						// only FOREIGN KEY
						foreignKeysTable.DefaultView.RowFilter =
							string.Format(" CONSTRAINT_TYPE='FOREIGN KEY' AND TABLE_NAME='{0}' ", table.TableName);

						// Fetch the rows
						foreach (DataRowView keysData in foreignKeysTable.DefaultView)
						{
							// table found!
							DataRow keyRow = keysData.Row;

							// foreign key
							SchemaForeignKey foreignKey = new SchemaForeignKey()
							{
								ForeignKeyName = keyRow["CONSTRAINT_NAME"].ToString(),
								LocalColumnName = keyRow["FKEY_FROM_COLUMN"].ToString(),
								ForeignColumnName = keyRow["FKEY_TO_COLUMN"].ToString(),
								ForeignTableName = keyRow["FKEY_TO_TABLE"].ToString()
							};

							// add foreign key
							table.ForeignKeys.Add(foreignKey);

							// apply local column
							SchemaColumn localColumn = table.FindColumn(foreignKey.LocalColumnName);
							foreignKey.LocalColumn = localColumn;
							
							//apply foreign table
							SchemaTable foreignTable = FindTable(tables, foreignKey.ForeignTableName);
							
							// referenced key
							if (!localColumn.PrimaryKey)
							{
								localColumn.IsReferenceKey = true;
								localColumn.IsReferenceKeyTable = foreignTable;
							}

							if (foreignTable != null)
							{
								foreignKey.ForeignTable = foreignTable;

								// apply foreign column
								SchemaColumn foreignColumn = foreignTable.FindColumn(foreignKey.ForeignColumnName);
								foreignKey.ForeignColumn = foreignColumn;
							}
							else
							{
								foreignKey.ForeignTable = null;
								foreignKey.ForeignColumn = null;
							}
						}
					}
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
