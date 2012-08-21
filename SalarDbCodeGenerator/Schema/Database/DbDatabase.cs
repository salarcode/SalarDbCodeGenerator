using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalarDbCodeGenerator.Schema.DbSchemaReaders;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Salar Khalilzadeh <salar2k@gmail.com>
// © 2012, All rights reserved
// 2012/07/06
// ====================================
namespace SalarDbCodeGenerator.Schema.Database
{
	public class DbDatabase
	{
 		#region properties
		public string DatabaseName { get; set; }
		public List<DbTable> SchemaTables { get; set; }
		public List<DbView> SchemaViews { get; set; }

		public DatabaseProvider Provider { get; set; }
 		#endregion

		#region public methods
		public DbDatabase()
		{
			SchemaTables = new List<DbTable>();
			SchemaViews = new List<DbView>();
		}
		#endregion
 	}
}
