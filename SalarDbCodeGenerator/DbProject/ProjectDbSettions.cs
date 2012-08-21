using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Data.SqlServerCe;
using System.Xml.Serialization;
using Oracle.DataAccess.Client;
using SalarDbCodeGenerator.Schema.DbSchemaReaders;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Salar Khalilzadeh <salar2k@gmail.com>
// © 2012, All rights reserved
// 2012/07/06
// ====================================
namespace SalarDbCodeGenerator.DbProject
{
	[Serializable]
	public class ProjectDbSettions
	{
		public struct SelectedTablesType
		{
			[XmlText]
			public string Name;
			[XmlAttribute]
			public bool Selected;
		}
		#region properties
		public DatabaseProvider DatabaseProvider { get; set; }
		public string DatabaseName { get; set; }
		public string ServerName { get; set; }
		public bool UseSqlAuthentication { get; set; }
		public string SqlUsername { get; set; }
		public string SqlPassword { get; set; }
		public int ConnectTimeout { get; set; }
		public DateTime LastFetch { get; set; }
		public bool OracleUseSysdbaRole { get; set; }

		public string PrefixForTables { get; set; }
		public string PrefixForViews { get; set; }
		public string SuffixForTables { get; set; }
		public string SuffixForViews { get; set; }

		public StringCollection IgnoredPrefixes { get; set; }
		public StringCollection IgnoredSuffixes { get; set; }

		[XmlElement("Table")]
		public List<SelectedTablesType> Tables { get; set; }
		[XmlElement("View")]
		public List<SelectedTablesType> Views { get; set; }
		#endregion

		#region public methods
		public ProjectDbSettions()
		{
			DatabaseProvider = DatabaseProvider.SQLServer;
			DatabaseName = "Master";
			ServerName = "(local)";
			UseSqlAuthentication = false;
			SqlUsername = "";
			SqlPassword = "";

			// Prefix and suffixes
			PrefixForTables = null;
			PrefixForViews = "vw";
			SuffixForTables = null;
			SuffixForViews = null;

			// 30 seconds
			ConnectTimeout = 15;
			LastFetch = DateTime.MinValue;
			IgnoredPrefixes = new StringCollection();
			IgnoredSuffixes = new StringCollection();
			Tables = new List<SelectedTablesType>();
			Views = new List<SelectedTablesType>();
		}

		public static ProjectDbSettions LoadDefaultSettings()
		{
			ProjectDbSettions settings = new ProjectDbSettions();

			settings.DatabaseProvider = DatabaseProvider.SQLServer;

			settings.IgnoredPrefixes.Add("tbl_");
			settings.IgnoredPrefixes.Add("tbl");
			settings.IgnoredPrefixes.Add("vw_");
			settings.IgnoredPrefixes.Add("vw");

			settings.IgnoredSuffixes.Add("tbl");
			settings.IgnoredSuffixes.Add("_tbl");
			settings.IgnoredSuffixes.Add("_vw");
			settings.IgnoredSuffixes.Add("vw");

			return settings;
		}

		public DbConnection GetNewConnection()
		{
			switch (DatabaseProvider)
			{
				case DatabaseProvider.SQLServer:
					return new SqlConnection(GetConnectionString());

				case DatabaseProvider.Oracle:
					return new OracleConnection(GetConnectionString());

				case DatabaseProvider.SqlCe4:
					return new SqlCeConnection(GetConnectionString());

				case DatabaseProvider.SQLite:
					var conn = new SQLiteConnection(GetConnectionString());
					if (!string.IsNullOrEmpty(SqlPassword))
					{
						conn.SetPassword(SqlPassword);
					}
					return conn;

				default:
					throw new NotSupportedException("Database type is not supported");
			}
		}

		public ExSchemaEngine GetSchemaEngine(DbConnection dbConnection)
		{
			switch (DatabaseProvider)
			{
				case DatabaseProvider.SQLServer:
					return new SQLSchemaEngine(dbConnection);

				case DatabaseProvider.Oracle:
					return new OracleSchemaEngine(dbConnection);

				case DatabaseProvider.SQLite:
					return new SQLiteSchemaEngine(dbConnection);

				case DatabaseProvider.SqlCe4:
					return new SQLCeSchemaEngine(dbConnection);

				default:
					throw new NotSupportedException("Database type is not supported");
			}
		}

		public string GetConnectionString()
		{
			string connStr = "";
			switch (DatabaseProvider)
			{
				case DatabaseProvider.SQLServer:
					if (UseSqlAuthentication)
						connStr = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Pwd={3};Connect Timeout={4}",
							ServerName,
							DatabaseName,
							SqlUsername,
							SqlPassword,
							ConnectTimeout
						);
					else
						connStr = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True;Connect Timeout={2}",
							ServerName,
							DatabaseName,
							ConnectTimeout
						);
					break;

