using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using SalarDbCodeGenerator.CodeGen.DbSchema;
using System.Text.RegularExpressions;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2009-9-30
// ====================================
namespace SalarDbCodeGenerator.CodeGen.SchemaEngines
{
	public class SQLSchemaEngine : ExSchemaEngine
	{
		private enum SQLServerVersions
		{
			SQL2000Below,
			SQL2000,
			SQL2005,
			SQL2008,
			SQL2008Above
		}

		//  Getting Foreign keys
		//SELECT        OBJECT_NAME(f.constid) AS 'ForeignKey', OBJECT_NAME(f.fkeyid) AS 'FKTable', c1.name AS 'FKColumnName', OBJECT_NAME(f.rkeyid) AS 'PKTable', 
		//                 c2.name AS 'PKColumnName'
		//FROM            sysforeignkeys AS f INNER JOIN
		//                 syscolumns AS c1 ON f.fkeyid = c1.id AND f.fkey = c1.colid INNER JOIN
		//                 syscolumns AS c2 ON f.rkeyid = c2.id AND f.rkey = c2.colid
		//ORDER BY OBJECT_NAME(f.rkeyid)

		//SELECT        objtype, objname, name, value
		//FROM            ::fn_listextendedproperty('MS_Description', 'user', 'dbo', 'table', 'tbl_AutoIncrement', 'column', NULL) AS fn_listextendedproperty_1

		//SELECT        *
		//FROM            ::fn_listextendedproperty('MS_Description', 'user', 'dbo', 'table', 'tbl_AutoIncrement', 'column', 'Column11') AS fn_listextendedproperty_1

		#region local variables
		private SqlConnection _dbConnection;
		#endregion

		#region public methods
		public SQLSchemaEngine(DbConnection dbConnection)
		{
			_dbConnection = (SqlConnection)dbConnection;
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

			schemaDatabase.SchemaViews = ReadViews();
			schemaDatabase.SchemaTables = ReadTables(schemaDatabase.SchemaViews);
		}

		public override string GetDataProviderClassName(DataProviderClassNames providerClassName)
		{
			switch (providerClassName)
			{
				case DataProviderClassNames.ClassPrefix:
					return "Sql";
				case DataProviderClassNames.ClassCommand:
					return "SqlCommand";
				case DataProviderClassNames.ClassConnection:
					return "SqlConnection";
				case DataProviderClassNames.ClassDataAdapter:
					return "SqlDataAdapter";
				case DataProviderClassNames.ClassDataReader:
					return "SqlDataReader";
				case DataProviderClassNames.ClassParameter:
					return "SqlParameter";
				case DataProviderClassNames.ClassTransaction:
					return "SqlTransaction";
				case DataProviderClassNames.ClassNamespace:
					return "System.Data.SqlClient";
				case DataProviderClassNames.AssemblyReference:
					return "System.Data";
				case DataProviderClassNames.StoredProcParamPrefix:
					return "@";
				default:
					return "";
			}
		}
		#endregion

		#region protected methods
		#endregion

		#region private methods
		/// <summary>
		/// Reads tables list. This method requires views list to prevent from conflict!
		/// </summary>
		private StringCollection ReadTablesList(StringCollection viewsList)
		{
			StringCollection result = new StringCollection();
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
		private List<SchemaTable> ReadTables(List<SchemaView> viewList)
		{
			List<SchemaTable> result = new List<SchemaTable>();

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
					if (ReadColumnsDescription)
						ApplyColumnsDescription(tableName, columns);

					// new table
					SchemaTable dbTable = new SchemaTable(tableName, columns);

					// table schema
					dbTable.TableSchemaName = row["TABLE_SCHEMA"].ToString();

					// add to results
					result.Add(dbTable);
				}


				// detect the sql server version
				SQLServerVersions sqlVersion = DetectSqlServerVersion(_dbConnection);

				if (ReadConstraintKeys)
					// The constraint keys will read here
					ApplyTablesConstraintKeys(result, sqlVersion);

				// it is time to read foreign keys!
				// foreign keys are requested?
				if (ReadTablesForeignKeys)
					ApplyTablesForeignKeys(result);

				// Normalize the constraints keys
				NormalizeTablesConstraintKeys(result, sqlVersion);

				if (ReadTablesForeignKeys)
					ApplyDetectedOneToOneRelation(result);

			}
			return result;
		}


