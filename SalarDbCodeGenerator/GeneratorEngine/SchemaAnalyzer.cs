using System;
using System.Collections.Generic;
using System.Linq;
using SalarDbCodeGenerator.DbProject;
using SalarDbCodeGenerator.Schema;
using SalarDbCodeGenerator.Schema.Database;
using SalarDbCodeGenerator.Schema.Patterns;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Salar Khalilzadeh <salar2k@gmail.com>
// © 2012, All rights reserved
// 2012/07/07
// ====================================
namespace SalarDbCodeGenerator.GeneratorEngine
{
	public class SchemaAnalyzer
	{
		#region local variables
		PatternProject _patternProject;
		ProjectDefinaton _projectDef;
		DbDatabase _database;
		#endregion

		public SchemaAnalyzer(ProjectDefinaton project, PatternProject pattern, DbDatabase database)
		{
			_patternProject = pattern;
			_projectDef = project;
			_database = database;
		}

		public void AnalyzeAndRename()
		{
			// Renaming and duplicate checking of names
			for (int i = 0; i < _database.SchemaTables.Count; i++)
			{
				var dbTable = _database.SchemaTables[i];
				dbTable.TableNameSchema = NaturalizeNames_TableName_Rename(dbTable.TableName);
				dbTable.TableNameSchemaCS = NaturalizeNames_TableName_Rename(dbTable.TableNameCS);
			}
			for (int i = 0; i < _database.SchemaTables.Count; i++)
			{
				var dbTable = _database.SchemaTables[i];
				dbTable.TableNameSchema = NaturalizeNames_TableSchemaName_Duplicate(dbTable, false);
				dbTable.TableNameSchemaCS = NaturalizeNames_TableSchemaNameCS_Duplicate(dbTable, false);
			}

			// Renaming and duplicate checking of names
			for (int i = 0; i < _database.SchemaViews.Count; i++)
			{
				var dbView = _database.SchemaViews[i];
				dbView.TableNameSchema = NaturalizeNames_ViewName_Rename(dbView.TableName);
				dbView.TableNameSchemaCS = NaturalizeNames_ViewName_Rename(dbView.TableNameCS);
			}
			for (int i = 0; i < _database.SchemaViews.Count; i++)
			{
				var dbView = _database.SchemaViews[i];
				dbView.TableNameSchema = NaturalizeNames_TableSchemaName_Duplicate(dbView, true);
				dbView.TableNameSchemaCS = NaturalizeNames_TableSchemaNameCS_Duplicate(dbView, true);
			}

			// renaming field names
			foreach (var dbTable in _database.SchemaTables)
			{
				for (int i = 0; i < dbTable.SchemaColumns.Count; i++)
				{
					var dbColumn = dbTable.SchemaColumns[i];
					dbColumn.FieldNameSchema = NaturalizeNames_FieldName(dbTable, dbColumn, dbColumn.FieldNameSchema, true);
					dbColumn.DataCondensedType = DotNetSchemaDataInfo.DetermineColumnCondensedType(dbColumn.DataTypeDotNet);
					dbColumn.DataTypeDotNet = NaturalizeNames_DotNetTypeClean(dbColumn.DataTypeDotNet);
					dbColumn.DataTypeNullable = Determine_DataTypeNullable(dbColumn);
					dbColumn.ExplicitCastDataType = Determine_ExplicitCastDataType(dbColumn);
				}
				for (int i = 0; i < dbTable.ForeignKeys.Count; i++)
				{
					var dbForeignKey = dbTable.ForeignKeys[i];
					dbForeignKey.ForeignTableNameInLocalTable = NaturalizeNames_ForeignTableFieldName(dbTable, dbForeignKey);
				}
			}
			// renaming field names
			foreach (var dbView in _database.SchemaViews)
			{
				for (int i = 0; i < dbView.SchemaColumns.Count; i++)
				{
					var dbColumn = dbView.SchemaColumns[i];
					dbColumn.FieldNameSchema = NaturalizeNames_FieldName(dbView, dbColumn, dbColumn.FieldNameSchema, true);
					dbColumn.DataCondensedType = DotNetSchemaDataInfo.DetermineColumnCondensedType(dbColumn.DataTypeDotNet);
					dbColumn.DataTypeDotNet = NaturalizeNames_DotNetTypeClean(dbColumn.DataTypeDotNet);
					dbColumn.DataTypeNullable = Determine_DataTypeNullable(dbColumn);
					dbColumn.ExplicitCastDataType = Determine_ExplicitCastDataType(dbColumn);
				}
			}
		}


