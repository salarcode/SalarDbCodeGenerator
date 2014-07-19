using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using SalarDbCodeGenerator.DbProject;
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
		public override void FillSchema(DbDatabase schemaDatabase)
		{
			if (schemaDatabase == null)
				throw new ArgumentNullException("schemaDatabase", "Database is not specified.");

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

		public override void Dispose()
		{
			if (_dbConnection != null)
				_dbConnection.Close();
			_dbConnection = null;
		}

		#endregion

		#region private methods
		/// <summary>
		/// Reads tables list. This method requires views list to prevent confliction!
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

						// is this view?
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
		private List<DbTable> ReadTables(List<DbView> viewList)
		{
			List<DbTable> result = new List<DbTable>();

			if (_dbConnection.State != ConnectionState.Open)
				_dbConnection.Open();
			try
			{
				using (DataTable tables = _dbConnection.GetSchema("Tables"))
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
								// no view
								continue;
							}
						}

						// read the fucking columns
						List<DbColumn> columns = ReadColumns(tableName);

						// read columns description
						if (ReadColumnsDescription)
							ApplyColumnsDescription(tableName, columns);

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
		private List<DbView> ReadViews()
		{
			// this fucking shit doesn't support views!
			return new List<DbView>();
		}

		/// <summary>
		/// Read columns schema from database
		/// </summary>
		private List<DbColumn> ReadColumns(string tableName)
		{
			List<DbColumn> result = new List<DbColumn>();

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
					var column = new DbColumn(columnName)
					{
						DataTypeDb = dr["DATA_TYPE"].ToString(),
						Length = Common.TryConvertInt32(dr["CHARACTER_MAXIMUM_LENGTH"].ToString(), 0),
						AllowNull = dr["IS_NULLABLE"].ToString().ToUpper().StartsWith("Y"),
						ColumnOrdinal = Convert.ToInt32(dr["ORDINAL_POSITION"]),
						NumericPrecision = Common.TryConvertInt32(dr["NUMERIC_PRECISION"].ToString(), 0),
						NumericScale = Common.TryConvertInt32(dr["NUMERIC_SCALE"].ToString(), 0),

						AutoIncrement = Common.TryConvertBoolean(dr["AUTOINC_INCREMENT"], false),
						//PrimaryKey = Convert.ToBoolean(dr["IsKey"]),
						UserDescription = dr["DESCRIPTION"].ToString()
					};

					var dotNetType = _sqlCeDataTypes.First(x => x.TypeName == column.DataTypeDb);
					column.DataTypeDotNet = dotNetType.DotNetDataType;
					column.DataTypeMaxLength = (int)dotNetType.ColumnSize;
					column.FieldNameSchema = DbSchemaNames.FieldName_RemoveInvalidChars(column.FieldNameSchema);

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
		private void FillColumnAdditionalInfo(DbColumn toSetColumn, string tableName, string columnName)
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
		private void ApplyColumnsDescription(string tableName, List<DbColumn> columns)
		{
			// there is no column!
			if (columns.Count == 0)
				return;

			// command format
			string descriptionSql = "SELECT * FROM __ExtendedProperties where ParentName=N'{0}' ";

			try
			{
				using (var adapter = new SqlCeDataAdapter(String.Format(descriptionSql, tableName), (SqlCeConnection)_dbConnection))
				{
					adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

					// description data table
					using (var descriptionData = new DataTable())
					{
						// Jjust to avoid stupid "Failed to enable constraints" error!
						using (var tempDs = new DataSet())
						{
							// Avoiding stupid "Failed to enable constraints" error!
							tempDs.EnforceConstraints = false;
							tempDs.Tables.Add(descriptionData);

							// Get from db
							adapter.Fill(descriptionData);
						}

						// if something found
						if (descriptionData.Rows.Count > 0)
							// find description if there is any
							foreach (var column in columns)
							{
								if (!string.IsNullOrEmpty(column.UserDescription))
									continue;

								// filter row to find the column
								descriptionData.DefaultView.RowFilter = " ObjectName='" + column.FieldNameDb + "' ";
								if (descriptionData.DefaultView.Count > 0)
								{
									// description found!
									column.UserDescription = descriptionData.DefaultView[0].Row["Value"].ToString();
									column.UserDescription = column.UserDescription.Replace("\r\n", " ").Replace("\n", " ");
								}
							}
					}
				}
			}
			catch
			{
				// Seems this database doesn't have __ExtendedProperties table
				// don't stop here!
				// TODO: inform user
			}
		}

		/// <summary>
		/// Reads specified table foreign keys.
		/// </summary>
		private void ApplyTablesForeignKeys(List<DbTable> tables)
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
							var manyMultiplicityKey_Local = new DbForeignKey()
							{
								ForeignKeyName = keyRow["CONSTRAINT_NAME"].ToString(),
								LocalColumnName = keyRow["UNIQUE_COLUMN_NAME"].ToString(),
								ForeignColumnName = keyRow["CONSTRAINT_COLUMN_NAME"].ToString(),
								ForeignTableName = keyRow["CONSTRAINT_TABLE_NAME"].ToString(),
								Multiplicity = DbForeignKey.ForeignKeyMultiplicity.ManyToOne
							};

							// check if it is already there
							if (primaryKeyTable.ForeignKeys.Exists(x => x.ForeignKeyName == manyMultiplicityKey_Local.ForeignKeyName))
								continue;

							manyMultiplicityKey_Local.UpdateAction =
								ConvertSqlCeForeignKeyAction(keyRow["UPDATE_RULE"].ToString());
							manyMultiplicityKey_Local.DeleteAction =
								ConvertSqlCeForeignKeyAction(keyRow["DELETE_RULE"].ToString());

							// to the list
							primaryKeyTable.ForeignKeys.Add(manyMultiplicityKey_Local);

							// apply local column
							DbColumn localColumn = primaryKeyTable.FindColumnDb(manyMultiplicityKey_Local.LocalColumnName);
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
								DbColumn foreignColumn = foreignKeyTable.FindColumnDb(manyMultiplicityKey_Local.ForeignColumnName);
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
							var oneMultiplicityKey_Foreign = new DbForeignKey()
							{
								ForeignKeyName = keyRow["CONSTRAINT_NAME"].ToString(),
								LocalColumnName = keyRow["CONSTRAINT_COLUMN_NAME"].ToString(),
								ForeignColumnName = keyRow["UNIQUE_COLUMN_NAME"].ToString(),
								ForeignTableName = keyRow["UNIQUE_CONSTRAINT_TABLE_NAME"].ToString(),
								Multiplicity = DbForeignKey.ForeignKeyMultiplicity.OneToMany
							};

							// check if it is already there
							if (foreignKeyTable.ForeignKeys.Exists(x => x.ForeignKeyName == oneMultiplicityKey_Foreign.ForeignKeyName))
								continue;

							oneMultiplicityKey_Foreign.UpdateAction =
								ConvertSqlCeForeignKeyAction(keyRow["UPDATE_RULE"].ToString());
							oneMultiplicityKey_Foreign.DeleteAction =
								ConvertSqlCeForeignKeyAction(keyRow["DELETE_RULE"].ToString());

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
					} // all keys
				}
			}
			finally
			{
				_dbConnection.Close();
			}
		}

		private DbForeignKeyAction ConvertSqlCeForeignKeyAction(string action)
		{
			switch (action)
			{
				case "NO ACTION":
					return DbForeignKeyAction.NoAction;

				case "CASCADE":
					return DbForeignKeyAction.Cascade;

				case "SET NULL":
					return DbForeignKeyAction.SetNull;

				case "SET DEFAULT":
					return DbForeignKeyAction.SetDefault;

				default:
					return DbForeignKeyAction.NotSet;
			}
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
