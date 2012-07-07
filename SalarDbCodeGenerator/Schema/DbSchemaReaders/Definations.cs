// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Programmer: Salar Khalilzadeh <salar2k@gmail.com>
// Copytight(c) 2012, All rights reserved
// 2012/07/06
// ====================================
namespace SalarDbCodeGenerator.Schema.DbSchemaReaders
{
	public enum DatabaseProvider
	{
		SQLServer,
		SqlCe4,
		Oracle,
		SQLite
	}

	public enum DataProviderClassNames
	{
		ClassPrefix,
		ClassCommand,
		ClassConnection,
		ClassDataAdapter,
		ClassDataReader,
		ClassParameter,
		ClassTransaction,
		ClassNamespace,
		AssemblyReference,
		StoredProcParamPrefix
	}

}
