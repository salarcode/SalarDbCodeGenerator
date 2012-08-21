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
	/// <summary>
	/// Foreign key relation details for table
	/// </summary>
	public class DbForeignKey
	{
		public enum ForeignKeyMultiplicity { ManyToOne, OneToMany, OneToOne,/* ManyToMany*/ }
		#region properties
 		/// <summary>
		/// Name of ForeignKey
		/// </summary>
		public string ForeignKeyName { get; set; }

		/// <summary>
		/// Represents the multiplicity information about the End of a relationship type.
		/// </summary>
		public ForeignKeyMultiplicity Multiplicity { get; set; }

		/// <summary>
		/// The column which is used as ForeignKey in current table
		/// </summary>
		public DbColumn LocalColumn { get; set; }

		/// <summary>
		/// The column which is used as ForeignKey in current table
		/// </summary>
		public string LocalColumnName { get; set; }

		/// <summary>
		/// Foreign column used for relation
		/// </summary>
		public DbColumn ForeignColumn { get; set; }

		/// <summary>
		/// Foreign column name used for relation
		/// </summary>
		public string ForeignColumnName { get; set; }

		/// <summary>
		/// Foreign table
		/// </summary>
		public DbTable ForeignTable { get; set; }

		private string _foreignTableName;

		/// <summary>
		/// Foreign table name
		/// </summary>
		public string ForeignTableName
		{
			get { return _foreignTableName; }
			set
			{
				_foreignTableName = value;
				ForeignTableNameInLocalTable = value;
			}
		}

		/// <summary>
		/// Foreign table name in local table
		/// </summary>
		public string ForeignTableNameInLocalTable { get; set; }


		public DbForeignKeyAction UpdateAction { get; set; }
		public DbForeignKeyAction DeleteAction { get; set; }

		public override string ToString()
		{
			return ForeignKeyName + " " + ForeignTableName;
		}
		#endregion
	}
}
