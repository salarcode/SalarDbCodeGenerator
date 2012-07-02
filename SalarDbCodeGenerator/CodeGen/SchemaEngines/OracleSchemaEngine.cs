using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using Oracle.DataAccess.Client;
using SalarDbCodeGenerator.CodeGen.DbSchema;
using SalarDbCodeGenerator.DbProject;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2010-11-21
// ====================================
namespace SalarDbCodeGenerator.CodeGen.SchemaEngines
{
	public class OracleSchemaEngine : ExSchemaEngine
	{
		private enum OracleServerVersions
		{
			Oracle8iBelow,
			Oracle8i,
			Oracle9i,
			Oracle10g,
			Oracle11g,
			Oracle11gAbove
		}

		#region local variables
		private OracleConnection _dbConnection;
		#endregion

		#region public methods
		public OracleSchemaEngine(DbConnection dbConnection)
		{
			_dbConnection = (OracleConnection)dbConnection;
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
					return "Oracle";
				case DataProviderClassNames.ClassCommand:
					return "OracleCommand";
				case DataProviderClassNames.ClassConnection:
					return "OracleConnection";
				case DataProviderClassNames.ClassDataAdapter:
					return "OracleDataAdapter";
				case DataProviderClassNames.ClassDataReader:
					return "OracleDataReader";
				case DataProviderClassNames.ClassParameter:
					return "OracleParameter";
				case DataProviderClassNames.ClassTransaction:
					return "OracleTransaction";
				case DataProviderClassNames.ClassNamespace:
					return "Oracle.DataAccess.Client";
				case DataProviderClassNames.AssemblyReference:
					return "Oracle.DataAccess";
				case DataProviderClassNames.StoredProcParamPrefix:
					return ":";
				default:
					return "";
			}
		}
		#endregion

		#region protected methods

		private DataTable _cache_IndexColumns;
		private DataTable Cache_IndexColumns
		{
			get
			{
				if (_cache_IndexColumns == null)
				{
					string[] restrictions = null;
					if (!string.IsNullOrWhiteSpace(this.SpecificOwner))
						restrictions = new string[] { SpecificOwner.ToUpper() };

					_cache_IndexColumns = _dbConnection.GetSchema("IndexColumns", restrictions);
				}
				return _cache_IndexColumns;
			}
		}

		private DataTable _cache_Indexes;
		private DataTable Cache_Indexes
		{
			get
			{
				if (_cache_Indexes == null)
				{
					string[] restrictions = null;
					if (!string.IsNullOrWhiteSpace(this.SpecificOwner))
						restrictions = new string[] { SpecificOwner.ToUpper() };

					_cache_Indexes = _dbConnection.GetSchema("Indexes", restrictions);
				}
				return _cache_Indexes;
			}
		}


		private DataTable _cache_ForeignKeys;
		private DataTable Cache_ForeignKeys
		{
			get
			{
				if (_cache_ForeignKeys == null)
				{
					string[] restrictions = null;
					if (!string.IsNullOrWhiteSpace(this.SpecificOwner))
						restrictions = new string[] { SpecificOwner.ToUpper() };

					_cache_ForeignKeys = _dbConnection.GetSchema("ForeignKeys", restrictions);
				}
				return _cache_ForeignKeys;
			}
		}

		private DataTable _cache_All_Constraints;
		private DataTable Cache_All_Constraints
		{
			get
			{
				const string all_Constraints =
					"SELECT   A.OWNER, A.TABLE_NAME, Column_Name,A.CONSTRAINT_NAME,CONSTRAINT_TYPE   FROM   ALL_CONSTRAINTS A INNER JOIN   ALL_CONS_COLUMNS B " +
					" ON (A.OWNER=B.OWNER AND B.CONSTRAINT_NAME = A.CONSTRAINT_NAME)";
				// CONSTRAINT_TYPE:
				// U == Unique
				// P == Primarykey
				// R == ForeignKey

				if (_cache_All_Constraints == null)
				{
					using (var adapter = new OracleDataAdapter(all_Constraints, _dbConnection))
					{
						_cache_All_Constraints = new DataTable();
						adapter.Fill(_cache_All_Constraints);
					}
				}
				return _cache_All_Constraints;
			}
		}

		private const string STR_ConstraintType_Unique = "U";
		private const string STR_ConstraintType_Primarykey = "P";
		private const string STR_ConstraintType_ForeignKey = "R";
		#endregion

