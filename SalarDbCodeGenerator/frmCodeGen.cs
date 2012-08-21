using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using SalarDbCodeGenerator.DbProject;
using SalarDbCodeGenerator.GeneratorEngine;
using SalarDbCodeGenerator.Presentation;
using SalarDbCodeGenerator.Schema.Database;
using SalarDbCodeGenerator.Schema.DbSchemaReaders;
using SalarDbCodeGenerator.Schema.Patterns;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Salar Khalilzadeh <salar2k@gmail.com>
// © 2012, All rights reserved
// 2012/07/06
// ====================================
namespace SalarDbCodeGenerator
{
	public partial class frmCodeGen : Form
	{
		public class RecentProjects
		{
			const int MaxItems = 7;
			public StringCollection ProjectsList { get; set; }
			internal ToolStripMenuItem RootManuItem;
			internal EventHandler ReopenClickHandler;

			public RecentProjects() { }
			public RecentProjects(ToolStripMenuItem rootManuItem, EventHandler reopenClickHandler)
			{
				ProjectsList = new StringCollection();
				RootManuItem = rootManuItem;
				ReopenClickHandler = reopenClickHandler;
			}

			private void MenuClearRecentFiles(object sender, EventArgs e)
			{
				ProjectsList.Clear();
				Rebuild();
			}

			public void Rebuild()
			{
				RootManuItem.DropDownItems.Clear();

				ToolStripMenuItem item;
				for (int i = 0; i < ProjectsList.Count; i++)
				{
					string name = Path.GetFileName(ProjectsList[i]);
					item = new ToolStripMenuItem(name, null, ReopenClickHandler);
					item.Tag = ProjectsList[i];
					RootManuItem.DropDownItems.Add(item);
				}

				if (ProjectsList.Count > 0)
				{
					RootManuItem.Enabled = true;
					RootManuItem.DropDownItems.Add(new ToolStripSeparator());
					item = new ToolStripMenuItem("Clear list", null, MenuClearRecentFiles);
					RootManuItem.DropDownItems.Add(item);
				}
				else
				{
					RootManuItem.Enabled = false;
				}
			}

			public static RecentProjects LoadRecentProjects(string xmlFile)
			{
				RecentProjects recentList;
				XmlSerializer loader = new XmlSerializer(typeof(RecentProjects));
				using (StreamReader reader = new StreamReader(xmlFile))
					recentList = (RecentProjects)loader.Deserialize(reader);
				return recentList;
			}

			public static void SaveRecentProjects(RecentProjects recentProjects, string xmlFile)
			{
				XmlSerializer saver = new XmlSerializer(typeof(RecentProjects));
				using (StreamWriter writer = new StreamWriter(xmlFile))
					saver.Serialize(writer, recentProjects);
			}

			public void NewFileOperation(string fileNamePath)
			{
				ProjectsList.Remove(fileNamePath);
				ProjectsList.Insert(0, fileNamePath);
				if (ProjectsList.Count > MaxItems)
				{
					ProjectsList.RemoveAt(ProjectsList.Count - 1);
				}
			}

			public void RemoveFile(string fileNamePath)
			{
				ProjectsList.Remove(fileNamePath);
			}
		}

		public frmCodeGen()
		{
			InitializeComponent();
			this.Icon = global::SalarDbCodeGenerator.Properties.Resources.AppIcon;
		}

		#region local variables
		bool _canFireEvents = false;
		bool _suppressItemChecked = false;
		bool _currentProject_Modified = false;
		bool _currentProject_Opened = false;
		string _currentProject_Filename = "";
		ProjectDefinaton _projectDefinaton;
		PatternProject _patternProject;
		RecentProjects _recentProjects;
		List<PatternProject> _patternProjectsList = new List<PatternProject>();
		#endregion

		#region field variables
		#endregion

		#region properties
		#endregion

		#region public methods
		#endregion

		#region protected methods
		#endregion

