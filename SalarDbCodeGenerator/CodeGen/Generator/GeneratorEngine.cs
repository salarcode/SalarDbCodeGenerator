using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SalarDbCodeGenerator.CodeGen.DbSchema;
using SalarDbCodeGenerator.CodeGen.PatternsSchema;
using SalarDbCodeGenerator.DbProject;
using System.Collections.Generic;
using SalarDbCodeGenerator.CodeGen.SchemaEngines;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2012-2-13
// ====================================
namespace SalarDbCodeGenerator.CodeGen.Generator
{
	public class GeneratorEngine
	{
		/// <summary>
		/// Used as parameter where to apply the list
		/// </summary>
		private enum PatternFileWhereToApply { Both, Tables, Views, None }

		public GeneratorEngine(ProjectDefinaton project, PatternProject pattern, SchemaDatabase database, ExSchemaEngine schemaEngine)
		{
			_patternProject = pattern;
			_projectDef = project;
			_database = database;
			_schemaEngine = schemaEngine;
			_optionGenerateUnselectedForeigns = false;
		}

		#region local variables
		PatternProject _patternProject;
		ProjectDefinaton _projectDef;
		SchemaDatabase _database;
		ExSchemaEngine _schemaEngine;
		bool _optionGenerateUnselectedForeigns;
		#endregion

		#region public methods
		/// <summary>
		/// Generates output files
		/// </summary>
		public void Generate()
		{
			//string patternsFolder = Path.GetDirectoryName(_projectDef.CodeGenSettings.CodeGenPatternFile);
			string patternsFolder = Path.GetDirectoryName(
				Common.AppVarPathMakeAbsolute(_projectDef.CodeGenSettings.CodeGenPatternFile));

			// schema tables
			SchemaTable[] schemaTables = _database.SchemaTables.ToArray();
			SchemaTable[] schemaViews = _database.SchemaViews.ToArray();

			// all the pattern files
			_patternProject.PatternFiles.ForEach(patFile =>
			{
				if (patFile.Action == PatternsListItemAction.Copy)
					return;

				var patternFile = new PatternFile();

				// load the pattern file
				patternFile.LoadFromFile(Path.Combine(patternsFolder, patFile.Path));

				switch (patternFile.Group)
				{
					case PatternConsts.PtternGroups.ProjectFile:
						// ignored here
						// In next loop we will apply projec files
						break;

					default:

						// Check if pattern is selected by user
						if (!_projectDef.CodeGenSettings.SelectedPatterns.Contains(patternFile.Name))
						{
							//continue;
							return;
						}
						else
							switch (patternFile.AppliesTo)
							{
								case PatternFileAppliesTo.GeneralOnce:

									PatternFileAppliesTo_GeneralOnceApplier(patternFile);
									break;

								case PatternFileAppliesTo.TablesAndViews_Each:
									PatternFileAppliesTo_TablesAndViewsApplier(schemaTables, schemaViews, PatternFileWhereToApply.Both, patternFile);
									break;

								case PatternFileAppliesTo.TablesAndViews_All:
									PatternFileAppliesTo_TablesAndViewsApplier(schemaTables, schemaViews, PatternFileWhereToApply.None, patternFile);
									break;

								case PatternFileAppliesTo.Tables_Each:
								case PatternFileAppliesTo.Tables_All:

									PatternFileAppliesTo_TablesAndViewsApplier(schemaTables, schemaViews, PatternFileWhereToApply.Tables, patternFile);
									break;

								case PatternFileAppliesTo.Views_Each:
								case PatternFileAppliesTo.Views_All:

									PatternFileAppliesTo_TablesAndViewsApplier(schemaTables, schemaViews, PatternFileWhereToApply.Views, patternFile);
									break;

							}

						break;
				}
			});

			_patternProject.PatternFiles.AsParallel().ForAll(patFile =>
			                                                 	{
			                                                 		var patFilePath = Path.Combine(patternsFolder, patFile.Path);
				if (patFile.Action == PatternsListItemAction.Copy)
				{
					var copyPath = _projectDef.GenerationPath;
					var copyPathDir = Common.ProjectPathMakeAbsolute(copyPath, _projectDef.ProjectFileName);
					if(!string.IsNullOrEmpty(patFile.ActionCopyPath))
					{
						copyPath = Path.Combine(copyPathDir, patFile.ActionCopyPath);
					}

					try
					{
						Directory.CreateDirectory(copyPathDir);
						File.Copy(patFilePath, copyPath, true);
					}
					catch (Exception)
					{
						// TODO: log failed
					}
					return;
				}

				var patternFile = new PatternFile();
				patternFile.LoadFromFile(patFilePath);

				// only for project files
				if (patternFile.Group == PatternConsts.PtternGroups.ProjectFile)
				{
					//PatternFile common = new PatternFile();
					//common.LoadFromFile(Path.Combine(patternsFolder, patFile.Path));

					// Check if pattern is selected by user
					if (_projectDef.CodeGenSettings.SelectedPatterns.Contains(patternFile.Name))
					{
						PatternFileAppliesTo_ProjectFileApplier(patternFile);
					}
				}
			});
		}
		#endregion

