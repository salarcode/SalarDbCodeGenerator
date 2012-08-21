using System;
using System.Data;
using System.Data.SQLite;
using System.Data.SqlServerCe;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using SalarSoft.DbCodeGenerator.CodeGen.DbSchema;
using SalarSoft.DbCodeGenerator.CodeGen.SchemaEngines;

namespace SalarSoft.DbCodeGenerator
{
	public partial class frmTest : Form
	{
		public frmTest()
		{
			InitializeComponent();
		}

		private void btnSchema_Click(object sender, EventArgs e)
		{
			SqliteTestMethods();
		}


		void SqliteTestMethods()
		{
			string connStr = @"data source=" + txtSqliteDB.Text;
			var conn = new SQLiteConnection(connStr);
			var engine =new SQLiteSchemaEngine(conn);

			var database = new SchemaDatabase();
			engine.ReadTablesForeignKeys = true;
			engine.FillSchema(database);
		}


		private void OracleGetShcema()
		{
			try
			{
				using (var conn = new OracleConnection(txtOrclConn.Text))
				{
					conn.Open();
					string[] restriction = new string[] { txtOrclOwner.Text };

					DataTable schema;
					if (txtOrclSchemaName.Text.ToLower() == "datatypes")
					{
						schema = conn.GetSchema(txtOrclSchemaName.Text);
					}
					else if (txtOrclSchemaName.Text.Length > 0)
					{
						schema = conn.GetSchema(txtOrclSchemaName.Text, restriction);
					}
					else
						schema = conn.GetSchema();

					grdGrid.DataSource = schema;
					grdColumns.DataSource = GetColumns(schema.Columns);
				}
			}
			catch
			{
				throw;
			}
		}

		private void Exitapp_Click(object sender, EventArgs e)
		{
			Environment.Exit(0);
		}

		private void txtSqliteDB_TextChanged(object sender, EventArgs e)
		{

		}

		private void btnOracleSchema_Click(object sender, EventArgs e)
		{
			OracleGetShcema();
		}

		private void frmTest_Load(object sender, EventArgs e)
		{

		}

		private void btnRunOracle_Click(object sender, EventArgs e)
		{
			try
			{
				using (var conn = new OracleConnection(txtOrclConn.Text))
				using (var da = new OracleDataAdapter(txtCmd.Text, conn))
				{
					var schema = new DataTable();
					da.Fill(schema);

					grdGrid.DataSource = schema;
					grdColumns.DataSource = GetColumns(schema.Columns);
				}
			}
			catch
			{
				throw;
			}
		}

		private void SQLCESchema_Click(object sender, EventArgs e)
		{
			try
			{
				using (var conn = new SqlCeConnection(txtSqlCeConn.Text))
				{
					conn.Open();
					//string[] restriction = new string[] { txtOrclOwner.Text };

					DataTable schema;
					if (txtSqlCeSchemaName.Text.Length > 0)
					{
						schema = conn.GetSchema(txtSqlCeSchemaName.Text);//, restriction);
					}
					else
						schema = conn.GetSchema();

					grdGrid.DataSource = schema;
					grdColumns.DataSource = GetColumns(schema.Columns);
				}
			}
			catch
			{
				throw;
			}
		}

		DataTable GetColumns(DataColumnCollection coll)
		{
			var r = new DataTable();
			r.Columns.Add("N");
			foreach (DataColumn c in coll)
			{
				r.Rows.Add(c.ColumnName);
			}

			return r;
		}

		private void RunSqlCeCmd_Click(object sender, EventArgs e)
		{
			try
			{
				using (var conn = new SqlCeConnection(txtSqlCeConn.Text))
				using (var da = new SqlCeDataAdapter(txtCmd.Text, conn))
				{
					DataTable schema = new DataTable();
					da.Fill(schema);

					grdGrid.DataSource = schema;
					grdColumns.DataSource = GetColumns(schema.Columns);
				}
			}
			catch
			{
				throw;
			}
		}

	}
}
