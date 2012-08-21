using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using SalarDbCodeGenerator.DbProject;
using SalarDbCodeGenerator.Properties;
using SalarDbCodeGenerator.Schema.DbSchemaReaders;

namespace SalarDbCodeGenerator.Presentation
{
	public partial class frmProjectDetails : frmBase
	{
		private bool _formExpanded;
		public enum FormEditMode
		{
			New,
			Edit
		}

		private struct RenamingComboboxItem
		{
			public string Name { get; set; }
			public ProjectRenaming.CaseChangeType CaseType { get; set; }
		}

		public frmProjectDetails()
		{
			InitializeComponent();
		}
		public frmProjectDetails(FormEditMode editmode)
			: this()
		{
			EditMode = editmode;
		}


		public FormEditMode EditMode { get; set; }
		public ProjectDefinaton ProjectInstance { get; set; }

		private void InitializeProjectModel()
		{
			if (ProjectInstance == null)
			{
				// load default settings
				ProjectInstance = ProjectDefinaton.LoadDefaultProject();
			}

			ProjectInstance.ProjectName = txtProjectName.Text;
			ProjectInstance.CodeGenSettings.DefaultNamespace = txtNamespace.Text;
			ProjectInstance.GenerationPath = txtGenerationPath.Text;
			ProjectInstance.CodeGenSettings.CodeGenPatternFile = txtPatternfile.Text;

			ProjectInstance.CodeGenSettings.GenerateTablesForeignKeys = chkGenForeignKey.Checked;
			ProjectInstance.CodeGenSettings.GenerateColumnsDescription = chkGenDescription.Checked;

			ProjectInstance.CodeGenSettings.GenerateConstraintKeys = chkTableConstraintKeys.Checked;

			ProjectInstance.RenamingOptions.UnderlineWordDelimiter = chkRenUnderlineWordDelimiter.Checked;
			ProjectInstance.RenamingOptions.RemoveUnderline.Enabled = chkRenRemUnderline.Checked;
			ProjectInstance.RenamingOptions.RemoveUnderline.Tables = chkRenUnderlineTables.Checked;
			ProjectInstance.RenamingOptions.RemoveUnderline.Properties = chkRenUnderlineProps.Checked;

			ProjectInstance.RenamingOptions.CaseChange.Enabled = chkRenamingCase.Checked;
			ProjectInstance.RenamingOptions.CaseChange.Tables = chkRenCaseTables.Checked;
			ProjectInstance.RenamingOptions.CaseChange.Properties = chkRenCaseProps.Checked;
			ProjectInstance.RenamingOptions.CaseChangeMode = (ProjectRenaming.CaseChangeType)cmbRenamingCase.SelectedValue;

			if (pagerDatabaseProvider.SelectedTab == tabSQLServer)
			{
				ProjectInstance.DbSettions.ServerName = txtSqlHost.Text;
				ProjectInstance.DbSettions.DatabaseName = txtSqlDbName.Text;
				ProjectInstance.DbSettions.SqlUsername = txtSqlUsername.Text;
				ProjectInstance.DbSettions.SqlPassword = txtSqlPassword.Text;
				ProjectInstance.DbSettions.UseSqlAuthentication = rbtnSqlAuthentication.Checked;
				ProjectInstance.DbSettions.ConnectTimeout = Convert.ToInt32(txtSqlConnectTimeout.Text);
				ProjectInstance.DbSettions.DatabaseProvider = DatabaseProvider.SQLServer;
			}
			else if (pagerDatabaseProvider.SelectedTab == tabSQLite)
			{
				ProjectInstance.DbSettions.ServerName = txtSQLiteDatabaseName.Text;
				try
				{
					ProjectInstance.DbSettions.DatabaseName = Path.GetFileNameWithoutExtension(txtSQLiteDatabaseName.Text);
				}
				catch { }
				ProjectInstance.DbSettions.SqlUsername = "";
				ProjectInstance.DbSettions.SqlPassword = txtSQLitePassword.Text;
				ProjectInstance.DbSettions.UseSqlAuthentication = false;
				ProjectInstance.DbSettions.ConnectTimeout = Convert.ToInt32(txtSQLiteConnectTimeout.Text);
				ProjectInstance.DbSettions.DatabaseProvider = DatabaseProvider.SQLite;
			}
			else if (pagerDatabaseProvider.SelectedTab == tabSqlCe)
			{
				ProjectInstance.DbSettions.ServerName = txtSqlCeDatabaseName.Text;
				try
				{
					ProjectInstance.DbSettions.DatabaseName = Path.GetFileNameWithoutExtension(txtSqlCeDatabaseName.Text);
				}
				catch { }
				ProjectInstance.DbSettions.SqlUsername = "";
				ProjectInstance.DbSettions.SqlPassword = txtSqlCePassword.Text;
				if (string.IsNullOrEmpty(txtSqlCePassword.Text))
					ProjectInstance.DbSettions.UseSqlAuthentication = false;
				else
					ProjectInstance.DbSettions.UseSqlAuthentication = true;

				ProjectInstance.DbSettions.ConnectTimeout = -1;
				ProjectInstance.DbSettions.DatabaseProvider = DatabaseProvider.SqlCe4;
			}
			else if (pagerDatabaseProvider.SelectedTab == tabOracle)
			{
				ProjectInstance.DbSettions.ServerName = txtOrclDataSource.Text;
				ProjectInstance.DbSettions.ConnectTimeout = Convert.ToInt32(txtOrclConnectTimeout.Text);

				ProjectInstance.DbSettions.UseSqlAuthentication = rbtnOrclSpecificUsernamePass.Checked;

				ProjectInstance.DbSettions.DatabaseName = txtOrclDbName.Text;
				ProjectInstance.DbSettions.SqlUsername = txtOrclUsername.Text;
				ProjectInstance.DbSettions.SqlPassword = txtOrclPassword.Text;
				ProjectInstance.DbSettions.OracleUseSysdbaRole = chkOrclUserRoleSYSDBA.Checked;

				ProjectInstance.DbSettions.DatabaseProvider = DatabaseProvider.Oracle;
			}

			ProjectInstance.DbSettions.SuffixForTables = txtSuffixForTables.Text;
			ProjectInstance.DbSettions.SuffixForViews = txtSuffixForViews.Text;

			ProjectInstance.DbSettions.PrefixForTables = txtPrefixForTables.Text;
			ProjectInstance.DbSettions.PrefixForViews = txtPrefixForViews.Text;

			ProjectInstance.DbSettions.IgnoredPrefixes.Clear();
			foreach (var item in lstIgnoredPrefixes.Items)
				ProjectInstance.DbSettions.IgnoredPrefixes.Add(item.ToString());

			ProjectInstance.DbSettions.IgnoredSuffixes.Clear();
			foreach (var item in lstIgnoredSuffixes.Items)
				ProjectInstance.DbSettions.IgnoredSuffixes.Add(item.ToString());
		}