		#region Names Naturalizers
		/// <summary>
		/// 
		/// </summary>
		internal static string NaturalizeNames_RenamingOptions(string name, ProjectRenaming opt, bool isTable, bool isProp)
		{
			if (string.IsNullOrWhiteSpace(name))
				return name;

			//var spliterRegex = new Regex(@"[A-Z0-9]*[a-z0-9]*|[_]*", RegexOptions.Compiled);
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

		/// <summary>
		/// Applies project settings to .NET data type
		/// </summary>
		private string NaturalizeNames_DotNetType(string dotNetTypeName)
		{
			return dotNetTypeName.Replace(SchemaColumn.DotNetArrayIdenticator, _patternProject.LanguageSettings.ArrayIdenticator);
		}

		/// <summary>
		/// Applies project settings to tables names
		/// </summary>
		private string NaturalizeNames_TableName(string name)
		{
			string result = name;

			// renaming options
			result = NaturalizeNames_RenamingOptions(result, _projectDef.RenamingOptions, true, false);

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
		private string NaturalizeNames_ViewName(string name)
		{
			string result = name;

			// renaming options
			result = NaturalizeNames_RenamingOptions(result, _projectDef.RenamingOptions, true, false);

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

			// Add prefix
			if (!string.IsNullOrEmpty(_projectDef.DbSettions.PrefixForViews))
				result = _projectDef.DbSettions.PrefixForViews + result;

			// Add suffix
			if (!string.IsNullOrEmpty(_projectDef.DbSettions.SuffixForViews))
				result = _projectDef.DbSettions.SuffixForViews + result;

			return result;
		}

		/// <summary>
		/// Applies project settings to tables\views names
		/// </summary>
		private string NaturalizeNames_TableViewName(SchemaTable schemaTable)
		{
			if (schemaTable.TableType == SchemaTable.TableTypeInfo.Table)
			{
				return NaturalizeNames_TableName(schemaTable.TableName);
			}
			else
				return NaturalizeNames_ViewName(schemaTable.TableName);
		}

		/// <summary>
		/// Applies project settings to fields name
		/// </summary>
		private string NaturalizeNames_FieldName(SchemaTable table, string name)
		{
			// suppress pattern
			string replacement = _patternProject.LanguageSettings.LanguageKeywordsSuppress;

			if (_patternProject.LanguageSettings.KeywordsCaseSensitive == false)
				name = name.ToLower();

			// renaming options
			name = NaturalizeNames_RenamingOptions(name, _projectDef.RenamingOptions, false, true);

			// replacement is required!
			bool replacementRequired = false;

			// remove names
			string removedBadChars = NaturalizeNames_FieldName_RemoveInvalidChars(name);

			// there can not be duplicate column name in normal
			// only search when the name is changed
			if (removedBadChars != name)
			{
				// changed!
				if (table.FindColumn(removedBadChars) != null)
					replacementRequired = true;

				name = removedBadChars;
			}


			foreach (var keyword in _patternProject.LanguageSettings.LanguageKeywords)
			{
				string keywordItem = keyword;

				if (_patternProject.LanguageSettings.KeywordsCaseSensitive == false)
					keywordItem = keywordItem.ToLower();

				if (name == keywordItem || replacementRequired)
				{
					// reset state
					replacementRequired = false;

					// the field name is a keyword!
					string newName = string.Format(replacement, name, "");
					int count = 0;
					do
					{
						if (count > 0)
						{
							newName = string.Format(replacement, name, count);
						}

						count++;
					}
					while (table.FindColumn(newName) != null);

					return newName;
				}
			}

			// field name is ok to be used
			return name;
		}

		/// <summary>
		/// Applies project settings to fields name
		/// </summary>
		private string NaturalizeNames_ForeignTableName(SchemaTable localTable, string localRenameTableName, string name, bool renamed, bool isInLocalTable)
		{
			// suppress pattern
			string replacement = _patternProject.LanguageSettings.LanguageKeywordsSuppress;

			// replacement is required!
			bool replacementRequired = false;
			int allowed = 0;
			//allowed = renamed ? 1 : 0;

			// member names can not be same as their enclosing type
			if (localTable.TableName == name || localRenameTableName == name)
			{
				replacementRequired = true;
			}

			// changed!
			if (!replacementRequired && localTable.ForeignKeys.Count(x => x.ForeignTableNameAsField == name) > allowed)
				replacementRequired = true;

			// if requred
			if (replacementRequired)
			{
				// the field name is a keyword!
				string newName = string.Format(replacement, name, "");
				int count = 0;
				do
				{
					if (count > 0)
					{
						newName = string.Format(replacement, name, count);
					}

					count++;
				} while (localTable.ForeignKeys.Any(x => x.ForeignTableNameAsField == newName));

				return newName;
			}
			return name;
		}

		private string NaturalizeNames_FieldName_RemoveInvalidChars(string name)
		{
			if (string.IsNullOrEmpty(name))
				return name;
			string newName = name.Replace(' ', '_').Replace('.', '_');
			if (char.IsNumber(newName[0]))
			{
				// suppress pattern
				newName = string.Format(_patternProject.LanguageSettings.LanguageKeywordsSuppress
					, ""
					, newName);
			}
			return newName;
		}

		#endregion

		#region Replacers
		string Replacer_PatternFileName(string fileName, string tableDotNetName, string tableNativeName, string tableDotNetNameCS)
		{
			fileName = Common.ReplaceEx(fileName, ReplaceConsts.ProjectName, _projectDef.ProjectName, StringComparison.CurrentCultureIgnoreCase);
			fileName = Common.ReplaceEx(fileName, ReplaceConsts.Namespace, _projectDef.CodeGenSettings.DefaultNamespace, StringComparison.CurrentCultureIgnoreCase);
			fileName = ReplaceExIgnoreCase(fileName, ReplaceConsts.DatabaseName, _projectDef.DbSettions.DatabaseName);

			if (tableDotNetName != null)
				fileName = Common.ReplaceEx(fileName, ReplaceConsts.TableName, tableDotNetName, StringComparison.CurrentCultureIgnoreCase);
			if (tableNativeName != null)
				fileName = Common.ReplaceEx(fileName, ReplaceConsts.TableNativeName, tableNativeName, StringComparison.CurrentCultureIgnoreCase);
			if (tableDotNetNameCS != null)
				fileName = Common.ReplaceEx(fileName, ReplaceConsts.TableNameCaseSensitive, tableDotNetNameCS, StringComparison.CurrentCultureIgnoreCase);
			return fileName;
		}

		string Replacer_PatternBaseContent(string baseContent, string tableDotNetName, string tableNativeName)
		{
			// Replacements
			if (tableDotNetName != null)
				baseContent = Common.ReplaceEx(baseContent, ReplaceConsts.TableName, tableDotNetName, StringComparison.CurrentCultureIgnoreCase);
			if (tableNativeName != null)
				baseContent = Common.ReplaceEx(baseContent, ReplaceConsts.TableNativeName, tableNativeName, StringComparison.CurrentCultureIgnoreCase);

			// general
			baseContent = Replacer_GeneratorGeneral(baseContent);

			// database provider
			baseContent = Replacer_DatabaseProvider(baseContent);

			return baseContent;
		}

		string Replacer_GeneratorGeneral(string content)
		{
			content = Common.ReplaceEx(content, ReplaceConsts.Namespace, _projectDef.CodeGenSettings.DefaultNamespace, StringComparison.CurrentCultureIgnoreCase);
			content = Common.ReplaceEx(content, ReplaceConsts.ProjectName, _projectDef.ProjectName, StringComparison.CurrentCultureIgnoreCase);
			content = ReplaceExIgnoreCase(content, ReplaceConsts.DatabaseName, _projectDef.DbSettions.DatabaseName);
			content = ReplaceExIgnoreCase(content, ReplaceConsts.Generator, AppConfig.AppGeneratorSign);
			content = ReplaceExIgnoreCase(content, ReplaceConsts.OperateDate, DateTime.Now.ToString());
			return content;
		}

		/// <summary>
		/// Replaces the database provider class name
		/// </summary>
		string Replacer_DatabaseProvider(string content)
		{
			content = ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderSpParamPrefix,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.StoredProcParamPrefix));
			content = ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderClassCommand,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.ClassCommand));
			content = ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderClassConnection,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.ClassConnection));
			content = ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderClassDataAdapter,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.ClassDataAdapter));
			content = ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderClassDataReader,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.ClassDataReader));
			content = ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderClassParameter,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.ClassParameter));
			content = ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderClassPrefix,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.ClassPrefix));
			content = ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderClassTransaction,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.ClassTransaction));
			content = ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderClassReferenceName,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.ClassNamespace));
			content = ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderAssemblyReference,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.AssemblyReference));

			return content;
		}

		/// <summary>
		/// Applies table data to pattern content replacement
		/// </summary>
		string Replacer_PatternContent_AppliesToTable(string content, SchemaTable table)
		{
			if (string.IsNullOrWhiteSpace(content))
			{
				return content;
			}

			// naturalize names
			string dotNetTableName = NaturalizeNames_TableViewName(table);

			content = ReplaceExIgnoreCase(content, ReplaceConsts.TableName, dotNetTableName);
			content = ReplaceExIgnoreCase(content, ReplaceConsts.TableNativeName, table.TableName);

			// general
			content = Replacer_GeneratorGeneral(content);

			// database provider class
			content = Replacer_DatabaseProvider(content);

			SchemaColumn autoKey = table.GetFirstAutoIncrementField();
			SchemaColumn primaryKey = table.GetPrimaryKey();

			if (autoKey == null)
				content = ReplaceExIgnoreCase(content, ReplaceConsts.AutoIncrementDotNetType, _patternProject.LanguageSettings.VoidDataType);
			else
				content = ReplaceExIgnoreCase(content, ReplaceConsts.AutoIncrementDotNetType, NaturalizeNames_DotNetType(autoKey.DotNetTypeClean));

			if (primaryKey == null)
			{
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDbType, "");
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDbTypeSize, "");
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDotNetType, _patternProject.LanguageSettings.VoidDataType);
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyName, "");
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyNativeName, "");
			}
			else
			{
				string dotNetFieldName = NaturalizeNames_FieldName(table, primaryKey.FieldName);

				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDbType, primaryKey.DbType);
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDbTypeSize, FieldType_ColumnDataTypeSize(primaryKey));
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDotNetType, NaturalizeNames_DotNetType(primaryKey.DotNetTypeClean));
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyName, dotNetFieldName);
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyNativeName, primaryKey.FieldName);
			}
			return content;
		}

		/// <summary>
		/// Applies table foreign keys to pattern content replacement
		/// </summary>
		string Replacer_PatternContent_AppliesToForeignKeys(string content, SchemaTable table)
		{
			// this section should be ignored if there is no foreign keys!
			if (table.ForeignKeys.Count == 0)
			{
				// No foreign keys! go away
				return "";
			}

			// naturalize names
			string dotNetLocalTableName = NaturalizeNames_TableViewName(table);

			// local table
			content = ReplaceExIgnoreCase(content, ReplaceConsts.LocalTableName, dotNetLocalTableName);
			content = ReplaceExIgnoreCase(content, ReplaceConsts.LocalTableNativeName, table.TableName);

			// the result
			string result = "";

			// fetch foreign keys
			foreach (var foreignKey in table.ForeignKeys)
			{
				// the content copy for each foreign key
				string foreignContent = content;

				// naturalize names
				foreignContent = Replacer_PatternContent_AppliesToForeignKey(foreignContent, table, foreignKey);


				if (!string.IsNullOrEmpty(foreignContent))
					// add it to the result
					result += ReplaceConsts.NewLine + foreignContent;
			}

			// the result
			return result;
		}

		/// <summary>
		/// Applies foreign key column data to pattern content replacement
		/// </summary>
		string Replacer_PatternContent_AppliesToForeignKey(string content, SchemaTable table, SchemaForeignKey foreignKey)
		{

			// NOTE: foreign keys are always for TABLEs
			// Checking if user has selected this table
			// Also checking the option!
			if (!_optionGenerateUnselectedForeigns && !UserHasSelectedTable(foreignKey.ForeignTable.TableName))
			{
				// User has not selected this foreign table
				return string.Empty;
			}

			// general
			content = Replacer_GeneratorGeneral(content);

			// database provider class
			content = Replacer_DatabaseProvider(content);

			// naturalize names
			string dotNetLocalTableName = NaturalizeNames_TableViewName(table);

			// local table
			content = ReplaceExIgnoreCase(content, ReplaceConsts.LocalTableName, dotNetLocalTableName);
			content = ReplaceExIgnoreCase(content, ReplaceConsts.LocalTableNativeName, table.TableName);

			// foreign table name
			string dotNetForeignTableName = NaturalizeNames_TableViewName(foreignKey.ForeignTable);

			// is table name renamed
			bool renamed = dotNetForeignTableName != foreignKey.ForeignTable.TableName;

			string dotNetForeignTableNameAsField;
			if (String.IsNullOrEmpty(foreignKey.ForeignTableNameAsField))
			{
				dotNetForeignTableNameAsField = NaturalizeNames_ForeignTableName(table, dotNetLocalTableName, dotNetForeignTableName, renamed, false);
				foreignKey.ForeignTableNameAsField = dotNetForeignTableNameAsField;
			}
			dotNetForeignTableNameAsField = foreignKey.ForeignTableNameAsField;
			foreignKey.ForeignTableName = dotNetForeignTableName;

			// foreign table
			content = ReplaceExIgnoreCase(content, ReplaceConsts.ForeignTableName, dotNetForeignTableName);
			content = ReplaceExIgnoreCase(content, ReplaceConsts.ForeignTableNameAsField, dotNetForeignTableNameAsField);
			content = ReplaceExIgnoreCase(content, ReplaceConsts.ForeignTableNativeName, foreignKey.ForeignTable.TableName);

			// naturalize field names
			string dotNetLocalFieldName = NaturalizeNames_FieldName(table, foreignKey.LocalColumnName);
			string dotNetForeignFieldName = NaturalizeNames_FieldName(table, foreignKey.ForeignColumnName);

			// ===================================
			// local field
			content = ReplaceExIgnoreCase(content, ReplaceConsts.LocalFieldDotNetType, NaturalizeNames_DotNetType(foreignKey.LocalColumn.DotNetTypeClean));
			content = ReplaceExIgnoreCase(content, ReplaceConsts.LocalFieldName, dotNetLocalFieldName);
			content = ReplaceExIgnoreCase(content, ReplaceConsts.LocalFieldNativeName, foreignKey.LocalColumn.FieldName);
			// no oridianl, foreignContent = ReplaceExIgnoreCase(foreignContent, ReplaceConsts.LocalFieldOrdinalValue, foreignKey.LocalColumn.ColumnOrdinal.ToString());
			// column description
			content = ReplaceExIgnoreCase(content, ReplaceConsts.LocalFieldDescription, foreignKey.LocalColumn.UserDescription);
			// Database field type
			content = ReplaceExIgnoreCase(content, ReplaceConsts.LocalFieldDbTypeSize, FieldType_ColumnDataTypeSize(foreignKey.LocalColumn));


			// ===================================
			// Foreign field
			content = ReplaceExIgnoreCase(content, ReplaceConsts.ForeignFieldDotNetType, NaturalizeNames_DotNetType(foreignKey.ForeignColumn.DotNetTypeClean));
			content = ReplaceExIgnoreCase(content, ReplaceConsts.ForeignFieldName, dotNetForeignFieldName);
			content = ReplaceExIgnoreCase(content, ReplaceConsts.ForeignFieldNativeName, foreignKey.ForeignColumn.FieldName);
			// no oridianl, foreignContent = ReplaceExIgnoreCase(foreignContent, ReplaceConsts.ForeignFieldOrdinalValue, foreignKey.ForeignColumn.ColumnOrdinal.ToString());
			// column description
			content = ReplaceExIgnoreCase(content, ReplaceConsts.ForeignFieldDescription, foreignKey.ForeignColumn.UserDescription);
			// Database field type
			content = ReplaceExIgnoreCase(content, ReplaceConsts.ForeignFieldDbTypeSize, FieldType_ColumnDataTypeSize(foreignKey.ForeignColumn));
			return content;
		}

		/// <summary>
		/// Applies table index constraintsto pattern content replacement
		/// </summary>
		string Replacer_PatternContent_AppliesToIndexConstraints(string content, SchemaTable table, bool uniqueKeys)
		{
			// check if there is no constraints
			if (table.ConstraintKeys.Count == 0)
			{
				return "";
			}

			// naturalize names
			string dotNetTableName = NaturalizeNames_TableViewName(table);

			content = Replacer_PatternBaseContent(content, dotNetTableName, table.TableName);

			// the result
			string result = "";

			// fetch constraints keys
			foreach (var indexKey in table.ConstraintKeys)
			{
				if (!indexKey.IsUnique && uniqueKeys)
				{
					// the key is not unique and it is requested, then go away
					continue;
				}
				if (indexKey.IsUnique && !uniqueKeys)
				{
					// the key is unique and it is not requested, then go away
					continue;
				}



				// the content copy for each foreign key
				string indexContent = content;

				// the column general replacer
				indexContent = Replacer_PatternContent_AppliesToColumn(indexContent, table, indexKey.KeyColumn);

				// the index specified replacer

				// naturalize field names
				string dotNetFieldName = NaturalizeNames_FieldName(table, indexKey.KeyColumnName);

				// ===================================
				indexContent = ReplaceExIgnoreCase(indexContent, ReplaceConsts.IndexKeyDbType, indexKey.KeyColumn.DbType);
				indexContent = ReplaceExIgnoreCase(indexContent, ReplaceConsts.IndexKeyDbTypeSize, FieldType_ColumnDataTypeSize(indexKey.KeyColumn));
				indexContent = ReplaceExIgnoreCase(indexContent, ReplaceConsts.IndexKeyDotNetType, NaturalizeNames_DotNetType(indexKey.KeyColumn.DotNetTypeClean));
				indexContent = ReplaceExIgnoreCase(indexContent, ReplaceConsts.IndexKeyName, dotNetFieldName);
				indexContent = ReplaceExIgnoreCase(indexContent, ReplaceConsts.IndexKeyNativeName, indexKey.KeyColumn.FieldName);
				indexContent = ReplaceExIgnoreCase(indexContent, ReplaceConsts.IndexName, indexKey.KeyName);

				// add it to the result
				result += ReplaceConsts.NewLine + indexContent;
			}

			return result;
		}

		/// <summary>
		/// Applies column data to pattern content replacement
		/// </summary>
		string Replacer_PatternContent_AppliesToColumn(string content, SchemaTable table, SchemaColumn column)
		{
			// naturalize names
			string dotNetTableName = NaturalizeNames_TableViewName(table);
			string dotNetFieldName = NaturalizeNames_FieldName(table, column.FieldName);

			// table of the column
			content = ReplaceExIgnoreCase(content, ReplaceConsts.TableName, dotNetTableName);
			content = ReplaceExIgnoreCase(content, ReplaceConsts.TableNativeName, table.TableName);

			// general
			content = Replacer_GeneratorGeneral(content);

			// database provider class
			content = Replacer_DatabaseProvider(content);

			// referenced table of the column
			if (column.IsReferenceKey && column.IsReferenceKeyTable != null)
			{
				var refTable = column.IsReferenceKeyTable;
				string refDotNetTableName = NaturalizeNames_TableViewName(refTable);

				content = ReplaceExIgnoreCase(content, ReplaceConsts.TableNameRefField, refDotNetTableName);
				content = ReplaceExIgnoreCase(content, ReplaceConsts.TableNativeNameRefField, refTable.TableName);
			}

			// column information
			content = ReplaceExIgnoreCase(content, ReplaceConsts.FieldDotNetType, NaturalizeNames_DotNetType(column.DotNetTypeClean));
			content = ReplaceExIgnoreCase(content, ReplaceConsts.FieldName, dotNetFieldName);
			content = ReplaceExIgnoreCase(content, ReplaceConsts.FieldNativeName, column.FieldName);

			content = ReplaceExIgnoreCase(content, ReplaceConsts.FieldOrdinalValue, column.ColumnOrdinal.ToString());
			content = ReplaceExIgnoreCase(content, ReplaceConsts.FieldIsPrimaryKey, column.PrimaryKey.ToString().ToLower());
			content = ReplaceExIgnoreCase(content, ReplaceConsts.FieldCanBeNull, column.Nullable.ToString().ToLower());

			// column description
			content = ReplaceExIgnoreCase(content, ReplaceConsts.FieldDescription, column.UserDescription);

			// Database field type
			content = ReplaceExIgnoreCase(content, ReplaceConsts.FieldDbType, column.DbType);
			content = ReplaceExIgnoreCase(content, ReplaceConsts.FieldDbTypeSize, FieldType_ColumnDataTypeSize(column));
			content = ReplaceExIgnoreCase(content, ReplaceConsts.FieldDbSize, FieldType_ColumnDataSize(column));



			if (column.AutoIncrement)
				content = ReplaceExIgnoreCase(content, ReplaceConsts.AutoIncrementDotNetType, NaturalizeNames_DotNetType(column.DotNetTypeClean));
			else
				content = ReplaceExIgnoreCase(content, ReplaceConsts.AutoIncrementDotNetType, _patternProject.LanguageSettings.VoidDataType);

			if (column.PrimaryKey)
			{

				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDbType, column.DbType);
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDbTypeSize, FieldType_ColumnDataTypeSize(column));
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDotNetType, NaturalizeNames_DotNetType(column.DotNetTypeClean));
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyName, dotNetFieldName);
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyNativeName, column.FieldName);
			}
			else
			{
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDbType, "");
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDbTypeSize, "");
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDotNetType, _patternProject.LanguageSettings.VoidDataType);
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyName, "");
				content = ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyNativeName, "");
			}

			return content;
		}


		string PatternContentAppliesTo_TablesAndViews(string baseContent,
			List<PatternContent> patternContent,
			SchemaTable[] tablesList,
			SchemaTable[] viewsList,
			PatternFile patternFile)
		{
			string appliedContent = "";

			// ---------------------------------
			// Here, all tables/views will apply

			foreach (var pattern in patternContent)
			{
				string replacementName = string.Format(ReplaceConsts.PatternContentReplacer, pattern.Name);
				int index = 0;

				// is there a pattern for that
				if (baseContent.IndexOf(replacementName) == -1)
					continue;

				switch (pattern.AppliesTo)
				{
					case PatternContentAppliesTo.General:
						// general part
						appliedContent = PatternContentAppliesTo_ProjectFileGeneral(pattern);

						// replace the content
						baseContent = baseContent.Replace(replacementName, appliedContent);

						break;

					case PatternContentAppliesTo.TablesAndViews_All:
						appliedContent = "";
						index = 0;

						// applying all the views
						for (int i = 0; i < tablesList.Length; i++)
						{
							var tbl = tablesList[i];
							if (!UserHasSelectedTable(tbl.TableName))
								continue;

							if (index > 0)
								appliedContent += pattern.ItemsSeperator;
							appliedContent += PatternContent_AppliesToTable(pattern, tbl);

							// the seperator identicator
							index++;

							// internal pattern contents
							if (pattern.PatternContents.Count > 0)
							{
								// nested call
								appliedContent = PatternContentAppliesTo_OneTable(appliedContent, pattern.PatternContents, tablesList, viewsList,
																				  tbl, patternFile);
							}
						}

						// seperator between tables and views!
						// we have views too
						if (viewsList.Length > 0 && _projectDef.DbSettions.HasSelectedView())
							appliedContent += pattern.ItemsSeperator;

						index = 0;
						// applying all the views
						for (int i = 0; i < viewsList.Length; i++)
						{
							var view = viewsList[i];

							// check if selected by user
							if (!UserHasSelectedView(view.TableName))
								continue;

							if (index > 0)
								appliedContent += pattern.ItemsSeperator;
							appliedContent += PatternContent_AppliesToTable(pattern, view);

							// the seperator identicator
							index++;

							// internal pattern contents
							if (pattern.PatternContents.Count > 0)
							{
								// nested call
								appliedContent = PatternContentAppliesTo_OneTable(appliedContent, pattern.PatternContents, tablesList, viewsList,
																				  view, patternFile);
							}
						}

						// base content
						if (!string.IsNullOrEmpty(pattern.BaseContent))
						{
							appliedContent = pattern.BaseContent.Replace(ReplaceConsts.PatternContentInnerContents, appliedContent);
						}

						// replace the content
						baseContent = baseContent.Replace(replacementName, appliedContent);
						break;

					case PatternContentAppliesTo.Tables_All:
						appliedContent = "";
						index = 0;

						// applying all the views
						for (int i = 0; i < tablesList.Length; i++)
						{
							var tbl = tablesList[i];
							if (!UserHasSelectedTable(tbl.TableName))
								continue;

							if (index > 0)
								appliedContent += pattern.ItemsSeperator;
							appliedContent += PatternContent_AppliesToTable(pattern, tbl);

							// the seperator identicator
							index++;

							// internal pattern contents
							if (pattern.PatternContents.Count > 0)
							{
								// nested call
								appliedContent = PatternContentAppliesTo_OneTable(appliedContent, pattern.PatternContents, tablesList, viewsList,
																				  tbl, patternFile);
							}
						}

						// base content
						if (!string.IsNullOrEmpty(pattern.BaseContent))
						{
							appliedContent = pattern.BaseContent.Replace(ReplaceConsts.PatternContentInnerContents, appliedContent);
						}

						// replace the content
						baseContent = baseContent.Replace(replacementName, appliedContent);
						break;

					case PatternContentAppliesTo.Views_All:
						index = 0;
						appliedContent = "";

						// applying all the views
						for (int i = 0; i < viewsList.Length; i++)
						{
							var view = viewsList[i];

							// check if selected by user
							if (!UserHasSelectedView(view.TableName))
								continue;

							if (index > 0)
								appliedContent += pattern.ItemsSeperator;
							appliedContent += PatternContent_AppliesToTable(pattern, view);

							// the seperator identicator
							index++;

							// internal pattern contents
							if (pattern.PatternContents.Count > 0)
							{
								// nested call
								appliedContent = PatternContentAppliesTo_OneTable(appliedContent, pattern.PatternContents, tablesList, viewsList,
																				  view, patternFile);
							}
						}

						// base content
						if (!string.IsNullOrEmpty(pattern.BaseContent))
						{
							appliedContent = pattern.BaseContent.Replace(ReplaceConsts.PatternContentInnerContents, appliedContent);
						}

						// replace the content
						baseContent = baseContent.Replace(replacementName, appliedContent);
						break;

				}
			}



			return baseContent;
		}

		string PatternContentAppliesTo_OneTable(string baseContent,
			List<PatternContent> patternContent,
			SchemaTable[] tablesList,
			SchemaTable[] viewsList,
			SchemaTable table,
			PatternFile patternFile)
		{
			string appliedContent = "";

			// ---------------------------------
			// Only one table is applying here!

			// table can not be null here
			if (table == null)
			{
				return baseContent;
			}

			foreach (var pattern in patternContent)
			{
				string replacementName = string.Format(ReplaceConsts.PatternContentReplacer, pattern.Name);

				// is there a pattern for that
				if (baseContent.IndexOf(replacementName) == -1)
					continue;

				switch (pattern.AppliesTo)
				{
					case PatternContentAppliesTo.General:
						// nothing!
						break;

					case PatternContentAppliesTo.Table:
						appliedContent = PatternContent_AppliesToTable(pattern, table);

						// base content
						if (!string.IsNullOrEmpty(pattern.BaseContent))
						{
							appliedContent = pattern.BaseContent.Replace(ReplaceConsts.PatternContentInnerContents, appliedContent);
						}

						// internal pattern contents
						if (pattern.PatternContents.Count > 0)
						{
							// nested call
							appliedContent = PatternContentAppliesTo_OneTable(appliedContent, pattern.PatternContents, tablesList, viewsList, table, patternFile);
						}

						// replace the content
						baseContent = baseContent.Replace(replacementName, appliedContent);
						break;

					case PatternContentAppliesTo.Columns:
						appliedContent = "";

						// fetch the columns and apply the replacement operation
						foreach (var tableColumn in table.SchemaColumns)
						{
							// Apply the replacement to the pattern content
							string columnReplace = PatternContent_AppliesToColumn(pattern, table, tableColumn);

							// The seperator
							if (!string.IsNullOrEmpty(columnReplace))
								appliedContent += columnReplace + pattern.ItemsSeperator;
						}


						// Remove additional ItemsSeperator
						if (!string.IsNullOrEmpty(pattern.ItemsSeperator)
							&& appliedContent.EndsWith(pattern.ItemsSeperator))
							appliedContent = appliedContent.Remove(appliedContent.Length - pattern.ItemsSeperator.Length, pattern.ItemsSeperator.Length);

						// internal pattern contents
						if (pattern.PatternContents.Count > 0)
						{
							// nested call
							appliedContent = PatternContentAppliesTo_OneTable(appliedContent, pattern.PatternContents, tablesList, viewsList, table, patternFile);
						}

						// replace in the main content
						baseContent = Common.ReplaceEx(baseContent, replacementName, appliedContent, StringComparison.CurrentCulture);
						break;

					case PatternContentAppliesTo.ForeignKeys:
						appliedContent = "";

						// fetch the columns and apply the replacement operation
						foreach (var foreignKey in table.ForeignKeys)
						{
							// Apply the replacement to the pattern content
							string columnReplace = PatternContent_AppliesToForeignKeys(pattern, table, foreignKey);

							// The seperator
							if (!string.IsNullOrEmpty(columnReplace))
								appliedContent += columnReplace + pattern.ItemsSeperator;
						}


						// Remove additional ItemsSeperator
						if (!string.IsNullOrEmpty(pattern.ItemsSeperator)
							&& appliedContent.EndsWith(pattern.ItemsSeperator))
							appliedContent = appliedContent.Remove(appliedContent.Length - pattern.ItemsSeperator.Length, pattern.ItemsSeperator.Length);

						// internal pattern contents
						if (pattern.PatternContents.Count > 0)
						{
							// nested call
							appliedContent = PatternContentAppliesTo_OneTable(appliedContent, pattern.PatternContents, tablesList, viewsList, table, patternFile);
						}

						// replace in the main content
						baseContent = Common.ReplaceEx(baseContent, replacementName, appliedContent, StringComparison.CurrentCulture);
						break;

					default:
						break;
				}
			}

			return baseContent;
		}


		/// <summary>
		/// Partial content replacer.
		/// </summary>
		string PatternContentAppliesTo_ProjectFileGeneral(PatternContent partialContent)
		{
			switch (partialContent.ContentKeyMode)
			{
				case PatternContentKeyMode.DatabaseProvider:

					PatternReplacement dbReplacer = null;

					switch (this._database.Provider)
					{
						case DatabaseProvider.Oracle:
							dbReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.DatabaseProvider_Oracle);
							break;

						case DatabaseProvider.SQLServer:
							dbReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.DatabaseProvider_SQLServer);
							break;

						case DatabaseProvider.SQLite:
							dbReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.DatabaseProvider_SQLite);
							break;

						case DatabaseProvider.SqlCe4:
							dbReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.DatabaseProvider_SqlCe4);
							break;
					}

					if (dbReplacer != null)
						// Replace the contents
						return PatternContent_ProjectFile_AppliesToGeneral(dbReplacer.Content);

					return "";

				case PatternContentKeyMode.TablePrimaryKey:
				case PatternContentKeyMode.TableForeignKey:
				case PatternContentKeyMode.FieldPrimaryKey:
				case PatternContentKeyMode.FieldForeignKey:
				case PatternContentKeyMode.FieldKeyType:
				case PatternContentKeyMode.FieldKeyReadType:
				case PatternContentKeyMode.TableAutoIncrement:
				case PatternContentKeyMode.OneReplacement:
					PatternReplacement oneReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.OneReplacement_TheReplacement);

					if (partialContent.Name == PatternConsts.PartialContents.ProjectAppConfig)
					{
						if (_projectDef.CodeGenSettings.SelectedPatterns.IndexOf(PatternConsts.PartialContents.PatternFileName_AppConfig) != -1)
						{
							// Replace the contents
							return PatternContent_ProjectFile_AppliesToGeneral(oneReplacer.Content);
						}
						else
							return "";
					}

					// Replace the contents
					return PatternContent_ProjectFile_AppliesToGeneral(oneReplacer.Content);

				default:
					return "";

			}
		}

		/// <summary>
		/// Partial content replacer.
		/// </summary>
		string PatternContentAppliesTo_ProjectFiles(PatternContent partialContent, string fileNamePath)
		{
			switch (partialContent.ContentKeyMode)
			{
				case PatternContentKeyMode.TablePrimaryKey:
				case PatternContentKeyMode.TableForeignKey:
				case PatternContentKeyMode.FieldPrimaryKey:
				case PatternContentKeyMode.FieldForeignKey:
				case PatternContentKeyMode.FieldKeyType:
				case PatternContentKeyMode.FieldKeyReadType:
				case PatternContentKeyMode.TableAutoIncrement:
				case PatternContentKeyMode.OneReplacement:
					PatternReplacement oneReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.OneReplacement_TheReplacement);

					if (partialContent.Name == PatternConsts.PartialContents.ProjectAppConfig)
					{
						if (_projectDef.CodeGenSettings.SelectedPatterns.IndexOf(PatternConsts.PartialContents.PatternFileName_AppConfig) != -1)
						{
							// Replace the contents
							return PatternContent_ProjectFile_AppliesToFiles(oneReplacer.Content, fileNamePath);
						}
						else
							return "";
					}

					// Replace the contents
					return PatternContent_ProjectFile_AppliesToFiles(oneReplacer.Content, fileNamePath);

				default:
					return "";

			}
		}


		/// <summary>
		/// Partial content replacer.
		/// </summary>
		string PatternContent_AppliesToTable(PatternContent partialContent, SchemaTable table)
		{
			// Find suitable replacement type
			switch (partialContent.ContentKeyMode)
			{
				case PatternContentKeyMode.OneReplacement:

					PatternReplacement oneReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.OneReplacement_TheReplacement);

					// Replace the contents
					return Replacer_PatternContent_AppliesToTable(oneReplacer.Content, table);

				case PatternContentKeyMode.DatabaseProvider:

					PatternReplacement dbReplacer = null;

					switch (this._database.Provider)
					{
						case DatabaseProvider.Oracle:
							dbReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.DatabaseProvider_Oracle);
							break;

						case DatabaseProvider.SQLServer:
							dbReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.DatabaseProvider_SQLServer);
							break;

						case DatabaseProvider.SQLite:
							dbReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.DatabaseProvider_SQLite);
							break;

						case DatabaseProvider.SqlCe4:
							dbReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.DatabaseProvider_SqlCe4);
							break;
					}

					if (dbReplacer == null)
						return "";

					// Replace the contents
					return Replacer_PatternContent_AppliesToTable(dbReplacer.Content, table);

				case PatternContentKeyMode.TableForeignKey:

					PatternReplacement normalKeyReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.TableForeignKey_NormalKey);

					// Replace the contents
					return Replacer_PatternContent_AppliesToForeignKeys(normalKeyReplacer.Content, table);

				case PatternContentKeyMode.TableIndexConstraint:

					PatternReplacement indexConstraintReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.TableIndexConstraint_NormalKey);

					// Replace the contents
					return Replacer_PatternContent_AppliesToIndexConstraints(indexConstraintReplacer.Content, table, false);

				case PatternContentKeyMode.TableUniqueConstraint:

					PatternReplacement uniqueConstraintReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.TableUniqueConstraint_NormalKey);

					// Replace the contents
					return Replacer_PatternContent_AppliesToIndexConstraints(uniqueConstraintReplacer.Content, table, true);

				case PatternContentKeyMode.TablePrimaryKey:

					PatternReplacement primaryReplacer;

					if (table.ReadOnly)
					{
						// Table is marked as read only, like views
						primaryReplacer = partialContent.GetReplacement(
							PatternConsts.ReplacementType.TablePrimaryKey_ReadOnlyTable);
					}
					else if (table.HasPrimaryKey())
					{
						// Table has a primary key
						primaryReplacer = partialContent.GetReplacement(
							PatternConsts.ReplacementType.TablePrimaryKey_WithPrimaryKey);
					}
					else
					{
						// There is no primary key, default
						primaryReplacer = partialContent.GetReplacement(
							PatternConsts.ReplacementType.TablePrimaryKey_NoPrimaryKey);
					}

					// Replace the contents
					return Replacer_PatternContent_AppliesToTable(primaryReplacer.Content, table);

				case PatternContentKeyMode.TableAutoIncrement:

					int autoCount = table.GetAutoIncrementCount();
					PatternReplacement autoReplacer;

					if (autoCount == 0)
					{
						autoReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.TableAutoIncrement_NoAutoIncrement);
					}
					else if (autoCount == 1)
					{
						autoReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.TableAutoIncrement_OneAutoIncrement);
					}
					else
					{
						autoReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.TableAutoIncrement_MoreAutoIncrement);
					}

					// Replace the contents
					return Replacer_PatternContent_AppliesToTable(autoReplacer.Content, table);

				case PatternContentKeyMode.FieldPrimaryKey:
				case PatternContentKeyMode.FieldForeignKey:
				case PatternContentKeyMode.FieldKeyType:
				case PatternContentKeyMode.FieldKeyReadType:
				default:
					// Ignored
					return "";
			}
		}

		/// <summary>
		/// Partial content replacer.
		/// </summary>
		string PatternContent_AppliesToColumn(PatternContent partialContent, SchemaTable table, SchemaColumn column)
		{
			switch (partialContent.ContentKeyMode)
			{
				case PatternContentKeyMode.FieldPrimaryKey:
					PatternReplacement primaryReplacer;

					if (column.PrimaryKey)
						primaryReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.FieldPrimaryKey_PrimaryKey);
					else
						primaryReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.FieldPrimaryKey_NormalField);

					// Replace the contents
					return Replacer_PatternContent_AppliesToColumn(primaryReplacer.Content, table, column);


				case PatternContentKeyMode.OneReplacement:
					PatternReplacement oneReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.OneReplacement_TheReplacement);

					// Replace the contents
					return Replacer_PatternContent_AppliesToColumn(oneReplacer.Content, table, column);


				case PatternContentKeyMode.DatabaseProvider:

					PatternReplacement dbReplacer = null;

					switch (this._database.Provider)
					{
						case DatabaseProvider.Oracle:
							dbReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.DatabaseProvider_Oracle);
							break;

						case DatabaseProvider.SQLServer:
							dbReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.DatabaseProvider_SQLServer);
							break;

						case DatabaseProvider.SQLite:
							dbReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.DatabaseProvider_SQLite);
							break;

						case DatabaseProvider.SqlCe4:
							dbReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.DatabaseProvider_SqlCe4);
							break;
					}

					if (dbReplacer == null)
						return "";

					// Replace the contents
					return Replacer_PatternContent_AppliesToColumn(dbReplacer.Content, table, column);

				case PatternContentKeyMode.FieldReferencedKeyType:
				case PatternContentKeyMode.FieldKeyType:
					PatternReplacement keyTypeReplacer;

					// this key is not reference type
					if (_patternProject.SeperateRefOtherColumns)
					{
						if (column.IsReferenceKey && partialContent.ContentKeyMode == PatternContentKeyMode.FieldKeyType)
							return "";
						if (!column.IsReferenceKey && partialContent.ContentKeyMode == PatternContentKeyMode.FieldReferencedKeyType)
							return "";
					}

					// Key type
					bool dotNetNullable = !FieldType_IsDataTypeNativeNullable(column);

					if (column.AutoIncrement && column.PrimaryKey)
					{
						keyTypeReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.FieldKeyType_AutoInrcementPrimaryKey);
					}
					else if (column.AutoIncrement && column.Nullable && !dotNetNullable)
					{
						// AutoIncrement
						// Nullable column
						// Nullable object
						keyTypeReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.FieldKeyType_AutoIncNativeNullable);
					}
					else if (column.AutoIncrement && column.Nullable && dotNetNullable)
					{
						keyTypeReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.FieldKeyType_AutoIncNullableType);
					}
					else if (column.AutoIncrement)
					{
						keyTypeReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.FieldKeyType_AutoInrcement);
					}
					else if (column.PrimaryKey)
					{
						keyTypeReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.FieldKeyType_PrimaryKey);
					}
					else if (column.Nullable && !dotNetNullable)
					{
						keyTypeReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.FieldKeyType_NativeNullable);
					}
					else if (column.Nullable && dotNetNullable)
					{
						keyTypeReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.FieldKeyType_NullableType);
					}
					else
					{
						keyTypeReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.FieldKeyType_NormalField);
					}

					// Replace the contents
					return Replacer_PatternContent_AppliesToColumn(keyTypeReplacer.Content, table, column);


				case PatternContentKeyMode.FieldKeyReadType:
					PatternReplacement keyRead;

					bool canConvert = FieldType_IsConvertToSupported(column); // TODO

					// how to read key type

					if (!column.Nullable && canConvert)
					{
						keyRead = partialContent.GetReplacement(PatternConsts.ReplacementType.FieldKeyReadType_NormalField_Convert);
					}
					else if (!column.Nullable && !canConvert)
					{
						keyRead = partialContent.GetReplacement(PatternConsts.ReplacementType.FieldKeyReadType_NormalField_Cast);
					}
					else if (column.Nullable && canConvert)
					{
						keyRead = partialContent.GetReplacement(PatternConsts.ReplacementType.FieldKeyReadType_Nullable_Convert);
					}
					else
					{
						keyRead = partialContent.GetReplacement(PatternConsts.ReplacementType.FieldKeyReadType_Nullable_Cast);
					}
					// Replace the contents
					return Replacer_PatternContent_AppliesToColumn(keyRead.Content, table, column);


				case PatternContentKeyMode.FieldForeignKey:
				// this will be used with foreign keys replacer
				case PatternContentKeyMode.TablePrimaryKey:
				case PatternContentKeyMode.TableForeignKey:
				case PatternContentKeyMode.TableAutoIncrement:
				default:
					// Ignored
					return "";
			}
		}

		/// <summary>
		/// Partial content replacer.
		/// </summary>
		string PatternContent_AppliesToForeignKeys(PatternContent partialContent, SchemaTable table, SchemaForeignKey foreignKey)
		{
			switch (partialContent.ContentKeyMode)
			{
				case PatternContentKeyMode.FieldForeignKey:
					PatternReplacement theReplacer;

					switch (foreignKey.Multiplicity)
					{
						case SchemaForeignKey.ForeignKeyMultiplicity.One:
							theReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.FieldForeignKey_MultiplicityOne);
							break;
						case SchemaForeignKey.ForeignKeyMultiplicity.Many:
							theReplacer = partialContent.GetReplacement(PatternConsts.ReplacementType.FieldForeignKey_MultiplicityMany);
							break;


						default:
							// not defined Multiplicity
							return string.Empty;
					}

					// Replace the contents
					return Replacer_PatternContent_AppliesToForeignKey(theReplacer.Content, table, foreignKey);

				default:
					// Ignored
					return string.Empty;
			}
		}

		/// <summary>
		/// Applies data to pattern content replacement
		/// </summary>
		string PatternContent_ProjectFile_AppliesToGeneral(string content)
		{
			string generationPath = _projectDef.GenerationPath;
			generationPath = Common.ProjectPathMakeAbsolute(generationPath, _projectDef.ProjectFileName);

			if (generationPath[generationPath.Length - 1] != Path.DirectorySeparatorChar)
				generationPath += Path.DirectorySeparatorChar;

			// Remove generation path form string
			content = Common.ReplaceEx(content, ReplaceConsts.ConnectionString, _projectDef.DbSettions.GetConnectionString(), StringComparison.CurrentCultureIgnoreCase);

			// general
			content = Replacer_GeneratorGeneral(content);

			// database provider reference
			content = Replacer_DatabaseProvider(content);

			return content;
		}

		/// <summary>
		/// Applies data to pattern content replacement for column
		/// </summary>
		string PatternContent_ProjectFile_AppliesToFiles(string content, string fileNamePath)
		{
			string generationPath = _projectDef.GenerationPath;
			generationPath = Common.ProjectPathMakeAbsolute(generationPath, _projectDef.ProjectFileName);

			if (generationPath[generationPath.Length - 1] != Path.DirectorySeparatorChar)
				generationPath += Path.DirectorySeparatorChar;

			// Remove generation path form string
			string projectItemPath = fileNamePath.Replace(generationPath, "");

			content = Common.ReplaceEx(content, ReplaceConsts.ProjectItemPath, projectItemPath, StringComparison.CurrentCultureIgnoreCase);
			content = Common.ReplaceEx(content, ReplaceConsts.ConnectionString, _projectDef.DbSettions.GetConnectionString(), StringComparison.CurrentCultureIgnoreCase);

			// general
			content = Replacer_GeneratorGeneral(content);

			return content;
		}
		#endregion

		#region Tables and Views replacements

		/// <summary>
		/// Checking to see if user has selected this table to be generated
		/// </summary>
		private bool UserHasSelectedTable(string tableName)
		{
			return _projectDef.DbSettions.IsTableSelected(tableName);
		}

		/// <summary>
		/// Checking to see if user has selected this view to be generated
		/// </summary>
		private bool UserHasSelectedView(string viewName)
		{
			return _projectDef.DbSettions.IsViewSelected(viewName);
		}

		/// <summary>
		/// Pattern file - AppliesTo - TablesAndViews
		/// </summary>
		private void PatternFileAppliesTo_TablesAndViewsApplier(SchemaTable[] tablesList,
			SchemaTable[] viewsList,
			PatternFileWhereToApply whatListToUse,
			PatternFile patternFile)
		{
			if (patternFile.AppliesToAllToOne)
			{
				var genPath = _projectDef.GenerationPath;
				genPath = Common.ProjectPathMakeAbsolute(genPath, _projectDef.ProjectFileName);

				// the destination filename
				string fileName = Replacer_PatternFileName(patternFile.FilePath, null, null, null);
				fileName = Path.Combine(genPath, fileName);

				// create directory
				Directory.CreateDirectory(Path.GetDirectoryName(fileName));

				// don't overwrite if exists an overwriting is not requested
				if (patternFile.Overwrite == false && File.Exists(fileName))
					return;

				// generated file content
				string genContent = patternFile.BaseContent;

				// BaseContent Replacements
				genContent = Replacer_PatternBaseContent(genContent, null, null);

				// for each pattern content
				genContent = PatternContentAppliesTo_TablesAndViews(genContent,
					patternFile.PatternContents,
					tablesList,
					viewsList,
					patternFile);

				// all is done! Write to file
				File.WriteAllText(fileName, genContent);
			}
			else
			{
				// combine the list
				List<SchemaTable> tablesAndViews = new List<SchemaTable>();
				if (whatListToUse == PatternFileWhereToApply.Both ||
					whatListToUse == PatternFileWhereToApply.Tables)
					tablesAndViews.AddRange(tablesList);
				if (whatListToUse == PatternFileWhereToApply.Both ||
					whatListToUse == PatternFileWhereToApply.Views)
					tablesAndViews.AddRange(viewsList);

				// surf in the tables/views
				tablesAndViews.AsParallel().ForAll(table =>
				{
					string tableNativeName = table.TableName;
					string tableDotNetName = table.TableName;
					string tableDotNetNameCS = table.TableNameCS;


					if (table.TableType == SchemaTable.TableTypeInfo.Table)
					{
						// check if selected by user
						if (!UserHasSelectedTable(table.TableName))
							//continue;
							return;

						tableDotNetName = NaturalizeNames_TableName(table.TableName);
						tableDotNetNameCS = NaturalizeNames_TableName(table.TableNameCS);
					}
					else if (table.TableType == SchemaTable.TableTypeInfo.View)
					{
						// check if selected by user
						if (!UserHasSelectedView(table.TableName))
							//continue;
							return;

						tableDotNetName = NaturalizeNames_ViewName(table.TableName);
						tableDotNetNameCS = NaturalizeNames_ViewName(table.TableNameCS);
					}
					else
					{
						// What?! Invalid table type!
						return;
					}

					var genPath = _projectDef.GenerationPath;
					genPath = Common.ProjectPathMakeAbsolute(genPath, _projectDef.ProjectFileName);

					// the destination filename
					string fileName = Replacer_PatternFileName(patternFile.FilePath,
										  tableDotNetName,
										  tableNativeName,
										  tableDotNetNameCS);
					fileName = Path.Combine(genPath, fileName);

					// create directory
					Directory.CreateDirectory(Path.GetDirectoryName(fileName));

					// don't overwrite if exists an overwriting is not requested
					if (patternFile.Overwrite == false && File.Exists(fileName))
						//continue;
						return;

					// generated file content
					string genContent = patternFile.BaseContent;

					// BaseContent Replacements
					genContent = Replacer_PatternBaseContent(genContent,
										  tableDotNetName,
										  tableNativeName);

					// for each pattern content
					genContent = PatternContentAppliesTo_OneTable(genContent,
						patternFile.PatternContents,
						tablesList,
						viewsList,
						table,
						patternFile);

					// all is done! Write to file
					File.WriteAllText(fileName, genContent);
				});
			}

		}

		/// <summary>
		/// Starts to apply project files to the pattern
		/// </summary>
		private void PatternFileAppliesTo_ProjectFileApplier(PatternFile commonPattern)
		{
			var generationPath = _projectDef.GenerationPath;
			generationPath = Common.ProjectPathMakeAbsolute(generationPath, _projectDef.ProjectFileName);

			string fileName = Common.ReplaceEx(commonPattern.FilePath, ReplaceConsts.ProjectName, _projectDef.ProjectName, StringComparison.CurrentCultureIgnoreCase);
			fileName = Path.Combine(generationPath, fileName);

			// create directory
			Directory.CreateDirectory(Path.GetDirectoryName(fileName));

			// don't overwrite if exists an overwriting is not requested
			if (commonPattern.Overwrite == false && File.Exists(fileName))
				return;


			// generated file content
			string genContent = commonPattern.BaseContent;

			// general
			genContent = Replacer_GeneratorGeneral(genContent);

			// database provider class
			genContent = Replacer_DatabaseProvider(genContent);

			// Replacements
			genContent = Common.ReplaceEx(genContent, ReplaceConsts.ConnectionString, _projectDef.DbSettions.GetConnectionString(), StringComparison.CurrentCultureIgnoreCase);


			// search for pattern in the general content
			foreach (PatternContent pattern in commonPattern.PatternContents)
			{
				string partialName = string.Format(ReplaceConsts.PatternContentReplacer, pattern.Name);

				// is there a pattern for that
				if (genContent.IndexOf(partialName) == -1)
					continue;

				if (pattern.AppliesTo == PatternContentAppliesTo.Table)
				{
					string contentToReplace = PatternContentAppliesTo_ProjectFileGeneral(pattern);

					genContent = genContent.Replace(partialName, contentToReplace);
				}
				else if (pattern.AppliesTo == PatternContentAppliesTo.General)
				{
					// nothing!
				}
				else if (pattern.AppliesTo == PatternContentAppliesTo.Columns)
				{
					// Get project generated files, extnsion is specfied by the project
					string codeFilePattern = String.Format("*{0}", _patternProject.FileExtension);

					// Search the files
					string[] projectFiles = Directory.GetFiles(generationPath, codeFilePattern, SearchOption.AllDirectories);

					string contentToReplace = "";
					foreach (var projectFile in projectFiles)
					{
						string columnReplace = PatternContentAppliesTo_ProjectFiles(pattern, projectFile);

						if (!string.IsNullOrEmpty(columnReplace))
							contentToReplace += columnReplace + pattern.ItemsSeperator;
					}

					// removing unnecessary ItemsSeperator
					if (!string.IsNullOrEmpty(pattern.ItemsSeperator) && contentToReplace.EndsWith(pattern.ItemsSeperator))
						contentToReplace = contentToReplace.Remove(contentToReplace.Length - pattern.ItemsSeperator.Length, pattern.ItemsSeperator.Length);

					// replace
					genContent = genContent.Replace(partialName, contentToReplace);
				}
			}

			// Write to file
			File.WriteAllText(fileName, genContent);
		}

		/// <summary>
		/// Starts to apply general replacements to the pattern
		/// </summary>
		private void PatternFileAppliesTo_GeneralOnceApplier(PatternFile patternFile)
		{
			string fileName = patternFile.FilePath;

			var generationPath = _projectDef.GenerationPath;
			generationPath = Common.ProjectPathMakeAbsolute(generationPath, _projectDef.ProjectFileName);

			// the general replacements
			fileName = Replacer_GeneratorGeneral(fileName);

			// the destination path
			fileName = Path.Combine(generationPath, fileName);

			// create directory
			Directory.CreateDirectory(Path.GetDirectoryName(fileName));

			// don't overwrite if exists an overwriting is not requested
			if (patternFile.Overwrite == false && File.Exists(fileName))
				return;

			// generated file content
			string genContent = patternFile.BaseContent;

			// general
			genContent = Replacer_GeneratorGeneral(genContent);

			// database provider
			genContent = Replacer_DatabaseProvider(genContent);

			// Write to file
			File.WriteAllText(fileName, genContent);
		}
		#endregion

		#region private common files
		/// <summary>
		/// Get field size
		/// </summary>
		private string FieldType_ColumnDataSize(SchemaColumn column)
		{
			if (column.Length > 0)
				return column.Length.ToString();
			if (column.CharacterMaxLength > 0)
				return column.CharacterMaxLength.ToString();
			return "0";
		}

		/// <summary>
		/// Get field type database full name
		/// </summary>
		private string FieldType_ColumnDataTypeSize(SchemaColumn column)
		{
			string cleanType = NaturalizeNames_DotNetType(column.DotNetTypeClean);

			if (cleanType == "String" &&
				column.DbType.ToLower().IndexOf(_patternProject.LanguageSettings.TextFieldIdenticator) == -1)
				return column.DbType + "(" + column.Length + ")";

			if (column.DbType.ToLower() == _patternProject.LanguageSettings.DbDecimalName.ToLower())
			{
				return _patternProject.LanguageSettings.DbDecimalType
					.Replace(ReplaceConsts.Pattern_LanguageSettings_Precision, column.NumericPrecision.ToString())
					.Replace(ReplaceConsts.Pattern_LanguageSettings_Scale, column.NumericScale.ToString());
			}
			else if (column.DbType.ToLower() == _patternProject.LanguageSettings.DbNumericName.ToLower())
			{
				return _patternProject.LanguageSettings.DbNumericType
					.Replace(ReplaceConsts.Pattern_LanguageSettings_Precision, column.NumericPrecision.ToString())
					.Replace(ReplaceConsts.Pattern_LanguageSettings_Scale, column.NumericScale.ToString());
			}
			return column.DbType;
		}

		private bool FieldType_IsDataTypeNativeNullable(SchemaColumn column)
		{
			string cleanType = NaturalizeNames_DotNetType(column.DotNetTypeClean);
			return cleanType == "String" || cleanType == "Object" || column.IsArray();
		}

		private bool FieldType_IsConvertToSupported(SchemaColumn column)
		{
			string cleanType = NaturalizeNames_DotNetType(column.DotNetTypeClean);
			return !(column.IsArray() || cleanType == "Object" || cleanType == "Guid");
		}

		public static string ReplaceExIgnoreCase(string original, string pattern, string replacement)
		{
			return Common.ReplaceEx(original, pattern, replacement, StringComparison.CurrentCultureIgnoreCase);
		}
		#endregion

	}
}
