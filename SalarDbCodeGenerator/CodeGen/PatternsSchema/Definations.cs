using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2010-8-10
// ====================================
namespace SalarSoft.DbCodeGenerator.CodeGen.PatternsSchema
{
	public enum PatternFileAppliesTo
	{
		GeneralOnce = 0,
		TablesAndViews_Each,
		TablesAndViews_All,
		Tables_Each,
		Tables_All,
		Views_Each,
		Views_All,
		ProjectFile
	}

	public enum PatternContentAppliesTo
	{
		General = 0,
		TablesAndViews_All,
		Tables_All,
		Views_All,

		Table,
		Columns,
		ForeignKeys
	}
	
	/// <summary>
	/// Content key mode for pattern content
	/// </summary>
	public enum PatternContentKeyMode
	{
		/// <summary>
		/// ReadOnlyTable, NoPrimaryKey, WithPrimaryKey
		/// </summary>
		TablePrimaryKey = 0,

		/// <summary>
		/// NormalKey
		/// </summary>
		TableForeignKey,

		/// <summary>
		/// NormalKey
		/// </summary>
		TableIndexConstraint,

		/// <summary>
		/// NormalKey
		/// </summary>
		TableUniqueConstraint,

		/// <summary>
		/// NormalField, PrimaryKey
		/// </summary>
		FieldPrimaryKey,

		/// <summary>
		/// MultiplicityOne, MultiplicityMany
		/// </summary>
		FieldForeignKey,

		/// <summary>
		/// AutoInrcementPrimaryKey ,AutoInrcement, PrimaryKey, AutoIncNativeNullable, AutoIncNullableType, NormalField, NativeNullable, NullableType
		/// </summary>
		FieldKeyType,
		FieldReferencedKeyType,

		/// <summary>
		/// NormalField_Convert, NormalField_Cast, Nullable_Convert, Nullable_Cast
		/// </summary>
		FieldKeyReadType,

		/// <summary>
		/// NoAutoIncrement, OneAutoIncrement, MoreAutoIncrement
		/// </summary>
		TableAutoIncrement,

		/// <summary>
		/// TheReplacement
		/// </summary>
		OneReplacement,

		/// <summary>
		/// SQLServer, Oracle, SQLite
		/// </summary>
		DatabaseProvider
	}
}
