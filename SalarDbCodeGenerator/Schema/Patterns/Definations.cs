using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalarDbCodeGenerator.Schema.Patterns
{
	public enum PatternFileAppliesTo
	{
		General = 0,
		TablesAndViewsEach,
		TablesAndViewsAll,
		TablesEach,
		TablesAll,
		ViewsEach,
		ViewsAll,
		ProjectFile
	}

	/// <summary>
	/// Content key mode for pattern content
	/// </summary>
	public enum PatternConditionKeyMode
	{
		/// <summary>
		/// TheReplacement
		/// </summary>
		TablesAndViewsAll,

		/// <summary>
		/// TheReplacement
		/// </summary>
		TablesAll,

		/// <summary>
		/// TheReplacement
		/// </summary>
		ViewsAll,

		/// <summary>
		/// ReadOnlyTable, NoPrimaryKey, WithPrimaryKey
		/// </summary>
		TablePrimaryKey,

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
		/// NoAutoIncrement, OneAutoIncrement, MoreAutoIncrement
		/// </summary>
		TableAutoIncrement,

		/// <summary>
		/// TheReplacement
		/// </summary>
		FieldsAll,
		Field,

		/// <summary>
		/// NormalField, PrimaryKey
		/// </summary>
		FieldsPrimaryKeyAll,
		FieldPrimaryKey,

		/// <summary>
		/// MultiplicityOne, MultiplicityMany
		/// </summary>
		FieldsForeignKeyAll,
		FieldForeignKey,

		/// <summary>
		/// NotSet, NoAction, Cascade, SetNull, SetDefault, Restrict
		/// </summary>
		ForeignKeyUpdateAction,

		/// <summary>
		/// NotSet, NoAction, Cascade, SetNull, SetDefault, Restrict
		/// </summary>
		ForeignKeyDeleteAction,

		/// <summary>
		/// AutoInrcementPrimaryKey ,AutoInrcement, PrimaryKey, AutoIncNativeNullable, AutoIncNullableType, NormalField, NativeNullable, NullableType
		/// </summary>
		FieldsKeyTypeAll,
		FieldsReferencedKeyTypeAll,
		FieldKeyType,
		FieldReferencedKeyType,

		/// <summary>
		/// NormalField_Convert, NormalField_Cast, Nullable_Convert, Nullable_Cast
		/// </summary>
		FieldsKeyReadTypeAll,
		FieldKeyReadType,

		/// <summary>
		/// None, String, Integer, Decimal
		/// </summary>
		FieldsCondensedTypeAll,
		FieldCondensedType,

		/// <summary>
		/// SQLServer, Oracle, SQLite
		/// </summary>
		DatabaseProvider,
		ProjectFiles,
		General
	}

	/// <summary>
	/// Content key mode for pattern content
	/// </summary>
	public static class ConditionKeyModeConsts
	{
		/// <summary>
		/// ReadOnlyTable, NoPrimaryKey, WithPrimaryKey
		/// </summary>
		public static class TablePrimaryKey
		{
			public const string ReadOnlyTable = "ReadOnlyTable";
			public const string NoPrimaryKey = "NoPrimaryKey";
			public const string WithPrimaryKey = "WithPrimaryKey";
		}

		/// <summary>
		/// NoAutoIncrement, OneAutoIncrement, MoreAutoIncrement
		/// </summary>
		public static class TableAutoIncrement
		{
			public const string NoAutoIncrement = "NoAutoIncrement";
			public const string OneAutoIncrement = "OneAutoIncrement";
			public const string MoreAutoIncrement = "MoreAutoIncrement";
		}


		/// <summary>
		/// NormalKey
		/// </summary>
		public static class TableForeignKey
		{
			public const string NormalKey = "NormalKey";
		}
		/// <summary>
		/// NormalKey
		/// </summary>
		public static class TableIndexConstraint
		{
			public const string NormalKey = "NormalKey";
		}

		/// <summary>
		/// NormalKey
		/// </summary>
		public static class TableUniqueConstraint
		{
			public const string NormalKey = "NormalKey";
		}

		/// <summary>
		/// NormalField, PrimaryKey
		/// </summary>
		public static class FieldPrimaryKey
		{
			public const string NormalField = "NormalField";
			public const string PrimaryKey = "PrimaryKey";
		}

		/// <summary>
		/// MultiplicityOne, MultiplicityMany
		/// </summary>
		public static class FieldForeignKey
		{
			public const string MultiplicityOne = "MultiplicityOne";
			public const string MultiplicityMany = "MultiplicityMany";
		}

		/// <summary>
		/// AutoInrcementPrimaryKey ,AutoInrcement, PrimaryKey, AutoIncNativeNullable, AutoIncNullableType, NormalField, NativeNullable, NullableType
		/// </summary>
		public static class FieldKeyType
		{
			public const string AutoInrcement = "AutoInrcement";
			public const string PrimaryKey = "PrimaryKey";
			public const string AutoInrcementPrimaryKey = "AutoInrcementPrimaryKey";
			public const string AutoIncNativeNullable = "AutoIncNativeNullable";
			public const string AutoIncNullableType = "AutoIncNullableType";
			public const string NormalField = "NormalField";
			public const string NativeNullable = "NativeNullable";
			public const string NullableType = "NullableType";
		}
		public static class FieldReferencedKeyType
		{

		}

		/// <summary>
		/// NormalField_Convert, NormalField_Cast, Nullable_Convert, Nullable_Cast
		/// </summary>
		public static class FieldKeyReadType
		{
			public const string NormalField_Convert = "NormalField_Convert";
			public const string NormalField_Cast = "NormalField_Cast";
			public const string Nullable_Convert = "Nullable_Convert";
			public const string Nullable_Cast = "Nullable_Cast";
		}

		/// <summary>
		/// None, String
		/// </summary>
		public static class FieldCondensedType
		{
			public const string None = "StringType_None";
			public const string String = "StringType_String";
			public const string Integer = "NumericType_Integer";
			public const string Decimal = "NumericType_Decimal";
		}

		/// <summary>
		/// TheReplacement
		/// </summary>
		public static class OneReplacement
		{
			public const string TheReplacement = "TheReplacement";
		}

		/// <summary>
		/// SQLServer, Oracle, SQLite
		/// </summary>
		public static class DatabaseProvider
		{
			public const string Oracle = "Oracle";
			public const string SQLServer = "SQLServer";
			public const string SQLite = "SQLite";
			public const string SqlCe4 = "SqlCe4";
		}


		public static class PartialContents
		{
			public const string ProjectAppConfig = "ProjectAppConfig";
			public const string PatternFileName_AppConfig = "AppConfig";
		}
		public static class PtternGroups
		{
			//public const string Base = "Base";
			//public const string Common = "Common";
			//public const string TableBLL = "TableBLL";
			//public const string TableDAL = "TableDAL";
			//public const string TableModel = "TableModel";
			//public const string StoredProcedure = "StoredProcedures";
			public const string ProjectFile = "ProjectFile";

		}

	}

}