		#region private methods
		/// <summary>
		/// Reads tables list. This method requires views list to prevent from conflict!
		/// </summary>
		private StringCollection ReadTablesList(StringCollection viewsList)
		{
			StringCollection result = new StringCollection();

			string[] restrictions = null;
			if (!string.IsNullOrWhiteSpace(this.SpecificOwner))
				restrictions = new string[] { SpecificOwner.ToUpper() };

			using (DataTable views = _dbConnection.GetSchema("TABLES", restrictions))
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

			string[] restrictions = null;
			if (!string.IsNullOrWhiteSpace(this.SpecificOwner))
			{
				restrictions = new string[] { SpecificOwner.ToUpper() };
			}

			using (DataTable views = _dbConnection.GetSchema("Views", restrictions))
			{
				foreach (DataRow row in views.Rows)
				{
					string viewName = row["VIEW_NAME"].ToString();

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

			string[] restrictions = null;
			if (!string.IsNullOrWhiteSpace(this.SpecificOwner))
			{
				restrictions = new string[] { SpecificOwner.ToUpper() };
			}

			using (DataTable tables = _dbConnection.GetSchema("TABLES", restrictions))
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
					var dbTable = new SchemaTable(tableName, columns);

					// table schema
					dbTable.TableSchemaName = row["OWNER"].ToString();

					// add to results
					result.Add(dbTable);
				}

				// correct tables name by case sesitivity usage!
				AssignCaseSensitiveTablesName(result);


				// detect the sql server version
				OracleServerVersions dbVersion = DetectVersion(_dbConnection);

				if (ReadConstraintKeys)
					// The constraint keys will read here
					ApplyTablesConstraintKeys(result, dbVersion);

				// it is time to read foreign keys!
				// foreign keys are requested?
				if (ReadTablesForeignKeys)
					ApplyTablesForeignKeys(result);

				// Normalize the constraints keys
				NormalizeTablesConstraintKeys(result, dbVersion);

				if (ReadTablesForeignKeys)
					ApplyDetectedOneToOneRelation(result);
			}
			return result;
		}

		/// <summary>
		/// Only oracle allows case sensitive table names!
		/// </summary>
		private void AssignCaseSensitiveTablesName(List<SchemaTable> tables)
		{
			if (tables.Count == 0)
				return;

			// the copy list
			var workingTables = new List<SchemaTable>();
			workingTables.AddRange(tables);

			// suppress pattern
			string suppressPattern = "{0}_{1}";

			// correct tables name by case sesitivity usage!
			for (int i = workingTables.Count - 1; i >= 0; i--)
			{
				var ct = workingTables[i];
				var ctName = ct.TableName.ToLower();

				// the tables with same name but different cases!
				var sameTables = workingTables.Where(x => x.TableName.ToLower() == ctName && x.TableName != ct.TableName).ToList();
				if (sameTables.Count == 0)
					continue;

				sameTables.Add(ct);

				// rename them!
				for (int j = 0; j < sameTables.Count; j++)
				{
					var toRename = sameTables[j];

					//remove from search list
					workingTables.Remove(toRename);

					toRename.TableNameCS = string.Format(suppressPattern, toRename.TableName, j);
				}
			}
		}