				case DatabaseProvider.SqlCe4:
					if (UseSqlAuthentication)
						connStr = string.Format("Data Source={0};Password={1}",
							ServerName,
							SqlPassword
						);
					else
						connStr = string.Format("Data Source={0};",
							ServerName
						);
					break;

				case DatabaseProvider.Oracle:
					const string connSpecificUser = "Data Source={0};User Id={1};Password={2};";
					const string connIntegratedSecurity = "Data Source={0};Integrated Security=SSPI;";
					const string connDbaPrivilege = "DBA PRIVILEGE=SYSDBA;";

					if (UseSqlAuthentication)
					{
						connStr = string.Format(connSpecificUser, ServerName, SqlUsername, SqlPassword);
					}
					else
						connStr = string.Format(connIntegratedSecurity, ServerName);

					if (OracleUseSysdbaRole)
						connStr += connDbaPrivilege;

					break;

				case DatabaseProvider.SQLite:
					connStr = string.Format("data source=={0};Default Timeout={1}",
						ServerName,
						ConnectTimeout
					);

					break;
			}
			return connStr;
		}

		/// <summary>
		/// Test connection if works
		/// </summary>
		public bool TestConnection()
		{
			try
			{
				using (DbConnection conn = GetNewConnection())
					conn.Open();
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Reloads tables and views list from database
		/// </summary>
		public void RefetchDatabaseCache()
		{
			using (DbConnection conn = GetNewConnection())
			using (ExSchemaEngine engine = GetSchemaEngine(conn))
			{
				StringCollection dbTables, dbViews;

				// Open the connection
				conn.Open();

				// Woot! no error!

				// the schema engine

				// shcema engine options
				engine.SpecificOwner = this.SqlUsername;

				// Read schema list from db
				engine.ReadViewsTablesList(out dbTables, out dbViews);

				List<SelectedTablesType> newTables = new List<SelectedTablesType>();
				List<SelectedTablesType> newViews = new List<SelectedTablesType>();

				// Adding new tables
				foreach (var tableName in dbTables)
				{
					bool added = false;
					// looking for previous setting
					foreach (var prevTable in Tables)
						if (prevTable.Name == tableName)
						{
							newTables.Add(new SelectedTablesType()
							{
								Name = tableName,
								Selected = prevTable.Selected
							});
							added = true;
							break;
						}

					if (added) continue;

					// nothing found, add as not selected
					newTables.Add(new SelectedTablesType()
					{
						Name = tableName,
						Selected = false
					});
				}

				// Adding new views
				foreach (var viewName in dbViews)
				{
					bool added = false;

					// looking for previous settings
					foreach (var prevView in Views)
						if (prevView.Name == viewName)
						{
							newViews.Add(new SelectedTablesType()
							{
								Name = viewName,
								Selected = prevView.Selected
							});
							added = true;
							break;
						}

					if (added) continue;

					// nothing found, add as not selected
					newViews.Add(new SelectedTablesType()
					{
						Name = viewName,
						Selected = false
					});
				}

				// Replacing the old ones
				Tables.Clear();
				Views.Clear();
				Tables = newTables;
				Views = newViews;

				// sort
				Views.Sort((x, y) => string.Compare(x.Name, y.Name));
				Tables.Sort((x, y) => string.Compare(x.Name, y.Name));
			}
		}

		public StringCollection GetSelectedTablesList()
		{
			var result = new StringCollection();
			foreach (var table in Tables)
			{
				if (table.Selected)
				{
					result.Add(table.Name);
				}
			}
			return result;
		}

		public StringCollection GetSelectedViewsList()
		{
			var result = new StringCollection();
			foreach (var view in Views)
			{
				if (view.Selected)
				{
					result.Add(view.Name);
				}
			}
			return result;
		}

		public bool IsTableSelected(string tableName)
		{
			foreach (var table in Tables)
			{
				if (table.Name == tableName)
				{
					return table.Selected;
				}
			}
			return false;
		}
		public bool IsViewSelected(string viewName)
		{
			foreach (var view in Views)
			{
				if (view.Name == viewName)
				{
					return view.Selected;
				}
			}
			return false;
		}

		public bool HasSelectedView()
		{
			foreach (var view in Views)
			{
				if (view.Selected)
				{
					return true;
				}
			}
			return false;
		}
		#endregion

		#region protected methods
		#endregion

		#region private methods
		#endregion

	}
}
