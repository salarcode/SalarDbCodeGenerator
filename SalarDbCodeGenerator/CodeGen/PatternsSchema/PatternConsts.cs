using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2010-03-09
// ====================================
namespace SalarSoft.DbCodeGenerator.CodeGen.PatternsSchema
{
	public static class PatternConsts
	{
		public static class ReplacementType
		{
			//OneReplacement
			public const string OneReplacement_TheReplacement = "TheReplacement";

			// DatabaseProvider
			public const string DatabaseProvider_Oracle = "Oracle";
			public const string DatabaseProvider_SQLServer = "SQLServer";
			public const string DatabaseProvider_SQLite = "SQLite";
			public const string DatabaseProvider_SqlCe4 = "SqlCe4";

			// TablePrimaryKey
			public const string TablePrimaryKey_ReadOnlyTable = "ReadOnlyTable";
			public const string TablePrimaryKey_NoPrimaryKey = "NoPrimaryKey";
			public const string TablePrimaryKey_WithPrimaryKey = "WithPrimaryKey";

			// NormalKey
			public const string TableForeignKey_NormalKey = "NormalKey";
			public const string TableIndexConstraint_NormalKey = "NormalKey";
			public const string TableUniqueConstraint_NormalKey = "NormalKey";

			// FieldForeignKey
			public const string FieldForeignKey_MultiplicityOne = "MultiplicityOne";
			public const string FieldForeignKey_MultiplicityMany = "MultiplicityMany";

			// FieldPrimaryKey
			public const string FieldPrimaryKey_NormalField = "NormalField";
			public const string FieldPrimaryKey_PrimaryKey = "PrimaryKey";

			// FieldKeyType
			public const string FieldKeyType_AutoInrcement = "AutoInrcement";
			public const string FieldKeyType_PrimaryKey = "PrimaryKey";
			public const string FieldKeyType_AutoInrcementPrimaryKey = "AutoInrcementPrimaryKey";
			public const string FieldKeyType_AutoIncNativeNullable = "AutoIncNativeNullable";
			public const string FieldKeyType_AutoIncNullableType = "AutoIncNullableType";
			public const string FieldKeyType_NormalField = "NormalField";
			public const string FieldKeyType_NativeNullable = "NativeNullable";
			public const string FieldKeyType_NullableType = "NullableType";

			// FieldKeyReadType
			public const string FieldKeyReadType_NormalField_Convert = "NormalField_Convert";
			public const string FieldKeyReadType_NormalField_Cast = "NormalField_Cast";
			public const string FieldKeyReadType_Nullable_Convert = "Nullable_Convert";
			public const string FieldKeyReadType_Nullable_Cast = "Nullable_Cast";

			// TableAutoIncrement
			public const string TableAutoIncrement_NoAutoIncrement = "NoAutoIncrement";
			public const string TableAutoIncrement_OneAutoIncrement = "OneAutoIncrement";
			public const string TableAutoIncrement_MoreAutoIncrement = "MoreAutoIncrement";
		}
		public static class PartialContents
		{
			public const string ProjectAppConfig = "ProjectAppConfig";
			public const string PatternFileName_AppConfig = "AppConfig";
		}
		public static class PtternGroups
		{
			public const string Base = "Base";
			public const string Common = "Common";
			public const string TableBLL = "TableBLL";
			public const string TableDAL = "TableDAL";
			public const string TableModel = "TableModel";
			public const string StoredProcedure = "StoredProcedures";
			public const string ProjectFile = "ProjectFile";

		}
	}
}