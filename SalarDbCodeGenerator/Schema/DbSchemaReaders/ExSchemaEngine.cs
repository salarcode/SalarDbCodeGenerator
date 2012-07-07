using System.Collections.Specialized;
using SalarDbCodeGenerator.Schema.Database;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2012/07/06
// ====================================
namespace SalarDbCodeGenerator.Schema.DbSchemaReaders
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


		public abstract void FillSchema(DbDatabase schemaDatabase);
		public abstract void ReadViewsTablesList(out StringCollection tables, out StringCollection views);

		/// <summary>
		/// Gets data provider class name
		/// </summary>
		/// <returns></returns>
		public abstract string GetDataProviderClassName(DataProviderClassNames providerClassName);

	}
}
