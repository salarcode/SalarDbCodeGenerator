using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Salar Khalilzadeh <salar2k@gmail.com>
// © 2012, All rights reserved
// 2012/07/06
// ====================================
namespace SalarDbCodeGenerator.Schema.Database
{
	public class DbView : DbTable
	{
		public DbView(string viewName)
			: base(viewName)
		{
			ReadOnly = true;
			TableType = TableTypeInfo.View;
		}
		public DbView(string viewName, List<DbColumn> schemaColumns)
			: base(viewName, schemaColumns)
		{
			ReadOnly = true;
			TableType = TableTypeInfo.View;
		}
	}
}
