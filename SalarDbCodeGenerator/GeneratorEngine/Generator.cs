using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SalarDbCodeGenerator.DbProject;
using SalarDbCodeGenerator.Schema.Database;
using SalarDbCodeGenerator.Schema.DbSchemaReaders;
using SalarDbCodeGenerator.Schema.Patterns;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Salar Khalilzadeh <salar2k@gmail.com>
// © 2012, All rights reserved
// 2012/07/21
// ====================================
namespace SalarDbCodeGenerator.GeneratorEngine
{
	public class Generator
	{
		/// <summary>
		/// Where will pattern file apply
		/// </summary>
		private enum PatternFileWhereToApply { Both, Tables, Views, None }

		#region local variables
		PatternProject _patternProject;
		ProjectDefinaton _projectDef;
		DbDatabase _database;
		ExSchemaEngine _schemaEngine;
		bool _optionGenerateUnselectedForeigns;
		#endregion

		public Generator(ProjectDefinaton project, PatternProject pattern, DbDatabase database, ExSchemaEngine schemaEngine)
		{
			_patternProject = pattern;
			_projectDef = project;
			_database = database;
			_schemaEngine = schemaEngine;
			_optionGenerateUnselectedForeigns = false;
		}

		#region public methods
		/// <summary>
		/// Let the engine start
		/// </summary>
		public void Generate()
		{
			string patternsFolder = Path.GetDirectoryName(
				Common.AppVarPathMakeAbsolute(_projectDef.CodeGenSettings.CodeGenPatternFile));

			// arrays! don't remember why I did this and too lazy to check! :)
			var schemaTables = _database.SchemaTables.ToArray();
			var schemaViews = _database.SchemaViews.ToArray();

			// all the pattern files
			_patternProject.PatternFiles.ForEach(
				patFile =>
				{
					var patternFilePath = Path.Combine(patternsFolder, patFile.Path);

					// Copy action requested
					if (patFile.Action == PatternsListItemAction.Copy)
					{
						var fileName = Path.GetFileName(patFile.Path);
						// Check if pattern is selected by user
						if (!_projectDef.CodeGenSettings.SelectedPatterns.Contains(fileName))
						{
							return;
						}

						var copyPath = _projectDef.GenerationPath;
						var copyPathDir = Common.ProjectPathMakeAbsolute(copyPath, _projectDef.ProjectFileName);
						if (!string.IsNullOrEmpty(patFile.ActionCopyPath))
						{
							copyPath = Path.Combine(copyPathDir, patFile.ActionCopyPath);
							copyPathDir = Path.GetDirectoryName(copyPath);
						}

						try
						{
							Directory.CreateDirectory(copyPathDir);
							File.Copy(patternFilePath, copyPath, true);
						}
						catch (Exception)
						{
							// TODO: log failed
						}
						return;
					}


					// load the pattern file
					var patternFile = PatternFile.ReadFromFile(patternFilePath);


					// Check if pattern is selected by user
					if (!_projectDef.CodeGenSettings.SelectedPatterns.Contains(patternFile.Name))
					{
						//continue;
						return;
					}

					switch (patternFile.Options.AppliesTo)
					{
						case PatternFileAppliesTo.General:
							PatternFileAppliesTo_GeneralApplier(patternFile);
							break;

						case PatternFileAppliesTo.TablesAndViewsEach:
							PatternFileAppliesTo_TablesAndViewsApplier(schemaTables, schemaViews,
																	   PatternFileWhereToApply.Both,
																	   patternFile);
							break;

						case PatternFileAppliesTo.TablesAndViewsAll:
							PatternFileAppliesTo_TablesAndViewsApplier(schemaTables, schemaViews,
																	   PatternFileWhereToApply.None,
																	   patternFile);
							break;

						case PatternFileAppliesTo.TablesEach:
						case PatternFileAppliesTo.TablesAll:
							PatternFileAppliesTo_TablesAndViewsApplier(schemaTables, schemaViews,
																	   PatternFileWhereToApply.Tables,
																	   patternFile);
							break;

						case PatternFileAppliesTo.ViewsEach:
						case PatternFileAppliesTo.ViewsAll:
							PatternFileAppliesTo_TablesAndViewsApplier(schemaTables, schemaViews,
																	   PatternFileWhereToApply.Views,
																	   patternFile);
							break;

						case PatternFileAppliesTo.ProjectFile:
							// only for project files
							PatternFileAppliesTo_ProjectFileApplier(patternFile);
							break;
					}
				});

		}
		#endregion


		#region PatternFileAppliesTo
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
		private void PatternFileAppliesTo_TablesAndViewsApplier(DbTable[] tablesList,
			DbTable[] viewsList,
			PatternFileWhereToApply whatListToUse,
			PatternFile patternFile)
		{
			if (patternFile.IsAllToOne)
			{
				var genPath = _projectDef.GenerationPath;
				genPath = Common.ProjectPathMakeAbsolute(genPath, _projectDef.ProjectFileName);

				// the destination filename
				string fileName = Replacer_PatternFileName(patternFile.Options.FilePath, null, null, null);
				fileName = Path.Combine(genPath, fileName);

				// don't overwrite if exists and overwriting is not requested
				if (patternFile.Options.Overwrite == false && File.Exists(fileName))
					return;

				// create directory
				Directory.CreateDirectory(Path.GetDirectoryName(fileName));

				// generated file content
				string genContent = patternFile.BaseContent;

				// BaseContent Replacements
				genContent = Replacer_PatternBaseContent(genContent, null, null);

				// for each pattern content
				genContent = PatternContentAppliesTo_TablesAndViews(genContent,
					patternFile.PatternContents,
					tablesList,
					viewsList);

				// all is done! Write to file
				File.WriteAllText(fileName, genContent);
			}
			else
			{
				// combine the list
				var tablesAndViews = new List<DbTable>();
				if (whatListToUse == PatternFileWhereToApply.Both ||
					whatListToUse == PatternFileWhereToApply.Tables)
					tablesAndViews.AddRange(tablesList);
				if (whatListToUse == PatternFileWhereToApply.Both ||
					whatListToUse == PatternFileWhereToApply.Views)
					tablesAndViews.AddRange(viewsList);

				// surf in the tables/views
				tablesAndViews.ForEach(table =>
				{
					if (table.TableType == DbTable.TableTypeInfo.Table)
					{
						// check if selected by user
						if (!UserHasSelectedTable(table.TableName))
							//continue;
							return;
					}
					else if (table.TableType == DbTable.TableTypeInfo.View)
					{
						// check if selected by user
						if (!UserHasSelectedView(table.TableName))
							//continue;
							return;
					}
					else
					{
						// What?! Invalid table type!
						return;
					}

					var genPath = _projectDef.GenerationPath;
					genPath = Common.ProjectPathMakeAbsolute(genPath, _projectDef.ProjectFileName);

					// the destination filename
					string fileName = Replacer_PatternFileName(patternFile.Options.FilePath,
										  table.TableNameSchema,
										  table.TableName,
										  table.TableNameSchemaCS);
					fileName = Path.Combine(genPath, fileName);

					// don't overwrite if exists an overwriting is not requested
					if (patternFile.Options.Overwrite == false && File.Exists(fileName))
						//continue;
						return;

					// create directory
					Directory.CreateDirectory(Path.GetDirectoryName(fileName));

					// generated file content
					string genContent = patternFile.BaseContent;

					// BaseContent Replacements
					genContent = Replacer_PatternBaseContent(genContent,
										  table.TableNameSchema,
										  table.TableName);

					// for each pattern content
					genContent = PatternContentAppliesTo_OneTable(
						genContent,
						patternFile.PatternContents,
						table,
						null,
						null);

					// all is done! Write to file
					File.WriteAllText(fileName, genContent);
				});
			}
		}