		#region private methods
		void InitializeApplication()
		{
			_projectDefinaton = ProjectDefinaton.LoadDefaultProject();
			_patternProject = new PatternProject();
			_recentProjects = new RecentProjects(mnuReopen, mnuReopenRecentFile_Click);
			Reload_ProjectDefinaton();

			// refresh presentation
			Refresh_Form(true);

			// recent projects list
			InitializeRecentMenu();

			// Modified
			SetModified(false);
		}
		void InitializeRecentMenu()
		{
			try
			{
				string recentFiles = AppConfig.RecentProjectsConfig;
				if (File.Exists(recentFiles))
				{
					_recentProjects = RecentProjects.LoadRecentProjects(recentFiles);
				}
				else
					_recentProjects = new RecentProjects(mnuReopen, mnuReopenRecentFile_Click);
				_recentProjects.RootManuItem = mnuReopen;
				_recentProjects.ReopenClickHandler = mnuReopenRecentFile_Click;
				_recentProjects.Rebuild();
			}
			catch { }
		}
		void FinalizeRecentMenu()
		{
			try
			{
				string recentFiles = AppConfig.RecentProjectsConfig;
				RecentProjects.SaveRecentProjects(_recentProjects, recentFiles);
			}
			catch { }
		}

		void ProccessCommandLine()
		{
			if (Program.ProgramArgs != null && Program.ProgramArgs.Length > 0)
			{
				if (File.Exists(Program.ProgramArgs[0]))
				{
					// Open the project
					UiAction_OpenProjectDirect(Program.ProgramArgs[0]);
				}
			}
		}

		/// <summary>
		/// Change modified status
		/// </summary>
		void SetModified(bool modified)
		{
			_currentProject_Modified = modified;
			if (_currentProject_Modified)
				lblModified.Text = "Modified";
			else
				lblModified.Text = "Ready";
		}

		#region Loading data from dataSource
		void Reload_ProjectDefinaton()
		{
			if (_projectDefinaton != null)
				Reload_ProjectDefinaton(
					Common.AppVarPathMakeAbsolute(_projectDefinaton.CodeGenSettings.CodeGenPatternFile));
		}

		void Reload_ProjectDefinaton(string patternProjectFile)
		{
			_patternProject = PatternProject.ReadFromFile(patternProjectFile);

			// realod combo
			Refresh_PatternProjectsCombo();
		}

