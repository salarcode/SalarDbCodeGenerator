using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using SalarDbCodeGenerator.Schema.Database;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Ing-Long Eric Kuo <draconpern@hotmail.com>
// © 2013, All rights reserved
// 2013/08/6
// ====================================
namespace SalarDbCodeGenerator.Schema.DbSchemaReaders
{
    public class MySQLSchemaEngine : ExSchemaEngine
    {
        #region local variables
        private MySqlConnection _dbConnection;
        #endregion

        #region public methods
        public MySQLSchemaEngine(DbConnection dbConnection)
        {
            _dbConnection = (MySqlConnection)dbConnection;
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

            schemaDatabase.SchemaViews = ReadViews();
            schemaDatabase.SchemaTables = ReadTables(schemaDatabase.SchemaViews);
        }

        public override string GetDataProviderClassName(DataProviderClassNames providerClassName)
        {
            switch (providerClassName)
            {
                case DataProviderClassNames.ClassPrefix:
                    return "MySql";
                case DataProviderClassNames.ClassCommand:
                    return "MySqlCommand";
                case DataProviderClassNames.ClassConnection:
                    return "MySqlConnection";
                case DataProviderClassNames.ClassDataAdapter:
                    return "MySqlDataAdapter";
                case DataProviderClassNames.ClassDataReader:
                    return "MySqlDataReader";
                case DataProviderClassNames.ClassParameter:
                    return "MySqlParameter";
                case DataProviderClassNames.ClassTransaction:
                    return "MySqlTransaction";
                case DataProviderClassNames.ClassNamespace:
                    return "MySql.Data.MySqlClient";
                case DataProviderClassNames.AssemblyReference:
                    return "MySql.Data";
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

        #region protected methods
        #endregion

        #region private methods
        /// <summary>
        /// Reads tables list. This method requires views list to prevent from conflict!
        /// </summary>
        private StringCollection ReadTablesList(StringCollection viewsList)
        {
            var result = new StringCollection();
            using (var views = _dbConnection.GetSchema("Tables"))
            {
                foreach (DataRow row in views.Rows)
                {
                    //this.SpecificOwner
                    string tableName = row["TABLE_NAME"].ToString();
                    string tableSchema = row["TABLE_SCHEMA"].ToString();

                    if (String.IsNullOrEmpty(SpecificOwner) || String.Equals(tableSchema, SpecificOwner, StringComparison.OrdinalIgnoreCase))
                    {
                        // search in views about this
                        if (viewsList.Contains(tableName))
                            continue;

                        // add to results
                        result.Add(tableName);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Reads views list
        /// </summary>
        private StringCollection ReadViewsList()
        {
            var result = new StringCollection();

            using (DataTable views = _dbConnection.GetSchema("Views"))
            {
                foreach (DataRow row in views.Rows)
                {
                    string viewName = row["TABLE_NAME"].ToString();
                    string tableSchema = row["TABLE_SCHEMA"].ToString();

                    if (String.IsNullOrEmpty(SpecificOwner) || String.Equals(tableSchema, SpecificOwner, StringComparison.OrdinalIgnoreCase))
                    {
                        // add to results
                        result.Add(viewName);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Reads tables schema from database
        /// </summary>
        private List<DbTable> ReadTables(List<DbView> viewList)
        {
            var result = new List<DbTable>();

            using (var tables = _dbConnection.GetSchema("Tables"))
            {
                foreach (DataRow row in tables.Rows)
                {
                    var tableName = row["TABLE_NAME"].ToString();
                    var ownerName = row["TABLE_SCHEMA"].ToString();

                    if (String.IsNullOrEmpty(SpecificOwner) || String.Equals(ownerName, SpecificOwner, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!IsTableSelected(tableName))
                            continue;

                        var jumpToNext = false;

                        // search in views about this
                        for (int i = 0; i < viewList.Count; i++)
                        {
                            if (viewList[i].TableName == tableName)
                            {
                                jumpToNext = true;
                                // we ignore view here
                                break;
                            }
                        }
                        if (jumpToNext) continue;

                        // View columns
                        var columns = ReadColumns(tableName, ownerName);

                        // read columns description
                        if (ReadColumnsDescription)
                            ApplyColumnsDescription(tableName, columns);

                        // new table and table schema
                        var dbTable = new DbTable(tableName, columns) {OwnerName = ownerName};

                        // add to results
                        result.Add(dbTable);
                    }
                }

                if (ReadConstraintKeys)
                    // The constraint keys will read here
                    ApplyTablesConstraintKeys(result, _dbConnection.Database);

                // it is time to read foreign keys!
                // foreign keys are requested?
                if (ReadTablesForeignKeys)
                    ApplyTablesForeignKeys(result);

                // Normalize the constraints keys
                NormalizeTablesConstraintKeys(result);

                if (ReadTablesForeignKeys)
                    ApplyDetectedOneToOneRelation(result);

            }
            return result;
        }

        /// <summary>
        /// Reads views schema from database
        /// </summary>
        private List<DbView> ReadViews()
        {
            var result = new List<DbView>();

            using (var views = _dbConnection.GetSchema("Views"))
            {
                foreach (DataRow row in views.Rows)
                {
                    var viewName = row["TABLE_NAME"].ToString();
                    var ownerName = row["TABLE_SCHEMA"].ToString();

                    if (String.IsNullOrEmpty(SpecificOwner) || String.Equals(ownerName, SpecificOwner, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!IsViewSelected(viewName))
                            continue;

                        // View columns
                        var columns = ReadColumns(viewName, ownerName);

                        // read columns description
                        if (ReadColumnsDescription)
                            ApplyColumnsDescription(viewName, columns);

                        // new view and view schema
                        var view = new DbView(viewName, columns) {OwnerName = ownerName};

                        // add to results
                        result.Add(view);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Read columns schema from database
        /// </summary>
        private List<DbColumn> ReadColumns(String tableName, string ownerName)
        {
            var result = new List<DbColumn>();

            using (var adapter = new MySqlDataAdapter(String.Format("SELECT * FROM {0}.{1} LIMIT 1", ownerName, tableName), _dbConnection))
            {
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                DataTable columnsSchema;

                // Jjust to avoid stupid "Failed to enable constraints" error!
                using (var tempDs = new DataSet())
                {
                    // Avoiding stupid "Failed to enable constraints" error!
                    tempDs.EnforceConstraints = false;

                    using (var columnsList = new DataTable())
                    {
                        tempDs.Tables.Add(columnsList);

                        // Get from db
                        adapter.Fill(columnsList);

                        // Get schema
                        using (var reader = new DataTableReader(columnsList))
                            columnsSchema = reader.GetSchemaTable();
                    }
                }


                // Used to get columns MySql DataType
                using (DataTable columnsDbTypeTable = _dbConnection.GetSchema("Columns"))
                {

                    // Fetch the rows
                    foreach (DataRow dr in columnsSchema.Rows)
                    {
                        var columnName = dr["ColumnName"].ToString();
                        var column = new DbColumn(columnName)
                        {
                            DataTypeDotNet = dr["DataType"].ToString(),
                            Length = Convert.ToInt32(dr["ColumnSize"]),
                            PrimaryKey = Convert.ToBoolean(dr["IsKey"]),
                            AutoIncrement = Convert.ToBoolean(dr["IsAutoIncrement"]),
                            AllowNull = Convert.ToBoolean(dr["AllowDBNull"]),
                            ColumnOrdinal = Convert.ToInt32(dr["ColumnOrdinal"]),
                            DataTypeMaxLength = Convert.ToInt32(dr["ColumnSize"]),                            
                        };
                        column.FieldNameSchema = DbSchemaNames.FieldName_RemoveInvalidChars(column.FieldNameSchema);

                        // Columns which needs additional fetch
                        var succeed = FillColumnAdditionalInfo(column, columnsDbTypeTable, tableName, columnName);

                        // if additional info readin is failed, don't add it to the list
                        if (succeed)
                        {
                            // Add to result
                            result.Add(column);
                        }
                        else
                        {
                            // TODO: inform the user
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Column additional information
        /// </summary>
        private bool FillColumnAdditionalInfo(DbColumn toSetColumn, DataTable columnsDbTypeTable, string tableName, string columnName)
        {
            var addInfo = columnsDbTypeTable.Select(String.Format("TABLE_NAME='{0}' AND COLUMN_NAME='{1}'",
                                              tableName,
                                              columnName));
            if (addInfo.Length == 0)
            {
                // can't find because of invalid name?? try this
                for (int i = 0; i < columnsDbTypeTable.Rows.Count; i++)
                {
                    var row = columnsDbTypeTable.Rows[i];
                    if (row["TABLE_NAME"].ToString() == tableName &&
                        row["COLUMN_NAME"].ToString() == columnName)
                    {
                        addInfo = new[] { row };
                        break;
                    }
                }
            }

            if (addInfo.Length == 0)
            {
                // sometimes when a column has invalid name for string format this happends!
                // Still no chance
                return false;
            }

            var columnInfo = addInfo[0];

            toSetColumn.DataTypeDb = columnInfo["DATA_TYPE"].ToString();
            toSetColumn.Owner = columnInfo["TABLE_SCHEMA"].ToString();

            var tempInfo = columnInfo["CHARACTER_MAXIMUM_LENGTH"];
            if (tempInfo != null && tempInfo != DBNull.Value)
            {
                try
                {
                    toSetColumn.DataTypeMaxLength = Convert.ToInt32(tempInfo);
                }
                catch (OverflowException o)
                {
                    toSetColumn.DataTypeMaxLength = -1;
                }

                if (toSetColumn.DataTypeMaxLength == -1)
                {
                    toSetColumn.DataTypeMaxLength = int.MaxValue;
                }
            }
            else
            {
                toSetColumn.DataTypeMaxLength = toSetColumn.Length;
            }

            tempInfo = columnInfo["NUMERIC_SCALE"];
            if (tempInfo != null && tempInfo != DBNull.Value)
                toSetColumn.NumericScale = Convert.ToInt32(tempInfo);
            else toSetColumn.NumericScale = -1;

            tempInfo = columnInfo["NUMERIC_PRECISION"];
            if (tempInfo != null && tempInfo != DBNull.Value)
                toSetColumn.NumericPrecision = Convert.ToInt32(tempInfo);
            else toSetColumn.NumericPrecision = -1;

            return true;
        }

        /// <summary>
        /// Reads specified table foreign keys.
        /// </summary>
        private void ApplyTablesForeignKeys(List<DbTable> tables)
        {
            // GENERAL command format
            const string foreignKeySql = @"select 
INFORMATION_SCHEMA.KEY_COLUMN_USAGE.CONSTRAINT_NAME AS ForeignKey, 
INFORMATION_SCHEMA.KEY_COLUMN_USAGE.TABLE_NAME AS FKTable, 
INFORMATION_SCHEMA.KEY_COLUMN_USAGE.COLUMN_Name as FKColumnName, 
INFORMATION_SCHEMA.KEY_COLUMN_USAGE.REFERENCED_TABLE_NAME AS PKTable, 
INFORMATION_SCHEMA.KEY_COLUMN_USAGE.REFERENCED_COLUMN_NAME AS PKColumnName,
INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS.UPDATE_RULE AS UPDATE_RULE,
INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS.DELETE_RULE AS DELETE_RULE
from INFORMATION_SCHEMA.KEY_COLUMN_USAGE
inner join INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS 
ON INFORMATION_SCHEMA.KEY_COLUMN_USAGE.CONSTRAINT_SCHEMA = INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS.CONSTRAINT_SCHEMA 
and INFORMATION_SCHEMA.KEY_COLUMN_USAGE.CONSTRAINT_NAME = INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS.CONSTRAINT_NAME 
where INFORMATION_SCHEMA.KEY_COLUMN_USAGE.constraint_SCHEMA = DATABASE()";
            try
            {
                using (var adapter = new MySqlDataAdapter(foreignKeySql, _dbConnection))
                {
                    adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                    // description data table
                    using (var keysData = new DataTable())
                    {
                        // Just to avoid stupid "Failed to enable constraints" error!
                        using (var tempDs = new DataSet())
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
                                    var manyMultiplicityKeyLocal = new DbForeignKey
                                                                       {
                                                                            ForeignKeyName = keysDataRow["ForeignKey"].ToString(),
                                                                            LocalColumnName = keysDataRow["PKColumnName"].ToString(),
                                                                            ForeignColumnName = keysDataRow["FKColumnName"].ToString(),
                                                                            ForeignTableName = keysDataRow["FKTable"].ToString(),
                                                                            Multiplicity = DbForeignKey.ForeignKeyMultiplicity.ManyToOne
                                                                        };
                                    // check if it is already there
                                    if (primaryKeyTable.ForeignKeys.Exists(
                                        x =>
                                        x.Multiplicity == DbForeignKey.ForeignKeyMultiplicity.ManyToOne &&
                                        x.ForeignKeyName == manyMultiplicityKeyLocal.ForeignKeyName))
                                        continue;

                                    manyMultiplicityKeyLocal.UpdateAction =
                                        ConvertMysqlForeignKeyAction(keysDataRow["UPDATE_RULE"].ToString());
                                    manyMultiplicityKeyLocal.DeleteAction =
                                        ConvertMysqlForeignKeyAction(keysDataRow["DELETE_RULE"].ToString());

                                    // to the list
                                    primaryKeyTable.ForeignKeys.Add(manyMultiplicityKeyLocal);

                                    // apply local column
                                    var localColumn = primaryKeyTable.FindColumnDb(manyMultiplicityKeyLocal.LocalColumnName);
                                    manyMultiplicityKeyLocal.LocalColumn = localColumn;
                                    if (!localColumn.PrimaryKey)
                                    {
                                        localColumn.IsReferenceKey = true;
                                        localColumn.IsReferenceKeyTable = primaryKeyTable;
                                    }

                                    if (foreignKeyTable != null)
                                    {
                                        // foreign table of that!
                                        manyMultiplicityKeyLocal.ForeignTable = foreignKeyTable;

                                        // apply foreign column
                                        DbColumn foreignColumn = foreignKeyTable.FindColumnDb(manyMultiplicityKeyLocal.ForeignColumnName);
                                        manyMultiplicityKeyLocal.ForeignColumn = foreignColumn;
                                    }
                                    else
                                    {
                                        manyMultiplicityKeyLocal.ForeignTable = null;
                                        manyMultiplicityKeyLocal.ForeignColumn = null;
                                    }
                                }

                                // one-to-? foreign relation will be added
                                if (foreignKeyTable != null)
                                {
                                    // foreign key many end
                                    var oneMultiplicityKeyForeign = new DbForeignKey
                                                                        {
                                                                            ForeignKeyName = keysDataRow["ForeignKey"].ToString(),
                                                                            LocalColumnName = keysDataRow["FKColumnName"].ToString(),
                                                                            ForeignColumnName = keysDataRow["PKColumnName"].ToString(),
                                                                            ForeignTableName = keysDataRow["PKTable"].ToString(),
                                                                            Multiplicity = DbForeignKey.ForeignKeyMultiplicity.OneToMany
                                                                        };
                                    // check if it is already there
                                    if (foreignKeyTable.ForeignKeys.Exists(
                                        x =>
                                        x.Multiplicity == DbForeignKey.ForeignKeyMultiplicity.OneToMany &&
                                        x.ForeignKeyName == oneMultiplicityKeyForeign.ForeignKeyName))
                                        continue;

                                    oneMultiplicityKeyForeign.UpdateAction =
                                        ConvertMysqlForeignKeyAction(keysDataRow["UPDATE_RULE"].ToString());
                                    oneMultiplicityKeyForeign.DeleteAction =
                                        ConvertMysqlForeignKeyAction(keysDataRow["DELETE_RULE"].ToString());

                                    // to the list
                                    foreignKeyTable.ForeignKeys.Add(oneMultiplicityKeyForeign);

                                    // apply local column
                                    DbColumn localColumn = foreignKeyTable.FindColumnDb(oneMultiplicityKeyForeign.LocalColumnName);
                                    oneMultiplicityKeyForeign.LocalColumn = localColumn;
                                    if (!localColumn.PrimaryKey)
                                    {
                                        localColumn.IsReferenceKey = true;
                                        localColumn.IsReferenceKeyTable = primaryKeyTable;
                                    }

                                    if (primaryKeyTable != null)
                                    {
                                        // foreign table of that!
                                        oneMultiplicityKeyForeign.ForeignTable = primaryKeyTable;

                                        // apply foreign column
                                        DbColumn foreignColumn = primaryKeyTable.FindColumnDb(oneMultiplicityKeyForeign.ForeignColumnName);
                                        oneMultiplicityKeyForeign.ForeignColumn = foreignColumn;
                                    }
                                    else
                                    {
                                        oneMultiplicityKeyForeign.ForeignTable = null;
                                        oneMultiplicityKeyForeign.ForeignColumn = null;
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
                // Seems this version of postgresql doesn't support this query!
                // don't stop here!
            }
        }

        /// <summary>
        /// Reads tables index keys
        /// </summary>
        private void ApplyTablesConstraintKeys(IEnumerable<DbTable> tables, string ownerName)
        {
            string constraintKeySql = @"SELECT INFORMATION_SCHEMA.COLUMNS.TABLE_NAME AS TableName, INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME AS ColumnName, 
INFORMATION_SCHEMA.KEY_COLUMN_USAGE.CONSTRAINT_NAME AS IndexName, INFORMATION_SCHEMA.COLUMNS.COLUMN_KEY='PRI' AS IsPrimaryKey, 
INFORMATION_SCHEMA.COLUMNS.COLUMN_KEY='UNI' AS IsUnique
FROM INFORMATION_SCHEMA.COLUMNS
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ON INFORMATION_SCHEMA.KEY_COLUMN_USAGE.TABLE_NAME = INFORMATION_SCHEMA.COLUMNS.TABLE_NAME AND INFORMATION_SCHEMA.KEY_COLUMN_USAGE.COLUMN_NAME = INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME
WHERE INFORMATION_SCHEMA.COLUMNS.TABLE_SCHEMA = DATABASE()";

            using (var adapter = new MySqlDataAdapter(constraintKeySql, _dbConnection))
            {
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                // description data table
                using (var keysData = new DataTable())
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
                                var constraintKey = new DbConstraintKey()
                                {
                                    IsUnique = keyRow["IsUnique"].ToString() == "1",
                                    KeyColumnName = keyRow["ColumnName"].ToString(),
                                    KeyName = keyRow["IndexName"].ToString()
                                };

                                // constraint keys
                                table.ConstraintKeys.Add(constraintKey);

                                // find key column
                                DbColumn keyColumn = table.FindColumnDb(constraintKey.KeyColumnName);
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
        private void ApplyDetectedOneToOneRelation(IEnumerable<DbTable> tables)
        {
            foreach (var table in tables)
                foreach (var fkey in table.ForeignKeys)
                {
                    //// already ont-to-?
                    //if (fkey.Multiplicity == DbForeignKey.ForeignKeyMultiplicity.OneToMany ||
                    //    fkey.Multiplicity == DbForeignKey.ForeignKeyMultiplicity.OneToOne)
                    //    continue;
                    // already ont-to-one
                    if (fkey.Multiplicity == DbForeignKey.ForeignKeyMultiplicity.OneToOne)
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
        private void NormalizeTablesConstraintKeys(IEnumerable<DbTable> result)
        {
            // look in tables list
            foreach (var table in result)
            {
                if (table.ConstraintKeys.Count == 0)
                {
                    continue;
                }

                var duplicateConstraints = new StringCollection();

                // fetching the contraints keys
                for (var j = table.ConstraintKeys.Count - 1; j >= 0; j--)
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
                    var index = table.ForeignKeys.FindIndex(x =>
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

                        if (notUniqueKeys.Count > 0)
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
            const string descriptionSql =
                    @"SELECT 	c.table_schema,
				                c.table_name,
				                c.column_name as objname,
				                pgd.description as value
                    FROM 		pg_catalog.pg_statio_all_tables as st 
				                    inner join pg_catalog.pg_description pgd on (pgd.objoid=st.relid) 
				                    inner join information_schema.columns c on (pgd.objsubid=c.ordinal_position and c.table_schema=st.schemaname and c.table_name=st.relname)
                    WHERE		c.table_name = '{0}'";

            try
            {
                using (var adapter = new MySqlDataAdapter(String.Format(descriptionSql, tableName), _dbConnection))
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
                                descriptionData.DefaultView.RowFilter = " objname='" + column.FieldNameDb + "' ";
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
                // Seems this version of postgresql doesn't support this query!
                // don't stop here!
                // TODO: inform user
            }
        }

        /// <summary>
        /// Finds table from list
        /// </summary>
        private static DbTable FindTable(IEnumerable<DbTable> tables, string tableName)
        {
            foreach (var table in tables)
            {
                if (table.TableName == tableName)
                    return table;
            }
            return null;
        }

        private static DbForeignKeyAction ConvertMysqlForeignKeyAction(string actionCode)
        {
            switch (actionCode)
            {
                case "NO ACTION":
                    return DbForeignKeyAction.NoAction;
                case "CASCADE":
                    return DbForeignKeyAction.Cascade;
                case "SET NULL":
                    return DbForeignKeyAction.SetNull;
                case "":
                    return DbForeignKeyAction.SetDefault;
                //case 4:
                //    return DbForeignKeyAction.Restrict;
            }
            return DbForeignKeyAction.NotSet;
        }
        #endregion
    }
}
