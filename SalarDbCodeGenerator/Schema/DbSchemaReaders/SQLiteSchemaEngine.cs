using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using SalarDbCodeGenerator.Schema.Database;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Salar Khalilzadeh <salar2k@gmail.com>
// © 2012, All rights reserved
// 2012/07/06
// ====================================
namespace SalarDbCodeGenerator.Schema.DbSchemaReaders
{
	public class SQLiteSchemaEngine : ExSchemaEngine
	{
		#region local variables
		private SQLiteConnection _dbConnection;
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
		public override void FillSchema(DbDatabase schemaDatabase)
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
					return "@";
				default:
					return "";
			}
		}

		public override void Dispose()
		{
			if (_dbConnection != null)
				_dbConnection.Close();
			_dbConnection = null;
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
			using (DataTable views = _dbConnection.GetSchema("Views"))
			{
				foreach (DataRow row in views.Rows)
				{
					string viewName = row["TABLE_NAME"].ToString();

					// add to results
					result.Add(viewName);
				}
			}
			return result;
		}


		/// <summary>
		/// Reads tables schema from database
		/// </summary>
		private List<DbTable> ReadTables(List<DbView> viewList)
		{
			List<DbTable> result = new List<DbTable>();

			if (_dbConnection.State != ConnectionState.Open)
				_dbConnection.Open();
			using (DataTable tables = _dbConnection.GetSchema("TABLES"))
			{
				foreach (DataRow row in tables.Rows)
				{
					string tableName = row["TABLE_NAME"].ToString();

					if (!IsTableSelected(tableName))
						continue;

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
					List<DbColumn> columns = ReadColumns(tableName);

					// read columns description
					//if (ReadColumnsDescription)
					//	ApplyColumnsDescription(tableName, columns);

					// new table
					var dbTable = new DbTable(tableName, columns);

					// table schema
					dbTable.OwnerName = row["TABLE_SCHEMA"].ToString();

					// add to results
					result.Add(dbTable);
				}

				// it is time to read foreign keys!
				// foreign keys are requested?
				if (ReadTablesForeignKeys)
				{
					ApplyTablesForeignKeys(result);
					ApplyDetectedOneToOneRelation(result);
				}

			}
			return result;
		}

		/// <summary>
		/// Detecting one-to-one relation
		/// </summary>
		private void ApplyDetectedOneToOneRelation(List<DbTable> tables)
		{
			foreach (var table in tables)
				foreach (var fkey in table.ForeignKeys)
				{
					// already ont-to-?
					if (fkey.Multiplicity == DbForeignKey.ForeignKeyMultiplicity.OneToMany ||
						fkey.Multiplicity == DbForeignKey.ForeignKeyMultiplicity.OneToOne)
						continue;

					if (fkey.LocalColumn == null || fkey.ForeignColumn == null)
						continue;
					bool localIsUnique = false;
					bool foreignIsUnique = false;

					if (fkey.ForeignColumn.PrimaryKey)
						foreignIsUnique = true;
					else
					{
						var fkeyC = table.ConstraintKeys.FirstOrDefault(x => x.KeyColumnName == fkey.ForeignColumnName);
						if (fkeyC != null)
						{
							if (fkeyC.IsUnique)
								foreignIsUnique = true;
						}
					}

					if (fkey.LocalColumn.PrimaryKey)
						localIsUnique = true;
					else
					{
						var lkeyC = table.ConstraintKeys.FirstOrDefault(x => x.KeyColumnName == fkey.LocalColumnName);
						if (lkeyC != null)
						{
							if (lkeyC.IsUnique)
								localIsUnique = true;
						}
					}

					// both are unique??
					if (localIsUnique && foreignIsUnique)
					{
						// this is one-to-one
						fkey.Multiplicity = DbForeignKey.ForeignKeyMultiplicity.OneToOne;
					}
				}
		}


		/// <summary>
		/// Reads views schema from database
		/// </summary>
		private List<DbView> ReadViews()
		{
			List<DbView> result = new List<DbView>();

			if (_dbConnection.State != ConnectionState.Open)
				_dbConnection.Open();

			using (DataTable views = _dbConnection.GetSchema("Views"))
			{
				foreach (DataRow row in views.Rows)
				{
					string viewName = row["TABLE_NAME"].ToString();

					if (!IsViewSelected(viewName))
						continue;

					// View columns
					List<DbColumn> columns = ReadColumns(viewName);

					// read columns description
					//if (ReadColumnsDescription)
					//	ApplyColumnsDescription(viewName, columns);

					// new view
					var view = new DbView(viewName, columns);

					// view schema
					view.OwnerName = row["TABLE_SCHEMA"].ToString();

					// add to results
					result.Add(view);
				}
			}
			return result;
		}

		/// <summary>
		/// Read columns schema from database
		/// </summary>
		private List<DbColumn> ReadColumns(string tableName)
		{
			var result = new List<DbColumn>();

			if (_dbConnection.State != ConnectionState.Open)
				_dbConnection.Open();

			using (var adapter = new SQLiteDataAdapter(String.Format("SELECT * FROM {0} LIMIT 1 ", tableName), _dbConnection))
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
						using (var reader = new DataTableReader(columnsList))
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
						DbColumn column = new DbColumn(columnName)
						{
							DataTypeDotNet = dr["DataType"].ToString(),
							Length = Convert.ToInt32(dr["ColumnSize"]),
							PrimaryKey = Convert.ToBoolean(dr["IsKey"]),
							AutoIncrement = Convert.ToBoolean(dr["IsAutoIncrement"]),
							AllowNull = Convert.ToBoolean(dr["AllowDBNull"]),
							ColumnOrdinal = Convert.ToInt32(dr["ColumnOrdinal"]),
						};
						column.FieldNameSchema = DbSchemaNames.FieldName_RemoveInvalidChars(column.FieldNameSchema);

						// Columns which needs additional fetch
						FillColumnAdditionalInfo(column, columnsDbTypeTable, tableName, columnName);

						// Add to result
						result.Add(column);
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Column additional information
		/// </summary>
		private void FillColumnAdditionalInfo(DbColumn toSetColumn, DataTable columnsDbTypeTable, string tableName, string columnName)
		{
			DataRow[] addInfo = columnsDbTypeTable.Select(String.Format("TABLE_NAME='{0}' AND COLUMN_NAME='{1}'",
											  tableName,
											  columnName));
			object tempInfo = null;
			DataRow columnInfo = addInfo[0];

			toSetColumn.DataTypeDb = columnInfo["DATA_TYPE"].ToString();
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
		private void ApplyColumnsDescription(string tableName, List<DbColumn> columns)
		{
			throw new NotSupportedException("SQLite doesn't have description field for columns.");
		}

		/// <summary>
		/// Reads specified table foreign keys.
		/// </summary>
		private void ApplyTablesForeignKeys(List<DbTable> tables)
		{
			if (_dbConnection.State != ConnectionState.Open)
				_dbConnection.Open();

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
						// foreign key found!
						DataRow keyRow = keysData.Row;

						var foreignKeyTableName = keyRow["FKEY_TO_TABLE"].ToString();
						var primaryKeyTableName = table.TableName;

						var foreignKeyTable = FindTable(tables, foreignKeyTableName);
						var primaryKeyTable = table;

						if (primaryKeyTable != null)
						{
							// foreign key
							var foreignKey = new DbForeignKey()
												{
													ForeignKeyName = keyRow["CONSTRAINT_NAME"].ToString(),
													LocalColumnName = keyRow["FKEY_FROM_COLUMN"].ToString(),
													ForeignColumnName = keyRow["FKEY_TO_COLUMN"].ToString(),
													ForeignTableName = keyRow["FKEY_TO_TABLE"].ToString(),
													Multiplicity = DbForeignKey.ForeignKeyMultiplicity.OneToMany
												};

							// add foreign key
							table.ForeignKeys.Add(foreignKey);

							// apply local column
							DbColumn localColumn = table.FindColumnDb(foreignKey.LocalColumnName);
							foreignKey.LocalColumn = localColumn;

							//apply foreign table
							DbTable foreignTable = foreignKeyTable;

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
								DbColumn foreignColumn = foreignTable.FindColumnDb(foreignKey.ForeignColumnName);
								foreignKey.ForeignColumn = foreignColumn;
							}
							else
							{
								foreignKey.ForeignTable = null;
								foreignKey.ForeignColumn = null;
							}
						}

						// adding the relation to the foreign table!
						if (foreignKeyTable != null)
						{
							// foreign key
							var oneMultiplicityKey_Foreign = new DbForeignKey()
							{
								ForeignKeyName = keyRow["CONSTRAINT_NAME"].ToString(),
								LocalColumnName = keyRow["FKEY_TO_COLUMN"].ToString(),
								ForeignColumnName = keyRow["FKEY_FROM_COLUMN"].ToString(),
								ForeignTableName = primaryKeyTableName,
								Multiplicity = DbForeignKey.ForeignKeyMultiplicity.ManyToOne
							};

							// check if it is already there
							if (foreignKeyTable.ForeignKeys.Exists(x => x.ForeignKeyName == oneMultiplicityKey_Foreign.ForeignKeyName))
								continue;

							// to the list
							foreignKeyTable.ForeignKeys.Add(oneMultiplicityKey_Foreign);

							// apply local column
							DbColumn localColumn = foreignKeyTable.FindColumnDb(oneMultiplicityKey_Foreign.LocalColumnName);
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
								DbColumn foreignColumn = primaryKeyTable.FindColumnDb(oneMultiplicityKey_Foreign.ForeignColumnName);
								oneMultiplicityKey_Foreign.ForeignColumn = foreignColumn;
							}
							else
							{
								oneMultiplicityKey_Foreign.ForeignTable = null;
								oneMultiplicityKey_Foreign.ForeignColumn = null;
							}

						}

					}
				}
			}
		}

		/// <summary>
		/// Finds table from list
		/// </summary>
		private DbTable FindTable(List<DbTable> tables, string tableName)
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
