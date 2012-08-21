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
	/// Constraint key details for table
	/// </summary>
	public class DbConstraintKey
	{
		#region properties
		/// <summary>
		/// Name of ForeignKey
		/// </summary>
		public string KeyName { get; set; }

		/// <summary>
		/// The column name used 
		/// </summary>
		public string KeyColumnName { get; set; }

		/// <summary>
		/// The column name used 
		/// </summary>
		public DbColumn KeyColumn { get; set; }

		/// <summary>
		/// Is constraint key
		/// </summary>
		public bool IsUnique { get; set; }
		#endregion

		public override string ToString()
		{
			return KeyName + ", " + KeyColumnName;
		}
	}
}