		void InitializeForm()
		{
			InitializeCaseChangeMode();
			if (ProjectInstance == null)
				return;
			txtProjectName.Text = ProjectInstance.ProjectName;
			txtNamespace.Text = ProjectInstance.CodeGenSettings.DefaultNamespace;
			txtGenerationPath.Text = ProjectInstance.GenerationPath;
			txtPatternfile.Text = ProjectInstance.CodeGenSettings.CodeGenPatternFile;

			chkGenForeignKey.Checked = ProjectInstance.CodeGenSettings.GenerateTablesForeignKeys;
			chkGenDescription.Checked = ProjectInstance.CodeGenSettings.GenerateColumnsDescription;

			chkTableConstraintKeys.Checked = ProjectInstance.CodeGenSettings.GenerateConstraintKeys;

			chkRenUnderlineWordDelimiter.Checked = ProjectInstance.RenamingOptions.UnderlineWordDelimiter;
			chkRenRemUnderline.Checked = ProjectInstance.RenamingOptions.RemoveUnderline.Enabled;
			chkRenUnderlineTables.Checked = ProjectInstance.RenamingOptions.RemoveUnderline.Tables;
			chkRenUnderlineProps.Checked = ProjectInstance.RenamingOptions.RemoveUnderline.Properties;

			chkRenamingCase.Checked = ProjectInstance.RenamingOptions.CaseChange.Enabled;
			chkRenCaseTables.Checked = ProjectInstance.RenamingOptions.CaseChange.Tables;
			chkRenCaseProps.Checked = ProjectInstance.RenamingOptions.CaseChange.Properties;
			cmbRenamingCase.SelectedValue = ProjectInstance.RenamingOptions.CaseChangeMode;

			if (ProjectInstance.DbSettions.DatabaseProvider == DatabaseProvider.SQLServer)
			{
				pagerDatabaseProvider.SelectedTab = tabSQLServer;

				txtSqlHost.Text = ProjectInstance.DbSettions.ServerName;
				txtSqlDbName.Text = ProjectInstance.DbSettions.DatabaseName;
				txtSqlConnectTimeout.Text = ProjectInstance.DbSettions.ConnectTimeout.ToString();
				txtSqlUsername.Text = ProjectInstance.DbSettions.SqlUsername;
				txtSqlPassword.Text = ProjectInstance.DbSettions.SqlPassword;
				rbtnSqlAuthentication.Checked = ProjectInstance.DbSettions.UseSqlAuthentication;
				rbtnSqlWindowsAuthentication.Checked = !ProjectInstance.DbSettions.UseSqlAuthentication;
			}
			else if (ProjectInstance.DbSettions.DatabaseProvider == DatabaseProvider.SQLite)
			{
				pagerDatabaseProvider.SelectedTab = tabSQLite;

				txtSQLiteDatabaseName.Text = ProjectInstance.DbSettions.ServerName;
				txtSQLitePassword.Text = ProjectInstance.DbSettions.SqlPassword;
				txtSQLiteConnectTimeout.Text = ProjectInstance.DbSettions.ConnectTimeout.ToString();
			}
			else if (ProjectInstance.DbSettions.DatabaseProvider == DatabaseProvider.SqlCe4)
			{
				pagerDatabaseProvider.SelectedTab = tabSqlCe;

				txtSqlCeDatabaseName.Text = ProjectInstance.DbSettions.ServerName;
				txtSqlCePassword.Text = ProjectInstance.DbSettions.SqlPassword;
			}
			else if (ProjectInstance.DbSettions.DatabaseProvider == DatabaseProvider.Oracle)
			{
				pagerDatabaseProvider.SelectedTab = tabOracle;

				txtOrclDataSource.Text = ProjectInstance.DbSettions.ServerName;
				txtOrclConnectTimeout.Text = ProjectInstance.DbSettions.ConnectTimeout.ToString();

				rbtnOrclSpecificUsernamePass.Checked = ProjectInstance.DbSettions.UseSqlAuthentication;
				rbtnOrclWindowsAuthentication.Checked = !ProjectInstance.DbSettions.UseSqlAuthentication;

				txtOrclDbName.Text = ProjectInstance.DbSettions.DatabaseName;
				txtOrclUsername.Text = ProjectInstance.DbSettions.SqlUsername;
				txtOrclPassword.Text = ProjectInstance.DbSettions.SqlPassword;
				chkOrclUserRoleSYSDBA.Checked = ProjectInstance.DbSettions.OracleUseSysdbaRole;
			}


			txtSuffixForTables.Text = ProjectInstance.DbSettions.SuffixForTables;
			txtSuffixForViews.Text = ProjectInstance.DbSettions.SuffixForViews;

			txtPrefixForTables.Text = ProjectInstance.DbSettions.PrefixForTables;
			txtPrefixForViews.Text = ProjectInstance.DbSettions.PrefixForViews;

			lstIgnoredPrefixes.Items.Clear();
			foreach (var item in ProjectInstance.DbSettions.IgnoredPrefixes)
				lstIgnoredPrefixes.Items.Add(item);

			lstIgnoredSuffixes.Items.Clear();
			foreach (var item in ProjectInstance.DbSettions.IgnoredSuffixes)
				lstIgnoredSuffixes.Items.Add(item);
		}

