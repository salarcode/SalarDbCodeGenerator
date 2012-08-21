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
namespace SalarDbCodeGenerator.GeneratorEngine
{
	public class ReplaceConsts
	{
		public const string NewLine = "\n";
		public const string PatternContentReplacer = "[#{0}#]";
		public const string PatternContentInnerContents = "[:InnerContent:]";
		public const string ConnectionString = "[:ConnectionString:]";
		public const string ConnectionStringUser = "[:ConnectionStringUser:]";
		public const string ConnectionStringPwd = "[:ConnectionStringPwd:]";
		public const string Namespace = "[:Namespace:]";
		public const string DatabaseName = "[:DatabaseName:]";

		public const string OperateDate = "[:OperateDate:]";
		public const string Generator = "[:Generator:]";

		public const string Pattern_LanguageSettings_Precision = "[:Precision:]";
		public const string Pattern_LanguageSettings_Scale = "[:Scale:]";

		public const string AutoIncrementDataType = "[:AutoIncrementDotNetType:]";

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
		public const string FieldDataType = "[:FieldDotNetType:]";
		public const string FieldName = "[:FieldName:]";
		public const string FieldNameDb = "[:FieldNameDb:]";
		public const string FieldDescription = "[:FieldDescription:]";

		public const string FieldOrdinalValue = "[:FieldOrdinalValue:]";
		public const string FieldIsPrimaryKey = "[:FieldIsPrimaryKey:]";
		public const string FieldCanBeNull = "[:FieldCanBeNull:]";

		// PrimaryKey
		public const string PrimaryKeyDbType = "[:PrimaryKeyDbType:]";
		public const string PrimaryKeyDbTypeSize = "[:PrimaryKeyDbTypeSize:]";
		public const string PrimaryKeyDataType = "[:PrimaryKeyDotNetType:]";
		public const string PrimaryKeyName = "[:PrimaryKeyName:]";
		public const string PrimaryKeyNameDb = "[:PrimaryKeyNameDb:]";

		// Constraint keys
		public const string IndexKeyDbType = "[:IndexKeyDbType:]";
		public const string IndexKeyDbTypeSize = "[:IndexKeyDbTypeSize:]";
		public const string IndexKeyDataType = "[:IndexKeyDotNetType:]";
		public const string IndexKeyName = "[:IndexKeyName:]";
		public const string IndexKeyNameDb = "[:IndexKeyNameDb:]";
		public const string IndexName = "[:IndexName:]";

		// Project
		public const string ProjectItemPath = "[:ProjectItemPath:]";
		public const string ProjectName = "[:ProjectName:]";
		public const string ProjectReference = "[:ProjectReference:]";

		// Table
		public const string TableName = "[:TableName:]";
		public const string TableNameSchemaCS = "[:TableNameCS:]";
		public const string TableNameDb = "[:TableNameDb:]";
		public const string TableNameRefField = "[:TableNameRefField:]";
		public const string TableNameDbRefField = "[:TableNameDbRefField:]";
		public const string TableOwnerName = "[:TableOwnerName:]";

		// ForeignKeys
		public const string LocalTableName = "[:LocalTableName:]";
		public const string LocalTableNameDb = "[:LocalTableNameDb:]";
		public const string ForeignTableName = "[:ForeignTableName:]";
		public const string ForeignTableNameAsField = "[:ForeignTableNameAsField:]";
		public const string ForeignTableNameDb = "[:ForeignTableNameDb:]";
		// ForeignKeys - LocalField
		public const string LocalFieldDbType = "[:LocalFieldDbType:]";
		public const string LocalFieldDbTypeSize = "[:LocalFieldDbTypeSize:]";
		public const string LocalFieldDataType = "[:LocalFieldDotNetType:]";
		public const string LocalFieldName = "[:LocalFieldName:]";
		public const string LocalFieldNameDb = "[:LocalFieldNameDb:]";
		public const string LocalFieldDescription = "[:LocalFieldDescription:]";
		// ForeignKeys - ForeignField
		public const string ForeignFieldDbType = "[:ForeignFieldDbType:]";
		public const string ForeignFieldDbTypeSize = "[:ForeignFieldDbTypeSize:]";
		public const string ForeignFieldDataType = "[:ForeignFieldDotNetType:]";
		public const string ForeignFieldName = "[:ForeignFieldName:]";
		public const string ForeignFieldNameDb = "[:ForeignFieldNameDb:]";
		public const string ForeignFieldDescription = "[:ForeignFieldDescription:]";

	}
 }