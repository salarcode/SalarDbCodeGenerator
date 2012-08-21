using System;
using System.Collections.Specialized;
using SalarDbCodeGenerator.Schema.Database;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Salar Khalilzadeh <salar2k@gmail.com>
// © 2012, All rights reserved
// 2012/07/06
// ====================================
namespace SalarDbCodeGenerator.Schema.DbSchemaReaders
{
	public abstract class ExSchemaEngine : IDisposable
	{
		/// <summary>
		/// Determines if the engine should read columns description, if supported.
		/// </summary>
		public bool ReadColumnsDescription { get; set; }

		/// <summary>
		/// Determines if the engine should read tables foreign keys, if supported.
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

		/// <summary>
		/// Determine if the reader should only read specified selected items schema
		/// </summary>
		public bool OnlyReadSelectedItems { get; set; }

		/// <summary>
		/// Selected tables list, only their schema will be read.
		/// </summary>
		public StringCollection SelectedTables { get; set; }

		/// <summary>
		/// Selected views list, only their schema will be read.
		/// </summary>
		public StringCollection SelectedViews { get; set; }


		public abstract void FillSchema(DbDatabase schemaDatabase);
		public abstract void ReadViewsTablesList(out StringCollection tables, out StringCollection views);

		/// <summary>
		/// Gets data provider class name
		/// </summary>
		/// <returns></returns>
		public abstract string GetDataProviderClassName(DataProviderClassNames providerClassName);

		public abstract void Dispose();

		/// <summary>
		/// Checking to see if user has selected this table to be generated
		/// </summary>
		protected bool IsTableSelected(string tableName)
		{
			if (!OnlyReadSelectedItems)
				return true;
			if (SelectedTables == null || SelectedTables.Count==0)
				return false;
			return SelectedTables.Contains(tableName);
		}

		/// <summary>
		/// Checking to see if user has selected this view to be generated
		/// </summary>
		protected bool IsViewSelected(string viewName)
		{
			if (!OnlyReadSelectedItems)
				return true;
			if (SelectedViews == null || SelectedViews.Count == 0)
				return false;
			return SelectedViews.Contains(viewName);
		}


	}
}
