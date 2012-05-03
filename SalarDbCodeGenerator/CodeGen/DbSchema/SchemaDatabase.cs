using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2010-5-25
// ====================================
namespace SalarSoft.DbCodeGenerator.CodeGen.DbSchema
{
	public class SchemaDatabase
	{
		#region local variables
		#endregion

		#region field variables
		#endregion

		#region properties
		public string DatabaseName { get; set; }
		public List<SchemaTable> SchemaTables { get; set; }
		public List<SchemaView> SchemaViews { get; set; }

		public DatabaseProvider Provider { get; set; }

		#endregion

		#region public methods
		public SchemaDatabase()
		{
			SchemaTables = new List<SchemaTable>();
			SchemaViews = new List<SchemaView>();
		}
		#endregion

		#region protected methods
		#endregion

		#region private methods
		#endregion

	}
}