		/// <summary>
		/// Reads views schema from database
		/// </summary>
		private List<SchemaView> ReadViews()
		{
			List<SchemaView> result = new List<SchemaView>();

			using (DataTable views = _dbConnection.GetSchema("Views"))
			{
				foreach (DataRow row in views.Rows)
				{
					string viewName = row["TABLE_NAME"].ToString();

					// View columns
					List<SchemaColumn> columns = ReadColumns(viewName);

					// read columns description
					if (ReadColumnsDescription)
						ApplyColumnsDescription(viewName, columns);

					// new view
					SchemaView view = new SchemaView(viewName, columns);

					// view schema
					view.TableSchemaName = row["TABLE_SCHEMA"].ToString();

					// add to results
					result.Add(view);
				}
			}
			return result;
		}

		/// <summary>
		/// Read columns schema from database
		/// </summary>
		private List<SchemaColumn> ReadColumns(String tableName)
		{
			List<SchemaColumn> result = new List<SchemaColumn>();

			using (SqlDataAdapter adapter = new SqlDataAdapter(String.Format("SELECT TOP 1 * FROM [{0}]", tableName), (SqlConnection)_dbConnection))
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
			if (addInfo.Length == 0)
			{
				// sometimes when a column has invalid name for string format this happends!
				return;
			}

			object tempInfo = null;
			DataRow columnInfo = addInfo[0];

			toSetColumn.DbType = columnInfo["DATA_TYPE"].ToString();
			toSetColumn.Owner = columnInfo["TABLE_SCHEMA"].ToString();

			tempInfo = columnInfo["CHARACTER_MAXIMUM_LENGTH"];
			if (tempInfo != null && tempInfo != DBNull.Value)
			{
				toSetColumn.CharacterMaxLength = Convert.ToInt32(tempInfo);
				if (toSetColumn.CharacterMaxLength == -1)
				{
					toSetColumn.LengthIsMax = true;
					toSetColumn.CharacterMaxLength = int.MaxValue;
				}
				else
				{
					toSetColumn.LengthIsMax = false;
				}
			}
			else
			{
				toSetColumn.CharacterMaxLength = toSetColumn.Length;
			}

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
		/// Reads specified table foreign keys.
		/// </summary>
		private void ApplyTablesForeignKeys(List<SchemaTable> tables)
		{
			// command format
			string foreignKeySql = "SELECT OBJECT_NAME(f.constid) AS 'ForeignKey', OBJECT_NAME(f.fkeyid) AS 'FKTable', " +
				" c1.name AS 'FKColumnName', OBJECT_NAME(f.rkeyid) AS 'PKTable', c2.name AS 'PKColumnName' " +
				" FROM sysforeignkeys AS f INNER JOIN " +
				"syscolumns AS c1 ON f.fkeyid = c1.id AND f.fkey = c1.colid INNER JOIN " +
				"syscolumns AS c2 ON f.rkeyid = c2.id AND f.rkey = c2.colid " +
				"ORDER BY OBJECT_NAME(f.rkeyid) ";

			try
			{
				using (SqlDataAdapter adapter = new SqlDataAdapter(foreignKeySql, (SqlConnection)_dbConnection))
				{
					adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

					// description data table
					using (DataTable keysData = new DataTable())
					{
						// Just to avoid stupid "Failed to enable constraints" error!
						using (DataSet tempDs = new DataSet())
						{
							// Avoiding stupid "Failed to enable constraints" error!
							tempDs.EnforceConstraints = false;
							tempDs.Tables.Add(keysData);

							// Get from db
							adapter.Fill(keysData);
						}

						if (keysData.Rows.Count > 0)
						{
							foreach (DataRow keysDataRow in keysData.Rows)
							{
								var foreignKeyTableName = keysDataRow["FKTable"].ToString();
								var primaryKeyTableName = keysDataRow["PKTable"].ToString();

								var foreignKeyTable = FindTable(tables, foreignKeyTableName);
								var primaryKeyTable = FindTable(tables, primaryKeyTableName);


								// one-to-many foreign relation will be added
								if (primaryKeyTable != null)
								{
									// foreign key many end
									var manyMultiplicityKey_Local = new SchemaForeignKey()
																		{
																			ForeignKeyName = keysDataRow["ForeignKey"].ToString(),
																			LocalColumnName = keysDataRow["PKColumnName"].ToString(),
																			ForeignColumnName = keysDataRow["FKColumnName"].ToString(),
																			ForeignTableName = keysDataRow["FKTable"].ToString(),
																			Multiplicity = SchemaForeignKey.ForeignKeyMultiplicity.Many
																		};

									// check if it is already there
									if (primaryKeyTable.ForeignKeys.Exists(x => x.ForeignKeyName == manyMultiplicityKey_Local.ForeignKeyName))
										continue;
									//if (primaryKeyTable.ForeignKeys.Exists(x =>
									//    x.ForeignColumnName == manyMultiplicityKey_Local.ForeignColumnName &&
									//    x.LocalColumnName == manyMultiplicityKey_Local.LocalColumnName))
									//    continue;

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
																			ForeignKeyName = keysDataRow["ForeignKey"].ToString(),
																			LocalColumnName = keysDataRow["FKColumnName"].ToString(),
																			ForeignColumnName = keysDataRow["PKColumnName"].ToString(),
																			ForeignTableName = keysDataRow["PKTable"].ToString(),
																			Multiplicity = SchemaForeignKey.ForeignKeyMultiplicity.One
																		};

									// check if it is already there
									if (foreignKeyTable.ForeignKeys.Exists(x => x.ForeignKeyName == oneMultiplicityKey_Foreign.ForeignKeyName))
										continue;
									//if (foreignKeyTable.ForeignKeys.Exists(x =>
									//    x.ForeignColumnName == oneMultiplicityKey_Foreign.ForeignColumnName &&
									//    x.LocalColumnName == oneMultiplicityKey_Foreign.LocalColumnName))
									//    continue;

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
							}// all foreign keys

							// look for one-to-one situation!



						}
					}
				}
			}
			catch
			{
				// Seems this version of SQL Server doesn't support this query!
				// don't stop here!
			}
		}

		/// <summary>
		/// Reads tables index keys
		/// </summary>
		private void ApplyTablesConstraintKeys(List<SchemaTable> tables, SQLServerVersions sqlVersion)
		{
			if (sqlVersion == SQLServerVersions.SQL2000 ||
				sqlVersion == SQLServerVersions.SQL2000Below)
			{
				// not supported
				return;
			}

			//// Table constraints for SQL Server 2005 and above
			//SELECT        sys.objects.name AS TableName, sys.columns.name AS ColumnName, sys.indexes.name AS IndexName, sys.indexes.is_unique AS IsUnique, 
			//                sys.indexes.is_primary_key AS IsPrimaryKey, sys.indexes.ignore_dup_key AS IgnoreDuplicateKey, sys.indexes.is_unique_constraint AS IsUniqueConstraintKey, 
			//                sys.indexes.is_disabled AS Disabled
			//FROM            sys.objects INNER JOIN
			//                sys.indexes INNER JOIN
			//                sys.index_columns INNER JOIN
			//                sys.columns ON sys.index_columns.object_id = sys.columns.object_id AND sys.index_columns.column_id = sys.columns.column_id ON 
			//                sys.indexes.object_id = sys.index_columns.object_id AND sys.indexes.index_id = sys.index_columns.index_id ON 
			//                sys.objects.object_id = sys.index_columns.object_id
			//WHERE        (sys.objects.is_ms_shipped = 0) AND (sys.objects.type = 'U')

			string constraintKeySql = "SELECT        sys.objects.name AS TableName, sys.columns.name AS ColumnName, sys.indexes.name AS IndexName, sys.indexes.is_unique AS IsUnique, " +
					"                sys.indexes.is_primary_key AS IsPrimaryKey, sys.indexes.ignore_dup_key AS IgnoreDuplicateKey, sys.indexes.is_unique_constraint AS IsUniqueConstraintKey,  " +
					"                sys.indexes.is_disabled AS Disabled " +
					"	FROM            sys.objects INNER JOIN " +
					"                sys.indexes INNER JOIN " +
					"                sys.index_columns INNER JOIN " +
					"                sys.columns ON sys.index_columns.object_id = sys.columns.object_id AND sys.index_columns.column_id = sys.columns.column_id ON  " +
					"                sys.indexes.object_id = sys.index_columns.object_id AND sys.indexes.index_id = sys.index_columns.index_id ON  " +
					"                sys.objects.object_id = sys.index_columns.object_id " +
					"	WHERE        (sys.objects.is_ms_shipped = 0) AND (sys.objects.type = 'U') ";

			using (SqlDataAdapter adapter = new SqlDataAdapter(constraintKeySql, _dbConnection))
			{
				adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

				// description data table
				using (DataTable keysData = new DataTable())
				{
					// Just to avoid stupid "Failed to enable constraints" error!
					using (DataSet tempDs = new DataSet())
					{
						// Avoiding stupid "Failed to enable constraints" error!
						tempDs.EnforceConstraints = false;
						tempDs.Tables.Add(keysData);

						// Get from db
						adapter.Fill(keysData);
					}

					if (keysData.Rows.Count > 0)
					{
						// find description if there is any
						foreach (var table in tables)
						{
							// filter row
							keysData.DefaultView.RowFilter = " TableName='" + table.TableName + "' AND IsPrimaryKey=0 ";

							// fetch findings, if there is any
							foreach (DataRowView keysDataRow in keysData.DefaultView)
							{
								// found table !
								DataRow keyRow = keysDataRow.Row;

								// constraint Key
								var constraintKey = new SchemaConstraintKey()
								{
									IsUnique = Convert.ToBoolean(keyRow["IsUnique"].ToString()),
									KeyColumnName = keyRow["ColumnName"].ToString(),
									KeyName = keyRow["IndexName"].ToString()
								};

								// constraint keys
								table.ConstraintKeys.Add(constraintKey);

								// find key column
								SchemaColumn keyColumn = table.FindColumn(constraintKey.KeyColumnName);
								constraintKey.KeyColumn = keyColumn;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Detecting one-to-one relation
		/// </summary>
		private void ApplyDetectedOneToOneRelation(List<SchemaTable> tables)
		{
			foreach (var table in tables)
				foreach (var fkey in table.ForeignKeys)
				{
					// already ont-to-?
					if (fkey.Multiplicity == SchemaForeignKey.ForeignKeyMultiplicity.One)
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
						fkey.Multiplicity = SchemaForeignKey.ForeignKeyMultiplicity.One;
					}
				}
		}

		/// <summary>
		/// Removes duplicate table constraints, PK > UK > IX
		/// </summary>
		private void NormalizeTablesConstraintKeys(List<SchemaTable> result, SQLServerVersions sqlVersion)
		{
			// look in tables list
			foreach (SchemaTable table in result)
			{
				if (table.ConstraintKeys.Count == 0)
				{
					continue;
				}

				StringCollection duplicateConstraints = new StringCollection();

				// fetching the contraints keys
				for (int j = table.ConstraintKeys.Count - 1; j >= 0; j--)
				{
					var constraintKey = table.ConstraintKeys[j];

					// no primary keys are allowed
					if (constraintKey.KeyColumn != null && constraintKey.KeyColumn.PrimaryKey)
					{
						// There is no need in keeping the primary key
						table.ConstraintKeys.RemoveAt(j);
						continue;
					}

					// first look in the foreign keys!
					int index = table.ForeignKeys.FindIndex(x =>
						x.LocalColumnName == constraintKey.KeyColumnName);

					if (index != -1)
					{
						// this is a foreign key and should not be here
						table.ConstraintKeys.RemoveAt(j);
						continue;
					}

					// if this is not a unique key
					// seach for a unique one if it is there
					if (constraintKey.IsUnique == false)
					{
						index = table.ConstraintKeys.FindIndex(x =>
							x.KeyColumnName == constraintKey.KeyColumnName
							&& x.IsUnique == true);

						if (index != -1)
						{
							// the same and the Unique key is already there!
							table.ConstraintKeys.RemoveAt(j);
							continue;
						}
					}
					else
					{
						var notUniqueKeys = table.ConstraintKeys.FindAll(x =>
							x.KeyColumnName == constraintKey.KeyColumnName
							&& x.IsUnique == false);

						if (notUniqueKeys != null && notUniqueKeys.Count > 0)
						{
							// remove them
							notUniqueKeys.ForEach(x => table.ConstraintKeys.Remove(x));
							continue;
						}
					}

					// look for duplication constraint key
					if (duplicateConstraints.Contains(constraintKey.KeyColumnName))
					{
						// the column with index is already there
						table.ConstraintKeys.RemoveAt(j);
						continue;
					}

					// all to the constraint key list
					duplicateConstraints.Add(constraintKey.KeyColumnName);
				}
			}
		}

		/// <summary>
		/// Reads columns description from SQLServer
		/// </summary>
		/// <param name="columns"></param>
		private void ApplyColumnsDescription(string tableName, List<SchemaColumn> columns)
		{
			// there is no column!
			if (columns.Count == 0)
				return;

			// command format
			string descriptionSql = "SELECT * FROM ::fn_listextendedproperty('MS_Description', 'user', 'dbo', 'table', N'{0}', 'column', NULL) AS func ";

			try
			{
				using (SqlDataAdapter adapter = new SqlDataAdapter(String.Format(descriptionSql, tableName), (SqlConnection)_dbConnection))
				{
					adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

					// description data table
					using (DataTable descriptionData = new DataTable())
					{
						// Jjust to avoid stupid "Failed to enable constraints" error!
						using (DataSet tempDs = new DataSet())
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
								// filter row to find the column
								descriptionData.DefaultView.RowFilter = " objname='" + column.FieldName + "' ";
								if (descriptionData.DefaultView.Count > 0)
								{
									// description found!
									column.UserDescription = descriptionData.DefaultView[0].Row["value"].ToString();
								}
							}
					}
				}
			}
			catch
			{
				// Seems this version of SQL Server doesn't support this query!
				// don't stop here!
				// TODO: inform user
			}
		}

		/// <summary>
		/// Column DBType name
		/// </summary>
		private string GetColumnDbDataType(DataTable columnsDbTypeTable, string tableName, string columnName)
		{
			DataRow[] drs;
			drs = columnsDbTypeTable.Select(String.Format("TABLE_NAME='{0}' AND COLUMN_NAME='{1}'",
											  tableName,
											  columnName));
			// Db Type
			string dbType = drs[0].ItemArray[7].ToString();

			return dbType;
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

		/// <summary>
		/// Detecting sql server version
		/// </summary>
		private SQLServerVersions DetectSqlServerVersion(DbConnection conn)
		{
			string versionStr = conn.ServerVersion;
			var majorMatch = Regex.Match(versionStr, @"(?<Major>\d+)\..*", RegexOptions.Compiled);
			if (majorMatch != null)
			{
				var majorVersion = Convert.ToInt16(majorMatch.Groups["Major"].Value);
				switch (majorVersion)
				{
					case 8:
						return SQLServerVersions.SQL2000;

					case 9:
						return SQLServerVersions.SQL2005;

					case 10:
						return SQLServerVersions.SQL2008;

					default:
						if (majorVersion > 10)
						{
							return SQLServerVersions.SQL2008Above;
						}
						return SQLServerVersions.SQL2000Below;
				}
			}
			else
				return SQLServerVersions.SQL2000Below;
		}
		#endregion
	}
}