		private void InitializeCaseChangeMode()
		{
			cmbRenamingCase.Items.Clear();
			var data = new List<RenamingComboboxItem>
			    {
					new RenamingComboboxItem
					{
						CaseType= ProjectRenaming.CaseChangeType.Capitalize,
						Name="Capitalize"
					},
					new RenamingComboboxItem
					{
						CaseType= ProjectRenaming.CaseChangeType.CamelCase,
						Name="camelCase"
					},
					new RenamingComboboxItem
					{
						CaseType= ProjectRenaming.CaseChangeType.PascalCase,
						Name="PascalCase"
					},

					new RenamingComboboxItem
					{
						CaseType= ProjectRenaming.CaseChangeType.Lower,
						Name="lower"
					},

					new RenamingComboboxItem
					{
						CaseType= ProjectRenaming.CaseChangeType.Upper,
						Name="UPPER"
					},
			    };
			cmbRenamingCase.DisplayMember = "Name";
			cmbRenamingCase.ValueMember = "CaseType";
			cmbRenamingCase.DataSource = data;
		}

		protected override bool ValidateForm()
		{
			bool result = true;
			string errMsg = "";
			Control focus = null;

			txtSqlDbName.Text = txtSqlDbName.Text.Trim();
			txtNamespace.Text = txtNamespace.Text.Trim();
			txtSqlPassword.Text = txtSqlPassword.Text.Trim();
			txtProjectName.Text = txtProjectName.Text.Trim();
			txtGenerationPath.Text = txtGenerationPath.Text.Trim();
			txtSqlHost.Text = txtSqlHost.Text.Trim();
			txtSqlUsername.Text = txtSqlUsername.Text.Trim();

			if (txtProjectName.Text.Length == 0)
			{
				errMsg += "\nProject name is required!";
				result = false;
				if (focus == null) focus = txtProjectName;
			}

			if (txtNamespace.Text.Length == 0)
			{
				errMsg += "\nNamespace is required!";
				result = false;
				if (focus == null) focus = txtNamespace;
			}

			if (txtGenerationPath.Text.Length == 0)
			{
				errMsg += "\nGeneration path is required!";
				result = false;
				if (focus == null) focus = txtGenerationPath;
			}

			if (txtPatternfile.Text.Length == 0 ||
				!File.Exists(Common.AppVarPathMakeAbsolute(txtPatternfile.Text)))
			{
				errMsg += "\nPattern file is required!";
				result = false;
				if (focus == null) focus = txtPatternfile;
			}

			if (pagerDatabaseProvider.SelectedTab == tabSQLServer)
			{
				if (txtSqlHost.Text.Length == 0)
				{
					errMsg += "\nSQLServer host is required!";
					result = false;
					if (focus == null) focus = txtSqlHost;
				}

				if (txtSqlDbName.Text.Length == 0)
				{
					errMsg += "\nDatabase name is required!";
					result = false;
					if (focus == null) focus = txtSqlDbName;
				}

				if (txtSqlConnectTimeout.Text.Length == 0)
				{
					errMsg += "\nConnect timeout is required!";
					result = false;
					if (focus == null) focus = txtSqlConnectTimeout;
				}
				else
				{
					int connect = 0;
					if (int.TryParse(txtSqlConnectTimeout.Text, out connect) == false)
					{
						errMsg += "\nConnect timeout should be number!";
						result = false;
						if (focus == null) focus = txtSqlConnectTimeout;
					}
				}

				if (rbtnSqlAuthentication.Checked && txtSqlUsername.Text.Length == 0)
				{
					errMsg += "\nUsername name is required!";
					result = false;
					if (focus == null) focus = txtSqlUsername;
				}
			}
			else if (pagerDatabaseProvider.SelectedTab == tabSQLServer)
			{
				if (!File.Exists(txtSQLiteDatabaseName.Text))
				{
					errMsg += "\nSQLite database is not valid!";
					result = false;
					if (focus == null) focus = txtSqlHost;
				}

				if (txtSQLiteConnectTimeout.Text.Length == 0)
				{
					errMsg += "\nConnect timeout is required!";
					result = false;
					if (focus == null) focus = txtSQLiteConnectTimeout;
				}
				else
				{
					int connect = 0;
					if (int.TryParse(txtSQLiteConnectTimeout.Text, out connect) == false)
					{
						errMsg += "\nConnect timeout should be number!";
						result = false;
						if (focus == null) focus = txtSQLiteConnectTimeout;
					}
				}
			}



			if (!result)
			{
				MessageBox.Show(errMsg, "Invalid form data", MessageBoxButtons.OK, MessageBoxIcon.Error);
				txtProjectName.Focus();
			}

			return result;
		}

