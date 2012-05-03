using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SalarSoft.DbCodeGenerator.CodeGen.DbSchema;
using System.Collections.Specialized;

namespace SalarSoft.DbCodeGenerator.CodeGen.SchemaEngines
{
	public abstract class ExSchemaEngine
	{
		/// <summary>
		/// Determines if the engine should read columns description, if is supported.
		/// </summary>
		public bool ReadColumnsDescription { get; set; }

		/// <summary>
		/// Determines if the engine should read tables foreign keys, if is supported.
		/// </summary>
		public bool ReadTablesForeignKeys { get; set; }

		/// <summary>
		/// Determines if the engine should generate tables constraint keys, if supported.
		/// </summary>
		public bool ReadConstraintKeys { get; set; }

		/// <summary>
		/// [Oracle only] Only get list from specific owner
		/// </summary>
		public string SpecificOwner { get; set; } 


		public abstract void FillSchema(SchemaDatabase schemaDatabase);
		public abstract void ReadViewsTablesList(out StringCollection tables, out StringCollection views);

		/// <summary>
		/// Gets data provider class name
		/// </summary>
		/// <returns></returns>
		public abstract string GetDataProviderClassName(DataProviderClassNames providerClassName);

	}
}
