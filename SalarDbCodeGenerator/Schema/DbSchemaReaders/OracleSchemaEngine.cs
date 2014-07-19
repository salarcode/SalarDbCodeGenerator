using Oracle.ManagedDataAccess.Client;
using SalarDbCodeGenerator.DbProject;
using SalarDbCodeGenerator.Schema.Database;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Salar Khalilzadeh <salar2k@gmail.com>
// © 2012, All rights reserved
// 2012/07/06
// ====================================
namespace SalarDbCodeGenerator.Schema.DbSchemaReaders
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
		public override void FillSchema(DbDatabase schemaDatabase)
		{
			if (schemaDatabase == null)
				throw new ArgumentNullException("schemaDatabase", "Database is not specified.");

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

		public override void Dispose()
		{
			if (_dbConnection != null)
				_dbConnection.Close();
			_dbConnection = null;
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
		private DataTable _cache_All_Sequences;
		private DataTable Cache_All_Sequences
		{
			get
			{
				const string all_sequences = "SELECT SEQUENCE_NAME FROM ALL_SEQUENCES";

				if (_cache_All_Sequences == null)
				{
					using (var adapter = new OracleDataAdapter(all_sequences, _dbConnection))
					{
						_cache_All_Sequences = new DataTable();
						adapter.Fill(_cache_All_Sequences);
					}
				}
				return _cache_All_Sequences;
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
		private List<DbTable> ReadTables(List<DbView> viewList)
		{
			List<DbTable> result = new List<DbTable>();

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

					if (!IsTableSelected(tableName))
						continue;

					var jumpToNext = false;
					// search in views about this
					foreach (var view in viewList)
					{
						if (view.TableName == tableName)
						{
							jumpToNext = true;
							// we ignore view here
							break;
						}
					}
					if (jumpToNext) continue;

					// View columns
					List<DbColumn> columns = ReadColumns(tableName);

					// read columns description
					if (ReadColumnsDescription)
						ApplyColumnsDescription(tableName, columns);

					// new table
					var dbTable = new DbTable(tableName, columns);

					// table schema
					dbTable.OwnerName = row["OWNER"].ToString();

					// add to results
					result.Add(dbTable);
				}

				// correct tables name by case sesitivity usage!
				AssignCaseSensitiveTablesName(result);

				ApplyTablesSequenceNames(result);

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
		private void AssignCaseSensitiveTablesName(List<DbTable> tables)
		{
			if (tables.Count == 0)
				return;

			// the copy list
			var workingTables = new List<DbTable>();
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
		private List<DbView> ReadViews()
		{
			List<DbView> result = new List<DbView>();

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

					if (!IsViewSelected(viewName))
						continue;

					// View columns
					List<DbColumn> columns = ReadColumns(viewName);

					// new view
					var view = new DbView(viewName, columns);

					// view schema
					view.OwnerName = row["OWNER"].ToString();

					// add to results
					result.Add(view);
				}
			}
			return result;
		}

		/// <summary>
		/// Read columns schema from database
		/// </summary>
		private List<DbColumn> ReadColumns(String tableName)
		{
			List<DbColumn> result = new List<DbColumn>();


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
					DbColumn column = new DbColumn(columnName)
					{
						Owner = dr["OWNER"].ToString(),
						DataTypeDotNet = FindMatchingDotNetDataType(dr["DATATYPE"].ToString()),

						DataTypeDb = dr["DATATYPE"].ToString(),
						Length = Common.TryConvertInt32(dr["LENGTH"].ToString(), 0),
						ColumnOrdinal = Common.TryConvertInt32(dr["ID"].ToString(), -1),
						AllowNull = dr["Nullable"].ToString().ToUpper() == "Y",

						NumericPrecision = Common.TryConvertInt32(dr["PRECISION"].ToString(), -1),
						NumericScale = Common.TryConvertInt32(dr["SCALE"].ToString(), -1),

						// needed to be read
						AutoIncrement = false,
						PrimaryKey = false,
						UserDescription = ""
					};
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
		private void ApplyTablesConstraintKeys(List<DbTable> tables, OracleServerVersions sqlVersion)
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
						// sorry! this is a primary key and it is already added
						// next!
						continue;
					}

					const string STR_IndexUniqueName = "UNIQUE";
					const string STR_IndexNonUniqueName = "NONUNIQUE";

					// constraint Key and its uniqueness
					var constraintKey = new DbConstraintKey()
					{
						IsUnique = (keyRow["UNIQUENESS"].ToString() == STR_IndexUniqueName),
						KeyColumnName = columnName,
						KeyName = indexName
					};

					// constraint keys
					table.ConstraintKeys.Add(constraintKey);

					// find key column
					DbColumn keyColumn = table.FindColumnDb(constraintKey.KeyColumnName);
					constraintKey.KeyColumn = keyColumn;
				}
			}
		}

		/// <summary>
		/// Reads specified table foreign keys.
		/// </summary>
		private void ApplyTablesForeignKeys(List<DbTable> tables)
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
					var oneMultiplicityKey = new DbForeignKey()
					{
						ForeignKeyName = foreignKeyConstraintName,
						LocalColumnName = foreignKeyColumnName,
						ForeignColumnName = primaryKeyColumnName,
						ForeignTableName = primaryKeyTableName,
						Multiplicity = DbForeignKey.ForeignKeyMultiplicity.OneToMany
					};

					// check if it is already there
					if (foreignKeyTable.ForeignKeys.Exists(x => x.ForeignKeyName == oneMultiplicityKey.ForeignKeyName))
						continue;

					//oneMultiplicityKey.UpdateAction =
					//    ConvertOracleForeignKeyAction(keysDataRow["UPDATE_RULE"].ToString());
					oneMultiplicityKey.DeleteAction =
						ConvertOracleForeignKeyAction(keysDataRow["DELETE_RULE"].ToString());

					// to the list
					foreignKeyTable.ForeignKeys.Add(oneMultiplicityKey);

					// apply local column
					DbColumn localColumn = foreignKeyTable.FindColumnDb(oneMultiplicityKey.LocalColumnName);
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
						DbColumn foreignColumn = primaryKeyTable.FindColumnDb(oneMultiplicityKey.ForeignColumnName);
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
					var manyMultiplicityKey = new DbForeignKey()
					{
						ForeignKeyName = primaryKeyConstraintName,
						LocalColumnName = primaryKeyColumnName,
						ForeignColumnName = foreignKeyColumnName,
						ForeignTableName = foreignKeyTableName,
						Multiplicity = DbForeignKey.ForeignKeyMultiplicity.ManyToOne
					};

					// check if it is already there
					if (primaryKeyTable.ForeignKeys.Exists(x => x.ForeignKeyName == manyMultiplicityKey.ForeignKeyName))
						continue;

					//manyMultiplicityKey.UpdateAction =
					//    ConvertOracleForeignKeyAction(keysDataRow["UPDATE_RULE"].ToString());
					manyMultiplicityKey.DeleteAction =
						ConvertOracleForeignKeyAction(keysDataRow["DELETE_RULE"].ToString());

					// to the list
					primaryKeyTable.ForeignKeys.Add(manyMultiplicityKey);

					// apply local column
					DbColumn localColumn = primaryKeyTable.FindColumnDb(manyMultiplicityKey.LocalColumnName);
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
						DbColumn foreignColumn = foreignKeyTable.FindColumnDb(manyMultiplicityKey.ForeignColumnName);
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
		/// Applies the sequence name for each primary key in the tables
		/// </summary>
		private void ApplyTablesSequenceNames(IEnumerable<DbTable> tables)
		{
			foreach (var table in tables)
			{
				foreach (var column in table.SchemaColumns)
				{
					if (!column.PrimaryKey) continue;
					column.SequenceName = GetTableColumnSequenceName(table.TableNameSchema, column);
				}
			}
		}

		private DbForeignKeyAction ConvertOracleForeignKeyAction(string action)
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

				case "RESTRICT":
					return DbForeignKeyAction.Restrict;

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
					// already one-to-(?)
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
		/// Removes duplicate table constraints, PK > UK > IX
		/// </summary>
		private void NormalizeTablesConstraintKeys(List<DbTable> result, OracleServerVersions sqlVersion)
		{
			// look in tables list
			foreach (DbTable table in result)
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
		private void ApplyColumnsDescription(string tableName, List<DbColumn> columns)
		{
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
								descriptionData.DefaultView.RowFilter = " column_name='" + column.FieldNameDb + "' ";
								if (descriptionData.DefaultView.Count > 0)
								{
									// description found!
									column.UserDescription = descriptionData.DefaultView[0].Row["comments"].ToString();
									column.UserDescription = column.UserDescription.Replace("\r\n", " ").Replace("\n", " ");
								}
							}
					}
				}
			}
			catch
			{
				// something is wrong! don't stop here!
				// TODO: inform user
			}
		}

		/// <summary>
		/// Finds the sequence name for the PRIMARY KEY!!!
		/// </summary>
		private string GetTableColumnSequenceName(string tableName, DbColumn column)
		{
			const string oracleSequenceExt = "_SEQ";
			const int maxNameLen = 31;

			var sequenceName = tableName + "_" + column.FieldNameDb + oracleSequenceExt;
			if (sequenceName.Length > maxNameLen)
				sequenceName = sequenceName.Substring(0, maxNameLen - 1);
			sequenceName = sequenceName.ToUpper();

			// reading primary key column info
			var squenceColumn = Cache_All_Sequences.Select(string.Format("SEQUENCE_NAME='{0}'",
				sequenceName));
			if (squenceColumn != null && squenceColumn.Length > 0)
			{
				return squenceColumn[0]["SEQUENCE_NAME"].ToString();
			}
			return string.Empty;
		}

		/// <summary>
		/// Finds the sequence name for the PRIMARY KEY!!!
		/// </summary>
		private string GetTableColumnSequenceName_FromDB(string tableName, DbColumn column)
		{
			const string oracleSequenceExt = "_SEQ";
			const int maxNameLen = 31;

			var sequenceName = tableName + "_" + column.FieldNameDb + oracleSequenceExt;
			if (sequenceName.Length > maxNameLen)
				sequenceName = sequenceName.Substring(0, maxNameLen - 1);
			sequenceName = sequenceName.ToUpper();

			// here we know the sequence name
			// but we have to be sure of it is there
			// so a query to the db is required!

			const string sequenceSelectCommand = "SELECT SEQUENCE_NAME FROM ALL_SEQUENCES WHERE SEQUENCE_NAME = N'{0}' ";

			try
			{
				using (var adapter = new OracleDataAdapter(
					String.Format(sequenceSelectCommand, sequenceName),
					(OracleConnection)_dbConnection))
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
						{
							var row = descriptionData.Rows[0];
							var seqName = row["SEQUENCE_NAME"].ToString();

							if (!string.IsNullOrWhiteSpace(seqName))
								return seqName;

							return string.Empty;
						}
					}
				}
			}
			catch
			{
				// something is wrong! don't stop here!
				// TODO: inform user
			}
			return string.Empty;
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
		private DbTable FindTable(List<DbTable> tables, string tableName)
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
