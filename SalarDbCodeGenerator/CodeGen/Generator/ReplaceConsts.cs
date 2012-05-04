using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2011-12-10
// ====================================
namespace SalarDbCodeGenerator.CodeGen.Generator
{
	public class ReplaceConsts
	{
		public const string NewLine = "\r\n";
		public const string PatternContentReplacer = "[#{0}#]";
		public const string PatternContentInnerContents = "[:InnerContent:]";
		public const string ConnectionString = "[:ConnectionString:]";
		public const string Namespace = "[:Namespace:]";
		public const string DatabaseName = "[:DatabaseName:]";

		public const string OperateDate = "[:OperateDate:]";
		public const string Generator = "[:Generator:]";

		public const string Pattern_LanguageSettings_Precision = "[:Precision:]";
		public const string Pattern_LanguageSettings_Scale = "[:Scale:]";

		public const string AutoIncrementDotNetType = "[:AutoIncrementDotNetType:]";

		// Database provider
		public const string ProviderClassPrefix = "[:ProviderClassPrefix:]";
		public const string ProviderClassCommand = "[:ProviderClassCommand:]";
		public const string ProviderClassConnection = "[:ProviderClassConnection:]";
		public const string ProviderClassDataAdapter = "[:ProviderClassDataAdapter:]";
		public const string ProviderClassDataReader = "[:ProviderClassDataReader:]";
		public const string ProviderClassParameter = "[:ProviderClassParameter:]";
		public const string ProviderClassTransaction = "[:ProviderClassTransaction:]";
		public const string ProviderClassReferenceName = "[:ProviderClassReferenceName:]";
		public const string ProviderAssemblyReference = "[:ProviderAssemblyReference:]";
		public const string ProviderSpParamPrefix = "[:ProviderSPParamPrefix:]";

		// Field
		public const string FieldDbType = "[:FieldDbType:]";
		public const string FieldDbTypeSize = "[:FieldDbTypeSize:]";
		public const string FieldDbSize = "[:FieldDbSize:]";
		public const string FieldDotNetType = "[:FieldDotNetType:]";
		public const string FieldName = "[:FieldName:]";
		public const string FieldNativeName = "[:FieldNativeName:]";
		public const string FieldDescription = "[:FieldDescription:]";

		public const string FieldOrdinalValue = "[:FieldOrdinalValue:]";
		public const string FieldIsPrimaryKey = "[:FieldIsPrimaryKey:]";
		public const string FieldCanBeNull = "[:FieldCanBeNull:]";

		// PrimaryKey
		public const string PrimaryKeyDbType = "[:PrimaryKeyDbType:]";
		public const string PrimaryKeyDbTypeSize = "[:PrimaryKeyDbTypeSize:]";
		public const string PrimaryKeyDotNetType = "[:PrimaryKeyDotNetType:]";
		public const string PrimaryKeyName = "[:PrimaryKeyName:]";
		public const string PrimaryKeyNativeName = "[:PrimaryKeyNativeName:]";

		// Constraint keys
		public const string IndexKeyDbType = "[:IndexKeyDbType:]";
		public const string IndexKeyDbTypeSize = "[:IndexKeyDbTypeSize:]";
		public const string IndexKeyDotNetType = "[:IndexKeyDotNetType:]";
		public const string IndexKeyName = "[:IndexKeyName:]";
		public const string IndexKeyNativeName = "[:IndexKeyNativeName:]";

		// Project
		public const string ProjectItemPath = "[:ProjectItemPath:]";
		public const string ProjectName = "[:ProjectName:]";
		public const string ProjectReference = "[:ProjectReference:]";

		// Table
		public const string TableName = "[:TableName:]";
		public const string TableNameCaseSensitive = "[:TableNameCS:]";
		public const string TableNativeName = "[:TableNativeName:]";
		public const string TableNameRefField = "[:TableNameRefField:]";
		public const string TableNativeNameRefField = "[:TableNativeNameRefField:]";

		// ForeignKeys
		public const string LocalTableName = "[:LocalTableName:]";
		public const string LocalTableNativeName = "[:LocalTableNativeName:]";
		public const string ForeignTableName = "[:ForeignTableName:]";
		public const string ForeignTableNameAsField = "[:ForeignTableNameAsField:]";
		public const string ForeignTableNativeName = "[:ForeignTableNativeName:]";
		// ForeignKeys - LocalField
		public const string LocalFieldDbType = "[:LocalFieldDbType:]";
		public const string LocalFieldDbTypeSize = "[:LocalFieldDbTypeSize:]";
		public const string LocalFieldDotNetType = "[:LocalFieldDotNetType:]";
		public const string LocalFieldName = "[:LocalFieldName:]";
		public const string LocalFieldNativeName = "[:LocalFieldNativeName:]";
		public const string LocalFieldDescription = "[:LocalFieldDescription:]";
		// ForeignKeys - ForeignField
		public const string ForeignFieldDbType = "[:ForeignFieldDbType:]";
		public const string ForeignFieldDbTypeSize = "[:ForeignFieldDbTypeSize:]";
		public const string ForeignFieldDotNetType = "[:ForeignFieldDotNetType:]";
		public const string ForeignFieldName = "[:ForeignFieldName:]";
		public const string ForeignFieldNativeName = "[:ForeignFieldNativeName:]";
		public const string ForeignFieldDescription = "[:ForeignFieldDescription:]";

	}
}