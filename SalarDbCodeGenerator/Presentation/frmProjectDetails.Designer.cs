namespace SalarDbCodeGenerator.Presentation
{
	partial class frmProjectDetails
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.dlgGenBrowse = new System.Windows.Forms.FolderBrowserDialog();
			this.dlgOpenPattern = new System.Windows.Forms.OpenFileDialog();
			this.dlgOpenSQLite = new System.Windows.Forms.OpenFileDialog();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.chkTableConstraintKeys = new System.Windows.Forms.CheckBox();
			this.chkGenForeignKey = new System.Windows.Forms.CheckBox();
			this.chkGenDescription = new System.Windows.Forms.CheckBox();
			this.gpGenOptions = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label24 = new System.Windows.Forms.Label();
			this.chkRenUnderlineWordDelimiter = new System.Windows.Forms.CheckBox();
			this.cmbRenamingCase = new System.Windows.Forms.ComboBox();
			this.chkRenamingCase = new System.Windows.Forms.CheckBox();
			this.chkRenCaseProps = new System.Windows.Forms.CheckBox();
			this.chkRenUnderlineProps = new System.Windows.Forms.CheckBox();
			this.chkRenCaseTables = new System.Windows.Forms.CheckBox();
			this.chkRenUnderlineTables = new System.Windows.Forms.CheckBox();
			this.chkRenRemUnderline = new System.Windows.Forms.CheckBox();
			this.btnDelIgnoredSuffixes = new System.Windows.Forms.Button();
			this.btnAddIgnoredSuffixes = new System.Windows.Forms.Button();
			this.btnDelIgnoredPrefixes = new System.Windows.Forms.Button();
			this.btnAddIgnoredPrefixes = new System.Windows.Forms.Button();
			this.lstIgnoredSuffixes = new System.Windows.Forms.ComboBox();
			this.lstIgnoredPrefixes = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.txtSuffixForViews = new System.Windows.Forms.TextBox();
			this.txtSuffixForTables = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.txtPrefixForViews = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.txtPrefixForTables = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnPatternProject = new System.Windows.Forms.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.btnGenPathBrowse = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.txtPatternfile = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtGenerationPath = new System.Windows.Forms.TextBox();
			this.txtNamespace = new System.Windows.Forms.TextBox();
			this.txtProjectName = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.pagerDatabaseProvider = new System.Windows.Forms.TabControl();
			this.tabSQLServer = new System.Windows.Forms.TabPage();
			this.btnSqlTestConnection = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.rbtnSqlAuthentication = new System.Windows.Forms.RadioButton();
			this.txtSqlHost = new System.Windows.Forms.TextBox();
			this.rbtnSqlWindowsAuthentication = new System.Windows.Forms.RadioButton();
			this.txtSqlDbName = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtSqlConnectTimeout = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtSqlUsername = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.txtSqlPassword = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tabSqlCe = new System.Windows.Forms.TabPage();
			this.btnSqlCeSelectDatabase = new System.Windows.Forms.Button();
			this.btnSqlCeTestConnection = new System.Windows.Forms.Button();
			this.txtSqlCeDatabaseName = new System.Windows.Forms.TextBox();
			this.txtSqlCePassword = new System.Windows.Forms.TextBox();
			this.label27 = new System.Windows.Forms.Label();
			this.label25 = new System.Windows.Forms.Label();
			this.tabOracle = new System.Windows.Forms.TabPage();
			this.lnkOracleConnStr = new System.Windows.Forms.LinkLabel();
			this.chkOrclUserRoleSYSDBA = new System.Windows.Forms.CheckBox();
			this.btnOrclTestConnection = new System.Windows.Forms.Button();
			this.label18 = new System.Windows.Forms.Label();
			this.rbtnOrclSpecificUsernamePass = new System.Windows.Forms.RadioButton();
			this.txtOrclDataSource = new System.Windows.Forms.TextBox();
			this.rbtnOrclWindowsAuthentication = new System.Windows.Forms.RadioButton();
			this.label20 = new System.Windows.Forms.Label();
			this.txtOrclConnectTimeout = new System.Windows.Forms.TextBox();
			this.label21 = new System.Windows.Forms.Label();
			this.txtOrclUsername = new System.Windows.Forms.TextBox();
			this.label22 = new System.Windows.Forms.Label();
			this.txtOrclPassword = new System.Windows.Forms.TextBox();
			this.tabSQLite = new System.Windows.Forms.TabPage();
			this.btnSQLiteSelectDatabase = new System.Windows.Forms.Button();
			this.btnSQLiteTestConnection = new System.Windows.Forms.Button();
			this.label16 = new System.Windows.Forms.Label();
			this.txtSQLiteDatabaseName = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.txtSQLiteConnectTimeout = new System.Windows.Forms.TextBox();
			this.label19 = new System.Windows.Forms.Label();
			this.txtSQLitePassword = new System.Windows.Forms.TextBox();
			this.tipHints = new System.Windows.Forms.ToolTip(this.components);
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnSlider = new System.Windows.Forms.Button();
			this.dlgOpenSqlCe = new System.Windows.Forms.OpenFileDialog();
			this.txtOrclDbName = new System.Windows.Forms.TextBox();
			this.label23 = new System.Windows.Forms.Label();
			this.groupBox4.SuspendLayout();
			this.gpGenOptions.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.pagerDatabaseProvider.SuspendLayout();
			this.tabSQLServer.SuspendLayout();
			this.tabSqlCe.SuspendLayout();
			this.tabOracle.SuspendLayout();
			this.tabSQLite.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// dlgGenBrowse
			// 
			this.dlgGenBrowse.Description = "Select generated project destination";
			// 
			// dlgOpenPattern
			// 
			this.dlgOpenPattern.DefaultExt = "SalarCodeGen DbPattern (*.dbpat)|*.dbpat";
			this.dlgOpenPattern.Filter = "SalarCodeGen DbPattern (*.dbpat)|*.dbpat";
			this.dlgOpenPattern.Title = "Change the Pattern";
			// 
			// dlgOpenSQLite
			// 
			this.dlgOpenSQLite.DefaultExt = "SQLite Database (*.sqlite;*.*)|*.sqlite;*.*";
			this.dlgOpenSQLite.Filter = "SQLite Database (*.sqlite;*.*)|*.sqlite;*.*";
			this.dlgOpenSQLite.Title = "Select SQLite database";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.chkTableConstraintKeys);
			this.groupBox4.Controls.Add(this.chkGenForeignKey);
			this.groupBox4.Controls.Add(this.chkGenDescription);
			this.groupBox4.Location = new System.Drawing.Point(12, 154);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(352, 70);
			this.groupBox4.TabIndex = 1;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Generator Settings";
			// 
			// chkTableConstraintKeys
			// 
			this.chkTableConstraintKeys.AutoSize = true;
			this.chkTableConstraintKeys.Location = new System.Drawing.Point(196, 19);
			this.chkTableConstraintKeys.Name = "chkTableConstraintKeys";
			this.chkTableConstraintKeys.Size = new System.Drawing.Size(132, 17);
			this.chkTableConstraintKeys.TabIndex = 1;
			this.chkTableConstraintKeys.Text = "Tables constraint keys";
			this.tipHints.SetToolTip(this.chkTableConstraintKeys, "Supported in SQL Server (2005, 2008)");
			this.chkTableConstraintKeys.UseVisualStyleBackColor = true;
			// 
			// chkGenForeignKey
			// 
			this.chkGenForeignKey.AutoSize = true;
			this.chkGenForeignKey.Location = new System.Drawing.Point(31, 19);
			this.chkGenForeignKey.Name = "chkGenForeignKey";
			this.chkGenForeignKey.Size = new System.Drawing.Size(155, 17);
			this.chkGenForeignKey.TabIndex = 0;
			this.chkGenForeignKey.Text = "Tables foreign key relations";
			this.chkGenForeignKey.UseVisualStyleBackColor = true;
			// 
			// chkGenDescription
			// 
			this.chkGenDescription.AutoSize = true;
			this.chkGenDescription.Location = new System.Drawing.Point(31, 42);
			this.chkGenDescription.Name = "chkGenDescription";
			this.chkGenDescription.Size = new System.Drawing.Size(120, 17);
			this.chkGenDescription.TabIndex = 2;
			this.chkGenDescription.Text = "Columns description";
			this.tipHints.SetToolTip(this.chkGenDescription, "Supported in SQL Server (Tables only, all versions)");
			this.chkGenDescription.UseVisualStyleBackColor = true;
			// 
			// gpGenOptions
			// 
			this.gpGenOptions.Controls.Add(this.groupBox3);
			this.gpGenOptions.Controls.Add(this.btnDelIgnoredSuffixes);
			this.gpGenOptions.Controls.Add(this.btnAddIgnoredSuffixes);
			this.gpGenOptions.Controls.Add(this.btnDelIgnoredPrefixes);
			this.gpGenOptions.Controls.Add(this.btnAddIgnoredPrefixes);
			this.gpGenOptions.Controls.Add(this.lstIgnoredSuffixes);
			this.gpGenOptions.Controls.Add(this.lstIgnoredPrefixes);
			this.gpGenOptions.Controls.Add(this.label10);
			this.gpGenOptions.Controls.Add(this.label9);
			this.gpGenOptions.Controls.Add(this.txtSuffixForViews);
			this.gpGenOptions.Controls.Add(this.txtSuffixForTables);
			this.gpGenOptions.Controls.Add(this.label14);
			this.gpGenOptions.Controls.Add(this.label12);
			this.gpGenOptions.Controls.Add(this.txtPrefixForViews);
			this.gpGenOptions.Controls.Add(this.label13);
			this.gpGenOptions.Controls.Add(this.txtPrefixForTables);
			this.gpGenOptions.Controls.Add(this.label11);
			this.gpGenOptions.Location = new System.Drawing.Point(372, 12);
			this.gpGenOptions.Name = "gpGenOptions";
			this.gpGenOptions.Size = new System.Drawing.Size(323, 493);
			this.gpGenOptions.TabIndex = 6;
			this.gpGenOptions.TabStop = false;
			this.gpGenOptions.Text = "Generation Options";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label24);
			this.groupBox3.Controls.Add(this.chkRenUnderlineWordDelimiter);
			this.groupBox3.Controls.Add(this.cmbRenamingCase);
			this.groupBox3.Controls.Add(this.chkRenamingCase);
			this.groupBox3.Controls.Add(this.chkRenCaseProps);
			this.groupBox3.Controls.Add(this.chkRenUnderlineProps);
			this.groupBox3.Controls.Add(this.chkRenCaseTables);
			this.groupBox3.Controls.Add(this.chkRenUnderlineTables);
			this.groupBox3.Controls.Add(this.chkRenRemUnderline);
			this.groupBox3.Location = new System.Drawing.Point(11, 307);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(302, 176);
			this.groupBox3.TabIndex = 10;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Renaming Options";
			// 
			// label24
			// 
			this.label24.BackColor = System.Drawing.SystemColors.Info;
			this.label24.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label24.Location = new System.Drawing.Point(157, 110);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(139, 56);
			this.label24.TabIndex = 8;
			this.label24.Text = "Warning:\r\nNot all patterns support renaming options.\r\nUse them at your own risk.";
			// 
			// chkRenUnderlineWordDelimiter
			// 
			this.chkRenUnderlineWordDelimiter.AutoSize = true;
			this.chkRenUnderlineWordDelimiter.Location = new System.Drawing.Point(148, 28);
			this.chkRenUnderlineWordDelimiter.Name = "chkRenUnderlineWordDelimiter";
			this.chkRenUnderlineWordDelimiter.Size = new System.Drawing.Size(148, 17);
			this.chkRenUnderlineWordDelimiter.TabIndex = 7;
			this.chkRenUnderlineWordDelimiter.Text = "Underline is word delimiter";
			this.chkRenUnderlineWordDelimiter.UseVisualStyleBackColor = true;
			// 
			// cmbRenamingCase
			// 
			this.cmbRenamingCase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbRenamingCase.Enabled = false;
			this.cmbRenamingCase.FormattingEnabled = true;
			this.cmbRenamingCase.Items.AddRange(new object[] {
            "Capitalize",
            "PascalCase",
            "camelCase",
            "lower",
            "UPPER"});
			this.cmbRenamingCase.Location = new System.Drawing.Point(64, 99);
			this.cmbRenamingCase.Name = "cmbRenamingCase";
			this.cmbRenamingCase.Size = new System.Drawing.Size(82, 21);
			this.cmbRenamingCase.TabIndex = 4;
			// 
			// chkRenamingCase
			// 
			this.chkRenamingCase.AutoSize = true;
			this.chkRenamingCase.Location = new System.Drawing.Point(13, 101);
			this.chkRenamingCase.Name = "chkRenamingCase";
			this.chkRenamingCase.Size = new System.Drawing.Size(53, 17);
			this.chkRenamingCase.TabIndex = 3;
			this.chkRenamingCase.Text = "Case:";
			this.chkRenamingCase.UseVisualStyleBackColor = true;
			this.chkRenamingCase.CheckedChanged += new System.EventHandler(this.chkRenamingCase_CheckedChanged);
			// 
			// chkRenCaseProps
			// 
			this.chkRenCaseProps.AutoSize = true;
			this.chkRenCaseProps.Checked = true;
			this.chkRenCaseProps.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkRenCaseProps.Enabled = false;
			this.chkRenCaseProps.Location = new System.Drawing.Point(26, 147);
			this.chkRenCaseProps.Name = "chkRenCaseProps";
			this.chkRenCaseProps.Size = new System.Drawing.Size(90, 17);
			this.chkRenCaseProps.TabIndex = 6;
			this.chkRenCaseProps.Text = "For properties";
			this.chkRenCaseProps.UseVisualStyleBackColor = true;
			// 
			// chkRenUnderlineProps
			// 
			this.chkRenUnderlineProps.AutoSize = true;
			this.chkRenUnderlineProps.Checked = true;
			this.chkRenUnderlineProps.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkRenUnderlineProps.Enabled = false;
			this.chkRenUnderlineProps.Location = new System.Drawing.Point(26, 74);
			this.chkRenUnderlineProps.Name = "chkRenUnderlineProps";
			this.chkRenUnderlineProps.Size = new System.Drawing.Size(98, 17);
			this.chkRenUnderlineProps.TabIndex = 2;
			this.chkRenUnderlineProps.Text = "From properties";
			this.chkRenUnderlineProps.UseVisualStyleBackColor = true;
			// 
			// chkRenCaseTables
			// 
			this.chkRenCaseTables.AutoSize = true;
			this.chkRenCaseTables.Checked = true;
			this.chkRenCaseTables.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkRenCaseTables.Enabled = false;
			this.chkRenCaseTables.Location = new System.Drawing.Point(26, 124);
			this.chkRenCaseTables.Name = "chkRenCaseTables";
			this.chkRenCaseTables.Size = new System.Drawing.Size(72, 17);
			this.chkRenCaseTables.TabIndex = 5;
			this.chkRenCaseTables.Tag = "";
			this.chkRenCaseTables.Text = "For tables";
			this.chkRenCaseTables.UseVisualStyleBackColor = true;
			// 
			// chkRenUnderlineTables
			// 
			this.chkRenUnderlineTables.AutoSize = true;
			this.chkRenUnderlineTables.Checked = true;
			this.chkRenUnderlineTables.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkRenUnderlineTables.Enabled = false;
			this.chkRenUnderlineTables.Location = new System.Drawing.Point(26, 51);
			this.chkRenUnderlineTables.Name = "chkRenUnderlineTables";
			this.chkRenUnderlineTables.Size = new System.Drawing.Size(80, 17);
			this.chkRenUnderlineTables.TabIndex = 1;
			this.chkRenUnderlineTables.Tag = "";
			this.chkRenUnderlineTables.Text = "From tables";
			this.chkRenUnderlineTables.UseVisualStyleBackColor = true;
			// 
			// chkRenRemUnderline
			// 
			this.chkRenRemUnderline.AutoSize = true;
			this.chkRenRemUnderline.Location = new System.Drawing.Point(13, 28);
			this.chkRenRemUnderline.Name = "chkRenRemUnderline";
			this.chkRenRemUnderline.Size = new System.Drawing.Size(133, 17);
			this.chkRenRemUnderline.TabIndex = 0;
			this.chkRenRemUnderline.Text = "Remove underline ( _ )";
			this.chkRenRemUnderline.UseVisualStyleBackColor = true;
			this.chkRenRemUnderline.CheckedChanged += new System.EventHandler(this.chkRenRemUnderline_CheckedChanged);
			// 
			// btnDelIgnoredSuffixes
			// 
			this.btnDelIgnoredSuffixes.Image = global::SalarDbCodeGenerator.Properties.Resources.Delete16;
			this.btnDelIgnoredSuffixes.Location = new System.Drawing.Point(211, 153);
			this.btnDelIgnoredSuffixes.Name = "btnDelIgnoredSuffixes";
			this.btnDelIgnoredSuffixes.Size = new System.Drawing.Size(40, 23);
			this.btnDelIgnoredSuffixes.TabIndex = 5;
			this.btnDelIgnoredSuffixes.UseVisualStyleBackColor = true;
			this.btnDelIgnoredSuffixes.Click += new System.EventHandler(this.btnDelIgnoredSuffixes_Click);
			// 
			// btnAddIgnoredSuffixes
			// 
			this.btnAddIgnoredSuffixes.Image = global::SalarDbCodeGenerator.Properties.Resources.Add16;
			this.btnAddIgnoredSuffixes.Location = new System.Drawing.Point(165, 153);
			this.btnAddIgnoredSuffixes.Name = "btnAddIgnoredSuffixes";
			this.btnAddIgnoredSuffixes.Size = new System.Drawing.Size(40, 23);
			this.btnAddIgnoredSuffixes.TabIndex = 4;
			this.btnAddIgnoredSuffixes.UseVisualStyleBackColor = true;
			this.btnAddIgnoredSuffixes.Click += new System.EventHandler(this.btnAddIgnoredSuffixes_Click);
			// 
			// btnDelIgnoredPrefixes
			// 
			this.btnDelIgnoredPrefixes.Image = global::SalarDbCodeGenerator.Properties.Resources.Delete16;
			this.btnDelIgnoredPrefixes.Location = new System.Drawing.Point(57, 153);
			this.btnDelIgnoredPrefixes.Name = "btnDelIgnoredPrefixes";
			this.btnDelIgnoredPrefixes.Size = new System.Drawing.Size(40, 23);
			this.btnDelIgnoredPrefixes.TabIndex = 2;
			this.btnDelIgnoredPrefixes.UseVisualStyleBackColor = true;
			this.btnDelIgnoredPrefixes.Click += new System.EventHandler(this.btnDelIgnoredPrefixes_Click);
			// 
			// btnAddIgnoredPrefixes
			// 
			this.btnAddIgnoredPrefixes.Image = global::SalarDbCodeGenerator.Properties.Resources.Add16;
			this.btnAddIgnoredPrefixes.Location = new System.Drawing.Point(11, 153);
			this.btnAddIgnoredPrefixes.Name = "btnAddIgnoredPrefixes";
			this.btnAddIgnoredPrefixes.Size = new System.Drawing.Size(40, 23);
			this.btnAddIgnoredPrefixes.TabIndex = 1;
			this.btnAddIgnoredPrefixes.UseVisualStyleBackColor = true;
			this.btnAddIgnoredPrefixes.Click += new System.EventHandler(this.btnAddIgnoredPrefixes_Click);
			// 
			// lstIgnoredSuffixes
			// 
			this.lstIgnoredSuffixes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
			this.lstIgnoredSuffixes.FormattingEnabled = true;
			this.lstIgnoredSuffixes.Location = new System.Drawing.Point(165, 49);
			this.lstIgnoredSuffixes.Name = "lstIgnoredSuffixes";
			this.lstIgnoredSuffixes.Size = new System.Drawing.Size(148, 111);
			this.lstIgnoredSuffixes.TabIndex = 3;
			// 
			// lstIgnoredPrefixes
			// 
			this.lstIgnoredPrefixes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
			this.lstIgnoredPrefixes.FormattingEnabled = true;
			this.lstIgnoredPrefixes.Location = new System.Drawing.Point(11, 49);
			this.lstIgnoredPrefixes.Name = "lstIgnoredPrefixes";
			this.lstIgnoredPrefixes.Size = new System.Drawing.Size(148, 111);
			this.lstIgnoredPrefixes.TabIndex = 0;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(156, 30);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(84, 13);
			this.label10.TabIndex = 1;
			this.label10.Text = "Ignored suffixes;";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(11, 30);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(85, 13);
			this.label9.TabIndex = 1;
			this.label9.Text = "Ignored prefixes:";
			// 
			// txtSuffixForViews
			// 
			this.txtSuffixForViews.Location = new System.Drawing.Point(101, 281);
			this.txtSuffixForViews.MaxLength = 20;
			this.txtSuffixForViews.Name = "txtSuffixForViews";
			this.txtSuffixForViews.Size = new System.Drawing.Size(212, 20);
			this.txtSuffixForViews.TabIndex = 9;
			// 
			// txtSuffixForTables
			// 
			this.txtSuffixForTables.Location = new System.Drawing.Point(101, 222);
			this.txtSuffixForTables.MaxLength = 20;
			this.txtSuffixForTables.Name = "txtSuffixForTables";
			this.txtSuffixForTables.Size = new System.Drawing.Size(212, 20);
			this.txtSuffixForTables.TabIndex = 7;
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(14, 284);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(81, 13);
			this.label14.TabIndex = 1;
			this.label14.Text = "Suffix for views:";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(13, 225);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(82, 13);
			this.label12.TabIndex = 1;
			this.label12.Text = "Suffix for tables:";
			// 
			// txtPrefixForViews
			// 
			this.txtPrefixForViews.Location = new System.Drawing.Point(101, 255);
			this.txtPrefixForViews.MaxLength = 20;
			this.txtPrefixForViews.Name = "txtPrefixForViews";
			this.txtPrefixForViews.Size = new System.Drawing.Size(212, 20);
			this.txtPrefixForViews.TabIndex = 8;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(14, 258);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(81, 13);
			this.label13.TabIndex = 1;
			this.label13.Text = "Prefix for views:";
			// 
			// txtPrefixForTables
			// 
			this.txtPrefixForTables.Location = new System.Drawing.Point(101, 196);
			this.txtPrefixForTables.MaxLength = 20;
			this.txtPrefixForTables.Name = "txtPrefixForTables";
			this.txtPrefixForTables.Size = new System.Drawing.Size(212, 20);
			this.txtPrefixForTables.TabIndex = 6;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(13, 199);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(82, 13);
			this.label11.TabIndex = 1;
			this.label11.Text = "Prefix for tables:";
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(538, 511);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.Location = new System.Drawing.Point(619, 511);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 3;
			this.btnOk.Text = "&OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnPatternProject);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.btnGenPathBrowse);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.txtPatternfile);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.txtGenerationPath);
			this.groupBox2.Controls.Add(this.txtNamespace);
			this.groupBox2.Controls.Add(this.txtProjectName);
			this.groupBox2.Location = new System.Drawing.Point(12, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(352, 136);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Project Information";
			// 
			// btnPatternProject
			// 
			this.btnPatternProject.Location = new System.Drawing.Point(318, 101);
			this.btnPatternProject.Name = "btnPatternProject";
			this.btnPatternProject.Size = new System.Drawing.Size(28, 20);
			this.btnPatternProject.TabIndex = 5;
			this.btnPatternProject.Text = "...";
			this.btnPatternProject.UseVisualStyleBackColor = true;
			this.btnPatternProject.Click += new System.EventHandler(this.btnPatternProject_Click);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(31, 104);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(79, 13);
			this.label8.TabIndex = 1;
			this.label8.Text = "Pattern project:";
			// 
			// btnGenPathBrowse
			// 
			this.btnGenPathBrowse.Location = new System.Drawing.Point(318, 75);
			this.btnGenPathBrowse.Name = "btnGenPathBrowse";
			this.btnGenPathBrowse.Size = new System.Drawing.Size(28, 20);
			this.btnGenPathBrowse.TabIndex = 3;
			this.btnGenPathBrowse.Text = "...";
			this.btnGenPathBrowse.UseVisualStyleBackColor = true;
			this.btnGenPathBrowse.Click += new System.EventHandler(this.btnGenPathBrowse_Click);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(24, 78);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(86, 13);
			this.label7.TabIndex = 1;
			this.label7.Text = "Generation path:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 52);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(104, 13);
			this.label6.TabIndex = 1;
			this.label6.Text = "Default Namespace:";
			// 
			// txtPatternfile
			// 
			this.txtPatternfile.Location = new System.Drawing.Point(116, 101);
			this.txtPatternfile.Name = "txtPatternfile";
			this.txtPatternfile.Size = new System.Drawing.Size(200, 20);
			this.txtPatternfile.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(38, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Project name:";
			// 
			// txtGenerationPath
			// 
			this.txtGenerationPath.Location = new System.Drawing.Point(116, 75);
			this.txtGenerationPath.Name = "txtGenerationPath";
			this.txtGenerationPath.Size = new System.Drawing.Size(200, 20);
			this.txtGenerationPath.TabIndex = 2;
			// 
			// txtNamespace
			// 
			this.txtNamespace.Location = new System.Drawing.Point(116, 49);
			this.txtNamespace.MaxLength = 128;
			this.txtNamespace.Name = "txtNamespace";
			this.txtNamespace.Size = new System.Drawing.Size(200, 20);
			this.txtNamespace.TabIndex = 1;
			// 
			// txtProjectName
			// 
			this.txtProjectName.Location = new System.Drawing.Point(116, 23);
			this.txtProjectName.MaxLength = 255;
			this.txtProjectName.Name = "txtProjectName";
			this.txtProjectName.Size = new System.Drawing.Size(200, 20);
			this.txtProjectName.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.pagerDatabaseProvider);
			this.groupBox1.Location = new System.Drawing.Point(12, 230);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(352, 275);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Project Database";
			// 
			// pagerDatabaseProvider
			// 
			this.pagerDatabaseProvider.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.pagerDatabaseProvider.Controls.Add(this.tabSQLServer);
			this.pagerDatabaseProvider.Controls.Add(this.tabSqlCe);
			this.pagerDatabaseProvider.Controls.Add(this.tabOracle);
			this.pagerDatabaseProvider.Controls.Add(this.tabSQLite);
			this.pagerDatabaseProvider.Location = new System.Drawing.Point(6, 19);
			this.pagerDatabaseProvider.Multiline = true;
			this.pagerDatabaseProvider.Name = "pagerDatabaseProvider";
			this.pagerDatabaseProvider.SelectedIndex = 0;
			this.pagerDatabaseProvider.ShowToolTips = true;
			this.pagerDatabaseProvider.Size = new System.Drawing.Size(340, 250);
			this.pagerDatabaseProvider.TabIndex = 0;
			// 
			// tabSQLServer
			// 
			this.tabSQLServer.Controls.Add(this.btnSqlTestConnection);
			this.tabSQLServer.Controls.Add(this.label2);
			this.tabSQLServer.Controls.Add(this.rbtnSqlAuthentication);
			this.tabSQLServer.Controls.Add(this.txtSqlHost);
			this.tabSQLServer.Controls.Add(this.rbtnSqlWindowsAuthentication);
			this.tabSQLServer.Controls.Add(this.txtSqlDbName);
			this.tabSQLServer.Controls.Add(this.label5);
			this.tabSQLServer.Controls.Add(this.txtSqlConnectTimeout);
			this.tabSQLServer.Controls.Add(this.label4);
			this.tabSQLServer.Controls.Add(this.txtSqlUsername);
			this.tabSQLServer.Controls.Add(this.label15);
			this.tabSQLServer.Controls.Add(this.txtSqlPassword);
			this.tabSQLServer.Controls.Add(this.label3);
			this.tabSQLServer.Location = new System.Drawing.Point(4, 25);
			this.tabSQLServer.Name = "tabSQLServer";
			this.tabSQLServer.Padding = new System.Windows.Forms.Padding(3);
			this.tabSQLServer.Size = new System.Drawing.Size(332, 221);
			this.tabSQLServer.TabIndex = 0;
			this.tabSQLServer.Text = "SQL Server";
			this.tabSQLServer.ToolTipText = "MS SQL Server (2000/2005/2008/Express)";
			this.tabSQLServer.UseVisualStyleBackColor = true;
			// 
			// btnSqlTestConnection
			// 
			this.btnSqlTestConnection.Location = new System.Drawing.Point(106, 182);
			this.btnSqlTestConnection.Name = "btnSqlTestConnection";
			this.btnSqlTestConnection.Size = new System.Drawing.Size(98, 23);
			this.btnSqlTestConnection.TabIndex = 7;
			this.btnSqlTestConnection.Text = "Test connection";
			this.btnSqlTestConnection.UseVisualStyleBackColor = true;
			this.btnSqlTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(21, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "SqlServer host:";
			// 
			// rbtnSqlAuthentication
			// 
			this.rbtnSqlAuthentication.AutoSize = true;
			this.rbtnSqlAuthentication.Location = new System.Drawing.Point(106, 107);
			this.rbtnSqlAuthentication.Name = "rbtnSqlAuthentication";
			this.rbtnSqlAuthentication.Size = new System.Drawing.Size(116, 17);
			this.rbtnSqlAuthentication.TabIndex = 4;
			this.rbtnSqlAuthentication.Text = "SQL authentication";
			this.rbtnSqlAuthentication.UseVisualStyleBackColor = true;
			// 
			// txtSqlHost
			// 
			this.txtSqlHost.Location = new System.Drawing.Point(106, 6);
			this.txtSqlHost.Name = "txtSqlHost";
			this.txtSqlHost.Size = new System.Drawing.Size(200, 20);
			this.txtSqlHost.TabIndex = 0;
			// 
			// rbtnSqlWindowsAuthentication
			// 
			this.rbtnSqlWindowsAuthentication.AutoSize = true;
			this.rbtnSqlWindowsAuthentication.Checked = true;
			this.rbtnSqlWindowsAuthentication.Location = new System.Drawing.Point(106, 84);
			this.rbtnSqlWindowsAuthentication.Name = "rbtnSqlWindowsAuthentication";
			this.rbtnSqlWindowsAuthentication.Size = new System.Drawing.Size(139, 17);
			this.rbtnSqlWindowsAuthentication.TabIndex = 3;
			this.rbtnSqlWindowsAuthentication.TabStop = true;
			this.rbtnSqlWindowsAuthentication.Text = "Windows authentication";
			this.rbtnSqlWindowsAuthentication.UseVisualStyleBackColor = true;
			// 
			// txtSqlDbName
			// 
			this.txtSqlDbName.Location = new System.Drawing.Point(106, 32);
			this.txtSqlDbName.Name = "txtSqlDbName";
			this.txtSqlDbName.Size = new System.Drawing.Size(200, 20);
			this.txtSqlDbName.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(44, 159);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(56, 13);
			this.label5.TabIndex = 1;
			this.label5.Text = "Password:";
			// 
			// txtSqlConnectTimeout
			// 
			this.txtSqlConnectTimeout.Location = new System.Drawing.Point(106, 58);
			this.txtSqlConnectTimeout.MaxLength = 3;
			this.txtSqlConnectTimeout.Name = "txtSqlConnectTimeout";
			this.txtSqlConnectTimeout.Size = new System.Drawing.Size(200, 20);
			this.txtSqlConnectTimeout.TabIndex = 2;
			this.txtSqlConnectTimeout.Text = "15";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(42, 133);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(58, 13);
			this.label4.TabIndex = 1;
			this.label4.Text = "Username:";
			// 
			// txtSqlUsername
			// 
			this.txtSqlUsername.Location = new System.Drawing.Point(106, 130);
			this.txtSqlUsername.Name = "txtSqlUsername";
			this.txtSqlUsername.Size = new System.Drawing.Size(200, 20);
			this.txtSqlUsername.TabIndex = 5;
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(13, 61);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(87, 13);
			this.label15.TabIndex = 1;
			this.label15.Text = "Connect timeout:";
			// 
			// txtSqlPassword
			// 
			this.txtSqlPassword.Location = new System.Drawing.Point(106, 156);
			this.txtSqlPassword.Name = "txtSqlPassword";
			this.txtSqlPassword.Size = new System.Drawing.Size(200, 20);
			this.txtSqlPassword.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(15, 35);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(85, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "Database name:";
			// 
			// tabSqlCe
			// 
			this.tabSqlCe.Controls.Add(this.btnSqlCeSelectDatabase);
			this.tabSqlCe.Controls.Add(this.btnSqlCeTestConnection);
			this.tabSqlCe.Controls.Add(this.txtSqlCeDatabaseName);
			this.tabSqlCe.Controls.Add(this.txtSqlCePassword);
			this.tabSqlCe.Controls.Add(this.label27);
			this.tabSqlCe.Controls.Add(this.label25);
			this.tabSqlCe.Location = new System.Drawing.Point(4, 25);
			this.tabSqlCe.Name = "tabSqlCe";
			this.tabSqlCe.Padding = new System.Windows.Forms.Padding(3);
			this.tabSqlCe.Size = new System.Drawing.Size(332, 221);
			this.tabSqlCe.TabIndex = 3;
			this.tabSqlCe.Text = "SqlServerCE4";
			this.tipHints.SetToolTip(this.tabSqlCe, "SQL Server Compact 4.0");
			this.tabSqlCe.UseVisualStyleBackColor = true;
			// 
			// btnSqlCeSelectDatabase
			// 
			this.btnSqlCeSelectDatabase.Location = new System.Drawing.Point(106, 32);
			this.btnSqlCeSelectDatabase.Name = "btnSqlCeSelectDatabase";
			this.btnSqlCeSelectDatabase.Size = new System.Drawing.Size(98, 23);
			this.btnSqlCeSelectDatabase.TabIndex = 26;
			this.btnSqlCeSelectDatabase.Text = "Browse";
			this.btnSqlCeSelectDatabase.UseVisualStyleBackColor = true;
			this.btnSqlCeSelectDatabase.Click += new System.EventHandler(this.btnSqlCeSelectDatabase_Click);
			// 
			// btnSqlCeTestConnection
			// 
			this.btnSqlCeTestConnection.Location = new System.Drawing.Point(106, 96);
			this.btnSqlCeTestConnection.Name = "btnSqlCeTestConnection";
			this.btnSqlCeTestConnection.Size = new System.Drawing.Size(98, 23);
			this.btnSqlCeTestConnection.TabIndex = 27;
			this.btnSqlCeTestConnection.Text = "Test connection";
			this.btnSqlCeTestConnection.UseVisualStyleBackColor = true;
			this.btnSqlCeTestConnection.Click += new System.EventHandler(this.btnSqlCeTestConnection_Click);
			// 
			// txtSqlCeDatabaseName
			// 
			this.txtSqlCeDatabaseName.Location = new System.Drawing.Point(106, 6);
			this.txtSqlCeDatabaseName.Name = "txtSqlCeDatabaseName";
			this.txtSqlCeDatabaseName.ReadOnly = true;
			this.txtSqlCeDatabaseName.Size = new System.Drawing.Size(200, 20);
			this.txtSqlCeDatabaseName.TabIndex = 20;
			// 
			// txtSqlCePassword
			// 
			this.txtSqlCePassword.Location = new System.Drawing.Point(106, 70);
			this.txtSqlCePassword.Name = "txtSqlCePassword";
			this.txtSqlCePassword.Size = new System.Drawing.Size(200, 20);
			this.txtSqlCePassword.TabIndex = 21;
			// 
			// label27
			// 
			this.label27.AutoSize = true;
			this.label27.Location = new System.Drawing.Point(51, 74);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(56, 13);
			this.label27.TabIndex = 22;
			this.label27.Text = "Password:";
			// 
			// label25
			// 
			this.label25.AutoSize = true;
			this.label25.Location = new System.Drawing.Point(3, 9);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(104, 13);
			this.label25.TabIndex = 23;
			this.label25.Text = "SqlCE 4.0 database:";
			// 
			// tabOracle
			// 
			this.tabOracle.Controls.Add(this.txtOrclDbName);
			this.tabOracle.Controls.Add(this.label23);
			this.tabOracle.Controls.Add(this.lnkOracleConnStr);
			this.tabOracle.Controls.Add(this.chkOrclUserRoleSYSDBA);
			this.tabOracle.Controls.Add(this.btnOrclTestConnection);
			this.tabOracle.Controls.Add(this.label18);
			this.tabOracle.Controls.Add(this.rbtnOrclSpecificUsernamePass);
			this.tabOracle.Controls.Add(this.txtOrclDataSource);
			this.tabOracle.Controls.Add(this.rbtnOrclWindowsAuthentication);
			this.tabOracle.Controls.Add(this.label20);
			this.tabOracle.Controls.Add(this.txtOrclConnectTimeout);
			this.tabOracle.Controls.Add(this.label21);
			this.tabOracle.Controls.Add(this.txtOrclUsername);
			this.tabOracle.Controls.Add(this.label22);
			this.tabOracle.Controls.Add(this.txtOrclPassword);
			this.tabOracle.Location = new System.Drawing.Point(4, 25);
			this.tabOracle.Name = "tabOracle";
			this.tabOracle.Padding = new System.Windows.Forms.Padding(3);
			this.tabOracle.Size = new System.Drawing.Size(332, 221);
			this.tabOracle.TabIndex = 2;
			this.tabOracle.Text = "Oracle";
			this.tabOracle.ToolTipText = "Oracle 8i/ 9i/ 10g/ 11g";
			this.tabOracle.UseVisualStyleBackColor = true;
			// 
			// lnkOracleConnStr
			// 
			this.lnkOracleConnStr.AutoSize = true;
			this.lnkOracleConnStr.Location = new System.Drawing.Point(308, 9);
			this.lnkOracleConnStr.Name = "lnkOracleConnStr";
			this.lnkOracleConnStr.Size = new System.Drawing.Size(13, 13);
			this.lnkOracleConnStr.TabIndex = 1;
			this.lnkOracleConnStr.TabStop = true;
			this.lnkOracleConnStr.Text = "?";
			this.lnkOracleConnStr.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkOracleConnStr_LinkClicked);
			// 
			// chkOrclUserRoleSYSDBA
			// 
			this.chkOrclUserRoleSYSDBA.AutoSize = true;
			this.chkOrclUserRoleSYSDBA.Location = new System.Drawing.Point(218, 186);
			this.chkOrclUserRoleSYSDBA.Name = "chkOrclUserRoleSYSDBA";
			this.chkOrclUserRoleSYSDBA.Size = new System.Drawing.Size(69, 17);
			this.chkOrclUserRoleSYSDBA.TabIndex = 8;
			this.chkOrclUserRoleSYSDBA.Text = "SYSDBA";
			this.chkOrclUserRoleSYSDBA.UseVisualStyleBackColor = true;
			// 
			// btnOrclTestConnection
			// 
			this.btnOrclTestConnection.Location = new System.Drawing.Point(106, 182);
			this.btnOrclTestConnection.Name = "btnOrclTestConnection";
			this.btnOrclTestConnection.Size = new System.Drawing.Size(98, 23);
			this.btnOrclTestConnection.TabIndex = 9;
			this.btnOrclTestConnection.Text = "Test connection";
			this.btnOrclTestConnection.UseVisualStyleBackColor = true;
			this.btnOrclTestConnection.Click += new System.EventHandler(this.btnOrclTestConnection_Click);
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(3, 9);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(97, 13);
			this.label18.TabIndex = 13;
			this.label18.Text = "Data source name:";
			// 
			// rbtnOrclSpecificUsernamePass
			// 
			this.rbtnOrclSpecificUsernamePass.AutoSize = true;
			this.rbtnOrclSpecificUsernamePass.Checked = true;
			this.rbtnOrclSpecificUsernamePass.Location = new System.Drawing.Point(106, 81);
			this.rbtnOrclSpecificUsernamePass.Name = "rbtnOrclSpecificUsernamePass";
			this.rbtnOrclSpecificUsernamePass.Size = new System.Drawing.Size(181, 17);
			this.rbtnOrclSpecificUsernamePass.TabIndex = 4;
			this.rbtnOrclSpecificUsernamePass.TabStop = true;
			this.rbtnOrclSpecificUsernamePass.Text = "Specific username and password";
			this.rbtnOrclSpecificUsernamePass.UseVisualStyleBackColor = true;
			// 
			// txtOrclDataSource
			// 
			this.txtOrclDataSource.Location = new System.Drawing.Point(106, 6);
			this.txtOrclDataSource.Name = "txtOrclDataSource";
			this.txtOrclDataSource.Size = new System.Drawing.Size(200, 20);
			this.txtOrclDataSource.TabIndex = 0;
			// 
			// rbtnOrclWindowsAuthentication
			// 
			this.rbtnOrclWindowsAuthentication.AutoSize = true;
			this.rbtnOrclWindowsAuthentication.Location = new System.Drawing.Point(106, 58);
			this.rbtnOrclWindowsAuthentication.Name = "rbtnOrclWindowsAuthentication";
			this.rbtnOrclWindowsAuthentication.Size = new System.Drawing.Size(139, 17);
			this.rbtnOrclWindowsAuthentication.TabIndex = 3;
			this.rbtnOrclWindowsAuthentication.Text = "Windows authentication";
			this.rbtnOrclWindowsAuthentication.UseVisualStyleBackColor = true;
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Location = new System.Drawing.Point(44, 133);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(56, 13);
			this.label20.TabIndex = 14;
			this.label20.Text = "Password:";
			// 
			// txtOrclConnectTimeout
			// 
			this.txtOrclConnectTimeout.Location = new System.Drawing.Point(106, 32);
			this.txtOrclConnectTimeout.MaxLength = 3;
			this.txtOrclConnectTimeout.Name = "txtOrclConnectTimeout";
			this.txtOrclConnectTimeout.Size = new System.Drawing.Size(200, 20);
			this.txtOrclConnectTimeout.TabIndex = 2;
			this.txtOrclConnectTimeout.Text = "15";
			// 
			// label21
			// 
			this.label21.AutoSize = true;
			this.label21.Location = new System.Drawing.Point(42, 107);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(58, 13);
			this.label21.TabIndex = 11;
			this.label21.Text = "Username:";
			// 
			// txtOrclUsername
			// 
			this.txtOrclUsername.Location = new System.Drawing.Point(106, 104);
			this.txtOrclUsername.Name = "txtOrclUsername";
			this.txtOrclUsername.Size = new System.Drawing.Size(200, 20);
			this.txtOrclUsername.TabIndex = 5;
			this.txtOrclUsername.TextChanged += new System.EventHandler(this.txtOrclUsername_TextChanged);
			// 
			// label22
			// 
			this.label22.AutoSize = true;
			this.label22.Location = new System.Drawing.Point(13, 35);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(87, 13);
			this.label22.TabIndex = 10;
			this.label22.Text = "Connect timeout:";
			// 
			// txtOrclPassword
			// 
			this.txtOrclPassword.Location = new System.Drawing.Point(106, 130);
			this.txtOrclPassword.Name = "txtOrclPassword";
			this.txtOrclPassword.Size = new System.Drawing.Size(200, 20);
			this.txtOrclPassword.TabIndex = 6;
			// 
			// tabSQLite
			// 
			this.tabSQLite.Controls.Add(this.btnSQLiteSelectDatabase);
			this.tabSQLite.Controls.Add(this.btnSQLiteTestConnection);
			this.tabSQLite.Controls.Add(this.label16);
			this.tabSQLite.Controls.Add(this.txtSQLiteDatabaseName);
			this.tabSQLite.Controls.Add(this.label17);
			this.tabSQLite.Controls.Add(this.txtSQLiteConnectTimeout);
			this.tabSQLite.Controls.Add(this.label19);
			this.tabSQLite.Controls.Add(this.txtSQLitePassword);
			this.tabSQLite.Location = new System.Drawing.Point(4, 25);
			this.tabSQLite.Name = "tabSQLite";
			this.tabSQLite.Padding = new System.Windows.Forms.Padding(3);
			this.tabSQLite.Size = new System.Drawing.Size(332, 221);
			this.tabSQLite.TabIndex = 1;
			this.tabSQLite.Text = "SQLite";
			this.tabSQLite.ToolTipText = "SQLite v3";
			this.tabSQLite.UseVisualStyleBackColor = true;
			// 
			// btnSQLiteSelectDatabase
			// 
			this.btnSQLiteSelectDatabase.Location = new System.Drawing.Point(106, 32);
			this.btnSQLiteSelectDatabase.Name = "btnSQLiteSelectDatabase";
			this.btnSQLiteSelectDatabase.Size = new System.Drawing.Size(98, 23);
			this.btnSQLiteSelectDatabase.TabIndex = 1;
			this.btnSQLiteSelectDatabase.Text = "Browse";
			this.btnSQLiteSelectDatabase.UseVisualStyleBackColor = true;
			this.btnSQLiteSelectDatabase.Click += new System.EventHandler(this.btnSQLiteSelectDatabase_Click);
			// 
			// btnSQLiteTestConnection
			// 
			this.btnSQLiteTestConnection.Location = new System.Drawing.Point(106, 122);
			this.btnSQLiteTestConnection.Name = "btnSQLiteTestConnection";
			this.btnSQLiteTestConnection.Size = new System.Drawing.Size(98, 23);
			this.btnSQLiteTestConnection.TabIndex = 4;
			this.btnSQLiteTestConnection.Text = "Test connection";
			this.btnSQLiteTestConnection.UseVisualStyleBackColor = true;
			this.btnSQLiteTestConnection.Click += new System.EventHandler(this.btnSQLiteTestConnection_Click);
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(11, 9);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(89, 13);
			this.label16.TabIndex = 13;
			this.label16.Text = "SQLite database:";
			// 
			// txtSQLiteDatabaseName
			// 
			this.txtSQLiteDatabaseName.Location = new System.Drawing.Point(106, 6);
			this.txtSQLiteDatabaseName.Name = "txtSQLiteDatabaseName";
			this.txtSQLiteDatabaseName.ReadOnly = true;
			this.txtSQLiteDatabaseName.Size = new System.Drawing.Size(200, 20);
			this.txtSQLiteDatabaseName.TabIndex = 0;
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(44, 99);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(56, 13);
			this.label17.TabIndex = 14;
			this.label17.Text = "Password:";
			// 
			// txtSQLiteConnectTimeout
			// 
			this.txtSQLiteConnectTimeout.Location = new System.Drawing.Point(106, 70);
			this.txtSQLiteConnectTimeout.MaxLength = 3;
			this.txtSQLiteConnectTimeout.Name = "txtSQLiteConnectTimeout";
			this.txtSQLiteConnectTimeout.Size = new System.Drawing.Size(200, 20);
			this.txtSQLiteConnectTimeout.TabIndex = 2;
			this.txtSQLiteConnectTimeout.Text = "30";
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(13, 73);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(87, 13);
			this.label19.TabIndex = 10;
			this.label19.Text = "Connect timeout:";
			// 
			// txtSQLitePassword
			// 
			this.txtSQLitePassword.Location = new System.Drawing.Point(106, 96);
			this.txtSQLitePassword.Name = "txtSQLitePassword";
			this.txtSQLitePassword.Size = new System.Drawing.Size(200, 20);
			this.txtSQLitePassword.TabIndex = 3;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnSlider);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(699, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(10, 541);
			this.panel1.TabIndex = 5;
			// 
			// btnSlider
			// 
			this.btnSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSlider.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
			this.btnSlider.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSlider.Image = global::SalarDbCodeGenerator.Properties.Resources.ToLeft10;
			this.btnSlider.Location = new System.Drawing.Point(0, 203);
			this.btnSlider.Name = "btnSlider";
			this.btnSlider.Size = new System.Drawing.Size(10, 135);
			this.btnSlider.TabIndex = 0;
			this.btnSlider.UseVisualStyleBackColor = false;
			this.btnSlider.Click += new System.EventHandler(this.btnSlider_Click);
			// 
			// dlgOpenSqlCe
			// 
			this.dlgOpenSqlCe.DefaultExt = "SqlCE 4 Database (*.sdf;*.*)|*.sdf;*.*";
			this.dlgOpenSqlCe.Filter = "SqlCE 4 Database (*.sdf;*.*)|*.sdf;*.*";
			this.dlgOpenSqlCe.Title = "Select SqlCE 4 database";
			// 
			// txtOrclDbName
			// 
			this.txtOrclDbName.Location = new System.Drawing.Point(106, 156);
			this.txtOrclDbName.Name = "txtOrclDbName";
			this.txtOrclDbName.Size = new System.Drawing.Size(200, 20);
			this.txtOrclDbName.TabIndex = 7;
			// 
			// label23
			// 
			this.label23.AutoSize = true;
			this.label23.Location = new System.Drawing.Point(15, 159);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(85, 13);
			this.label23.TabIndex = 16;
			this.label23.Text = "Database name:";
			// 
			// frmProjectDetails
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(709, 541);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.gpGenOptions);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "frmProjectDetails";
			this.Text = "Project Options";
			this.Load += new System.EventHandler(this.frmNewProject_Load);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.gpGenOptions.ResumeLayout(false);
			this.gpGenOptions.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.pagerDatabaseProvider.ResumeLayout(false);
			this.tabSQLServer.ResumeLayout(false);
			this.tabSQLServer.PerformLayout();
			this.tabSqlCe.ResumeLayout(false);
			this.tabSqlCe.PerformLayout();
			this.tabOracle.ResumeLayout(false);
			this.tabOracle.PerformLayout();
			this.tabSQLite.ResumeLayout(false);
			this.tabSQLite.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtSqlDbName;
		private System.Windows.Forms.TextBox txtSqlHost;
		private System.Windows.Forms.TextBox txtProjectName;
		private System.Windows.Forms.RadioButton rbtnSqlAuthentication;
		private System.Windows.Forms.RadioButton rbtnSqlWindowsAuthentication;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtSqlPassword;
		private System.Windows.Forms.TextBox txtSqlUsername;
		private System.Windows.Forms.Button btnSqlTestConnection;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtGenerationPath;
		private System.Windows.Forms.TextBox txtNamespace;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnGenPathBrowse;
		private System.Windows.Forms.FolderBrowserDialog dlgGenBrowse;
		private System.Windows.Forms.Button btnPatternProject;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtPatternfile;
		private System.Windows.Forms.OpenFileDialog dlgOpenPattern;
		private System.Windows.Forms.GroupBox gpGenOptions;
		private System.Windows.Forms.ComboBox lstIgnoredSuffixes;
		private System.Windows.Forms.ComboBox lstIgnoredPrefixes;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox txtPrefixForTables;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox txtSuffixForTables;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox txtSuffixForViews;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox txtPrefixForViews;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Button btnDelIgnoredSuffixes;
		private System.Windows.Forms.Button btnAddIgnoredSuffixes;
		private System.Windows.Forms.Button btnDelIgnoredPrefixes;
		private System.Windows.Forms.Button btnAddIgnoredPrefixes;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox txtSqlConnectTimeout;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.CheckBox chkGenForeignKey;
		private System.Windows.Forms.CheckBox chkGenDescription;
		private System.Windows.Forms.TabControl pagerDatabaseProvider;
		private System.Windows.Forms.TabPage tabSQLServer;
		private System.Windows.Forms.TabPage tabSQLite;
		private System.Windows.Forms.Button btnSQLiteSelectDatabase;
		private System.Windows.Forms.Button btnSQLiteTestConnection;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox txtSQLiteDatabaseName;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.TextBox txtSQLiteConnectTimeout;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.TextBox txtSQLitePassword;
		private System.Windows.Forms.OpenFileDialog dlgOpenSQLite;
		private System.Windows.Forms.CheckBox chkTableConstraintKeys;
		private System.Windows.Forms.ToolTip tipHints;
		private System.Windows.Forms.TabPage tabOracle;
		private System.Windows.Forms.Button btnOrclTestConnection;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.RadioButton rbtnOrclSpecificUsernamePass;
		private System.Windows.Forms.TextBox txtOrclDataSource;
		private System.Windows.Forms.RadioButton rbtnOrclWindowsAuthentication;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TextBox txtOrclConnectTimeout;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.TextBox txtOrclUsername;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.TextBox txtOrclPassword;
		private System.Windows.Forms.CheckBox chkOrclUserRoleSYSDBA;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnSlider;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.ComboBox cmbRenamingCase;
		private System.Windows.Forms.CheckBox chkRenamingCase;
		private System.Windows.Forms.CheckBox chkRenCaseProps;
		private System.Windows.Forms.CheckBox chkRenUnderlineProps;
		private System.Windows.Forms.CheckBox chkRenCaseTables;
		private System.Windows.Forms.CheckBox chkRenUnderlineTables;
		private System.Windows.Forms.CheckBox chkRenRemUnderline;
		private System.Windows.Forms.CheckBox chkRenUnderlineWordDelimiter;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.TabPage tabSqlCe;
		private System.Windows.Forms.Button btnSqlCeSelectDatabase;
		private System.Windows.Forms.Button btnSqlCeTestConnection;
		private System.Windows.Forms.TextBox txtSqlCeDatabaseName;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.TextBox txtSqlCePassword;
		private System.Windows.Forms.OpenFileDialog dlgOpenSqlCe;
		private System.Windows.Forms.LinkLabel lnkOracleConnStr;
		private System.Windows.Forms.TextBox txtOrclDbName;
		private System.Windows.Forms.Label label23;
	}
}