		bool TestSqlConnection(string connStr)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(connStr))
					conn.Open();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		bool TestSqlCeConnection(string connStr)
		{
			try
			{
				using (SqlCeConnection conn = new SqlCeConnection(connStr))
					conn.Open();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		bool TestSQLiteConnection(string connStr, string sqlitePassword)
		{
			try
			{
				using (SQLiteConnection conn = new SQLiteConnection(connStr))
				{
					if (!string.IsNullOrEmpty(sqlitePassword))
					{
						conn.SetPassword(sqlitePassword);
					}
					conn.Open();
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		bool TestOracleConnection(string connStr, out string message)
		{
			message = "";
			try
			{
				using (var conn = new OracleConnection(connStr))
				{
					conn.Open();
				}
				return true;
			}
			catch (Exception ex)
			{
				message = Common.GetExceptionTechMessage(ex, null);
				return false;
			}
		}


		void MoreOptionsExpand(bool expanded)
		{
			this._formExpanded = expanded;
			if (expanded)
			{
				btnSlider.Image = Resources.ToLeft10;
				this.Size = new Size(715, this.Size.Height);
			}
			else
			{
				btnSlider.Image = Resources.ToRight10;
				this.Size = new Size(383, this.Size.Height);
			}
		}


		private void frmNewProject_Load(object sender, EventArgs e)
		{
			if (EditMode == FormEditMode.New)
			{
				MoreOptionsExpand(false);
				Text = "New Project";
			}
			else
			{
				MoreOptionsExpand(true);
				Text = "Project Options";
			}
			InitializeForm();
		}

		private void btnGenPathBrowse_Click(object sender, EventArgs e)
		{
			var absolutePath = txtGenerationPath.Text;
			if (File.Exists(ProjectInstance.ProjectFileName))
				absolutePath = Common.ProjectPathMakeAbsolute(absolutePath, ProjectInstance.ProjectFileName);

			dlgGenBrowse.SelectedPath = absolutePath;
			if (dlgGenBrowse.ShowDialog() == DialogResult.OK)
			{
				if (File.Exists(ProjectInstance.ProjectFileName))
					txtGenerationPath.Text = Common.ProjectPathMakeRelative(dlgGenBrowse.SelectedPath, ProjectInstance.ProjectFileName);
				else
					txtGenerationPath.Text = dlgGenBrowse.SelectedPath;
			}
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			if (ValidateForm())
			{
				InitializeProjectModel();
				DialogResult = DialogResult.OK;
				Close();
			}
		}

		private void btnTestConnection_Click(object sender, EventArgs e)
		{
			string connStr;
			if (rbtnSqlAuthentication.Checked)
				connStr = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Pwd={3};Connect Timeout={4}",
					txtSqlHost.Text,
					txtSqlDbName.Text,
					txtSqlUsername.Text,
					txtSqlPassword.Text,
					15
				);
			else
				connStr = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True;Connect Timeout={2}",
					txtSqlHost.Text,
					txtSqlDbName.Text,
					15
				);

			PleaseWait.ShowPleaseWait("Testing connection to database server", true, false);
			if (TestSqlConnection(connStr))
			{
				PleaseWait.Abort();
				MessageBox.Show("Connection test succeed", "Test succeed", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				PleaseWait.Abort();
				MessageBox.Show("Connection test failed!", "Test failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnPatternProject_Click(object sender, EventArgs e)
		{
			if (dlgOpenPattern.ShowDialog() == DialogResult.OK)
			{
				var path = Common.AppVarPathMakeRelative(dlgOpenPattern.FileName);
				txtPatternfile.Text = path;
			}
		}

		private void btnAddIgnoredPrefixes_Click(object sender, EventArgs e)
		{
			lstIgnoredPrefixes.Text = lstIgnoredPrefixes.Text.Trim();
			if (lstIgnoredPrefixes.Text.Length > 0 && !lstIgnoredPrefixes.Items.Contains(lstIgnoredPrefixes.Text))
			{
				lstIgnoredPrefixes.Items.Add(lstIgnoredPrefixes.Text);
				lstIgnoredPrefixes.Text = "";
			}
		}

		private void btnDelIgnoredPrefixes_Click(object sender, EventArgs e)
		{
			if (lstIgnoredPrefixes.SelectedIndex >= 0)
			{
				lstIgnoredPrefixes.Items.RemoveAt(lstIgnoredPrefixes.SelectedIndex);
				lstIgnoredPrefixes.Text = "";
			}
		}

		private void btnAddIgnoredSuffixes_Click(object sender, EventArgs e)
		{
			lstIgnoredSuffixes.Text = lstIgnoredSuffixes.Text.Trim();
			if (lstIgnoredSuffixes.Text.Length > 0 && !lstIgnoredSuffixes.Items.Contains(lstIgnoredSuffixes.Text))
			{
				lstIgnoredSuffixes.Items.Add(lstIgnoredSuffixes.Text);
				lstIgnoredSuffixes.Text = "";
			}
		}

		private void btnDelIgnoredSuffixes_Click(object sender, EventArgs e)
		{
			if (lstIgnoredSuffixes.SelectedIndex >= 0)
			{
				lstIgnoredSuffixes.Items.RemoveAt(lstIgnoredSuffixes.SelectedIndex);
				lstIgnoredSuffixes.Text = "";
			}
		}

		private void btnSQLiteSelectDatabase_Click(object sender, EventArgs e)
		{
			if (dlgOpenSQLite.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				txtSQLiteDatabaseName.Text = dlgOpenSQLite.FileName;
			}
		}

		private void btnSQLiteTestConnection_Click(object sender, EventArgs e)
		{
			string connStr = string.Format("data source=={0};Default Timeout={1}",
					txtSQLiteDatabaseName.Text,
					15
				);

			PleaseWait.ShowPleaseWait("Testing connection to SQLite database", true, false);
			if (TestSQLiteConnection(connStr, txtSQLitePassword.Text))
			{
				PleaseWait.Abort();
				MessageBox.Show("Connection test succeed", "Test succeed", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				PleaseWait.Abort();
				MessageBox.Show("Connection test failed!", "Test failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnOrclTestConnection_Click(object sender, EventArgs e)
		{
			const string connSpecificUser = "Data Source={0};User Id={1};Password={2};";
			const string connIntegratedSecurity = "Data Source={0};Integrated Security=SSPI;";
			const string connDbaPrivilege = "DBA PRIVILEGE=SYSDBA;";
			string connStr = "";

			if (rbtnOrclSpecificUsernamePass.Checked)
			{
				connStr = string.Format(connSpecificUser, txtOrclDataSource.Text, txtOrclUsername.Text, txtOrclPassword.Text);
			}
			else
				connStr = string.Format(connIntegratedSecurity, txtOrclDataSource.Text);

			if (chkOrclUserRoleSYSDBA.Checked)
				connStr += connDbaPrivilege;


			PleaseWait.ShowPleaseWait("Testing connection to Oracle database", true, false);
			string message;
			if (TestOracleConnection(connStr, out message))
			{
				PleaseWait.Abort();
				MessageBox.Show("Connection test succeed", "Test succeed", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				PleaseWait.Abort();
				MessageBox.Show("Connection test failed!\n" + message, "Test failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}

		private void btnSlider_Click(object sender, EventArgs e)
		{
			MoreOptionsExpand(!_formExpanded);
		}

		private void chkRenamingCase_CheckedChanged(object sender, EventArgs e)
		{
			bool active = chkRenamingCase.Checked;
			cmbRenamingCase.Enabled = active;
			chkRenCaseTables.Enabled = active;
			chkRenCaseProps.Enabled = active;
		}

		private void chkRenRemUnderline_CheckedChanged(object sender, EventArgs e)
		{
			bool active = chkRenRemUnderline.Checked;
			chkRenUnderlineTables.Enabled = active;
			chkRenUnderlineProps.Enabled = active;
		}

		private void btnSqlCeSelectDatabase_Click(object sender, EventArgs e)
		{
			if (dlgOpenSqlCe.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				txtSqlCeDatabaseName.Text = dlgOpenSqlCe.FileName;
			}

		}

		private void btnSqlCeTestConnection_Click(object sender, EventArgs e)
		{
			string connStr;
			bool useSqlAuthentication = !string.IsNullOrEmpty(txtSqlCePassword.Text);

			if (useSqlAuthentication)
				connStr = string.Format("Data Source={0};Password={1}",
					txtSqlCeDatabaseName.Text,
					txtSqlCePassword.Text
				);
			else
				connStr = string.Format("Data Source={0};",
					txtSqlCeDatabaseName.Text
				);

			PleaseWait.ShowPleaseWait("Testing connection to SqlCE database", true, false);
			if (TestSqlCeConnection(connStr))
			{
				PleaseWait.Abort();
				MessageBox.Show("Connection test succeed", "Test succeed", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				PleaseWait.Abort();
				MessageBox.Show("Connection test failed!", "Test failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void lnkOracleConnStr_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				Process.Start(new ProcessStartInfo("http://www.connectionstrings.com/oracle#p12")
								{
									UseShellExecute = true,
								});
				MessageBox.Show("Oracle Connection String Template:\n\n" +
				"(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=xxx.xxx.xxx.xxx)(PORT=xxxx)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=xxxxx)))\n\n" +
				"Also visit http://www.connectionstrings.com/oracle#p12 for more information."
				, "Params", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception)
			{
			}
		}

		private void txtOrclUsername_TextChanged(object sender, EventArgs e)
		{
			if (txtOrclDbName.Text.Length == 0)
				txtOrclDbName.Text = txtOrclUsername.Text;
		}
	}
}
