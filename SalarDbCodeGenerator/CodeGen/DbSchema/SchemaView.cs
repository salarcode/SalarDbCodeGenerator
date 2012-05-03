using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2009-9-30
// ====================================
namespace SalarSoft.DbCodeGenerator.CodeGen.DbSchema
{
	public class SchemaView : SchemaTable
	{
		public SchemaView(string viewName)
			: base(viewName)
		{
			ReadOnly = true;
			TableType = TableTypeInfo.View;
		}
		public SchemaView(string viewName, List<SchemaColumn> schemaColumns)
			: base(viewName, schemaColumns)
		{
			ReadOnly = true;
			TableType = TableTypeInfo.View;
		}
	}
}