		/// <summary>
		/// Starts to apply general replacements to the pattern
		/// </summary>
		private void PatternFileAppliesTo_GeneralApplier(PatternFile patternFile)
		{
			string fileName = patternFile.Options.FilePath;

			var generationPath = _projectDef.GenerationPath;
			generationPath = Common.ProjectPathMakeAbsolute(generationPath, _projectDef.ProjectFileName);

			// the general replacements
			fileName = Replacer_GeneratorGeneral(fileName);

			// the destination path
			fileName = Path.Combine(generationPath, fileName);

			// create directory
			Directory.CreateDirectory(Path.GetDirectoryName(fileName));

			// don't overwrite if exists an overwriting is not requested
			if (patternFile.Options.Overwrite == false && File.Exists(fileName))
				return;

			// generated file content
			string genContent = patternFile.BaseContent;

			// general
			genContent = Replacer_GeneratorGeneral(genContent);

			// database provider
			genContent = Replacer_DatabaseProvider(genContent);

			// search for pattern in the general content
			foreach (PatternContent pattern in patternFile.PatternContents)
			{
				string partialName = string.Format(ReplaceConsts.PatternContentReplacer, pattern.Name);

				// is there a pattern for that
				if (genContent.IndexOf(partialName) == -1)
					continue;

				if (pattern.ConditionKeyMode == PatternConditionKeyMode.General ||
				   pattern.ConditionKeyMode == PatternConditionKeyMode.DatabaseProvider)
				{
					string contentToReplace = PatternContentAppliesTo_General(pattern);

					genContent = genContent.Replace(partialName, contentToReplace);
				}
			}

			// Write to file
			File.WriteAllText(fileName, genContent);
		}

		/// <summary>
		/// Starts to apply project files to the pattern
		/// </summary>
		private void PatternFileAppliesTo_ProjectFileApplier(PatternFile commonPattern)
		{
			var generationPath = _projectDef.GenerationPath;
			generationPath = Common.ProjectPathMakeAbsolute(generationPath, _projectDef.ProjectFileName);

			string fileName = Common.ReplaceEx(commonPattern.Options.FilePath, ReplaceConsts.ProjectName, _projectDef.ProjectName, StringComparison.CurrentCultureIgnoreCase);
			fileName = Path.Combine(generationPath, fileName);

			// don't overwrite if exists an overwriting is not requested
			if (commonPattern.Options.Overwrite == false && File.Exists(fileName))
				return;

			// create directory
			Directory.CreateDirectory(Path.GetDirectoryName(fileName));

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

				if (pattern.ConditionKeyMode == PatternConditionKeyMode.ProjectFiles)
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
				else if (pattern.ConditionKeyMode == PatternConditionKeyMode.General ||
					pattern.ConditionKeyMode == PatternConditionKeyMode.DatabaseProvider)
				{
					string contentToReplace = PatternContentAppliesTo_General(pattern);

					genContent = genContent.Replace(partialName, contentToReplace);
				}
			}

			// Write to file
			File.WriteAllText(fileName, genContent);
		}

		#endregion

		#region  PatternContentAppliesTo

		/// <summary>
		/// Partial content replacer.
		/// </summary>
		string PatternContentAppliesTo_ProjectFiles(PatternContent partialContent, string fileNamePath)
		{
			var oneReplacer = partialContent.GetFirst();
			return Replacer_ConditionItem_ProjectFile(oneReplacer.ContentText, fileNamePath);
		}

		/// <summary>
		/// Partial content replacer.
		/// </summary>
		string PatternContentAppliesTo_General(PatternContent partialContent)
		{
			if (partialContent.ConditionKeyMode == PatternConditionKeyMode.DatabaseProvider)
			{
				ConditionItem dbReplacer = null;
				switch (this._database.Provider)
				{
					case DatabaseProvider.Oracle:
						dbReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.DatabaseProvider.Oracle);
						break;

					case DatabaseProvider.SQLServer:
						dbReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.DatabaseProvider.SQLServer);
						break;

					case DatabaseProvider.SQLite:
						dbReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.DatabaseProvider.SQLite);
						break;

