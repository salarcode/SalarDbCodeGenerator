namespace SalarSoft.DbCodeGenerator
{
	partial class frmTest
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
			this.grdGrid = new System.Windows.Forms.DataGridView();
			this.txtSqliteDB = new System.Windows.Forms.TextBox();
			this.btnSchema = new System.Windows.Forms.Button();
			this.Exitapp = new System.Windows.Forms.Button();
			this.btnOracleSchema = new System.Windows.Forms.Button();
			this.txtOrclConn = new System.Windows.Forms.TextBox();
			this.txtOrclSchemaName = new System.Windows.Forms.TextBox();
			this.txtOrclOwner = new System.Windows.Forms.TextBox();
			this.txtCmd = new System.Windows.Forms.TextBox();
			this.btnRunOracle = new System.Windows.Forms.Button();
			this.SQLCESchema = new System.Windows.Forms.Button();
			this.txtSqlCeConn = new System.Windows.Forms.TextBox();
			this.txtSqlCeSchemaName = new System.Windows.Forms.TextBox();
			this.grdColumns = new System.Windows.Forms.DataGridView();
			this.RunSqlCeCmd = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.grdGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grdColumns)).BeginInit();
			this.SuspendLayout();
			// 
			// grdGrid
			// 
			this.grdGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grdGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.grdGrid.Location = new System.Drawing.Point(12, 203);
			this.grdGrid.Name = "grdGrid";
			this.grdGrid.Size = new System.Drawing.Size(572, 356);
			this.grdGrid.TabIndex = 11;
			// 
			// txtSqliteDB
			// 
			this.txtSqliteDB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtSqliteDB.Location = new System.Drawing.Point(12, 12);
			this.txtSqliteDB.Name = "txtSqliteDB";
			this.txtSqliteDB.Size = new System.Drawing.Size(704, 20);
			this.txtSqliteDB.TabIndex = 0;
			this.txtSqliteDB.Text = "G:\\Programming\\C#.Net\\SalarSoft.DbCodeGenerator\\SalarSoft.DbCodeGenerator\\SampleD" +
    "B\\SQLiteSampleDb.sqlite";
			this.txtSqliteDB.TextChanged += new System.EventHandler(this.txtSqliteDB_TextChanged);
			// 
			// btnSchema
			// 
			this.btnSchema.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSchema.Location = new System.Drawing.Point(722, 10);
			this.btnSchema.Name = "btnSchema";
			this.btnSchema.Size = new System.Drawing.Size(94, 23);
			this.btnSchema.TabIndex = 1;
			this.btnSchema.Text = "SQLiteSchema";
			this.btnSchema.UseVisualStyleBackColor = true;
			this.btnSchema.Click += new System.EventHandler(this.btnSchema_Click);
			// 
			// Exitapp
			// 
			this.Exitapp.Location = new System.Drawing.Point(12, 174);
			this.Exitapp.Name = "Exitapp";
			this.Exitapp.Size = new System.Drawing.Size(75, 23);
			this.Exitapp.TabIndex = 13;
			this.Exitapp.Text = "Exit App";
			this.Exitapp.UseVisualStyleBackColor = true;
			this.Exitapp.Click += new System.EventHandler(this.Exitapp_Click);
			// 
			// btnOracleSchema
			// 
			this.btnOracleSchema.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOracleSchema.Location = new System.Drawing.Point(722, 39);
			this.btnOracleSchema.Name = "btnOracleSchema";
			this.btnOracleSchema.Size = new System.Drawing.Size(94, 23);
			this.btnOracleSchema.TabIndex = 5;
			this.btnOracleSchema.Text = "OracleSchema";
			this.btnOracleSchema.UseVisualStyleBackColor = true;
			this.btnOracleSchema.Click += new System.EventHandler(this.btnOracleSchema_Click);
			// 
			// txtOrclConn
			// 
			this.txtOrclConn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtOrclConn.Location = new System.Drawing.Point(12, 39);
			this.txtOrclConn.Name = "txtOrclConn";
			this.txtOrclConn.Size = new System.Drawing.Size(488, 20);
			this.txtOrclConn.TabIndex = 2;
			this.txtOrclConn.Text = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.154.1" +
    "30)(PORT=1522)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl)));USER ID=SE" +
    "CRETARIAT;Password=1;";
			this.txtOrclConn.TextChanged += new System.EventHandler(this.txtSqliteDB_TextChanged);
			// 
			// txtOrclSchemaName
			// 
			this.txtOrclSchemaName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtOrclSchemaName.Location = new System.Drawing.Point(612, 39);
			this.txtOrclSchemaName.Name = "txtOrclSchemaName";
			this.txtOrclSchemaName.Size = new System.Drawing.Size(104, 20);
			this.txtOrclSchemaName.TabIndex = 4;
			this.txtOrclSchemaName.Text = "Views";
			// 
			// txtOrclOwner
			// 
			this.txtOrclOwner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtOrclOwner.Location = new System.Drawing.Point(506, 38);
			this.txtOrclOwner.Name = "txtOrclOwner";
			this.txtOrclOwner.Size = new System.Drawing.Size(100, 20);
			this.txtOrclOwner.TabIndex = 3;
			this.txtOrclOwner.Text = "SECRETARIAT";
			// 
			// txtCmd
			// 
			this.txtCmd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtCmd.Location = new System.Drawing.Point(13, 66);
			this.txtCmd.Multiline = true;
			this.txtCmd.Name = "txtCmd";
			this.txtCmd.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtCmd.Size = new System.Drawing.Size(703, 48);
			this.txtCmd.TabIndex = 6;
			// 
			// btnRunOracle
			// 
			this.btnRunOracle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRunOracle.Location = new System.Drawing.Point(722, 68);
			this.btnRunOracle.Name = "btnRunOracle";
			this.btnRunOracle.Size = new System.Drawing.Size(94, 23);
			this.btnRunOracle.TabIndex = 7;
			this.btnRunOracle.Text = "RunOracle";
			this.btnRunOracle.UseVisualStyleBackColor = true;
			this.btnRunOracle.Click += new System.EventHandler(this.btnRunOracle_Click);
			// 
			// SQLCESchema
			// 
			this.SQLCESchema.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SQLCESchema.Location = new System.Drawing.Point(722, 121);
			this.SQLCESchema.Name = "SQLCESchema";
			this.SQLCESchema.Size = new System.Drawing.Size(94, 23);
			this.SQLCESchema.TabIndex = 10;
			this.SQLCESchema.Text = "SqlCeSchema";
			this.SQLCESchema.UseVisualStyleBackColor = true;
			this.SQLCESchema.Click += new System.EventHandler(this.SQLCESchema_Click);
			// 
			// txtSqlCeConn
			// 
			this.txtSqlCeConn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtSqlCeConn.Location = new System.Drawing.Point(13, 121);
			this.txtSqlCeConn.Name = "txtSqlCeConn";
			this.txtSqlCeConn.Size = new System.Drawing.Size(593, 20);
			this.txtSqlCeConn.TabIndex = 8;
			this.txtSqlCeConn.Text = "Data Source=E:\\Programming\\C#.NET\\SalarSoft.DbCodeGenerator\\SalarSoft.DbCodeGener" +
    "ator\\SampleDB\\CodeGenCE4.sdf;Password=1";
			// 
			// txtSqlCeSchemaName
			// 
			this.txtSqlCeSchemaName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtSqlCeSchemaName.Location = new System.Drawing.Point(612, 120);
			this.txtSqlCeSchemaName.Name = "txtSqlCeSchemaName";
			this.txtSqlCeSchemaName.Size = new System.Drawing.Size(104, 20);
			this.txtSqlCeSchemaName.TabIndex = 9;
			this.txtSqlCeSchemaName.Text = "Tables";
			// 
			// grdColumns
			// 
			this.grdColumns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grdColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.grdColumns.Location = new System.Drawing.Point(591, 203);
			this.grdColumns.Name = "grdColumns";
			this.grdColumns.Size = new System.Drawing.Size(225, 356);
			this.grdColumns.TabIndex = 12;
			// 
			// RunSqlCeCmd
			// 
			this.RunSqlCeCmd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.RunSqlCeCmd.Location = new System.Drawing.Point(722, 150);
			this.RunSqlCeCmd.Name = "RunSqlCeCmd";
			this.RunSqlCeCmd.Size = new System.Drawing.Size(94, 23);
			this.RunSqlCeCmd.TabIndex = 10;
			this.RunSqlCeCmd.Text = "RunSqlCeCmd";
			this.RunSqlCeCmd.UseVisualStyleBackColor = true;
			this.RunSqlCeCmd.Click += new System.EventHandler(this.RunSqlCeCmd_Click);
			// 
			// frmTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(828, 571);
			this.Controls.Add(this.grdColumns);
			this.Controls.Add(this.txtSqlCeConn);
			this.Controls.Add(this.RunSqlCeCmd);
			this.Controls.Add(this.SQLCESchema);
			this.Controls.Add(this.btnRunOracle);
			this.Controls.Add(this.txtCmd);
			this.Controls.Add(this.txtOrclOwner);
			this.Controls.Add(this.txtSqlCeSchemaName);
			this.Controls.Add(this.txtOrclSchemaName);
			this.Controls.Add(this.Exitapp);
			this.Controls.Add(this.btnOracleSchema);
			this.Controls.Add(this.btnSchema);
			this.Controls.Add(this.txtOrclConn);
			this.Controls.Add(this.txtSqliteDB);
			this.Controls.Add(this.grdGrid);
			this.Name = "frmTest";
			this.Text = "frmTest";
			this.Load += new System.EventHandler(this.frmTest_Load);
			((System.ComponentModel.ISupportInitialize)(this.grdGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grdColumns)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtSqliteDB;
		private System.Windows.Forms.Button btnSchema;
		public System.Windows.Forms.DataGridView grdGrid;
		private System.Windows.Forms.Button Exitapp;
		private System.Windows.Forms.Button btnOracleSchema;
		private System.Windows.Forms.TextBox txtOrclConn;
		private System.Windows.Forms.TextBox txtOrclSchemaName;
		private System.Windows.Forms.TextBox txtOrclOwner;
		private System.Windows.Forms.TextBox txtCmd;
		private System.Windows.Forms.Button btnRunOracle;
		private System.Windows.Forms.Button SQLCESchema;
		private System.Windows.Forms.TextBox txtSqlCeConn;
		private System.Windows.Forms.TextBox txtSqlCeSchemaName;
		private System.Windows.Forms.DataGridView grdColumns;
		private System.Windows.Forms.Button RunSqlCeCmd;
	}
}