		/// <summary>
		/// Reads views schema from database
		/// </summary>
		private List<SchemaView> ReadViews()
		{
			List<SchemaView> result = new List<SchemaView>();

			string[] restrictions = null;
			if (!string.IsNullOrWhiteSpace(this.SpecificOwner))
			{
				restrictions = new string[] { SpecificOwner.ToUpper() };
			}

			using (DataTable views = _dbConnection.GetSchema("Views", restrictions))
			{
				foreach (DataRow row in views.Rows)
				{
					string viewName = row["VIEW_NAME"].ToString();

					// View columns
					List<SchemaColumn> columns = ReadColumns(viewName);

					// new view
					SchemaView view = new SchemaView(viewName, columns);

					// view schema
					view.TableSchemaName = row["OWNER"].ToString();

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


			string[] restrictions = null;
			if (!string.IsNullOrWhiteSpace(this.SpecificOwner))
			{
				restrictions = new string[]
					               	{
					               		SpecificOwner.ToUpper(),
										tableName
					               	};
			}

			// Used to get columns Sql DataType
			using (DataTable columnsDbTypeTable = _dbConnection.GetSchema("COLUMNS", restrictions))
			{

				// Fetch the rows
				foreach (DataRow dr in columnsDbTypeTable.Rows)
				{
					string columnName = dr["COLUMN_NAME"].ToString();
					SchemaColumn column = new SchemaColumn(columnName)
					{
						Owner = dr["OWNER"].ToString(),
						DotNetType = FindMatchingDotNetDataType(dr["DATATYPE"].ToString()),

						DbType = dr["DATATYPE"].ToString(),
						Length = Common.TryConvertInt32(dr["LENGTH"].ToString(), 0),
						ColumnOrdinal = Common.TryConvertInt32(dr["ID"].ToString(), -1),
						Nullable = dr["Nullable"].ToString().ToUpper() == "Y",

						NumericPrecision = Common.TryConvertInt32(dr["PRECISION"].ToString(), -1),
						NumericScale = Common.TryConvertInt32(dr["SCALE"].ToString(), -1),

						// needed to be readen
						AutoIncrement = false,
						PrimaryKey = false,
						UserDescription = ""
					};

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
			DataRow[] addInfo = Cache_All_Constraints.Select(string.Format("OWNER='{0}' AND TABLE_NAME='{1}' AND CONSTRAINT_TYPE='{2}' AND COLUMN_NAME='{3}'",
				SpecificOwner.ToUpper(),
				tableName,
				STR_ConstraintType_Primarykey,
				columnName));

			if (addInfo != null && addInfo.Length > 0)
			{
				toSetColumn.PrimaryKey = true;
			}
		}

		/// <summary>
		/// Reads tables index keys
		/// </summary>
		private void ApplyTablesConstraintKeys(List<SchemaTable> tables, OracleServerVersions sqlVersion)
		{
			if (Cache_Indexes.Rows.Count == 0)
				return;

			// find description if there is any
			foreach (var table in tables)
			{
				// filter row
				Cache_Indexes.DefaultView.RowFilter = string.Format("TABLE_NAME='{0}'", table.TableName);

				// fetch findings, if there is any
				foreach (DataRowView keysDataRow in Cache_Indexes.DefaultView)
				{
					// found table !
					DataRow keyRow = keysDataRow.Row;
					var indexName = keyRow["INDEX_NAME"].ToString();

					// it should not be a primary key!
					DataRow[] indexColumnInfo = Cache_IndexColumns.Select(string.Format("INDEX_NAME='{0}'", indexName));

					// column information
					if (indexColumnInfo == null || indexColumnInfo.Length == 0)
						continue;
					var columnName = indexColumnInfo[0]["COLUMN_NAME"].ToString();

					// check if this is aprimary key!
					DataRow[] primaryKeyInfo = Cache_All_Constraints.Select(string.Format("OWNER='{0}' AND TABLE_NAME='{1}' AND CONSTRAINT_TYPE='{2}' AND COLUMN_NAME='{3}'",
						SpecificOwner.ToUpper(),
						table.TableName,
						STR_ConstraintType_Primarykey,
						columnName));

					if (primaryKeyInfo != null && primaryKeyInfo.Length > 0)
					{
						// sorry! this is a primary key and is already added
						// next!
						continue;
					}

					const string STR_IndexUniqueName = "UNIQUE";
					const string STR_IndexNonUniqueName = "NONUNIQUE";

					// constraint Key
					var constraintKey = new SchemaConstraintKey()
					{
						IsUnique = (keyRow["UNIQUENESS"].ToString() == STR_IndexUniqueName),
						KeyColumnName = columnName,
						KeyName = indexName
					};

					// constraint keys
					table.ConstraintKeys.Add(constraintKey);

					// find key column
					SchemaColumn keyColumn = table.FindColumn(constraintKey.KeyColumnName);
					constraintKey.KeyColumn = keyColumn;
				}
			}
		}

		/// <summary>
		/// Reads specified table foreign keys.
		/// </summary>
		private void ApplyTablesForeignKeys(List<SchemaTable> tables)
		{
			if (Cache_ForeignKeys.Rows.Count == 0)
				return;

			// adding the foreign keys
			foreach (DataRow keysDataRow in Cache_ForeignKeys.Rows)
			{
				var foreignKeyTableName = keysDataRow["FOREIGN_KEY_TABLE_NAME"].ToString();
				var primaryKeyTableName = keysDataRow["PRIMARY_KEY_TABLE_NAME"].ToString();

				var foreignKeyConstraintName = keysDataRow["FOREIGN_KEY_CONSTRAINT_NAME"].ToString();
				var primaryKeyConstraintName = keysDataRow["PRIMARY_KEY_CONSTRAINT_NAME"].ToString();

				string foreignKeyColumnName = null;
				string primaryKeyColumnName = null;

				// read the columns info
				DataRow[] columnInfo;

				// reading foreign key column info
				columnInfo = Cache_All_Constraints.Select(string.Format("CONSTRAINT_NAME='{0}'",
					foreignKeyConstraintName));

				if (columnInfo != null && columnInfo.Length > 0)
				{
					foreignKeyColumnName = columnInfo[0]["COLUMN_NAME"].ToString();
				}

				// reading primary key column info
				columnInfo = Cache_All_Constraints.Select(string.Format("CONSTRAINT_NAME='{0}'",
					primaryKeyConstraintName));

				if (columnInfo != null && columnInfo.Length > 0)
				{
					primaryKeyColumnName = columnInfo[0]["COLUMN_NAME"].ToString();
				}

				// there should be column names!
				if (foreignKeyColumnName == null || primaryKeyColumnName == null)
					continue;

				// find schema tables model
				var foreignKeyTable = FindTable(tables, foreignKeyTableName);
				var primaryKeyTable = FindTable(tables, primaryKeyTableName);

				// there should be tables!
				if (foreignKeyTable == null || primaryKeyTable == null)
					continue;

				if (foreignKeyTable != null)
				{

					// foreign key many end
					var oneMultiplicityKey = new SchemaForeignKey()
					{
						ForeignKeyName = foreignKeyConstraintName,
						LocalColumnName = foreignKeyColumnName,
						ForeignColumnName = primaryKeyColumnName,
						ForeignTableName = primaryKeyTableName,
						Multiplicity = SchemaForeignKey.ForeignKeyMultiplicity.One
					};

					// check if it is already there
					if (foreignKeyTable.ForeignKeys.Exists(x => x.ForeignKeyName == oneMultiplicityKey.ForeignKeyName))
						continue;
					//if (foreignKeyTable.ForeignKeys.Exists(x =>
					//    x.ForeignColumnName == oneMultiplicityKey.ForeignColumnName &&
					//    x.LocalColumnName == oneMultiplicityKey.LocalColumnName))
					//    continue;

					// to the list
					foreignKeyTable.ForeignKeys.Add(oneMultiplicityKey);

					// apply local column
					SchemaColumn localColumn = foreignKeyTable.FindColumn(oneMultiplicityKey.LocalColumnName);
					oneMultiplicityKey.LocalColumn = localColumn;
					if (!localColumn.PrimaryKey)
					{
						localColumn.IsReferenceKey = true;
						localColumn.IsReferenceKeyTable = primaryKeyTable;
					}

					if (primaryKeyTable != null)
					{
						// foreign table of that!
						oneMultiplicityKey.ForeignTable = primaryKeyTable;

						// apply foreign column
						SchemaColumn foreignColumn = primaryKeyTable.FindColumn(oneMultiplicityKey.ForeignColumnName);
						oneMultiplicityKey.ForeignColumn = foreignColumn;
					}
					else
					{
						oneMultiplicityKey.ForeignTable = null;
						oneMultiplicityKey.ForeignColumn = null;
					}
				}

				if (primaryKeyTable != null)
				{

					// foreign key many end
					var manyMultiplicityKey = new SchemaForeignKey()
					{
						ForeignKeyName = primaryKeyConstraintName,
						LocalColumnName = primaryKeyColumnName,
						ForeignColumnName = foreignKeyColumnName,
						ForeignTableName = foreignKeyTableName,
						Multiplicity = SchemaForeignKey.ForeignKeyMultiplicity.Many
					};

					// check if it is already there
					if (primaryKeyTable.ForeignKeys.Exists(x => x.ForeignKeyName == manyMultiplicityKey.ForeignKeyName))
						continue;
					//if (primaryKeyTable.ForeignKeys.Exists(x =>
					//    x.ForeignColumnName == manyMultiplicityKey.ForeignColumnName &&
					//    x.LocalColumnName == manyMultiplicityKey.LocalColumnName))
					//    continue;

					// to the list
					primaryKeyTable.ForeignKeys.Add(manyMultiplicityKey);

					// apply local column
					SchemaColumn localColumn = primaryKeyTable.FindColumn(manyMultiplicityKey.LocalColumnName);
					manyMultiplicityKey.LocalColumn = localColumn;
					if (!localColumn.PrimaryKey)
					{
						localColumn.IsReferenceKey = true;
						localColumn.IsReferenceKeyTable = primaryKeyTable;
					}

					if (foreignKeyTable != null)
					{
						// foreign table of that!
						manyMultiplicityKey.ForeignTable = foreignKeyTable;

						// apply foreign column
						SchemaColumn foreignColumn = foreignKeyTable.FindColumn(manyMultiplicityKey.ForeignColumnName);
						manyMultiplicityKey.ForeignColumn = foreignColumn;
					}
					else
					{
						manyMultiplicityKey.ForeignTable = null;
						manyMultiplicityKey.ForeignColumn = null;
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
					// already one-to-(?)
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
		private void NormalizeTablesConstraintKeys(List<SchemaTable> result, OracleServerVersions sqlVersion)
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
		private void ApplyColumnsDescription(string tableName, List<SchemaColumn> columns)
		{
			//throw new NotSupportedException("Oracle doesn't have description field for columns.");
			// there is no column!
			if (columns.Count == 0)
				return;

			// command format
			const string descriptionSql = "SELECT comments, column_name FROM user_col_comments WHERE table_name = '{0}'";

			try
			{
				using (var adapter = new OracleDataAdapter(String.Format(descriptionSql, tableName), (OracleConnection)_dbConnection))
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
								// filter row to find the column
								descriptionData.DefaultView.RowFilter = " column_name='" + column.FieldName + "' ";
								if (descriptionData.DefaultView.Count > 0)
								{
									// description found!
									column.UserDescription = descriptionData.DefaultView[0].Row["comments"].ToString();
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
		/// Converting oracle datatype to DotNet usable datatype
		/// </summary>
		private string FindMatchingDotNetDataType(string dataType)
		{
			switch (dataType.ToUpper())
			{
				case "BFILE":
					return "System.Byte[]";

				case "BINARY_DOUBLE":
					return "System.Double";

				case "BINARY_FLOAT":
					return "System.Decimal";

				case "BLOB":
					return "System.Byte[]";

				case "CHAR":
					return "System.String";

				case "VARCHAR2":
					return "System.String";

				case "CLOB":
					return "System.String";

				case "DATE":
					return "System.DateTime";

				case "NUMBER":
					return "System.Decimal";

				case "FLOAT":
					return "System.Decimal";

				case "INTERVAL DAY TO SECOND":
					return "System.TimeSpan";

				case "INTERVAL YEAR TO MONTH":
					return "System.TimeSpan";

				case "LONG":
					return "System.String";

				case "LONG RAW":
					return "System.String";

				case "NCHAR":
					return "System.String";

				case "NVARCHAR2":
					return "System.String";

				case "NCLOB":
					return "System.String";

				case "RAW":
					return "System.Byte[]";

				case "ROWID":
					return "System.String";

				case "TIMESTAMP":
					return "System.DateTime";

				case "TIMESTAMP WITH LOCAL TIME ZONE":
					return "System.DateTime";

				case "TIMESTAMP WITH TIME ZONE":
					return "System.DateTime";

				case "UROWID":
					return "System.String";

				case "XMLTYPE":
					return "System.String";

				default:
					return "System.String";
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
		/// Detecting oracle version
		/// </summary>
		private OracleServerVersions DetectVersion(DbConnection conn)
		{
			string versionStr = conn.ServerVersion;
			var majorMatch = Regex.Match(versionStr, @"(?<Major>\d+)\..*", RegexOptions.Compiled);
			if (majorMatch != null)
			{
				var majorVersion = Convert.ToInt16(majorMatch.Groups["Major"].Value);
				switch (majorVersion)
				{
					case 8:
						return OracleServerVersions.Oracle8i;

					case 9:
						return OracleServerVersions.Oracle9i;

					case 10:
						return OracleServerVersions.Oracle10g;

					case 11:
						return OracleServerVersions.Oracle11g;

					default:
						if (majorVersion > 11)
						{
							return OracleServerVersions.Oracle11gAbove;
						}
						return OracleServerVersions.Oracle8iBelow;
				}
			}
			else
				return OracleServerVersions.Oracle8iBelow;
		}
		#endregion
	}
}