					case DatabaseProvider.SqlCe4:
						dbReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.DatabaseProvider.SqlCe4);
						break;
				}


				if (dbReplacer != null)
					// Replace the contents
					return Replacer_ConditionItem_AppliesToGeneral(dbReplacer.ContentText);

			}
			else if (partialContent.ConditionKeyMode == PatternConditionKeyMode.General)
			{
				var dbReplacer = partialContent.GetFirst();
				if (dbReplacer != null)
					// Replace the contents
					return Replacer_ConditionItem_AppliesToGeneral(dbReplacer.ContentText);
			}

			return "";
		}

		/// <summary>
		/// One table, one column or one foreignKey in table!
		/// </summary>
		string PatternContentAppliesTo_OneTable(string baseContent,
			List<PatternContent> patternContent,
			DbTable table,
			DbColumn column,
			DbForeignKey foreignKey)
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

				switch (pattern.ConditionKeyMode)
				{
					case PatternConditionKeyMode.General:
						// nothing!
						break;

					case PatternConditionKeyMode.TablesAll:
					case PatternConditionKeyMode.ViewsAll:
					case PatternConditionKeyMode.TablesAndViewsAll:
						// for one table? Meh, we do nothing!
						break;

					case PatternConditionKeyMode.TableAutoIncrement:
					case PatternConditionKeyMode.TableIndexConstraint:
					case PatternConditionKeyMode.TablePrimaryKey:
					case PatternConditionKeyMode.TableUniqueConstraint:
						appliedContent = ConditionItem_AppliesToTable(pattern, table);

						// base content
						if (!string.IsNullOrEmpty(pattern.BaseContent))
						{
							appliedContent = pattern.BaseContent.Replace(ReplaceConsts.PatternContentInnerContents, appliedContent);
						}

						// internal pattern contents
						if (pattern.ConditionContents.Count > 0)
						{
							// nested call
							appliedContent = PatternContentAppliesTo_OneTable(appliedContent, pattern.ConditionContents, table, null, null);
						}

						// replace the content
						baseContent = baseContent.Replace(replacementName, appliedContent);
						break;


					case PatternConditionKeyMode.Field:
					case PatternConditionKeyMode.FieldCondensedType:
					case PatternConditionKeyMode.FieldKeyReadType:
					case PatternConditionKeyMode.FieldKeyType:
					case PatternConditionKeyMode.FieldPrimaryKey:
					case PatternConditionKeyMode.FieldReferencedKeyType:
						appliedContent = "";

						// no special column is specified
						if (column == null)
						{
							// replace in the main content
							baseContent = Common.ReplaceEx(baseContent, replacementName, appliedContent, StringComparison.CurrentCulture);
						}
						else
						{
							// Apply the replacement to the pattern content
							string columnReplace = ConditionItem_AppliesToColumn(pattern, table, column);

							// The seperator
							if (!string.IsNullOrEmpty(columnReplace))
							{
								// internal pattern contents
								// FOR EACH column
								if (pattern.ConditionContents.Count > 0)
								{
									// nested call
									columnReplace = PatternContentAppliesTo_OneTable(
										columnReplace,
										pattern.ConditionContents,
										table,
										column,
										null);
								}
								appliedContent += columnReplace + pattern.ItemsSeperator;
							}
						}


						// Remove additional ItemsSeperator
						if (!string.IsNullOrEmpty(pattern.ItemsSeperator)
							&& appliedContent.EndsWith(pattern.ItemsSeperator))
							appliedContent = appliedContent.Remove(appliedContent.Length - pattern.ItemsSeperator.Length, pattern.ItemsSeperator.Length);

						// internal pattern contents
						// FOR EACH column
						if (pattern.ConditionContents.Count > 0)
						{
							// nested call
							appliedContent = PatternContentAppliesTo_OneTable(appliedContent, pattern.ConditionContents, table, null, null);
						}

						// replace in the main content
						baseContent = Common.ReplaceEx(baseContent, replacementName, appliedContent, StringComparison.CurrentCulture);
						break;

					case PatternConditionKeyMode.FieldsAll:
					case PatternConditionKeyMode.FieldsCondensedTypeAll:
					case PatternConditionKeyMode.FieldsKeyReadTypeAll:
					case PatternConditionKeyMode.FieldsKeyTypeAll:
					case PatternConditionKeyMode.FieldsPrimaryKeyAll:
					case PatternConditionKeyMode.FieldsReferencedKeyTypeAll:
						appliedContent = "";

						// fetch the columns and apply the replacement operation
						foreach (var tableColumn in table.SchemaColumns)
						{
							// Apply the replacement to the pattern content
							string columnReplace = ConditionItem_AppliesToColumn(pattern, table, tableColumn);

							// The seperator
							if (!string.IsNullOrEmpty(columnReplace))
							{
								// internal pattern contents
								// FOR EACH column
								if (pattern.ConditionContents.Count > 0)
								{
									// nested call
									columnReplace = PatternContentAppliesTo_OneTable(
										columnReplace,
										pattern.ConditionContents,
										table,
										tableColumn, null);
								}
								appliedContent += columnReplace + pattern.ItemsSeperator;
							}
						}

						// Remove additional ItemsSeperator
						if (!string.IsNullOrEmpty(pattern.ItemsSeperator)
							&& appliedContent.EndsWith(pattern.ItemsSeperator))
							appliedContent = appliedContent.Remove(appliedContent.Length - pattern.ItemsSeperator.Length, pattern.ItemsSeperator.Length);

						// internal pattern contents
						// FOR EACH column
						if (pattern.ConditionContents.Count > 0)
						{
							// nested call
							appliedContent = PatternContentAppliesTo_OneTable(appliedContent, pattern.ConditionContents, table, null, null);
						}

						// replace in the main content
						baseContent = Common.ReplaceEx(baseContent, replacementName, appliedContent, StringComparison.CurrentCulture);
						break;

					case PatternConditionKeyMode.ForeignKeyDeleteAction:
					case PatternConditionKeyMode.ForeignKeyUpdateAction:
					case PatternConditionKeyMode.FieldForeignKey:
						appliedContent = "";

						if (foreignKey == null)
						{
							// replace in the main content
							baseContent = Common.ReplaceEx(baseContent, replacementName, appliedContent, StringComparison.CurrentCulture);
						}
						else
						{
							// Apply the replacement to the pattern content
							string columnReplace = ConditionItem_AppliesToForeignKeyColumns(pattern, table, foreignKey);

							// The seperator
							if (!string.IsNullOrEmpty(columnReplace))
							{
								// internal pattern contents
								if (pattern.ConditionContents.Count > 0)
								{
									// nested call
									columnReplace = PatternContentAppliesTo_OneTable(
										columnReplace,
										pattern.ConditionContents,
										table,
										null,
										foreignKey);
								}

								appliedContent += columnReplace + pattern.ItemsSeperator;
							}
						}

						// Remove additional ItemsSeperator
						if (!string.IsNullOrEmpty(pattern.ItemsSeperator)
							&& appliedContent.EndsWith(pattern.ItemsSeperator))
							appliedContent = appliedContent.Remove(appliedContent.Length - pattern.ItemsSeperator.Length, pattern.ItemsSeperator.Length);

						// internal pattern contents
						if (pattern.ConditionContents.Count > 0)
						{
							// nested call
							appliedContent = PatternContentAppliesTo_OneTable(appliedContent, pattern.ConditionContents, table, null, null);
						}

						// replace in the main content
						baseContent = Common.ReplaceEx(baseContent, replacementName, appliedContent, StringComparison.CurrentCulture);
						break;

					case PatternConditionKeyMode.FieldsForeignKeyAll:
					case PatternConditionKeyMode.TableForeignKey:
						appliedContent = "";


						// fetch the columns and apply the replacement operation
						foreach (var dbForeignKey in table.ForeignKeys)
						{
							// Apply the replacement to the pattern content
							string columnReplace = ConditionItem_AppliesToForeignKeyColumns(pattern, table, dbForeignKey);

							// The seperator
							if (!string.IsNullOrEmpty(columnReplace))
							{
								// internal pattern contents
								if (pattern.ConditionContents.Count > 0)
								{
									// nested call
									columnReplace = PatternContentAppliesTo_OneTable(
										columnReplace,
										pattern.ConditionContents,
										table,
										null,
										dbForeignKey);
								}

								appliedContent += columnReplace + pattern.ItemsSeperator;
							}
						}

						// Remove additional ItemsSeperator
						if (!string.IsNullOrEmpty(pattern.ItemsSeperator)
							&& appliedContent.EndsWith(pattern.ItemsSeperator))
							appliedContent = appliedContent.Remove(appliedContent.Length - pattern.ItemsSeperator.Length, pattern.ItemsSeperator.Length);

						// internal pattern contents
						if (pattern.ConditionContents.Count > 0)
						{
							// nested call
							appliedContent = PatternContentAppliesTo_OneTable(appliedContent, pattern.ConditionContents, table, null, null);
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


		string PatternContentAppliesTo_TablesAndViews(
			string baseContent,
			List<PatternContent> patternContent,
			DbTable[] tablesList,
			DbTable[] viewsList)
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

				switch (pattern.ConditionKeyMode)
				{
					//case PatternContentAppliesTo.General:
					//    // general part
					//    appliedContent = PatternContentAppliesTo_ProjectFileGeneral(pattern);

					//    // replace the content
					//    baseContent = baseContent.Replace(replacementName, appliedContent);

					//    break;
					case PatternConditionKeyMode.DatabaseProvider:
						ConditionItem dbReplacer = null;
						switch (this._database.Provider)
						{
							case DatabaseProvider.Oracle:
								dbReplacer = pattern.GetReplacement(ConditionKeyModeConsts.DatabaseProvider.Oracle);
								break;

							case DatabaseProvider.SQLServer:
								dbReplacer = pattern.GetReplacement(ConditionKeyModeConsts.DatabaseProvider.SQLServer);
								break;

							case DatabaseProvider.SQLite:
								dbReplacer = pattern.GetReplacement(ConditionKeyModeConsts.DatabaseProvider.SQLite);
								break;

							case DatabaseProvider.SqlCe4:
								dbReplacer = pattern.GetReplacement(ConditionKeyModeConsts.DatabaseProvider.SqlCe4);
								break;
						}

						if (dbReplacer != null)
						{
							appliedContent = Replacer_ConditionItem_AppliesToGeneral(dbReplacer.ContentText);

							// replace the content
							baseContent = baseContent.Replace(replacementName, appliedContent);
						}
						break;


					case PatternConditionKeyMode.TablesAndViewsAll:
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
							appliedContent += ConditionItem_AppliesToTableAndViewsAll(pattern, tbl);

							// the seperator identicator
							index++;

							// internal pattern contents
							if (pattern.ConditionContents.Count > 0)
							{
								// nested call
								appliedContent = PatternContentAppliesTo_OneTable(
									appliedContent,
									pattern.ConditionContents,
									tbl,
									null, null);
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
							appliedContent += ConditionItem_AppliesToTableAndViewsAll(pattern, view);

							// the seperator identicator
							index++;

							// internal pattern contents
							if (pattern.ConditionContents.Count > 0)
							{
								// nested call
								appliedContent = PatternContentAppliesTo_OneTable(
									appliedContent,
									pattern.ConditionContents,
									view,
									null, null);
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

					case PatternConditionKeyMode.TablesAll:
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
							appliedContent += ConditionItem_AppliesToTableAndViewsAll(pattern, tbl);

							// the seperator identicator
							index++;

							// internal pattern contents
							if (pattern.ConditionContents.Count > 0)
							{
								// nested call
								appliedContent = PatternContentAppliesTo_OneTable(
									appliedContent, pattern.ConditionContents,
									tbl,
									null, null);
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

					case PatternConditionKeyMode.ViewsAll:
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
							appliedContent += ConditionItem_AppliesToTableAndViewsAll(pattern, view);

							// the seperator identicator
							index++;

							// internal pattern contents
							if (pattern.ConditionContents.Count > 0)
							{
								// nested call
								appliedContent = PatternContentAppliesTo_OneTable(
									appliedContent, pattern.ConditionContents,
									view,
									null, null);
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
		#endregion

		#region Content Replacers

		/// <summary>
		/// Applies data to pattern content replacement
		/// </summary>
		string Replacer_ConditionItem_AppliesToGeneral(string content)
		{
			string generationPath = _projectDef.GenerationPath;
			generationPath = Common.ProjectPathMakeAbsolute(generationPath, _projectDef.ProjectFileName);

			if (generationPath[generationPath.Length - 1] != Path.DirectorySeparatorChar)
				generationPath += Path.DirectorySeparatorChar;

			// Remove generation path form string

			// general
			content = Replacer_GeneratorGeneral(content);

			// database provider reference
			content = Replacer_DatabaseProvider(content);

			return content;
		}

		/// <summary>
		/// Applies data to pattern content replacement for column
		/// </summary>
		string Replacer_ConditionItem_ProjectFile(string content, string fileNamePath)
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

		/// <summary>
		/// Applies table data to pattern content replacement
		/// </summary>
		string Replacer_ConditionItem_AppliesToTable(string content, DbTable table)
		{
			if (string.IsNullOrWhiteSpace(content))
			{
				return content;
			}

			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.TableName, table.TableNameSchema);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.TableNameDb, table.TableName);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.TableOwnerName, table.OwnerName);

			// general
			content = Replacer_GeneratorGeneral(content);

			// database provider class
			content = Replacer_DatabaseProvider(content);

			var autoKey = table.GetFirstAutoIncrementField();
			var primaryKey = table.GetPrimaryKey();

			if (autoKey == null)
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.AutoIncrementDataType, _patternProject.LanguageSettings.VoidDataType);
			else
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.AutoIncrementDataType, autoKey.DataTypeDotNet);

			if (primaryKey == null)
			{
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDbType, "");
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDbTypeSize, "");
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDataType, _patternProject.LanguageSettings.VoidDataType);
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyName, "");
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyNameDb, "");
			}
			else
			{
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDbType, primaryKey.DataTypeDb);
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDbTypeSize, FieldType_ColumnDataTypeSize(primaryKey));
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDataType, primaryKey.DataTypeDotNet);
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyName, primaryKey.FieldNameSchema);
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyNameDb, primaryKey.FieldNameDb);
			}
			return content;
		}

		/// <summary>
		/// Applies column data to pattern content replacement
		/// </summary>
		string Replacer_ConditionItem_AppliesToColumn(string content, DbTable table, DbColumn column)
		{
			// table of the column
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.TableName, table.TableNameSchema);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.TableNameDb, table.TableName);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.TableOwnerName, table.OwnerName);

			// general
			content = Replacer_GeneratorGeneral(content);

			// database provider class
			content = Replacer_DatabaseProvider(content);

			// referenced table of the column
			if (column.IsReferenceKey && column.IsReferenceKeyTable != null)
			{
				var refTable = column.IsReferenceKeyTable;

				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.TableNameRefField, refTable.TableNameSchema);
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.TableNameDbRefField, refTable.TableName);
			}

			// column information
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.FieldDataType, column.DataTypeDotNet);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.FieldName, column.FieldNameSchema);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.FieldNameDb, column.FieldNameDb);

			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.FieldOrdinalValue, column.ColumnOrdinal.ToString());
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.FieldIsPrimaryKey, column.PrimaryKey.ToString().ToLower());
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.FieldCanBeNull, column.AllowNull.ToString().ToLower());

			// column description
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.FieldDescription, column.UserDescription);

			// Database field type
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.FieldDbType, column.DataTypeDb);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.FieldDbTypeSize, FieldType_ColumnDataTypeSize(column));
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.FieldDbSize, FieldType_ColumnDataSize(column));



			if (column.AutoIncrement)
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.AutoIncrementDataType, column.DataTypeDotNet);
			else
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.AutoIncrementDataType, _patternProject.LanguageSettings.VoidDataType);

			if (column.PrimaryKey)
			{
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDbType, column.DataTypeDb);
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDbTypeSize, FieldType_ColumnDataTypeSize(column));
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDataType, column.DataTypeDotNet);
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyName, column.FieldNameSchema);
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyNameDb, column.FieldNameDb);
			}
			else
			{
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDbType, "");
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDbTypeSize, "");
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyDataType, _patternProject.LanguageSettings.VoidDataType);
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyName, "");
				content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.PrimaryKeyNameDb, "");
			}

			return content;
		}

		/// <summary>
		/// Applies table foreign keys to pattern content replacement
		/// </summary>
		string Replacer_ConditionItem_AppliesToForeignKeys(string content, DbTable table)
		{
			// this section should be ignored if there is no foreign keys!
			if (table.ForeignKeys.Count == 0)
			{
				// No foreign keys! go away
				return "";
			}

			// local table
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.LocalTableName, table.TableNameSchema);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.LocalTableNameDb, table.TableName);

			// the result
			string result = "";

			// fetch foreign keys
			foreach (var foreignKey in table.ForeignKeys)
			{
				// the content copy for each foreign key
				string foreignContent = content;

				// naturalize names
				foreignContent = Replacer_ConditionItem_AppliesToForeignKey(foreignContent, table, foreignKey);


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
		string Replacer_ConditionItem_AppliesToForeignKey(string content, DbTable table, DbForeignKey foreignKey)
		{

			// NOTE: foreign keys are always for TABLEs
			// Checking if user has selected this table
			// Also checking the option!
			if (!_optionGenerateUnselectedForeigns && !UserHasSelectedTable(foreignKey.ForeignTableName))
			{
				// User has not selected this foreign table
				return string.Empty;
			}

			// general
			content = Replacer_GeneratorGeneral(content);

			// database provider class
			content = Replacer_DatabaseProvider(content);

			// local table
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.LocalTableName, table.TableNameSchema);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.LocalTableNameDb, table.TableName);

			// foreign table
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.ForeignTableNameAsField, foreignKey.ForeignTableNameInLocalTable);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.ForeignTableName, foreignKey.ForeignTable.TableNameSchema);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.ForeignTableNameDb, foreignKey.ForeignTable.TableName);

			// ===================================
			// local field
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.LocalFieldDataType, foreignKey.LocalColumn.DataTypeDotNet);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.LocalFieldName, foreignKey.LocalColumn.FieldNameSchema);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.LocalFieldNameDb, foreignKey.LocalColumn.FieldNameDb);
			// no oridianl, foreignContent = ReplaceExIgnoreCase(foreignContent, ReplaceConsts.LocalFieldOrdinalValue, foreignKey.LocalColumn.ColumnOrdinal.ToString());
			// column description
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.LocalFieldDescription, foreignKey.LocalColumn.UserDescription);
			// Database field type
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.LocalFieldDbTypeSize, FieldType_ColumnDataTypeSize(foreignKey.LocalColumn));


			// ===================================
			// Foreign field
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.ForeignFieldDataType, foreignKey.ForeignColumn.DataTypeDotNet);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.ForeignFieldName, foreignKey.ForeignColumn.FieldNameSchema);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.ForeignFieldNameDb, foreignKey.ForeignColumn.FieldNameDb);
			// no oridianl, foreignContent = ReplaceExIgnoreCase(foreignContent, ReplaceConsts.ForeignFieldOrdinalValue, foreignKey.ForeignColumn.ColumnOrdinal.ToString());
			// column description
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.ForeignFieldDescription, foreignKey.ForeignColumn.UserDescription);
			// Database field type
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.ForeignFieldDbTypeSize, FieldType_ColumnDataTypeSize(foreignKey.ForeignColumn));
			return content;
		}

		/// <summary>
		/// Applies table index constraintsto pattern content replacement
		/// </summary>
		string Replacer_ConditionItem_AppliesToIndexConstraints(string content, DbTable table, bool uniqueKeys)
		{
			// check if there is no constraints
			if (table.ConstraintKeys.Count == 0)
			{
				return "";
			}

			content = Replacer_PatternBaseContent(content, table.TableNameSchema, table.TableName);

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
				indexContent = Replacer_ConditionItem_AppliesToColumn(indexContent, table, indexKey.KeyColumn);

				// ===================================
				indexContent = Common.ReplaceExIgnoreCase(indexContent, ReplaceConsts.IndexKeyDbType, indexKey.KeyColumn.DataTypeDb);
				indexContent = Common.ReplaceExIgnoreCase(indexContent, ReplaceConsts.IndexKeyDbTypeSize, FieldType_ColumnDataTypeSize(indexKey.KeyColumn));
				indexContent = Common.ReplaceExIgnoreCase(indexContent, ReplaceConsts.IndexKeyDataType, indexKey.KeyColumn.DataTypeDotNet);
				indexContent = Common.ReplaceExIgnoreCase(indexContent, ReplaceConsts.IndexKeyName, indexKey.KeyColumn.FieldNameSchema);
				indexContent = Common.ReplaceExIgnoreCase(indexContent, ReplaceConsts.IndexKeyNameDb, indexKey.KeyColumn.FieldNameDb);
				indexContent = Common.ReplaceExIgnoreCase(indexContent, ReplaceConsts.IndexName, indexKey.KeyName);

				// add it to the result
				result += ReplaceConsts.NewLine + indexContent;
			}

			return result;
		}

		#endregion

		#region ConditionItem_AppliesTo
		string ConditionItem_AppliesToTableAndViewsAll(PatternContent partialContent, DbTable table)
		{
			var conditionContent = partialContent.GetFirst();
			return Replacer_ConditionItem_AppliesToTable(conditionContent.ContentText, table);
		}

		/// <summary>
		/// Partial content replacer.
		/// </summary>
		string ConditionItem_AppliesToTable(PatternContent partialContent, DbTable table)
		{
			// Find suitable replacement type
			switch (partialContent.ConditionKeyMode)
			{
				//case PatternConditionKeyMode.DatabaseProvider:

				//    ConditionItem dbReplacer = null;

				//    switch (this._database.Provider)
				//    {
				//        case DatabaseProvider.Oracle:
				//            dbReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.DatabaseProvider.Oracle);
				//            break;

				//        case DatabaseProvider.SQLServer:
				//            dbReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.DatabaseProvider.SQLServer);
				//            break;

				//        case DatabaseProvider.SQLite:
				//            dbReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.DatabaseProvider.SQLite);
				//            break;

				//        case DatabaseProvider.SqlCe4:
				//            dbReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.DatabaseProvider.SqlCe4);
				//            break;
				//    }

				//    if (dbReplacer == null)
				//        return "";

				//    // Replace the contents
				//    return Replacer_ConditionItem_AppliesToTable(dbReplacer.ContentText, table);

				case PatternConditionKeyMode.TableForeignKey:

					var normalKeyReplacer = partialContent.GetFirst();

					// Replace the contents
					return Replacer_ConditionItem_AppliesToForeignKeys(normalKeyReplacer.ContentText, table);

				case PatternConditionKeyMode.TableIndexConstraint:

					var indexConstraintReplacer = partialContent.GetFirst();

					// Replace the contents
					return Replacer_ConditionItem_AppliesToIndexConstraints(indexConstraintReplacer.ContentText, table, false);

				case PatternConditionKeyMode.TableUniqueConstraint:

					var uniqueConstraintReplacer = partialContent.GetFirst();

					// Replace the contents
					return Replacer_ConditionItem_AppliesToIndexConstraints(uniqueConstraintReplacer.ContentText, table, true);

				case PatternConditionKeyMode.TablePrimaryKey:

					ConditionItem primaryReplacer;

					if (table.ReadOnly)
					{
						// Table is marked as read only, like views
						primaryReplacer = partialContent.GetReplacement(
							ConditionKeyModeConsts.TablePrimaryKey.ReadOnlyTable);
					}
					else if (table.HasPrimaryKey())
					{
						// Table has a primary key
						primaryReplacer = partialContent.GetReplacement(
							ConditionKeyModeConsts.TablePrimaryKey.WithPrimaryKey);
					}
					else
					{
						// There is no primary key, default
						primaryReplacer = partialContent.GetReplacement(
							ConditionKeyModeConsts.TablePrimaryKey.NoPrimaryKey);
					}

					// Replace the contents
					return Replacer_ConditionItem_AppliesToTable(primaryReplacer.ContentText, table);

				case PatternConditionKeyMode.TableAutoIncrement:

					int autoCount = table.GetAutoIncrementCount();
					ConditionItem autoReplacer;

					if (autoCount == 0)
					{
						autoReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.TableAutoIncrement.NoAutoIncrement);
					}
					else if (autoCount == 1)
					{
						autoReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.TableAutoIncrement.OneAutoIncrement);
					}
					else
					{
						autoReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.TableAutoIncrement.MoreAutoIncrement);
					}

					// Replace the contents
					return Replacer_ConditionItem_AppliesToTable(autoReplacer.ContentText, table);

				default:
					// Ignored
					return "";
			}
		}

		/// <summary>
		/// Partial content replacer.
		/// </summary>
		string ConditionItem_AppliesToColumn(PatternContent partialContent, DbTable table, DbColumn column)
		{
			switch (partialContent.ConditionKeyMode)
			{
				//case PatternConditionKeyMode.DatabaseProvider:
				//    ConditionItem dbReplacer = null;

				//    switch (this._database.Provider)
				//    {
				//        case DatabaseProvider.Oracle:
				//            dbReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.DatabaseProvider._Oracle);
				//            break;

				//        case DatabaseProvider.SQLServer:
				//            dbReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.DatabaseProvider.SQLServer);
				//            break;

				//        case DatabaseProvider.SQLite:
				//            dbReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.DatabaseProvider.SQLite);
				//            break;

				//        case DatabaseProvider.SqlCe4:
				//            dbReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.DatabaseProvider.SqlCe4);
				//            break;
				//    }

				//    if (dbReplacer == null)
				//        return "";

				//    // Replace the contents
				//    return Replacer_ConditionItem_AppliesToColumn(dbReplacer.Content, table, column);		

				case PatternConditionKeyMode.FieldsAll:
				case PatternConditionKeyMode.Field:
					var replacer = partialContent.GetFirst();

					// Replace the contents
					return Replacer_ConditionItem_AppliesToColumn(replacer.ContentText, table, column);

				case PatternConditionKeyMode.FieldsCondensedTypeAll:
				case PatternConditionKeyMode.FieldCondensedType:
					ConditionItem replacerCondensedType = null;

					switch (column.DataCondensedType)
					{
						case DbColumn.ColumnCondensedType.None:
							replacerCondensedType = partialContent.GetReplacement(ConditionKeyModeConsts.FieldCondensedType.None);
							break;

						case DbColumn.ColumnCondensedType.String:
							replacerCondensedType = partialContent.GetReplacement(ConditionKeyModeConsts.FieldCondensedType.String);
							break;

						case DbColumn.ColumnCondensedType.Decimal:
							replacerCondensedType = partialContent.GetReplacement(ConditionKeyModeConsts.FieldCondensedType.Decimal);
							break;

						case DbColumn.ColumnCondensedType.Integer:
							replacerCondensedType = partialContent.GetReplacement(ConditionKeyModeConsts.FieldCondensedType.Integer);
							break;
					}

					// Replace the contents
					if (replacerCondensedType != null)
						return Replacer_ConditionItem_AppliesToColumn(replacerCondensedType.ContentText, table, column);
					return string.Empty;

				case PatternConditionKeyMode.FieldsPrimaryKeyAll:
				case PatternConditionKeyMode.FieldPrimaryKey:
					ConditionItem primaryReplacer;

					if (column.PrimaryKey)
						primaryReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.FieldPrimaryKey.PrimaryKey);
					else
						primaryReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.FieldPrimaryKey.NormalField);

					// Replace the contents
					return Replacer_ConditionItem_AppliesToColumn(primaryReplacer.ContentText, table, column);

				case PatternConditionKeyMode.FieldReferencedKeyType:
				case PatternConditionKeyMode.FieldKeyType:
				case PatternConditionKeyMode.FieldsReferencedKeyTypeAll:
				case PatternConditionKeyMode.FieldsKeyTypeAll:
					ConditionItem keyTypeReplacer;

					// this key is not reference type
					if (_patternProject.SeperateReferenceColumns)
					{
						if (column.IsReferenceKey && partialContent.ConditionKeyMode == PatternConditionKeyMode.FieldsKeyTypeAll)
							return "";
						if (!column.IsReferenceKey && partialContent.ConditionKeyMode == PatternConditionKeyMode.FieldsReferencedKeyTypeAll)
							return "";
					}

					// Key type
					bool dataTypeNotNullable = !column.DataTypeNullable;

					if (column.AutoIncrement && column.PrimaryKey)
					{
						keyTypeReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.FieldKeyType.AutoInrcementPrimaryKey);
					}
					else if (column.AutoIncrement && column.AllowNull && !dataTypeNotNullable)
					{
						// AutoIncrement
						// Nullable column
						// Nullable object
						keyTypeReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.FieldKeyType.AutoIncNativeNullable);
					}
					else if (column.AutoIncrement && column.AllowNull && dataTypeNotNullable)
					{
						keyTypeReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.FieldKeyType.AutoIncNullableType);
					}
					else if (column.AutoIncrement)
					{
						keyTypeReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.FieldKeyType.AutoInrcement);
					}
					else if (column.PrimaryKey)
					{
						keyTypeReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.FieldKeyType.PrimaryKey);
					}
					else if (column.AllowNull && !dataTypeNotNullable)
					{
						keyTypeReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.FieldKeyType.NativeNullable);
					}
					else if (column.AllowNull && dataTypeNotNullable)
					{
						keyTypeReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.FieldKeyType.NullableType);
					}
					else
					{
						keyTypeReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.FieldKeyType.NormalField);
					}

					// Replace the contents
					return Replacer_ConditionItem_AppliesToColumn(keyTypeReplacer.ContentText, table, column);


				case PatternConditionKeyMode.FieldsKeyReadTypeAll:
				case PatternConditionKeyMode.FieldKeyReadType:
					ConditionItem keyRead;

					bool canConvert = !column.ExplicitCastDataType; // TODO

					// how to read key type

					if (!column.AllowNull && canConvert)
					{
						keyRead = partialContent.GetReplacement(ConditionKeyModeConsts.FieldKeyReadType.NormalField_Convert);
					}
					else if (!column.AllowNull && !canConvert)
					{
						keyRead = partialContent.GetReplacement(ConditionKeyModeConsts.FieldKeyReadType.NormalField_Cast);
					}
					else if (column.AllowNull && canConvert)
					{
						keyRead = partialContent.GetReplacement(ConditionKeyModeConsts.FieldKeyReadType.Nullable_Convert);
					}
					else
					{
						keyRead = partialContent.GetReplacement(ConditionKeyModeConsts.FieldKeyReadType.Nullable_Cast);
					}
					// Replace the contents
					return Replacer_ConditionItem_AppliesToColumn(keyRead.ContentText, table, column);

				default:
					// Ignored
					return "";
			}
		}

		/// <summary>
		/// Partial content replacer.
		/// </summary>
		string ConditionItem_AppliesToForeignKeyColumns(PatternContent partialContent, DbTable table, DbForeignKey foreignKey)
		{
			switch (partialContent.ConditionKeyMode)
			{
				case PatternConditionKeyMode.FieldsForeignKeyAll:
					ConditionItem theReplacer;

					switch (foreignKey.Multiplicity)
					{
						case DbForeignKey.ForeignKeyMultiplicity.OneToMany:
							theReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.FieldForeignKey.MultiplicityOne);
							break;
						case DbForeignKey.ForeignKeyMultiplicity.ManyToOne:
							theReplacer = partialContent.GetReplacement(ConditionKeyModeConsts.FieldForeignKey.MultiplicityMany);
							break;


						default:
							// not defined Multiplicity
							return string.Empty;
					}

					// Replace the contents
					return Replacer_ConditionItem_AppliesToForeignKey(theReplacer.ContentText, table, foreignKey);

				case PatternConditionKeyMode.ForeignKeyUpdateAction:
					theReplacer = partialContent.GetReplacement(foreignKey.UpdateAction.ToString());
					if (theReplacer == null)
						return string.Empty;
					return Replacer_ConditionItem_AppliesToForeignKey(theReplacer.ContentText, table, foreignKey);

				case PatternConditionKeyMode.ForeignKeyDeleteAction:
					theReplacer = partialContent.GetReplacement(foreignKey.DeleteAction.ToString());
					if (theReplacer == null)
						return string.Empty;
					return Replacer_ConditionItem_AppliesToForeignKey(theReplacer.ContentText, table, foreignKey);

				default:
					// Ignored
					return string.Empty;
			}
		}

		#endregion

		#region Replacers
		private string Replacer_PatternBaseContent(string baseContent, string tableNameSchema, string tableNameDb)
		{
			// Replacements
			if (tableNameSchema != null)
				baseContent = Common.ReplaceEx(baseContent, ReplaceConsts.TableName, tableNameSchema, StringComparison.CurrentCultureIgnoreCase);
			if (tableNameDb != null)
				baseContent = Common.ReplaceEx(baseContent, ReplaceConsts.TableNameDb, tableNameDb, StringComparison.CurrentCultureIgnoreCase);

			// general
			baseContent = Replacer_GeneratorGeneral(baseContent);

			// database provider
			baseContent = Replacer_DatabaseProvider(baseContent);

			return baseContent;
		}

		private string Replacer_GeneratorGeneral(string content)
		{
			content = Common.ReplaceEx(content, ReplaceConsts.Namespace, _projectDef.CodeGenSettings.DefaultNamespace, StringComparison.CurrentCultureIgnoreCase);
			content = Common.ReplaceEx(content, ReplaceConsts.ProjectName, _projectDef.ProjectName, StringComparison.CurrentCultureIgnoreCase);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.DatabaseName, _projectDef.DbSettions.DatabaseName);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.Generator, AppConfig.AppGeneratorSign);
			content = Common.ReplaceExIgnoreCase(content, ReplaceConsts.OperateDate, DateTime.Now.ToString());
			content = Common.ReplaceEx(content, ReplaceConsts.ConnectionString, _projectDef.DbSettions.GetConnectionString(), StringComparison.CurrentCultureIgnoreCase);
			content = Common.ReplaceEx(content, ReplaceConsts.ConnectionStringPwd, _projectDef.DbSettions.SqlPassword, StringComparison.CurrentCultureIgnoreCase);
			content = Common.ReplaceEx(content, ReplaceConsts.ConnectionStringUser, _projectDef.DbSettions.SqlUsername, StringComparison.CurrentCultureIgnoreCase);
			return content;
		}

		/// <summary>
		/// Replaces the database provider class name
		/// </summary>
		private string Replacer_DatabaseProvider(string content)
		{
			content = Common.ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderSpParamPrefix,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.StoredProcParamPrefix));
			content = Common.ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderClassCommand,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.ClassCommand));
			content = Common.ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderClassConnection,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.ClassConnection));
			content = Common.ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderClassDataAdapter,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.ClassDataAdapter));
			content = Common.ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderClassDataReader,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.ClassDataReader));
			content = Common.ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderClassParameter,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.ClassParameter));
			content = Common.ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderClassPrefix,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.ClassPrefix));
			content = Common.ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderClassTransaction,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.ClassTransaction));
			content = Common.ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderClassReferenceName,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.ClassNamespace));
			content = Common.ReplaceExIgnoreCase(content,
						  ReplaceConsts.ProviderAssemblyReference,
						  _schemaEngine.GetDataProviderClassName(DataProviderClassNames.AssemblyReference));

			return content;
		}

		private string Replacer_PatternFileName(string fileName, string tableSchemaName, string tableNameDb, string tableNameSchemaCS)
		{
			fileName = Common.ReplaceEx(fileName, ReplaceConsts.ProjectName, _projectDef.ProjectName, StringComparison.CurrentCultureIgnoreCase);
			fileName = Common.ReplaceEx(fileName, ReplaceConsts.Namespace, _projectDef.CodeGenSettings.DefaultNamespace, StringComparison.CurrentCultureIgnoreCase);
			fileName = Common.ReplaceExIgnoreCase(fileName, ReplaceConsts.DatabaseName, _projectDef.DbSettions.DatabaseName);

			if (tableSchemaName != null)
				fileName = Common.ReplaceEx(fileName, ReplaceConsts.TableName, tableSchemaName, StringComparison.CurrentCultureIgnoreCase);
			if (tableNameDb != null)
				fileName = Common.ReplaceEx(fileName, ReplaceConsts.TableNameDb, tableNameDb, StringComparison.CurrentCultureIgnoreCase);
			if (tableNameSchemaCS != null)
				fileName = Common.ReplaceEx(fileName, ReplaceConsts.TableNameSchemaCS, tableNameSchemaCS, StringComparison.CurrentCultureIgnoreCase);
			return fileName;
		}


		/// <summary>
		/// Get field size
		/// </summary>
		private string FieldType_ColumnDataSize(DbColumn column)
		{
			if (column.Length > 0)
				return column.Length.ToString();
			if (column.DataTypeMaxLength > 0)
				return column.DataTypeMaxLength.ToString();
			return "0";
		}

		/// <summary>
		/// Get field type database full name
		/// </summary>
		private string FieldType_ColumnDataTypeSize(DbColumn column)
		{
			string cleanType = (column.DataTypeDotNet);

			if (cleanType == "String" &&
				column.DataTypeDb.ToLower().IndexOf(_patternProject.LanguageSettings.TextFieldIdenticator) == -1)
				return column.DataTypeDb + "(" + column.Length + ")";

			if (column.DataTypeDb.ToLower() == _patternProject.LanguageSettings.DbDecimalName.ToLower())
			{
				return _patternProject.LanguageSettings.DbDecimalType
					.Replace(ReplaceConsts.Pattern_LanguageSettings_Precision, column.NumericPrecision.ToString())
					.Replace(ReplaceConsts.Pattern_LanguageSettings_Scale, column.NumericScale.ToString());
			}
			else if (column.DataTypeDb.ToLower() == _patternProject.LanguageSettings.DbNumericName.ToLower())
			{
				return _patternProject.LanguageSettings.DbNumericType
					.Replace(ReplaceConsts.Pattern_LanguageSettings_Precision, column.NumericPrecision.ToString())
					.Replace(ReplaceConsts.Pattern_LanguageSettings_Scale, column.NumericScale.ToString());
			}
			return column.DataTypeDb;
		}
		#endregion

	}
}