		private string NaturalizeNames_DotNetTypeClean(string dataTypeDotNet)
		{
			string _dotNetType = dataTypeDotNet;
			if (string.IsNullOrEmpty(_dotNetType))
				return _dotNetType;
			var stringCompare = StringComparison.CurrentCulture;
			if (!_patternProject.LanguageSettings.KeywordsCaseSensitive)
				stringCompare = StringComparison.InvariantCultureIgnoreCase;

			_dotNetType = _dotNetType.Replace(DotNetSchemaDataInfo.DotNetArrayIdenticator, _patternProject.LanguageSettings.ArrayIdenticator);

			foreach (var dataType in _patternProject.LanguageSettings.ExplicitCastDataTypes)
			{
				var type = "." + dataType;
				if (_dotNetType.EndsWith(type, stringCompare))
				{
					return _dotNetType;
				}
				if (_dotNetType.Equals(dataType, stringCompare))
				{
					return _dotNetType;
				}
			}

			if (_dotNetType.StartsWith("System."))
			{
				if (_dotNetType.IndexOf(".", "System.".Length) != -1)
				{
					return _dotNetType;
				}
				return _dotNetType.Remove(0, "System.".Length);
			}
			return _dotNetType;
		}

		private bool Determine_ExplicitCastDataType(DbColumn dbColumn)
		{
			if (dbColumn.IsArray(_patternProject.LanguageSettings.ArrayIdenticator))
				return true;
			var stringCompare = StringComparison.CurrentCulture;
			if (!_patternProject.LanguageSettings.KeywordsCaseSensitive)
				stringCompare = StringComparison.InvariantCultureIgnoreCase;

			// any type that has any dot (.) in it!
			if (dbColumn.DataTypeDotNet.IndexOf('.') != -1)
				return true;

			foreach (var dataType in _patternProject.LanguageSettings.ExplicitCastDataTypes)
			{
				if (dataType.Equals(dbColumn.DataTypeDotNet, stringCompare))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Column data type is nullabe
		/// </summary>
		private bool Determine_DataTypeNullable(DbColumn dbColumn)
		{
			if (dbColumn.IsArray(_patternProject.LanguageSettings.ArrayIdenticator))
				return true;
			var stringCompare = StringComparison.CurrentCulture;
			if (!_patternProject.LanguageSettings.KeywordsCaseSensitive)
				stringCompare = StringComparison.InvariantCultureIgnoreCase;

			foreach (var dataType in _patternProject.LanguageSettings.NullableDataTypes)
			{
				var type = "." + dataType;
				if (dbColumn.DataTypeDotNet.EndsWith(type, stringCompare))
				{
					return true;
				}
				if (dataType.Equals(dbColumn.DataTypeDotNet, stringCompare))
				{
					return true;
				}
			}
			return false;
		}

		///// <summary>
		///// Applies project settings to .NET data type
		///// </summary>
		//private string NaturalizeNames_DotNetType(string dotNetTypeName)
		//{
		//    return dotNetTypeName.Replace(DbColumn.DotNetArrayIdenticator, _patternProject.LanguageSettings.ArrayIdenticator);
		//}

		/// <summary>
		/// Applies project settings to fields name
		/// </summary>
		/// <param name="table"></param>
		/// <param name="fieldName"></param>
		/// <param name="isAlreadyMember">is the field aready member of the table, or it is an external</param>
		/// <returns></returns>
		private string NaturalizeNames_FieldName(DbTable table, DbColumn column, string fieldName, bool isAlreadyMember)
		{
			if (string.IsNullOrEmpty(fieldName))
				return fieldName;
			var newName = fieldName;

			var stringCompare = StringComparison.InvariantCulture;
			if (_patternProject.LanguageSettings.KeywordsCaseSensitive == false)
				stringCompare = StringComparison.InvariantCultureIgnoreCase;

			// suppress pattern
			string replacement = _patternProject.LanguageSettings.LanguageKeywordsSuppress;

			// renaming options
			newName = NaturalizeNames_RenamingOptions(newName, _projectDef.RenamingOptions, false, true);

			// remove names
			newName = NaturalizeNames_Name_RemoveInvalidChars(newName);

			int initReplacePartCount = 0;
			string initReplacePartStr = "";

			// column name should not be same
			if (newName.Equals(table.TableNameSchema, stringCompare) ||
				newName.Equals(table.TableNameSchemaCS, stringCompare))
			{
				var renamedName = string.Format(replacement, newName, initReplacePartStr);
				initReplacePartCount++;
				initReplacePartStr = initReplacePartCount.ToString();

				// no duplicate
				while (table.FindColumnSchema(renamedName) != null)
				{
					renamedName = string.Format(replacement, newName, initReplacePartStr);
					initReplacePartCount++;
					initReplacePartStr = initReplacePartCount.ToString();
				}

				newName = renamedName;
			}

			// field name is not changed and is a member
			if (newName.Equals(fieldName, stringCompare) && isAlreadyMember)
			{
				var sameNameColumns =
					table.SchemaColumns.Where(x => x.FieldNameSchema.Equals(newName, stringCompare)).ToList();

				// no more than one accurance, including itself
				if (sameNameColumns.Count > 1 &&
					sameNameColumns.IndexOf(column) > 0)
				{
					var renamedName = string.Format(replacement, newName, initReplacePartStr);
					initReplacePartCount++;
					initReplacePartStr = initReplacePartCount.ToString();

					// no duplicate
					while (table.FindColumnSchema(renamedName) != null)
					{
						renamedName = string.Format(replacement, newName, initReplacePartStr);
						initReplacePartCount++;
						initReplacePartStr = initReplacePartCount.ToString();
					}
					newName = renamedName;
				}
			}
			else
			{
				if (table.FindColumnSchema(newName) != null)
				{
					var renamedName = string.Format(replacement, newName, initReplacePartStr);
					initReplacePartCount++;
					initReplacePartStr = initReplacePartCount.ToString();

					// no duplicate
					while (table.FindColumnSchema(renamedName) != null)
					{
						renamedName = string.Format(replacement, newName, initReplacePartStr);
						initReplacePartCount++;
						initReplacePartStr = initReplacePartCount.ToString();
					}
					newName = renamedName;
				}
			}

			// checking keyword match if only not changed field name
			if (newName.Equals(fieldName, stringCompare))
			{
				// ignoring keywords
				foreach (var keyword in _patternProject.LanguageSettings.LanguageKeywords)
				{
					// keyword match
					if (newName.Equals(keyword, stringCompare))
					{
						var renamedName = string.Format(replacement, newName, initReplacePartStr);
						initReplacePartCount++;
						initReplacePartStr = initReplacePartCount.ToString();

						// no duplicate
						while (table.FindColumnSchema(renamedName) != null)
						{
							renamedName = string.Format(replacement, newName, initReplacePartStr);
							initReplacePartCount++;
							initReplacePartStr = initReplacePartCount.ToString();
						}

						newName = renamedName;
						// name is chaned and check is no longer required
						break;
					}
				}
			}

			// field name is ok to be used
			return newName;
		}


		/// <summary>
		/// Applies project settings to fields name
		/// </summary> 
		private string NaturalizeNames_ForeignTableFieldName(DbTable table, DbForeignKey foreignKey)
		{
			if (string.IsNullOrEmpty(foreignKey.ForeignTableNameInLocalTable))
				return foreignKey.ForeignTableNameInLocalTable;
			var newName = NaturalizeNames_TableName_Rename(foreignKey.ForeignTableNameInLocalTable);

			var stringCompare = StringComparison.InvariantCulture;
			if (_patternProject.LanguageSettings.KeywordsCaseSensitive == false)
				stringCompare = StringComparison.InvariantCultureIgnoreCase;

			// suppress pattern
			string replacement = _patternProject.LanguageSettings.LanguageKeywordsSuppress;

			int initReplacePartCount = 0;
			string initReplacePartStr = "";

			// column name should not be the same
			if (newName.Equals(table.TableNameSchema, stringCompare) ||
				newName.Equals(table.TableNameSchemaCS, stringCompare))
			{
				var renamedName = string.Format(replacement, newName, initReplacePartStr);
				initReplacePartCount++;
				initReplacePartStr = initReplacePartCount.ToString();

				// no duplicate
				while (table.FindColumnSchema(renamedName) != null ||
					table.ForeignKeys.Any(x => x.ForeignTableNameInLocalTable.Equals(renamedName, stringCompare)))
				{
					renamedName = string.Format(replacement, newName, initReplacePartStr);
					initReplacePartCount++;
					initReplacePartStr = initReplacePartCount.ToString();
				}
				newName = renamedName;
			}

			// foreign name is not changed and is a member
			if (newName.Equals(foreignKey.ForeignTableNameInLocalTable, stringCompare))
			{
				var sameNameForeignKeys =
					table.ForeignKeys.Where(x => x.ForeignTableNameInLocalTable.Equals(newName, stringCompare)).ToList();

				// no more than one occurrence, including itself
				if (table.FindColumnSchema(newName) != null ||
					(sameNameForeignKeys.Count > 1 && sameNameForeignKeys.IndexOf(foreignKey) > 0))
				{
					var renamedName = string.Format(replacement, newName, initReplacePartStr);
					initReplacePartCount++;
					initReplacePartStr = initReplacePartCount.ToString();

					// no duplicate
					while (table.FindColumnSchema(renamedName) != null ||
						table.ForeignKeys.Any(x => x.ForeignTableNameInLocalTable.Equals(renamedName, stringCompare)))
					{
						renamedName = string.Format(replacement, newName, initReplacePartStr);
						initReplacePartCount++;
						initReplacePartStr = initReplacePartCount.ToString();
					}
					newName = renamedName;
				}
			}
			else
			{
				if (table.FindColumnSchema(newName) != null ||
						table.ForeignKeys.Any(x => x.ForeignTableNameInLocalTable.Equals(newName, stringCompare)))
				{
					var renamedName = string.Format(replacement, newName, initReplacePartStr);
					initReplacePartCount++;
					initReplacePartStr = initReplacePartCount.ToString();

					// no duplicate
					while (table.FindColumnSchema(renamedName) != null ||
						   table.ForeignKeys.Any(x => x.ForeignTableNameInLocalTable.Equals(renamedName, stringCompare)))
					{
						renamedName = string.Format(replacement, newName, initReplacePartStr);
						initReplacePartCount++;
						initReplacePartStr = initReplacePartCount.ToString();
					}
					newName = renamedName;
				}
			}

			// checking keyword match if only foreign name is not changed
			if (newName.Equals(foreignKey.ForeignTableNameInLocalTable, stringCompare))
			{
				// ignoring keywords
				foreach (var keyword in _patternProject.LanguageSettings.LanguageKeywords)
				{
					// keyword match
					if (newName.Equals(keyword, stringCompare))
					{
						var renamedName = string.Format(replacement, newName, initReplacePartStr);
						initReplacePartCount++;
						initReplacePartStr = initReplacePartCount.ToString();

						// no duplicate
						while (table.FindColumnSchema(renamedName) != null ||
							table.ForeignKeys.Any(x => x.ForeignTableNameInLocalTable.Equals(renamedName, stringCompare)))
						{
							renamedName = string.Format(replacement, newName, initReplacePartStr);
							initReplacePartCount++;
							initReplacePartStr = initReplacePartCount.ToString();
						}

						newName = renamedName;
						// name is chaned and check is no longer required
						break;
					}
				}
			}

			// foreign name is ok to be used
			return newName;
		}


		private string NaturalizeNames_Name_RemoveInvalidChars(string name)
		{
			if (string.IsNullOrEmpty(name))
				return name;
			string newName = name;
			if (char.IsNumber(newName[0]))
			{
				// suppress pattern
				newName = string.Format(_patternProject.LanguageSettings.LanguageKeywordsSuppress
										, ""
										, newName);
			}
			var supressChar = _patternProject.LanguageSettings.LanguageInvalidCharsSuppress;
			foreach (var c in _patternProject.LanguageSettings.LanguageInvalidCharsArray)
			{
				newName = newName.Replace(c, supressChar);
			}
			return newName;
		}


		/// <summary>
		/// </summary>
		private string NaturalizeNames_TableSchemaName_Duplicate(DbTable table, bool checkViews)
		{
			string newName = table.TableNameSchema;

			// suppress pattern
			string replacement = _patternProject.LanguageSettings.LanguageKeywordsSuppress;

			int initReplacePartCount = 0;
			string initReplacePartStr = "";

			if (checkViews)
			{
				var sameNameTables = _database.SchemaTables.Where(x => x.TableNameSchema == newName).ToList();
				sameNameTables.AddRange(_database.SchemaViews.Where(x => x.TableNameSchema == newName));

				if (sameNameTables.Count > 1 && sameNameTables.IndexOf(table) > 0)
				{
					var renamedName = string.Format(replacement, newName, initReplacePartStr);
					initReplacePartCount++;
					initReplacePartStr = initReplacePartCount.ToString();

					while (_database.SchemaTables.Any(x => x.TableNameSchema == renamedName) ||
						_database.SchemaViews.Any(x => x.TableNameSchema == renamedName))
					{
						renamedName = string.Format(replacement, newName, initReplacePartStr);
						initReplacePartCount++;
						initReplacePartStr = initReplacePartCount.ToString();
					}
					newName = renamedName;
				}
			}
			else
			{
				var sameNameTables = _database.SchemaTables.Where(x => x.TableNameSchema == newName).ToList();

				if (sameNameTables.Count > 1 && sameNameTables.IndexOf(table) > 0)
				{
					var renamedName = string.Format(replacement, newName, initReplacePartStr);
					initReplacePartCount++;
					initReplacePartStr = initReplacePartCount.ToString();

					while (_database.SchemaTables.Any(x => x.TableNameSchema == renamedName))
					{
						renamedName = string.Format(replacement, newName, initReplacePartStr);
						initReplacePartCount++;
						initReplacePartStr = initReplacePartCount.ToString();
					}
					newName = renamedName;
				}
			}
			return newName;
		}

		/// <summary>
		/// </summary>
		private string NaturalizeNames_TableSchemaNameCS_Duplicate(DbTable table, bool checkViews)
		{
			string newName = table.TableNameSchemaCS;

			// suppress pattern
			string replacement = _patternProject.LanguageSettings.LanguageKeywordsSuppress;

			int initReplacePartCount = 0;
			string initReplacePartStr = "";

			if (checkViews)
			{
				var sameNameTables = _database.SchemaTables.Where(x => x.TableNameSchemaCS == newName).ToList();
				sameNameTables.AddRange(_database.SchemaViews.Where(x => x.TableNameSchemaCS == newName));

				if (sameNameTables.Count > 1 && sameNameTables.IndexOf(table) > 0)
				{
					var renamedName = string.Format(replacement, newName, initReplacePartStr);
					initReplacePartCount++;
					initReplacePartStr = initReplacePartCount.ToString();

					while (_database.SchemaTables.Any(x => x.TableNameSchemaCS == renamedName) ||
						_database.SchemaViews.Any(x => x.TableNameSchemaCS == renamedName))
					{
						renamedName = string.Format(replacement, newName, initReplacePartStr);
						initReplacePartCount++;
						initReplacePartStr = initReplacePartCount.ToString();
					}
					newName = renamedName;
				}
			}
			else
			{
				var sameNameTables = _database.SchemaTables.Where(x => x.TableNameSchemaCS == newName).ToList();

				if (sameNameTables.Count > 1 && sameNameTables.IndexOf(table) > 0)
				{
					var renamedName = string.Format(replacement, newName, initReplacePartStr);
					initReplacePartCount++;
					initReplacePartStr = initReplacePartCount.ToString();

					while (_database.SchemaTables.Any(x => x.TableNameSchemaCS == renamedName))
					{
						renamedName = string.Format(replacement, newName, initReplacePartStr);
						initReplacePartCount++;
						initReplacePartStr = initReplacePartCount.ToString();
					}
					newName = renamedName;
				}
			}
			return newName;
		}


		/// <summary>
		/// Applies project settings to tables names
		/// </summary>
		private string NaturalizeNames_TableName_Rename(string name)
		{
			string result = name;

			// remove invalids
			result = NaturalizeNames_Name_RemoveInvalidChars(result);

			// Remove prefixes
			foreach (var str in _projectDef.DbSettions.IgnoredPrefixes)
			{
				if (result.StartsWith(str, StringComparison.CurrentCultureIgnoreCase))
					result = result.Remove(0, str.Length);
			}

			// Remove suffixes
			foreach (var str in _projectDef.DbSettions.IgnoredSuffixes)
			{
				if (result.EndsWith(str, StringComparison.CurrentCultureIgnoreCase))
					result = result.Remove(result.Length - str.Length, str.Length);
			}

			// renaming options
			result = NaturalizeNames_RenamingOptions(result, _projectDef.RenamingOptions, true, false);

			// Add prefix
			if (!string.IsNullOrEmpty(_projectDef.DbSettions.PrefixForTables))
				result = _projectDef.DbSettions.PrefixForTables + result;

			// Add suffix
			if (!string.IsNullOrEmpty(_projectDef.DbSettions.SuffixForTables))
				result = _projectDef.DbSettions.SuffixForTables + result;

			return result;
		}

		/// <summary>
		/// Applies project settings to view names
		/// </summary>
		private string NaturalizeNames_ViewName_Rename(string name)
		{
			string result = name;

			// remove invalids
			result = NaturalizeNames_Name_RemoveInvalidChars(result);

			// Remove prefixes
			foreach (var str in _projectDef.DbSettions.IgnoredPrefixes)
			{
				if (result.StartsWith(str, StringComparison.CurrentCultureIgnoreCase))
					result = result.Remove(0, str.Length);
			}

			// Remove suffixes
			foreach (var str in _projectDef.DbSettions.IgnoredSuffixes)
			{
				if (result.EndsWith(str, StringComparison.CurrentCultureIgnoreCase))
					result = result.Remove(result.Length - str.Length, str.Length);
			}

			// renaming options
			result = NaturalizeNames_RenamingOptions(result, _projectDef.RenamingOptions, true, false);

			// Add prefix
			if (!string.IsNullOrEmpty(_projectDef.DbSettions.PrefixForViews))
				result = _projectDef.DbSettions.PrefixForViews + result;

			// Add suffix
			if (!string.IsNullOrEmpty(_projectDef.DbSettions.SuffixForViews))
				result = _projectDef.DbSettions.SuffixForViews + result;

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		internal static string NaturalizeNames_RenamingOptions(string name, ProjectRenaming opt, bool isTable, bool isProp)
		{
			if (string.IsNullOrWhiteSpace(name))
				return name;

			var underlineSplit = name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

			bool shoulRemoveUnderline = false;
			if (opt.RemoveUnderline.Enabled)
			{
				if ((isTable && opt.RemoveUnderline.Tables) ||
					(isProp && opt.RemoveUnderline.Properties))
				{
					shoulRemoveUnderline = true;
				}
			}

			if (opt.CaseChange.Enabled)
			{
				switch (opt.CaseChangeMode)
				{
					case ProjectRenaming.CaseChangeType.Capitalize:

						var resultCapt = new List<string>();
						if (opt.UnderlineWordDelimiter)
						{
							// deliminination

							foreach (string match in underlineSplit)
							{
								var value = match;
								if (!string.IsNullOrWhiteSpace(value))
								{
									// first letter of each part
									var fc = value[0];
									var fother = value.Substring(1, value.Length - 1);
									value = fc.ToString().ToUpper() + fother.ToLower();

									resultCapt.Add(value);
								}
							}

							// join the result according to underlines
							if (shoulRemoveUnderline)
								name = string.Concat(resultCapt.ToArray());
							else
								name = string.Join("_", resultCapt.ToArray());
						}
						else
						{
							if (shoulRemoveUnderline)
								name = string.Concat(underlineSplit);
							else
								name = string.Join("_", underlineSplit);

							// only first word is upper case
							var fc = name[0];
							var fother = name.Substring(1, name.Length - 1);
							name = fc.ToString().ToUpper() + fother.ToLower();
						}

						break;

					case ProjectRenaming.CaseChangeType.CamelCase:
						var resultCamel = new List<string>();

						foreach (string match in underlineSplit)
						{
							var value = match;
							if (!string.IsNullOrWhiteSpace(value))
							{
								// first letter of each part
								var fc = value[0];
								var fother = value.Substring(1, value.Length - 1);
								value = fc.ToString().ToUpper() + fother;

								resultCamel.Add(value);
							}
						}

						if (resultCamel.Count > 0)
						{
							// first part is lower case
							resultCamel[0] = resultCamel[0].ToLower();
						}

						// join the result according to underlines
						if (shoulRemoveUnderline)
							name = string.Concat(resultCamel.ToArray());
						else
							name = string.Join("_", resultCamel.ToArray());

						break;

					case ProjectRenaming.CaseChangeType.PascalCase:
						var resultPascal = new List<string>();
						if (opt.UnderlineWordDelimiter)
						{
							// deliminination

							foreach (string match in underlineSplit)
							{
								var value = match;
								if (!string.IsNullOrWhiteSpace(value))
								{
									// first letter of each part
									var fc = value[0];
									var fother = value.Substring(1, value.Length - 1);
									value = fc.ToString().ToUpper() + fother;

									resultPascal.Add(value);
								}
							}

							// join the result according to underlines
							if (shoulRemoveUnderline)
								name = string.Concat(resultPascal.ToArray());
							else
								name = string.Join("_", resultPascal.ToArray());
						}
						else
						{
							if (shoulRemoveUnderline)
								name = string.Concat(underlineSplit);
							else
								name = string.Join("_", underlineSplit);

							// only first word is upper case
							var fc = name[0];
							var fother = name.Substring(1, name.Length - 1);
							name = fc.ToString().ToUpper() + fother;
						}
						break;

					case ProjectRenaming.CaseChangeType.Lower:
						if (shoulRemoveUnderline)
							name = string.Concat(underlineSplit);
						else
							name = string.Join("_", underlineSplit);
						name = name.ToLower();
						break;

					case ProjectRenaming.CaseChangeType.Upper:
						if (shoulRemoveUnderline)
							name = string.Concat(underlineSplit);
						else
							name = string.Join("_", underlineSplit);
						name = name.ToUpper();
						break;
				}
			}

			return name;
		}

	}
}