		void Reload_DatabaseCache()
		{
			PleaseWait.ShowPleaseWait("Connecting to database server", true, false);
			if (_projectDefinaton.DbSettions.TestConnection())
			{
				PleaseWait.WaitingState = "Fetching database info";

				_projectDefinaton.DbSettions.RefetchDatabaseCache();
				_projectDefinaton.DbSettions.LastFetch = DateTime.Now;

				PleaseWait.Abort();
			}
			else
			{
				PleaseWait.Abort();
				MessageBox.Show("Can not connect to the server. Please check database connection settings.", "Database refetch", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region Refreshing presentation data
		void Refresh_Form()
		{
			Refresh_ProjectDefinaton();
			Refresh_PatternsList();
			Refresh_DatabaseCache();
			Refresh_GenerateFiles();
		}

		void Refresh_Form(bool newProject)
		{
			Refresh_PatternProjectsList();

			Refresh_ProjectDefinaton();
			if (newProject)
				Refresh_PatternsList(true);
			else
				Refresh_PatternsList(false);
			Refresh_DatabaseCache();
			Refresh_GenerateFiles();
		}

		void Refresh_DatabaseCache()
		{
			lstTables.Items.Clear();
			_suppressItemChecked = true;
			try
			{
				foreach (var table in _projectDefinaton.DbSettions.Tables)
				{
					ListViewItem listItem = new ListViewItem(table.Name);
					listItem.Checked = table.Selected;
					lstTables.Items.Add(listItem);
				}
			}
			finally
			{
				_suppressItemChecked = false;
			}


			lstViews.Items.Clear();
			_suppressItemChecked = true;
			try
			{
				foreach (var view in _projectDefinaton.DbSettions.Views)
				{
					ListViewItem listItem = new ListViewItem(view.Name);
					listItem.Checked = view.Selected;
					lstViews.Items.Add(listItem);
				}
			}
			finally
			{
				_suppressItemChecked = false;
			}

		}

		void Refresh_ProjectDefinaton()
		{
			if (_projectDefinaton == null)
				return;
			lblInfoProjectName.Text = _projectDefinaton.ProjectName;
			if (_projectDefinaton.LastGeneration == DateTime.MinValue)
				lblInfoLastGeneration.Text = "(None)";
			else
				lblInfoLastGeneration.Text = _projectDefinaton.LastGeneration.ToString();
			lblInfoGenerationPath.Text = _projectDefinaton.GenerationPath;
			lblInfoDbServer.Text = _projectDefinaton.DbSettions.ServerName;
		}

		void Refresh_PatternsList()
		{
			Refresh_PatternsList(false);
		}

		void Refresh_PatternsList(bool applyAllAsSelected)
		{
			if (_patternProject == null)
				return;

			// info
			lblPatternLanguage.Text = _patternProject.Language;
			lblPatternDatabases.Text = _patternProject.SupportedDatabases;
			tipHints.SetToolTip(pnlPatternInfo, _patternProject.Description);
			tipHints.SetToolTip(lblPatternLanguage, _patternProject.Description);
			tipHints.SetToolTip(lblPatternDatabases, _patternProject.Description);

			// SelectedPatterns
			StringCollection selPatterns = _projectDefinaton.CodeGenSettings.SelectedPatterns;

			// Items cehcked
			_suppressItemChecked = true;
			try
			{

				// Clean up
				lstPatterns.Groups.Clear();
				lstPatterns.Items.Clear();
				var actionCopyGroupName = "Copy Action";

				// Load patterns
				List<PatternFile> patternsList = LoadPatternsInfo(_patternProject.PatternFiles);
				var patternCopyActionList =
					_patternProject.PatternFiles.Where(x => x.Action == PatternsListItemAction.Copy).ToList();

				// Groups
				List<string> listGroups = new List<string>();
				foreach (var item in patternsList)
				{
					if (!listGroups.Contains(item.Options.Group))
					{
						ListViewGroup group = new ListViewGroup(item.Options.Group, item.Options.Group);
						lstPatterns.Groups.Add(group);
						listGroups.Add(item.Options.Group);
					}
				}
				if (patternCopyActionList.Count > 0)
				{
					if (!listGroups.Contains(actionCopyGroupName))
					{
						var group = new ListViewGroup(actionCopyGroupName, actionCopyGroupName);
						lstPatterns.Groups.Add(group);
						listGroups.Add(actionCopyGroupName);
					}
				}

				// clear all if others should be selected
				if (applyAllAsSelected)
				{
					selPatterns.Clear();
				}

				// Items
				foreach (var item in patternsList)
				{
					var listItem = new ListViewItem();
					if (applyAllAsSelected)
					{
						listItem.Checked = true;
						selPatterns.Add(item.Name);
					}
					else
					{
						if (selPatterns.Contains(item.Name))
							listItem.Checked = true;
						else
						{
							listItem.Checked = false;
						}
					}
					listItem.Text = item.Name;
					listItem.ToolTipText = item.Description + "\n" + item.ToSummaryString();
					listItem.Group = lstPatterns.Groups[item.Options.Group];
					lstPatterns.Items.Add(listItem);
				}

				foreach (var file in patternCopyActionList)
				{
					var listItem = new ListViewItem();
					var fileName = Path.GetFileName(file.Path);
					if (applyAllAsSelected)
					{
						listItem.Checked = true;
						selPatterns.Add(fileName);
					}
					else
					{
						if (selPatterns.Contains(fileName))
							listItem.Checked = true;
						else
						{
							listItem.Checked = false;
						}
					}
					listItem.Text = fileName;
					listItem.ToolTipText = "Copy to: " + file.ActionCopyPath;
					listItem.Group = lstPatterns.Groups[actionCopyGroupName];
					lstPatterns.Items.Add(listItem);
				}


			}
			finally
			{
				_suppressItemChecked = false;
			}
		}

		void Refresh_GenerateFiles()
		{
			if (_projectDefinaton == null)
				return;

			var generationPath = _projectDefinaton.GenerationPath;
			generationPath = Common.ProjectPathMakeAbsolute(generationPath, _projectDefinaton.ProjectFileName);

			if (Directory.Exists(generationPath) == false)
			{
				treGenFiles.Nodes.Clear();
				return;
			}

			treGenFiles.Nodes.Clear();
			TreeNode baseNode = new TreeNode
			{
				Text = Path.GetFileName(Path.GetDirectoryName(Path.Combine(generationPath, "temp"))),
				ToolTipText = _projectDefinaton.GenerationPath,
				ImageKey = "Folder",
				SelectedImageKey = "Folder",
				Tag = "folder"
			};
			treGenFiles.Nodes.Add(baseNode);
			AddChildNodes(baseNode, generationPath);
		}

		/// <summary>
		/// Reads pattern projects list
		/// </summary>
		void Refresh_PatternProjectsList()
		{
			string patPrjExt = "*" + AppConfig.PatternProjectExtension;
			try
			{
				string[] projectFiles = Directory.GetFiles(AppConfig.PatternProjectsDirectory, patPrjExt, SearchOption.AllDirectories);

				_patternProjectsList.Clear();
				foreach (var project in projectFiles)
				{
					try
					{
						// load the pattern
						PatternProject patPrj = PatternProject.ReadFromFile(project);

						// add to list
						_patternProjectsList.Add(patPrj);
					}
					catch (Exception)
					{
						// failed!
						// TODO: notify user about failed pattern
					}
				}

				// sort patterns list
				_patternProjectsList.Sort((x, y) => string.Compare(x.Name, y.Name));

				//_patternProjectsList.Sort(new Comparison<PatternProject>((x, y) => string.Compare(x.Name, y.Name)));

				//// sort patterns list
				//_patternProjectsList.Sort(new Comparison<PatternProject>(delegate(PatternProject x, PatternProject y)
				//{
				//    return string.Compare(x.Name, y.Name);
				//}));

				// pattern projects
				Refresh_PatternProjectsCombo();
			}
			catch (Exception ex)
			{
				throw new Exception("Failed to read pattern projects list.", ex);
			}
		}

		void Refresh_PatternProjectsCombo()
		{
			if (_patternProjectsList == null || _patternProjectsList.Count == 0)
			{
				//cmbPatternProjectFile.Items.Clear();
				return;
			}

			// display
			cmbPatternProjectFile.DisplayMember = "Name";
			cmbPatternProjectFile.DataSource = _patternProjectsList;

			// selected project
			if (_patternProject != null && _projectDefinaton != null)
			{
				bool found = false;
				_patternProjectsList.ForEach(x =>
				{
					if (x.Equals(_patternProject))
					{
						cmbPatternProjectFile.SelectedItem = x;
						cmbPatternProjectFile.Text = x.Name;
						found = true;
					}
				});

				if (!found)
				{
					bool fireOrgValue = _canFireEvents;
					_canFireEvents = false;
					try
					{
						// add to cache list
						_patternProjectsList.Insert(0, _patternProject);

						// reset items
						cmbPatternProjectFile.DataSource = null;

						// reset data source
						cmbPatternProjectFile.DisplayMember = "Name";
						cmbPatternProjectFile.DataSource = _patternProjectsList;

						// set selected item
						cmbPatternProjectFile.SelectedItem = _patternProject;
						cmbPatternProjectFile.Text = _patternProject.Name;
					}
					finally
					{
						// reset the value
						_canFireEvents = fireOrgValue;
					}

				}
			}
		}

		#endregion

		private List<PatternFile> LoadPatternsInfo(List<PatternProject.PatternFile> patternsList)
		{
			List<PatternFile> result = new List<PatternFile>();
			string patternsFolder = Path.GetDirectoryName(
				Common.AppVarPathMakeAbsolute(_projectDefinaton.CodeGenSettings.CodeGenPatternFile));

			foreach (var pattern in patternsList)
			{
				if (pattern.Action == PatternsListItemAction.Generate)
				{
					PatternFile patternFile = PatternFile.ReadFromFile(Path.Combine(patternsFolder, pattern.Path));
					result.Add(patternFile);
				}
			}
			return result;
		}

		private void AddFiles(TreeNode parentNode, string[] files)
		{
			TreeNode node;
			foreach (var f in files)
			{
				node = new TreeNode();
				node.Text = Path.GetFileName(f);
				node.ToolTipText = Path.GetDirectoryName(f);
				if (Path.GetExtension(f).ToLower() == ".cs")
				{
					node.ImageKey = "FileCS";
					node.SelectedImageKey = "FileCS";
				}
				else if (Path.GetExtension(f).ToLower() == ".vb")
				{
					node.ImageKey = "FileVB";
					node.SelectedImageKey = "FileVB";
				}
				else
				{
					node.ImageKey = "File";
					node.SelectedImageKey = "File";
				}
				node.Tag = "file";
				parentNode.Nodes.Add(node);
			}
		}

		private int AddChildNodes(TreeNode parentNode, string directory)
		{
			string codeFilePattern = String.Format("*{0}", _patternProject.FileExtension);

			int filesCount = 0;


			string[] folders = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);

			TreeNode node;
			foreach (var dir in folders)
			{
				node = new TreeNode();
				node.Text = Path.GetFileName(dir);
				node.ToolTipText = Path.GetDirectoryName(dir);
				node.ImageKey = "Folder";
				node.SelectedImageKey = "Folder";
				node.Tag = "folder";

				// add child nodes
				if (AddChildNodes(node, dir) > 0)
					parentNode.Nodes.Add(node);
			}


			string[] files;
			//files = Directory.GetFiles(directory, codeFilePattern, SearchOption.TopDirectoryOnly);
			//AddFiles(parentNode, files);
			//filesCount += files.Length;

			files = Directory.GetFiles(directory, "*.cs", SearchOption.TopDirectoryOnly);
			AddFiles(parentNode, files);
			filesCount += files.Length;

			files = Directory.GetFiles(directory, "*.vb", SearchOption.TopDirectoryOnly);
			AddFiles(parentNode, files);
			filesCount += files.Length;

			files = Directory.GetFiles(directory, "*.config", SearchOption.TopDirectoryOnly);
			AddFiles(parentNode, files);
			filesCount += files.Length;

			files = Directory.GetFiles(directory, "*.csproj", SearchOption.TopDirectoryOnly);
			AddFiles(parentNode, files);
			filesCount += files.Length;

			files = Directory.GetFiles(directory, "*.vbproj", SearchOption.TopDirectoryOnly);
			AddFiles(parentNode, files);
			filesCount += files.Length;

			return filesCount;
		}

		void UiAction_ProjectOptions()
		{
			using (var frm = new frmProjectDetails(frmProjectDetails.FormEditMode.Edit))
			{
				frm.ProjectInstance = (ProjectDefinaton)_projectDefinaton.Clone();
				if (frm.ShowDialog() == DialogResult.OK)
				{
					_projectDefinaton = frm.ProjectInstance;
					Refresh_Form();

					SetModified(true);
				}
			}
		}
		void UiAction_NewProject()
		{
			using (var frmNew = new frmProjectDetails(frmProjectDetails.FormEditMode.New))
			{
				frmNew.ProjectInstance = ProjectDefinaton.LoadDefaultProject();
				if (frmNew.ShowDialog() == DialogResult.OK)
				{
					_projectDefinaton = frmNew.ProjectInstance;
					_patternProject = PatternProject.ReadFromFile(
						Common.AppVarPathMakeAbsolute(_projectDefinaton.CodeGenSettings.CodeGenPatternFile));
					Refresh_Form(true);


					SetModified(true);
					_currentProject_Opened = false;
					// Reload everything
				}
			}
		}
		void UiAction_OpenProjectDirect(string projectFile)
		{
			if (!File.Exists(projectFile))
			{
				if (MessageBox.Show("Project file does not exists:\n" + projectFile + "\n\nWould you like to remove it from recent list?", "Open project", MessageBoxButtons.YesNo,
								MessageBoxIcon.Stop) == DialogResult.Yes)
				{
					_recentProjects.RemoveFile(projectFile);
					_recentProjects.Rebuild();
				}
				return;
			}

			try
			{
				ProjectDefinaton openProject = ProjectDefinaton.LoadFromFile(projectFile);

				// Load patterns first
				try
				{
					Reload_ProjectDefinaton(
						Common.AppVarPathMakeAbsolute(openProject.CodeGenSettings.CodeGenPatternFile));
				}
				catch
				{
					if (MessageBox.Show("Failed to open the pattern project from selected project!\nWould you like ignore this error and continue?", "Open project", MessageBoxButtons.YesNo, MessageBoxIcon.Stop)
						== DialogResult.No)
					{
						return;
					}
					// continue to read others!
				}

				// assign new project
				_projectDefinaton = openProject;

				// refresh form
				Refresh_Form();

				// save the file name
				_currentProject_Filename = projectFile;
				_currentProject_Opened = true;

				// menu list!
				_recentProjects.NewFileOperation(projectFile);
				_recentProjects.Rebuild();

				// The project is fresh
				SetModified(false);
			}
			catch (Exception)
			{
				MessageBox.Show("Failed to open the selected project!", "Open project", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
		}

		void UiAction_OpenProject()
		{
			if (dlgOpenProject.ShowDialog() == DialogResult.OK)
			{
				UiAction_OpenProjectDirect(dlgOpenProject.FileName);
			}
		}

		bool UiAction_SaveProject()
		{
			if (!_currentProject_Opened)
			{
				dlgSaveProject.Title = "Save Project";
				if (dlgSaveProject.ShowDialog() == DialogResult.OK)
				{
					string saveProect = dlgSaveProject.FileName;
					ProjectDefinaton.SaveToFile(_projectDefinaton, saveProect);

					_currentProject_Filename = saveProect;
					_currentProject_Opened = true;

					Refresh_ProjectDefinaton();
					SetModified(false);

					// menu list!
					_recentProjects.NewFileOperation(saveProect);
					_recentProjects.Rebuild();


					return true;
				}
				else
					return false;
			}
			else
			{
				ProjectDefinaton.SaveToFile(_projectDefinaton, _currentProject_Filename);
				SetModified(false);
				return true;
			}

		}
		void UiAction_SaveAsProject()
		{
			dlgSaveProject.Title = "Save Project As";
			if (dlgSaveProject.ShowDialog() == DialogResult.OK)
			{
				string saveProect = dlgSaveProject.FileName;
				ProjectDefinaton.SaveToFile(_projectDefinaton, saveProect);
				_currentProject_Filename = saveProect;
				_currentProject_Opened = true;

				Refresh_ProjectDefinaton();

				SetModified(false);

				_recentProjects.NewFileOperation(saveProect);
				_recentProjects.Rebuild();
			}
		}
		void UiAction_SelectNewPattern()
		{
			var codeGenFile = Common.AppVarPathMakeAbsolute(_projectDefinaton.CodeGenSettings.CodeGenPatternFile);
			dlgOpenPattern.FileName = codeGenFile;
			dlgOpenPattern.InitialDirectory = Path.GetDirectoryName(codeGenFile);
			if (dlgOpenPattern.ShowDialog() == DialogResult.OK)
			{
				string projectFile = dlgOpenPattern.FileName;
				try
				{
					_patternProject = PatternProject.ReadFromFile(projectFile);

					var prjRelativePath = Common.AppVarPathMakeRelative(projectFile);
					_projectDefinaton.CodeGenSettings.CodeGenPatternFile = prjRelativePath;

					Refresh_PatternProjectsCombo();

					SetModified(true);
				}
				catch (Exception)
				{
					MessageBox.Show("Failed to open the selected pattern!", "Pattern project", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				}

				Refresh_PatternsList(true);
			}
		}
		void UiAction_ChangePatternFromCombo()
		{
			if (cmbPatternProjectFile.SelectedIndex >= 0)
			{
				PatternProject prjPattern = (PatternProject)cmbPatternProjectFile.SelectedItem;
				// if they are same
				if (_patternProject.Equals(prjPattern))
					return;

				if (MessageBox.Show(string.Format("Do you want to change the pattern project to '{0}'?", prjPattern.Name), "Pattern Project", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					_patternProject = prjPattern;
					var patFile = Common.AppVarPathMakeRelative(_patternProject.PatternFileName);
					_projectDefinaton.CodeGenSettings.CodeGenPatternFile = patFile;

					SetModified(true);

					// reload pattern
					Refresh_PatternsList(true);
				}
				else
				{
					cmbPatternProjectFile.Text = _patternProject.Name;
					cmbPatternProjectFile.SelectedItem = _patternProject;
				}
			}
		}
		void UiAction_Refetch()
		{
			Reload_DatabaseCache();
			Refresh_DatabaseCache();

			SetModified(true);
		}

		private void UiAction_RefreshGeneratedFiles()
		{
			Refresh_GenerateFiles();
		}
		private void UiAction_About()
		{
			using (frmAbout frm = new frmAbout())
				frm.ShowDialog();
		}
		private void UiAction_Generate()
		{
			PleaseWait.ShowPleaseWait("Connecting to database server", true, false);
			using (System.Data.Common.DbConnection conn = _projectDefinaton.DbSettions.GetNewConnection())
			using (ExSchemaEngine schemaEngine = _projectDefinaton.DbSettions.GetSchemaEngine(conn))
			{
				// Connection to database
				conn.Open();

				// Reading database
				PleaseWait.WaitingState = "Reading database schema";

				// 1 ==========================
				// Database schema reader
				var schemaDatabase = new DbDatabase();
				schemaDatabase.Provider = _projectDefinaton.DbSettions.DatabaseProvider;

				// shcema engine options
				schemaEngine.SpecificOwner = _projectDefinaton.DbSettions.SqlUsername;

				// columns descriptions
				schemaEngine.ReadColumnsDescription = _projectDefinaton.CodeGenSettings.GenerateColumnsDescription;
				schemaEngine.ReadTablesForeignKeys = _projectDefinaton.CodeGenSettings.GenerateTablesForeignKeys;
				schemaEngine.ReadConstraintKeys = _projectDefinaton.CodeGenSettings.GenerateConstraintKeys;

				// only selected tables
				schemaEngine.OnlyReadSelectedItems = true;
				schemaEngine.SelectedTables = _projectDefinaton.DbSettions.GetSelectedTablesList();
				schemaEngine.SelectedViews = _projectDefinaton.DbSettions.GetSelectedViewsList();

				// read database schema
				schemaEngine.FillSchema(schemaDatabase);

				PleaseWait.WaitingState = "Analyzing database schema";
				// 2 ======================
				var alanyzer = new SchemaAnalyzer(_projectDefinaton, _patternProject, schemaDatabase);
				alanyzer.AnalyzeAndRename();

				PleaseWait.WaitingState = "Generating output files";
				// 3 ==========================
				// Start the generator
				var engine = new Generator(_projectDefinaton, _patternProject, schemaDatabase, schemaEngine);
				engine.Generate();

				// Update last generation
				_projectDefinaton.LastGeneration = DateTime.Now;

				// 4 ==========================
				// Reaload the form
				Refresh_Form();

				// Abort the please wait
				PleaseWait.Abort();

				// Data is modified
				SetModified(true);

				// to active tab
				tabMainProject.SelectedTab = tabGenFiles;
			}
		}
		#endregion

		#region form events
		private void frmCodeGen_Load(object sender, EventArgs e)
		{
			InitializeApplication();
		}
		private void frmCodeGen_Shown(object sender, EventArgs e)
		{
			this.Text += " " + AppConfig.AppVersionFull;
			_canFireEvents = true;

			try
			{
				if (AppConfig.IsFilesAssociated() == false)
				{
					AppConfig.AssociateApplicationFiles();
					BrendanGrant.Helpers.FileAssociation.ShellNotification.NotifyOfChange();
				}
			}
			catch
			{
				// ignore the shit!
				MessageBox.Show("Failed to associate application files. \nTo associate application files run the application with administrator rights.", "Failed to associate", MessageBoxButtons.OK, MessageBoxIcon.Error);
				// and continue
			}

			// Command line
			ProccessCommandLine();

			PleaseWait.Abort();
			this.Activate();
			this.Focus();
		}

		private void mnuExit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void mnuReopenRecentFile_Click(object sender, EventArgs e)
		{
			var menu = (ToolStripMenuItem)sender;
			UiAction_OpenProjectDirect(menu.Tag.ToString());
		}


		private void lnkGenerationPath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (_projectDefinaton == null)
				return;

			var generationPath = _projectDefinaton.GenerationPath;
			generationPath = Common.ProjectPathMakeAbsolute(generationPath, _projectDefinaton.ProjectFileName);

			try
			{
				var info = new ProcessStartInfo(generationPath);
				info.UseShellExecute = true;
				Process.Start(info);
			}
			catch
			{
				MessageBox.Show("Failed to open the output location:\n" + generationPath, "Output path", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
		}

		private void btnNewProject_Click(object sender, EventArgs e)
		{
			UiAction_NewProject();
		}

		private void btnChangePattern_Click(object sender, EventArgs e)
		{
			UiAction_SelectNewPattern();
		}

		private void btnOpenProject_Click(object sender, EventArgs e)
		{
			UiAction_OpenProject();
		}

		private void btnRefetchDatabase_Click(object sender, EventArgs e)
		{
			UiAction_Refetch();
		}

		private void btnSaveProject_Click(object sender, EventArgs e)
		{
			UiAction_SaveProject();
		}

		private void btnSaveAs_Click(object sender, EventArgs e)
		{
			UiAction_SaveAsProject();
		}

		private void mnuOptions_Click(object sender, EventArgs e)
		{
			UiAction_ProjectOptions();
		}

		private void btnProjectOptions_Click(object sender, EventArgs e)
		{
			UiAction_ProjectOptions();
		}

		private void mnuRefetch_Click(object sender, EventArgs e)
		{
			UiAction_Refetch();
		}

		private void mnuNew_Click(object sender, EventArgs e)
		{
			UiAction_NewProject();
		}

		private void mnuOpen_Click(object sender, EventArgs e)
		{
			UiAction_OpenProject();
		}

		private void mnuSave_Click(object sender, EventArgs e)
		{
			UiAction_SaveProject();
		}

		private void mnuSaveAs_Click(object sender, EventArgs e)
		{
			UiAction_SaveAsProject();
		}

		private void btnRefrshGenFiles_Click(object sender, EventArgs e)
		{
			UiAction_RefreshGeneratedFiles();
		}

		private void cmbPatternProjectFile_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_canFireEvents) return;
			UiAction_ChangePatternFromCombo();
		}

		private void treGenFiles_AfterSelect(object sender, TreeViewEventArgs e)
		{
			var tag = (string)e.Node.Tag;
			try
			{
				if (tag == "file")
				{
					string fileName = Path.Combine(e.Node.ToolTipText, e.Node.Text);

					txtCodeEditor.LoadFile(fileName);
					txtCodeEditor.IsReadOnly = true;
				}
			}
			catch
			{
				txtCodeEditor.Text = "Failed to read file content!";
			}
		}

		private void lstPatterns_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			if (!_canFireEvents) return;
			if (_suppressItemChecked) return;

			if (_patternProject != null && _projectDefinaton != null)
			{
				string patternName = e.Item.Text;
				if (e.Item.Checked)
				{
					if (!_projectDefinaton.CodeGenSettings.SelectedPatterns.Contains(patternName))
						_projectDefinaton.CodeGenSettings.SelectedPatterns.Add(patternName);
				}
				else
				{
					_projectDefinaton.CodeGenSettings.SelectedPatterns.Remove(patternName);
				}
			}
		}

		private void lstViews_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			if (!_canFireEvents) return;
			if (_suppressItemChecked) return;

			if (_projectDefinaton != null)
			{
				string itemName = e.Item.Text;
				for (int i = 0; i < _projectDefinaton.DbSettions.Views.Count; i++)
				{
					ProjectDbSettions.SelectedTablesType view = _projectDefinaton.DbSettions.Views[i];
					if (view.Name == itemName)
					{
						// apply check
						view.Selected = e.Item.Checked;

						// replace by new changes
						_projectDefinaton.DbSettions.Views[i] = view;

						// done!
						break;
					}
				}
			}
		}

		private void lstTables_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			if (!_canFireEvents) return;
			if (_suppressItemChecked) return;

			if (_projectDefinaton != null)
			{
				string itemName = e.Item.Text;
				for (int i = 0; i < _projectDefinaton.DbSettions.Tables.Count; i++)
				{
					ProjectDbSettions.SelectedTablesType table = _projectDefinaton.DbSettions.Tables[i];
					if (table.Name == itemName)
					{
						// apply check
						table.Selected = e.Item.Checked;

						// replace by new changes
						_projectDefinaton.DbSettions.Tables[i] = table;

						// done!
						break;
					}
				}
			}
		}

		private void mnuAbout_Click(object sender, EventArgs e)
		{
			UiAction_About();
		}


		private void btnStartGeneration_Click(object sender, EventArgs e)
		{
			UiAction_Generate();
		}

		private void mnuStartGenerate_Click(object sender, EventArgs e)
		{
			UiAction_Generate();
		}

		private void frmCodeGen_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_currentProject_Modified)
			{
				DialogResult closeBox = MessageBox.Show("You have unsaved changes, would you like to save the project?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (closeBox == DialogResult.Yes)
				{
					if (UiAction_SaveProject())
					{
						// continue to close
					}
					else
					{
						// canceled
						e.Cancel = true;
					}
				}
				else if (closeBox == DialogResult.Cancel)
				{
					e.Cancel = true;
				}
				else if (closeBox == DialogResult.No)
				{
					// nothing!
				}
			}
		}

		private void frmCodeGen_FormClosed(object sender, FormClosedEventArgs e)
		{
			FinalizeRecentMenu();
		}

		private void lstPatterns_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		#endregion


	}
}
